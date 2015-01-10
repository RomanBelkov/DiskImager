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
    internal class Globals
    {
        private const int MAX_BUFFER_SIZE = 1 * 1024 * 1024;
        private const int DEFAULT_COMPRESSION_LEVEL = 3;

        private static int _maxBufferSize = MAX_BUFFER_SIZE;
        private static int _compressionLevel = DEFAULT_COMPRESSION_LEVEL;

        public static int MaxBufferSize { get { return _maxBufferSize; } set { _maxBufferSize = value;  } }

        public static int CompressionLevel { get { return _compressionLevel; } set { _compressionLevel = value;  } }
    }
}
