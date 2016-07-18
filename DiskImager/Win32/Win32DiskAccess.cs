﻿/**
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
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace DynamicDevices.DiskWriter.Win32
{
    internal class Win32DiskAccess
    {
        #region Fields

        SafeFileHandle _partitionHandle;
        SafeFileHandle _diskHandle;

        #endregion

        #region Win32DiskAccess Members

        public event TextHandler OnLogMsg;

        public event ProgressHandler OnProgress;

        //check if I need it
        //public event EventHandler OnDiskChanged;

        public Handle Open(string drivePath)
        {
            int intOut;

            //
            // Now that we've dismounted the logical volume mounted on the removable drive we can open up the physical disk to write
            //
            var diskHandle = NativeMethods.CreateFile(drivePath, NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
            if (diskHandle.IsInvalid)
            {
                LogMsg(@"Failed to open device: " + Marshal.GetHRForLastWin32Error());
                return null;
            }

            var success = NativeMethods.DeviceIoControl(diskHandle, NativeMethods.FSCTL_LOCK_VOLUME, null, 0, null, 0, out intOut, IntPtr.Zero);
            if (!success)
            {
                LogMsg(@"Failed to lock device");
                diskHandle.Dispose();
                return null;
            }

            _diskHandle = diskHandle;

            return new Handle();
        }

        public bool LockDrive(string drivePath)
        {
            int intOut;

            //
            // Unmount partition (Todo: Note that we currently only handle unmounting of one partition, which is the usual case for SD Cards)
            //

            //
            // Open the volume
            //
            var partitionHandle = NativeMethods.CreateFile(@"\\.\" + drivePath, NativeMethods.GENERIC_READ, NativeMethods.FILE_SHARE_READ, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
            if (partitionHandle.IsInvalid)
            {
                LogMsg(@"Failed to open device");
                partitionHandle.Dispose();
                return false;
            }

            //
            // Lock it
            //
            var success = NativeMethods.DeviceIoControl(partitionHandle, NativeMethods.FSCTL_LOCK_VOLUME, null, 0, null, 0, out intOut, IntPtr.Zero);
            if (!success)
            {
                LogMsg(@"Failed to lock device");
                partitionHandle.Dispose();
                return false;
            }

            //
            // Dismount it
            //
            success = NativeMethods.DeviceIoControl(partitionHandle, NativeMethods.FSCTL_DISMOUNT_VOLUME, null, 0, null, 0, out intOut, IntPtr.Zero);
            if (!success)
            {
                LogMsg(@"Error dismounting volume: " + Marshal.GetHRForLastWin32Error());
                NativeMethods.DeviceIoControl(partitionHandle, NativeMethods.FSCTL_UNLOCK_VOLUME, null, 0, null, 0, out intOut, IntPtr.Zero);
                partitionHandle.Dispose();
                return false;
            }

            _partitionHandle = partitionHandle;

            return true;
        }


        public void UnlockDrive()
        {
            if(_partitionHandle != null)
            {
                _partitionHandle.Dispose();
                _partitionHandle = null;
            }
        }

        public bool UnmountDrive()
        {
            int intOut;

            var success = NativeMethods.DeviceIoControl(_partitionHandle, NativeMethods.IOCTL_STORAGE_EJECT_MEDIA, null, 0, null, 0, out intOut, IntPtr.Zero);
            if (!success)
            {
                LogMsg(@"Error dismounting volume: " + Marshal.GetHRForLastWin32Error());
                NativeMethods.DeviceIoControl(_partitionHandle, NativeMethods.FSCTL_UNLOCK_VOLUME, null, 0, null, 0, out intOut, IntPtr.Zero);
                _partitionHandle.Dispose();
                return false;
            }
            return true;
        }

        public int Read(byte[] buffer, int readMaxLength, out int readBytes)
        {
            readBytes = 0;

            if(_diskHandle == null)
                return -1;

            return NativeMethods.ReadFile(_diskHandle, buffer, readMaxLength, out readBytes, IntPtr.Zero);
        }

        public int Write(byte[] buffer, int bytesToWrite, out int wroteBytes)
        {
            wroteBytes = 0;
            if(_diskHandle == null)
                return -1;

            return NativeMethods.WriteFile(_diskHandle, buffer, bytesToWrite, out wroteBytes, IntPtr.Zero);
        }

        public void Close()
        {
            if (_diskHandle != null)
            {
                _diskHandle.Dispose();
                _diskHandle = null;
            }
        }

        public string GetPhysicalPathForLogicalPath(string logicalPath)
        {
            var diskIndex = -1;

            //
            // Now that we've dismounted the logical volume mounted on the removable drive we can open up the physical disk to write
            //
            var diskHandle = NativeMethods.CreateFile(@"\\.\" + logicalPath, NativeMethods.GENERIC_READ, NativeMethods.FILE_SHARE_READ, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
            if (diskHandle.IsInvalid)
            {
                LogMsg(@"Failed to open device: " + Marshal.GetHRForLastWin32Error());
                return null;
            }

            var vdeSize = Marshal.SizeOf(typeof(VolumeDiskExtents));
            var vdeBlob = Marshal.AllocHGlobal(vdeSize);
            uint numBytesRead = 0;

           var success = NativeMethods.DeviceIoControl(diskHandle, NativeMethods.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, IntPtr.Zero,
                                                    0, vdeBlob, (uint)vdeSize, ref numBytesRead, IntPtr.Zero);

            var vde = (VolumeDiskExtents)Marshal.PtrToStructure(vdeBlob, typeof(VolumeDiskExtents));
            if (success)
            {
                if (vde.NumberOfDiskExtents == 1)
                    diskIndex = vde.DiskExtent1.DiskNumber;
            }
            else
            {
                LogMsg(@"Failed get physical disk: " + Marshal.GetHRForLastWin32Error());
            }
            Marshal.FreeHGlobal(vdeBlob);

            diskHandle.Dispose();
            
            var path = "";
            if(diskIndex >= 0)
                path = @"\\.\PhysicalDrive" + diskIndex;

            return path;

        }

        public long GetDriveSize(string drivePath)
        {
            //
            // Now that we've dismounted the logical volume mounted on the removable drive we can open up the physical disk to write
            //
            var diskHandle = NativeMethods.CreateFile(drivePath, NativeMethods.GENERIC_WRITE, 0, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
            if (diskHandle.IsInvalid)
            {
                LogMsg( @"Failed to open device: " + Marshal.GetHRForLastWin32Error());
                return -1;
            }

            //
            // Get drive size (NOTE: that WMI and IOCTL_DISK_GET_DRIVE_GEOMETRY don't give us the right value so we do it this way)
            //
            long size = -1;

            var geometrySize = Marshal.SizeOf(typeof(DiskGeometryEx));
            var geometryBlob = Marshal.AllocHGlobal(geometrySize);
            uint numBytesRead = 0;

            var success = NativeMethods.DeviceIoControl(diskHandle, NativeMethods.IOCTL_DISK_GET_DRIVE_GEOMETRY_EX, IntPtr.Zero,
                                                    0, geometryBlob, (uint)geometrySize, ref numBytesRead, IntPtr.Zero);

            var geometry = (DiskGeometryEx)Marshal.PtrToStructure(geometryBlob, typeof(DiskGeometryEx));
            if (success)
                size = geometry.DiskSize;

            Marshal.FreeHGlobal(geometryBlob);

            diskHandle.Dispose();

            return size;
        }

        #endregion

        //private void Progress(int progressValue)
        //{
        //    if (OnProgress != null)
        //        OnProgress(this, progressValue);
        //}

        private void LogMsg(string msg)
        {
            if (OnLogMsg != null)
                OnLogMsg(this, msg);
        }

    }
}
