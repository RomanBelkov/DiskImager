/**
 *  DiskImager - a tool for writing / reading images on SD cards
 *
 *  Copyright 2013, 2014 by Alex J. Lennon <ajlennon@dynamicdevices.co.uk>
 *
 *  Licensed under GNU General Public License 3.0 or later. 
 *  Some rights reserved. See LICENSE, AUTHORS.
 *
 * @license GPL-3.0+ <http://www.gnu.org/licenses/gpl-3.0.en.html>
 */

using System.Runtime.InteropServices;

namespace DynamicDevices.DiskWriter
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DiskGeometry
    {
        public long Cylinders;
        public int MediaType;
        public int TracksPerCylinder;
        public int SectorsPerTrack;
        public int BytesPerSector;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DiskGeometryEx
    {
        public DiskGeometry Geometry;
        public long DiskSize;
        public byte Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DISK_EXTENT
    {
        public  int DiskNumber;
        public ulong StartingOffset;
        public ulong ExtentLength;
    } 

    [StructLayout(LayoutKind.Sequential)]
    internal struct VolumeDiskExtents
    {
        public uint NumberOfDiskExtents;
        public DISK_EXTENT DiskExtent1;
    }
}
