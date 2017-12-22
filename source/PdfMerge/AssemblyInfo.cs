// =============================================================================
// Project: PdfMerge - An Open Source Pdf Splitter/Merger with bookmark
// importing.
//
// Uses PdfSharp library (http://www.pdfsharp.net).
//
// Also uses version 4.1.6 of the iTextSharp library
// (http://itextsharp.svn.sourceforge.net/viewvc/itextsharp/tags/iTextSharp_4_1_6/)
// iTextSharp is included as an unmodified DLL used per the terms of the GNU LGPL and the Mozilla Public License.
// See the readme.doc file included with this package.
// =============================================================================
// File: AssemblyInfo.cs
// Description: Assembly info
// =============================================================================
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
//   1.9 Nov  7/2012 C. Van Lingen  <V1.22> Updated documentation.
//   1.8 Oct 21/2012 C. Van Lingen  <V1.21> Added right click context menu for
//                                  copying filenames to bookmarks and duplicating
//                                  lines.  Added tooltips.
//   1.7 Oct  7/2012 C. Van Lingen  <V1.20> Migrated to PdfSharp 1.32
//                                  Added use of CompatiblePdfReader based
//                                  on iTextSharp DLL
//                                  Added pagination and annotation
//   1.6 Mar 15/2009 C. Van Lingen  <V1.19> Migrated to PdfSharp 1.20
//                                  Various GUI updates
//   1.5 Jul 30/2008 C. Van Lingen  (V1.18) Added XML command file support and improved
//                                  GUI
//   1.4 Jan  1/2008 C. Van Lingen  (V1.17) Replaced merge tool with PDF sharp
//                                  (handles up to version 1.6 PDF formats)
//   1.3 Jul 23/2007 C. Van Lingen  (V1.16) Change to deal with bookmark format produced by Adlib software
//                                  used in CADM to apply watermarks to front cover
//   1.2 Dec  7/2006 C. Van Lingen  (V1.15) Improved regx pattern match for goto lines
//                                  (was missing bookmarks after pdftk reformat)
//                                  Improved histmerge dialog to resize better
//                                  Fixed issues with not handling spaces in attachment filenames
//   1.2 Oct 17/2006 C. Van Lingen  (V1.14) Added use of PdfTk to preprocess for multiple version tags (V14)
//                                  (Previous fix was not adequate)
//   1.1 Mar  8/2006 C. Van Lingen  Updated to support generations and multiple
//                                  levels of bookmark from a file (V1.13)
//   1.0 Jan 28/2006 C. Van Lingen  Added bookmark code (V1.12) and specific
//                                  integrations
//
// =============================================================================

using System.Reflection;

[assembly: AssemblyTitle("PdfMerge")]
[assembly: AssemblyDescription("Open Source PDF Splitter And Merger")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("PdfMerge")]
[assembly: AssemblyProduct("PdfMerge\nWritten by Charles Van Lingen\n\nUsing PdfSharp library from Empira Software - www.pdfsharp.com\n\nAlso using version 4.1.6 of the iTextSharp DLL\n\nSee included readme.doc for license terms.\n")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("2.0.*")]
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
