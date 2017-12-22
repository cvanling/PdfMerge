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
// File: PdfBookmark.cs
// Description: Class for adding PDF bookmarks
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
//   1.0 Jan  1/2008 C. Van Lingen  (V1.17) Replaced merge tool with PDF sharp
//                                  (handles up to version 1.6 PDF formats)
// =============================================================================
namespace PdfMerge.SplitMergeLib
{
    using System;
    using System.Collections;
    using System.IO;

    /// <summary>
    /// Multi-level class to contain a bookmark tree list
    /// </summary>
    public class PdfBookmark
    {
        private static int objCount;

        private int level;
        private int pageIndex;
        private string title;

        private PdfBookmark parent;
        private PdfBookmark prev;
        private PdfBookmark next;

        private ArrayList bookmarkObjects;

        public PdfBookmark(int create_at_level)
        {
            this.level = create_at_level;
            this.bookmarkObjects = new ArrayList();
            if (this.level == 0)
            {
                objCount = 0;
            }
            else
            {
                ++objCount;
            }
        }

        public void Clear()
        {
            if (this.level != 0)
            {
                throw new Exception("Clear cannot be called at sublevel of bookmark class");
            }

            this.bookmarkObjects.Clear();
            this.bookmarkObjects = new ArrayList();
            objCount = 0;
        }

        public int GetCount()
        {
            return objCount;
        }

        public int AddBookmark(string title, int page, int add_at_level)
        {
            if (add_at_level == this.level)
            {
                // add to this object
                this.bookmarkObjects.Add(new PdfBookmark(this.level + 1));
                PdfBookmark mark = this.bookmarkObjects[this.bookmarkObjects.Count - 1] as PdfBookmark;
                mark.pageIndex = page - 1;
                mark.title = title.Replace(">", string.Empty);
                mark.title = mark.title.Replace("(", string.Empty);
                mark.title = mark.title.Replace(")", string.Empty);
                mark.parent = this;
                if (this.bookmarkObjects.Count > 1)
                {
                    mark.prev = this.bookmarkObjects[this.bookmarkObjects.Count - 2] as PdfBookmark;
                }

                if (mark.prev != null)
                {
                    mark.prev.next = mark;
                }
            }
            else
            {
                // add to a sub object
                if (this.bookmarkObjects.Count == 0)
                {
                    throw new Exception("Bookmark hierarchy is invalid");
                }

                PdfBookmark mark = this.bookmarkObjects[this.bookmarkObjects.Count - 1] as PdfBookmark;
                mark.AddBookmark(title, page, add_at_level);
            }

            return this.GetCount();
        }

        public void WriteToDoc(ref PdfSharp.Pdf.PdfOutline outline, ref PdfSharp.Pdf.PdfDocument pdfDoc, int startPage)
        {
            PdfSharp.Pdf.PdfOutline sub = null;
            if (this.title != null)
            {
                if (outline == null)
                {
                    sub = pdfDoc.Outlines.Add(this.title, pdfDoc.Pages[startPage + this.pageIndex]);
                }
                else
                {
                    sub = outline.Outlines.Add(this.title, pdfDoc.Pages[startPage + this.pageIndex]);
                }
            }

            // command all subobjects to write as well
            foreach (PdfBookmark bm in this.bookmarkObjects)
            {
                bm.WriteToDoc(ref sub, ref pdfDoc, startPage);
            }
        }
    }
}
