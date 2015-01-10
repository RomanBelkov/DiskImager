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
using System.Drawing;
using System.Reflection;

namespace DynamicDevices.DiskWriter
{
    internal class Utility
    {
        public static Icon GetAppIcon()
        {
            var fileName = Assembly.GetEntryAssembly().Location;

            var hLibrary = NativeMethods.LoadLibrary(fileName);
            if (!hLibrary.Equals(IntPtr.Zero))
            {
                var hIcon = NativeMethods.LoadIcon(hLibrary, "#32512");
                if (!hIcon.Equals(IntPtr.Zero))
                    return Icon.FromHandle(hIcon);
            }
            return null; //no icon was retrieved
        }
    }
}
