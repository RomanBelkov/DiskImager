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
    public enum EnumCompressionType
    {
        None = 0,
        Zip = 1,
        Gzip = 2,
        Targzip = 3
    }
}