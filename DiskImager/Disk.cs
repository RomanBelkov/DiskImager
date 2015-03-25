/**
 *  DiskImager - a tool for writing / reading images on SD cards
 *
 *  Copyright 2015 by Roman Belkov <romanbelkov@gmail.com>
 *  Copyright 2013, 2014 by Alex J. Lennon <ajlennon@dynamicdevices.co.uk>
 *
 *  Licensed under GNU General Public License 3.0 or later. 
 *  Some rights reserved. See LICENSE, AUTHORS.
 *
 * @license GPL-3.0+ <http://www.gnu.org/licenses/gpl-3.0.en.html>
 */

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DynamicDevices.DiskWriter.Properties;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using XZ.NET;

namespace DynamicDevices.DiskWriter
{
    public delegate void TextHandler( object sender, string message);

    public delegate void ProgressHandler(object sender, int progressPercentage);

    internal class Disk
    {
        public bool IsCancelling { get; set;}

        private event TextHandler _onLogMsg;

        public event TextHandler OnLogMsg
        {
            add
            {
                _onLogMsg -= value;
                _onLogMsg += value;
                _diskAccess.OnLogMsg -= value;
                _diskAccess.OnLogMsg += value;
            }
            remove
            {
                _onLogMsg -= value;
                _diskAccess.OnLogMsg -= value;
            }
        }

        private event ProgressHandler _onProgress;

        public event ProgressHandler OnProgress
        {
            add
            {
                _onProgress -= value;
                _onProgress += value;
                _diskAccess.OnProgress += value;
            }
            remove
            {
                _onProgress -= value;
                _diskAccess.OnProgress -= value; 
            }
        }

        private readonly IDiskAccess _diskAccess;

        /// <summary>
        /// Construct Disk object with underlying platform specific disk access implementation
        /// </summary>
        /// <param name="diskAccess"></param>
        public Disk(IDiskAccess diskAccess)
        {
            _diskAccess = diskAccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <param name="fileName"></param>
        /// <param name="eCompType"></param>
        /// <param name="removeAfter"></param>
        /// <returns></returns>
        public bool WriteDrive(string driveLetter, string fileName, EnumCompressionType eCompType, bool removeAfter)
        {
            IsCancelling = false;

            var dtStart = DateTime.Now;

            if(!File.Exists(fileName))
                throw new ArgumentException(fileName + Resources.Disk_WriteDrive__doesn_t_exist);

            //
            // Get physical drive partition for logical partition
            // 
            var physicalDrive = _diskAccess.GetPhysicalPathForLogicalPath(driveLetter);
            if (string.IsNullOrEmpty(physicalDrive))
            {
                LogMsg(Resources.Disk_WriteDrive_Error__Couldn_t_map_partition_to_physical_drive);
                _diskAccess.UnlockDrive();
                return false;
            }

            //
            // Lock logical drive
            //
            var success = _diskAccess.LockDrive(driveLetter);
            if (!success)
            {
                LogMsg(Resources.Disk_WriteDrive_Failed_to_lock_drive);
                return false;
            }            

            //
            // Get drive size 
            //
            var driveSize = _diskAccess.GetDriveSize(physicalDrive);
            if (driveSize <= 0)
            {
                LogMsg(Resources.Disk_WriteDrive_Failed_to_get_device_size);
                _diskAccess.UnlockDrive();
                return false;
            }

            //
            // Open the physical drive
            // 
            var physicalHandle = _diskAccess.Open(physicalDrive);
            if (physicalHandle == null)
            {
                LogMsg(Resources.Disk_WriteDrive_Failed_to_open_physical_drive);
                _diskAccess.UnlockDrive();
                return false;
            }

            var buffer = new byte[Globals.MaxBufferSize];
            long offset = 0;

            var fileLength = new FileInfo(fileName).Length;

            var uncompressedlength = fileLength;

            var errored = true;

            using (var basefs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Stream fs;

                switch (eCompType)
                {
                    case EnumCompressionType.Zip:
                     
                        var zipFile = new ZipFile(basefs);
                
                        var ze = (from ZipEntry zipEntry in zipFile
                                        where zipEntry.IsFile
                                        select zipEntry).FirstOrDefault();

                        if(ze == null)
                        {
                            LogMsg(Resources.Disk_WriteDrive_Error_reading_zip_input_stream);
                            goto readfail2;                                
                        }

                        var zis = zipFile.GetInputStream(ze);

                        uncompressedlength = ze.Size;
                        fs = zis;
                     
                        break;

                    case EnumCompressionType.Gzip:
                     
                        var gzis = new GZipInputStream(basefs) {IsStreamOwner = true};

                        uncompressedlength = gzis.Length;
                        fs = gzis;
                     
                        break;

                    case EnumCompressionType.Targzip:
                     
                        var gzos = new GZipInputStream(basefs) {IsStreamOwner = true};
                        var tis = new TarInputStream(gzos);

                        TarEntry tarEntry;
                        do
                        {
                            tarEntry = tis.GetNextEntry();
                        } while (tarEntry.IsDirectory);

                        uncompressedlength = tarEntry.Size;
                        fs = tis;
                     
                        break;

                    case EnumCompressionType.XZ:
                     
                        var xzs = new XZInputStream(basefs);
                        
                        uncompressedlength = xzs.Length;
                        fs = xzs;

                        break;

                    default:

                        // No compression - direct to file stream
                        fs = basefs;
                        uncompressedlength = fs.Length;

                        break;
                }

                var bufferOffset = 0;
                using (var br = new BinaryReader(fs))
                {
                    while (offset < uncompressedlength && !IsCancelling)
                    {
                        // Note: There's a problem writing certain lengths to the underlying physical drive.
                        //       This appears when we try to read from a compressed stream as it gives us
                        //       "strange" lengths which then fail to be written via Writefile() so try to build
                        //       up a decent block of bytes here...
                        int readBytes;
                        do
                        {
                            readBytes = br.Read(buffer, bufferOffset, buffer.Length - bufferOffset);
                            bufferOffset += readBytes;
                        } while (bufferOffset < Globals.MaxBufferSize && readBytes != 0);
 
                        int wroteBytes;
                        var bytesToWrite = bufferOffset;
                        var trailingBytes = 0;

                        // Assume that the underlying physical drive will at least accept powers of two!
                        if(!IsPowerOfTwo((ulong)bufferOffset))
                        {
                            // Find highest bit (32-bit max)
                            var highBit = 31;
                            for (; ((bufferOffset & (1 << highBit)) == 0) && highBit >= 0; highBit--)
                            {
                            }

                            // Work out trailing bytes after last power of two
                            var lastPowerOf2 = 1 << highBit;

                            bytesToWrite = lastPowerOf2;
                            trailingBytes = bufferOffset - lastPowerOf2;
                        }

                        if (_diskAccess.Write(buffer, bytesToWrite, out wroteBytes) < 0)
                        {
                            LogMsg(Resources.Disk_WriteDrive_Error_writing_data_to_drive__ + Marshal.GetHRForLastWin32Error());
                            goto readfail1;
                        }

                        if (wroteBytes != bytesToWrite)
                        {
                            LogMsg(Resources.Disk_WriteDrive_Error_writing_data_to_drive___past_EOF_);
                            goto readfail1;
                        }

                        // Move trailing bytes up - Todo: Suboptimal
                        if (trailingBytes > 0)
                        {
                            Buffer.BlockCopy(buffer, bufferOffset - trailingBytes, buffer, 0, trailingBytes);
                            bufferOffset = trailingBytes;
                        }
                        else
                        {
                            bufferOffset = 0;
                        }
                        offset += (uint)wroteBytes;

                        var percentDone = (int)(100 * offset / uncompressedlength);
                        var tsElapsed = DateTime.Now.Subtract(dtStart);
                        var bytesPerSec = offset / tsElapsed.TotalSeconds;

                        Progress(percentDone);
                        
                        LogMsg(Resources.Disk_WriteDrive_Wrote_ + percentDone + @"%, " + (offset / (1024 * 1024)) + @" MB / " +
                                                     (uncompressedlength / (1024 * 1024) + " MB, " +
                                                      string.Format("{0:F}", (bytesPerSec / (1024 * 1024))) + @" MB/sec," +  Resources.Disk_WriteDrive_Elapsed_time__ + tsElapsed.ToString(@"dd\.hh\:mm\:ss")));
                    }
                }

                if (fs is ZipOutputStream)
                {
                    ((ZipOutputStream)fs).CloseEntry();
                    ((ZipOutputStream)fs).Close();
                } else if (fs is TarOutputStream)
                {
                    ((TarOutputStream)fs).CloseEntry();
                    fs.Close();
                } else if (fs is GZipOutputStream)
                {
                    fs.Close();
                } else if (fs is XZOutputStream)
                {
                    fs.Close();
                }
            }
            errored = false;

            if (removeAfter && !IsCancelling)
                _diskAccess.UnmountDrive();

        readfail1:
            _diskAccess.Close();
        readfail2:
            _diskAccess.UnlockDrive();

            var tstotalTime = DateTime.Now.Subtract(dtStart);

            if (IsCancelling)
                LogMsg(Resources.Disk_WriteDrive_Cancelled);
            else 
                LogMsg(Resources.Disk_WriteDrive_All_Done___Wrote_ + offset + Resources.Disk_WriteDrive__bytes__Elapsed_time_ + tstotalTime.ToString(@"dd\.hh\:mm\:ss"));

            Progress(0);
            return !errored;
        }

        /// <summary>
        /// Read data direct from drive to file
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <param name="fileName"></param>
        /// <param name="eCompType"></param>
        /// <returns></returns>
        public bool ReadDrive(string driveLetter, string fileName, EnumCompressionType eCompType, bool bUseMBR)
        {
            IsCancelling = false;

            var dtStart = DateTime.Now;

            //
            // Map to physical drive
            // 
            var physicalDrive = _diskAccess.GetPhysicalPathForLogicalPath(driveLetter);
            if(string.IsNullOrEmpty(physicalDrive))
            {
                LogMsg(Resources.Disk_WriteDrive_Error__Couldn_t_map_partition_to_physical_drive);
                _diskAccess.UnlockDrive();
                return false;
            }

            //
            // Lock logical drive
            //
            var success = _diskAccess.LockDrive(driveLetter);
            if (!success)
            {
                LogMsg(Resources.Disk_WriteDrive_Failed_to_lock_drive);
                return false;
            }

            //
            // Get drive size 
            //
            var driveSize = _diskAccess.GetDriveSize(physicalDrive);
            if(driveSize <= 0)
            {
                LogMsg(Resources.Disk_WriteDrive_Failed_to_get_device_size);
                _diskAccess.UnlockDrive();
                return false;                
            }

            var readSize = driveSize;

            //
            // Open the physical drive
            // 
            var physicalHandle = _diskAccess.Open(physicalDrive);
            if (physicalHandle == null)
            {
                LogMsg(Resources.Disk_WriteDrive_Failed_to_open_physical_drive);
                _diskAccess.UnlockDrive();
                return false;
            }

            //
            // Start doing the read
            //

            var buffer = new byte[Globals.MaxBufferSize];
            var offset = 0L;

            using(var basefs = (Stream)new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                Stream fs;

                switch (eCompType)
                {
                    case EnumCompressionType.Zip:
                        var zfs = new ZipOutputStream(basefs);

                        // Default to middle of the range compression
                        zfs.SetLevel(Globals.CompressionLevel);

                        var fi = new FileInfo(fileName);
                        var entryName = fi.Name;
                        entryName = entryName.ToLower().Replace(".zip", "");
                        entryName = ZipEntry.CleanName(entryName);
                        var zipEntry = new ZipEntry(entryName) {DateTime = fi.LastWriteTime};
                        zfs.IsStreamOwner = true;

                        // Todo: Consider whether size needs setting for older utils ?

                        zfs.PutNextEntry(zipEntry);

                        fs = zfs;

                        break;

                    case EnumCompressionType.Gzip:
                        
                        var gzis = new GZipOutputStream(basefs);
                        gzis.SetLevel(Globals.CompressionLevel);
                        gzis.IsStreamOwner = true;

                        fs = gzis;
                        
                        break;

                    case EnumCompressionType.Targzip:
                        
                        var gzos = new GZipOutputStream(basefs);
                        gzos.SetLevel(Globals.CompressionLevel);
                        gzos.IsStreamOwner = true;

                        var tos = new TarOutputStream(gzos);

                        fs = tos;
                        
                        break;

                    case EnumCompressionType.XZ:

                        var xzs = new XZOutputStream(basefs);
                        fs = xzs;

                        break;

                    default:

                        // No compression - direct to file stream
                        fs = basefs;

                        break;
                }

                    while (offset < readSize && !IsCancelling)
                    {
                        // NOTE: If we provide a buffer that extends past the end of the physical device ReadFile() doesn't
                        //       seem to do a partial read. Deal with this by reading the remaining bytes at the end of the
                        //       drive if necessary

                        var readMaxLength =
                            (int)
                            ((((ulong) readSize - (ulong) offset) < (ulong) buffer.Length)
                                 ? ((ulong) readSize - (ulong) offset)
                                 : (ulong) buffer.Length);

                        int readBytes;
                        if (_diskAccess.Read(buffer, readMaxLength, out readBytes) < 0)
                        {
                            LogMsg(Resources.Disk_ReadDrive_Error_reading_data_from_drive__ +
                                           Marshal.GetHRForLastWin32Error());
                            goto readfail1;
                        }

                        if (readBytes == 0)
                        {
                            LogMsg(Resources.Disk_ReadDrive_Error_reading_data_from_drive___past_EOF_);
                            goto readfail1;
                        }

                        // Check MBR
                        if (bUseMBR && offset == 0)
                        {
                            var truncatedSize = ParseMBRForSize(buffer);
                            
                            if(truncatedSize > driveSize)
                            {
                                LogMsg(Resources.Disk_ReadDrive_Problem_with_filesystem__It_reports_it_is_larger_than_the_disk_);
                                goto readfail1;
                            }

                            if(truncatedSize == 0)
                            {
                                LogMsg(Resources.Disk_ReadDrive_No_valid_partitions_on_drive);
                                goto readfail1;
                            }

                            readSize = truncatedSize;
                        }

                        if(offset == 0)
                        {
                            switch (eCompType)
                            {
                                case EnumCompressionType.Targzip:
                                    var fi = new FileInfo(fileName);
                                    var entryName = fi.Name;
                                    entryName = entryName.ToLower().Replace(".tar.gz", "");
                                    entryName = entryName.ToLower().Replace(".tgz", "");

                                    var tarEntry = TarEntry.CreateTarEntry(entryName);
                                    tarEntry.Size = readSize;
                                    tarEntry.ModTime = DateTime.SpecifyKind(fi.LastWriteTime, DateTimeKind.Utc);

                                    ((TarOutputStream) fs).PutNextEntry(tarEntry);

                                    break;
                            }
                        }

                        fs.Write(buffer, 0, readBytes);

                        offset += (uint) readBytes;

                        var percentDone = (int) (100*offset/readSize);
                        var tsElapsed = DateTime.Now.Subtract(dtStart);
                        var bytesPerSec = offset/tsElapsed.TotalSeconds;

                        Progress(percentDone);
                        LogMsg(Resources.Disk_ReadDrive_Read_ + percentDone + @"%, " + (offset/(1024*1024)) + @" MB / " +
                                       (readSize/(1024*1024) + @" MB" + Resources.Disk_ReadDrive__Physical__ + (driveSize/(1024*1024)) + " MB), " +
                                        string.Format("{0:F}", (bytesPerSec/(1024*1024))) + @" MB/sec," + Resources.Disk_ReadDrive_Elapsed_time__ +
                                        tsElapsed.ToString(@"dd\.hh\:mm\:ss")));

                    }
                
                    if (fs is ZipOutputStream)
                    {
                        ((ZipOutputStream)fs).CloseEntry();
                        ((ZipOutputStream)fs).Close();
                    } else if (fs is TarOutputStream)
                    {
                        ((TarOutputStream) fs).CloseEntry();
                        fs.Close();
                    } else if (fs is GZipOutputStream)
                    {
                        fs.Close();
                    } else if (fs is XZOutputStream)
                    {
                        fs.Close();
                    }
            }

        readfail1:
            
            _diskAccess.Close();
            
            _diskAccess.UnlockDrive();
            
            var tstotalTime = DateTime.Now.Subtract(dtStart);

            if (IsCancelling)
                LogMsg(Resources.Disk_WriteDrive_Cancelled);
            else
                LogMsg(Resources.Disk_ReadDrive_All_Done___Read_ + offset + Resources.Disk_WriteDrive__bytes__Elapsed_time_ + tstotalTime.ToString(@"dd\.hh\:mm\:ss"));
            Progress(0);
            return true;
        }

        #region Support

        private static bool IsPowerOfTwo(ulong x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        private void Progress(int progressValue)
        {
            if (_onProgress != null)
                _onProgress(this, progressValue);
        }

        private void LogMsg(string msg)
        {
            if (_onLogMsg != null)
                _onLogMsg(this, msg);
        }
        
        private Int64 ParseMBRForSize(byte[] buffer)
        {
            // The whole situation with restoring struct from bytes is quite confusing
            // because you have to GCHandle so if You know something for sure
            // you can fix it
            var pinnedInfos = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var mbr = (MBR)Marshal.PtrToStructure(pinnedInfos.AddrOfPinnedObject(), typeof(MBR));
            pinnedInfos.Free();

            // Checking magic bytes here
            if (mbr.firstBootSignature != 0x55 && mbr.secondBootSignature != 0xAA)
            {
                LogMsg("Problem: MBR signature is not correct");
                return 0;
            }

            // Following part is attempt to determine the size of all partitions 
            // by last byte of one of them
            var endOfLastPartition = 0L;

            if(mbr.firstPartition.type != EnumPartitionType.EMPTY)
            {
                endOfLastPartition = (mbr.firstPartition.sectorsFromMBRToPartition + mbr.firstPartition.sectorsInPartition) * 512;
            }

            if (mbr.secondPartition.type != EnumPartitionType.EMPTY)
            {
                var endOfPartition = (mbr.secondPartition.sectorsFromMBRToPartition + mbr.secondPartition.sectorsInPartition) * 512;
                if (endOfLastPartition < endOfPartition) 
                {
                    endOfLastPartition = endOfPartition;
                }
            }

            if (mbr.thirdPartition.type != EnumPartitionType.EMPTY)
            {
                var endOfPartition = (mbr.thirdPartition.sectorsFromMBRToPartition + mbr.thirdPartition.sectorsInPartition) * 512;
                if (endOfLastPartition < endOfPartition)
                {
                    endOfLastPartition = endOfPartition;
                }
            }

            if (mbr.fourthPartition.type != EnumPartitionType.EMPTY)
            {
                var endOfPartition = (mbr.fourthPartition.sectorsFromMBRToPartition + mbr.fourthPartition.sectorsInPartition) * 512;
                if (endOfLastPartition < endOfPartition)
                {
                    endOfLastPartition = endOfPartition;
                }
            }

            return endOfLastPartition;
        }

        #endregion
    }

    [StructLayout(LayoutKind.Explicit, Size = 512)]
    struct MBR
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=446)]
        public byte[] bootstrapCode;

        [FieldOffset(446)]
        public PBR firstPartition;

        [FieldOffset(462)]
        public PBR secondPartition;

        [FieldOffset(478)]
        public PBR thirdPartition;

        [FieldOffset(494)]
        public PBR fourthPartition;

        [FieldOffset(510)]
        public byte firstBootSignature;

        [FieldOffset(511)] 
        public byte secondBootSignature;
    }

    enum EnumPartitionState : byte
    {
      INACTIVE = 0,
      ACTIVE = 0x80,
    }

    /*
         0  Empty           24  NEC DOS         81  Minix / old Lin bf  Solaris
         1  FAT12           27  Hidden NTFS Win 82  Linux swap / So c1  DRDOS/sec (FAT-
         2  XENIX root      39  Plan 9          83  Linux           c4  DRDOS/sec (FAT-
         3  XENIX usr       3c  PartitionMagic  84  OS/2 hidden C:  c6  DRDOS/sec (FAT-
         4  FAT16 <32M      40  Venix 80286     85  Linux extended  c7  Syrinx
         5  Extended        41  PPC PReP Boot   86  NTFS volume set da  Non-FS data
         6  FAT16           42  SFS             87  NTFS volume set db  CP/M / CTOS / .
         7  HPFS/NTFS/exFAT 4d  QNX4.x          88  Linux plaintext de  Dell Utility
         8  AIX             4e  QNX4.x 2nd part 8e  Linux LVM       df  BootIt
         9  AIX bootable    4f  QNX4.x 3rd part 93  Amoeba          e1  DOS access
         a  OS/2 Boot Manag 50  OnTrack DM      94  Amoeba BBT      e3  DOS R/O
         b  W95 FAT32       51  OnTrack DM6 Aux 9f  BSD/OS          e4  SpeedStor
         c  W95 FAT32 (LBA) 52  CP/M            a0  IBM Thinkpad hi eb  BeOS fs
         e  W95 FAT16 (LBA) 53  OnTrack DM6 Aux a5  FreeBSD         ee  GPT
         f  W95 Ext'd (LBA) 54  OnTrackDM6      a6  OpenBSD         ef  EFI (FAT-12/16/
        10  OPUS            55  EZ-Drive        a7  NeXTSTEP        f0  Linux/PA-RISC b
        11  Hidden FAT12    56  Golden Bow      a8  Darwin UFS      f1  SpeedStor
        12  Compaq diagnost 5c  Priam Edisk     a9  NetBSD          f4  SpeedStor
        14  Hidden FAT16 <3 61  SpeedStor       ab  Darwin boot     f2  DOS secondary
        16  Hidden FAT16    63  GNU HURD or Sys af  HFS / HFS+      fb  VMware VMFS
        17  Hidden HPFS/NTF 64  Novell Netware  b7  BSDI fs         fc  VMware VMKCORE
        18  AST SmartSleep  65  Novell Netware  b8  BSDI swap       fd  Linux raid auto
        1b  Hidden W95 FAT3 70  DiskSecure Mult bb  Boot Wizard hid fe  LANstep
        1c  Hidden W95 FAT3 75  PC/IX           be  Solaris boot    ff  BBT
        1e  Hidden W95 FAT1 80  Old Minix
    */

    enum EnumPartitionType : byte
    {
       EMPTY = 0,
       FAT12 = 1,
       FAT16_LESS_THAN_32MB = 0x04, 
       EXT_MSDOS = 0x05,
       FAT16_GREATER_THAN_32MB = 0x06, 
       FAT32 = 0x0B,
       FAT32_LBA = 0x0C,
       FAT16_LBA = 0x0E,
       EXT_MSDOS_LBA = 0x0F,
       LINUX_EXT2 = 0x83,
       LINUX_SWAP = 0x82,
    }
       
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    struct PBR
    {
        public EnumPartitionState state;

        public byte startHead;

        public byte startCylinderHighSector;

        public byte startCylinderLow;

        public EnumPartitionType type;

        public byte endHead;

        public byte endCylinderHighSector;

        public byte endCylinderLow;

        public uint sectorsFromMBRToPartition;

        public uint sectorsInPartition;

        public byte GetStartHead()
        {
            return startHead;
        }

        public ushort GetStartSector()
        {
            return (ushort)(startCylinderHighSector & 0x3F);
        }

        public ushort GetStartCylinder()
        {
            return (ushort)((((ushort)(startCylinderHighSector & 0xC0)) << 2) +startCylinderLow);
        }

        public byte GetEndHead()
        {
            return endHead;
        }

        public ushort GetEndSector()
        {
            return (ushort)(endCylinderHighSector & 0x3F);
        }

        public ushort GetEndCylinder()
        {
            return (ushort)((((ushort)(endCylinderHighSector & 0xC0)) << 2) + endCylinderLow);
        }

        public uint GetStartLBA()
        {
            return (uint) GetStartCylinder()*GetStartHead()*GetStartSector();
        }

        public uint GetEndLBA()
        {
            return (uint)GetEndCylinder() * GetEndHead() * GetEndSector();
        }

        public uint GetStartBytes()
        {
            return GetStartLBA()*512;
        }

        public uint GetEndBytes()
        {
            return GetStartLBA() * 512;            
        }
    }
}