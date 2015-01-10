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

namespace DynamicDevices.DiskWriter
{
    internal interface IDiskAccess
    {
        event TextHandler OnLogMsg;

        event ProgressHandler OnProgress;

        Handle Open(string drivePath);

        bool LockDrive(string drivePath);

        void UnlockDrive();

        int Read(byte[] buffer, int readMaxLength, out int readBytes);

        int Write(byte[] buffer, int bytesToWrite, out int wroteBytes);

        void Close();

        string GetPhysicalPathForLogicalPath(string logicalPath);

        long GetDriveSize(string drivePath);
    }
}
