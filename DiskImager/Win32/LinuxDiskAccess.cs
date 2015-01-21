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

using System;

namespace DynamicDevices.DiskWriter.Win32
{
    internal class LinuxDiskAccess : IDiskAccess
    {
        #region IDiskAccess Members

        public event TextHandler OnLogMsg;

        public event ProgressHandler OnProgress;

        public Handle Open(string drivePath)
        {
            throw new NotImplementedException();
        }

        public bool LockDrive(string drivePath)
        {
            throw new NotImplementedException();
        }


        public void UnlockDrive()
        {
            throw new NotImplementedException();
        }

        public bool UnmountDrive()
        {
            throw new NotImplementedException();
        }

        public int Read(byte[] buffer, int readMaxLength, out int readBytes)
        {
            throw new NotImplementedException();
        }

        public int Write(byte[] buffer, int bytesToWrite, out int wroteBytes)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public string GetPhysicalPathForLogicalPath(string logicalPath)
        {
            throw new NotImplementedException();
        }

        public long GetDriveSize(string drivePath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
