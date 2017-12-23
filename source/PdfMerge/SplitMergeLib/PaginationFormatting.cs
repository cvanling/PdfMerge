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
// File: PaginationFormatting.cs
// Description: Pagination formats
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
// =============================================================================
//
// Revision History:
//
//   1.0 Dec 20/2017 C. Van Lingen  <V2.00> Initial release
// =============================================================================
namespace PdfMerge.SplitMergeLib
{
    public class PaginationFormatting
    {
        public enum PaginationFormats
        {
            PF_Page_1_of_N = 0,
            PF_1_fwdslash_N = 1,
            PF_Page_1 = 2,
            PF_1 = 3
        }

        public static string GetFormattedPageString(int startPage, int page, int pageCount, PaginationFormats pageFmt)
        {
            if (startPage == 1)
            {
                switch (pageFmt)
                {
                    case PaginationFormats.PF_Page_1_of_N:
                        return string.Format("Page {0} of {1}", page, pageCount);
                    case PaginationFormats.PF_1_fwdslash_N:
                        return string.Format("{0} / {1}", page, pageCount);
                    case PaginationFormats.PF_Page_1:
                        return string.Format("Page {0}", page);
                    case PaginationFormats.PF_1:
                        return string.Format("{0}", page);
                    default:
                        throw new System.Exception("Bad pagination format");
                }
            }
            else
            {
                switch (pageFmt)
                {
                    case PaginationFormats.PF_Page_1_of_N:
                    case PaginationFormats.PF_Page_1:
                        return string.Format("Page {0}", page);
                    case PaginationFormats.PF_1_fwdslash_N:
                    case PaginationFormats.PF_1:
                        return string.Format("{0}", page);
                    default:
                        throw new System.Exception("Bad pagination format");
                }
            }
        }
    }
}
