//=============================================================================
// Project: PdfMerge - An Open Source Pdf Splitter/Merger with bookmark 
// importing. 
//
// Uses PdfSharp library (http://sourceforge.net/projects/pdfsharp).
//
// Also uses version 4.1.6 of the iTextSharp library 
// (http://itextsharp.svn.sourceforge.net/viewvc/itextsharp/tags/iTextSharp_4_1_6/)
// iTextSharp is included as an unmodified DLL under the LGPL terms.  
// See the readme.doc file included with this package.
//=============================================================================
// File: AssemblyInfo.cs
// Description: Assembly info
//=============================================================================
// Authors:
//   Charles Van Lingen <mailto:charles.vanlingen@gmail.com>
//
// Copyright (c) 2006-2018 Charles Van Lingen
//
// http://sourceforge.net/projects/pdfmerge
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
// Revision History:
//
//   1.9 Nov  7/2012 C. Van Lingen  <V22> Updated documentation.
//   1.8 Oct 21/2012 C. Van Lingen  <V21> Added right click context menu for
//                                  copying filenames to bookmarks and duplicating
//                                  lines.  Added tooltips.
//   1.7 Oct  7/2012 C. Van Lingen  <V20> Migrated to PdfSharp 1.32
//                                  Added use of CompatiblePdfReader based
//                                  on iTextSharp DLL
//                                  Added pagination and annotation
//   1.6 Mar 15/2009 C. Van Lingen  <V19> Migrated to PdfSharp 1.20
//                                  Various GUI updates
//   1.5 Jul 30/2008 C. Van Lingen  (V18) Added XML command file support and improved
//                                  GUI
//   1.4 Jan  1/2008 C. Van Lingen  (V17) Replaced merge tool with PDF sharp 
//                                  (handles up to version 1.6 PDF formats)
//   1.3 Jul 23/2007 C. Van Lingen  (V16) Change to deal with bookmark format produced by Adlib software
//                                  used in CADM to apply watermarks to front cover
//   1.2 Dec  7/2006 C. Van Lingen  (V15) Improved regx pattern match for goto lines  
//                                  (was missing bookmarks after pdftk reformat)
//                                  Improved histmerge dialog to resize better
//                                  Fixed issues with not handling spaces in attachment filenames
//   1.2 Oct 17/2006 C. Van Lingen  (V14) Added use of PdfTk to preprocess for multiple version tags (V14)
//                                  (Previous fix was not adequate)
//   1.1 Mar  8/2006 C. Van Lingen  Updated to support generations and multiple 
//                                  levels of bookmark from a file (V13)
//   1.0 Jan 28/2006 C. Van Lingen  Added bookmark code (V12) and specific 
//                                  CodeOne integrations
//
//=============================================================================

using System.Reflection;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("PdfMerge")]
[assembly: AssemblyDescription("Open Source PDF Splitter And Merger")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("PdfMerge")]
[assembly: AssemblyProduct("PdfMerge for CodeOne\nWritten by Charles Van Lingen\n\nUsing PdfSharp library from Empira Software - www.pdfsharp.com\n\nAlso using version 4.1.6 of the iTextSharp DLL\n\nSee included readme.doc for license terms.\n")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("2.0.*")]

//
// In order to sign your assembly you must specify a key to use. Refer to the 
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below to control which key is used for signing. 
//
// Notes: 
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers to a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers to a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the 
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key 
//           in the KeyFile is installed into the CSP and used.
//   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative to the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile 
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
