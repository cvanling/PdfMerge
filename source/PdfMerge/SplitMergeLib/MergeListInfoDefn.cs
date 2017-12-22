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
// File: SplitMergeCmdFile.cs
// Description: Top level class for merge/split from a command file
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
    public class MergeListInfoDefn
    {
        public MergeListInfoDefn()
        {
            this.HasInfo = false;
            this.InfoTitle = string.Empty;
            this.InfoSubject = string.Empty;
            this.InfoAuthor = string.Empty;
            this.StartPage = 1;
            this.NumberPages = false;
            this.Annotation = string.Empty;
            this.OutFilename = "merged.pdf";
        }

        public string InfoAuthor { get; set; }

        public string InfoTitle { get; set; }

        public string InfoSubject { get; set; }

        public string OutFilename { get; set; }

        public string Annotation { get; set; }

        public bool NumberPages { get; set; }

        public int StartPage { get; set; }

        public bool HasInfo { get; set; }

        /// <summary>
        /// Gets ascii representation
        /// </summary>
        public string Descriptor
        {
            get
            {
                string s = "[info];";
                if (this.InfoTitle.Length > 0)
                {
                    s += this.InfoTitle;
                    if (this.InfoSubject.Length > 0)
                    {
                        s += ";" + this.InfoSubject;
                        if (this.InfoAuthor.Length > 0)
                        {
                            s += ";" + this.InfoAuthor;
                        }
                    }
                }

                return s;
            }
        }
    }
}
