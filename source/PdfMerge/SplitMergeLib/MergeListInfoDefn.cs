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
// File: MergeListInfoDefn.cs
// Description: Definition of a merge item
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
//   1.0 Dec 22/2017 C. Van Lingen  <V2.00> Moved to separate class
// =============================================================================

namespace PdfMerge.SplitMergeLib
{
    public class MergeListInfoDefn : System.ICloneable
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
            this.OutFilename = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "merged.pdf");
            this.PaginationFormat = PaginationFormatting.PaginationFormats.PF_Page_1_of_N;
        }

        public PaginationFormatting.PaginationFormats PaginationFormat { get; set; }

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

        public static bool Compare(MergeListInfoDefn m1, MergeListInfoDefn m2)
        {
            if (m1.Annotation != m2.Annotation)
            {
                return false;
            }

            if (m1.HasInfo != m2.HasInfo)
            {
                return false;
            }

            if (m1.InfoAuthor != m2.InfoAuthor)
            {
                return false;
            }

            if (m1.InfoSubject != m2.InfoSubject)
            {
                return false;
            }

            if (m1.InfoTitle != m2.InfoTitle)
            {
                return false;
            }

            if (m1.OutFilename != m2.OutFilename)
            {
                return false;
            }

            if (m1.PaginationFormat != m2.PaginationFormat)
            {
                return false;
            }

            if (m1.StartPage != m2.StartPage)
            {
                return false;
            }

            if (m1.NumberPages != m2.NumberPages)
            {
                return false;
            }

            return true;
        }

        public object Clone()
        {
            MergeListInfoDefn clone = new MergeListInfoDefn();
            clone.Annotation = this.Annotation;
            clone.HasInfo = this.HasInfo;
            clone.InfoAuthor = this.InfoAuthor;
            clone.InfoSubject = this.InfoSubject;
            clone.InfoTitle = this.InfoTitle;
            clone.OutFilename = this.OutFilename;
            clone.PaginationFormat = this.PaginationFormat;
            clone.StartPage = this.StartPage;
            clone.NumberPages = this.NumberPages;
            return clone;
        }
    }
}
