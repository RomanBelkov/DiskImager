DiskImager
==========

A .NET Windows utility for reading / writing SD cards and USB devices.

Licensed under GNU General Public License 3.0 or later.
Some rights reserved. See LICENSE, AUTHORS.

The current release can be downloaded here: https://github.com/RomanBelkov/DiskImager/releases/latest

Utility was tested on Windows 7/8/8.1/8.1 Pro.

## Description ##
This utility is a .NET implementation of Win32DiskImager with a couple of features authors wanted to use:

- writing images to a number of SD cards at once

- reads/writes images to/from compressed file formats: ZIP, TGZ, GZ, XZ

- remembers the last file you read/wrote 

- unmounts drives after write

- provides more file filters within file dialog for typical image files (.img, .bin, .sdcard)

- it also *might* be slightly faster when dealing with uncompressed read/write

*NOTE: This application is under development and could possibly cause damage to your computer drive(s). 
We cannot take responsibility for any damage caused or losses incurred through use of this utility. Use at own risk!*

Credits: Inspired by the excellent Win32DiskImager.

Contact
=======
Roman Belkov  - romanbelkov@gmail.com

Alex J Lennon - ajlennon@dynamicdevices.co.uk
