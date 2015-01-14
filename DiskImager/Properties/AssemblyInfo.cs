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

using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("DiskImager")]
#if DEBUG
[assembly: AssemblyDescription("Windows Disk Imager - reads/writes images to removable storage (DEBUG BUILD)")]
#else
[assembly: AssemblyDescription("Windows Disk Writer - reads/writes images to removable storage")]
#endif
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Dynamic Devices Ltd")]
[assembly: AssemblyProduct("DiskImager")]
[assembly: AssemblyCopyright("Copyright © Dynamic Devices 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3a86a357-2384-4264-8f2d-bdddabace819")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.2.1.*")]
[assembly: AssemblyFileVersion("1.2.1.0")]
