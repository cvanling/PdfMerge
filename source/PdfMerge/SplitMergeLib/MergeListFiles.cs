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
// File: MergeListFiles.cs
// Description: Definition for a single merge file entry
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
//   1.0 Dec 20/2017 C. Van Lingen  Initial release
//
// =============================================================================

namespace PdfMerge.SplitMergeLib
{
    using System;

    public class MergeListFiles : ICloneable
    {
        public MergeListFiles()
        {
            this.Path = null;
            this.Pages = "all";
            this.Bookmark = null;
            this.Level = 0;
            this.Include = true;
        }

        public string Path { get; set; }

        public string Pages { get; set; }

        public string Bookmark { get; set; }

        public int Level { get; set; }

        public bool Include { get; set; }

        /// <summary>
        /// Gets description of current operation for GUI display
        /// </summary>
        public string Descriptor
        {
            get
            {
                string s = string.Empty;

                // [path of pdf file];[page range];[import];[bookmark];[level]
                s += this.Path + ";";
                s += this.Pages + ";";
                if (this.Include == true)
                {
                    s += "include";
                }
                else
                {
                    s += "exclude";
                }

                if (this.Bookmark != null)
                {
                    s += ";" + this.Bookmark;
                    if (this.Level > 0)
                    {
                        s += ";" + this.Level.ToString();
                    }
                }

                return s;
            }
        }

        public static bool Compare(MergeListFiles m1, MergeListFiles m2)
        {
            if (m1.Path != m2.Path)
            {
                return false;
            }

            if (m1.Pages != m2.Pages)
            {
                return false;
            }

            if (m1.Bookmark != m2.Bookmark)
            {
                return false;
            }

            if (m1.Level != m2.Level)
            {
                return false;
            }

            if (m1.Include != m2.Include)
            {
                return false;
            }

            return true;
        }

        public object Clone()
        {
            MergeListFiles clone = new MergeListFiles();
            clone.Path = this.Path;
            clone.Pages = this.Pages;
            clone.Bookmark = this.Bookmark;
            clone.Level = this.Level;
            clone.Include = this.Include;
            return clone;
        }
    }
}
