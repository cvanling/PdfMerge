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
// File: PdfSharpSplitterMerger.cs
// Description: Class to wrap PDFSharp functions for merging
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
//   1.3 Jan 27/2018 C. Van Lingen  <V2.01> Improvements to page list input
//                                  Added code to create file for unit testing
//   1.2 Dec 22/2017 C. Van Lingen  <V2.00> Added pagination formatting
//                                  Fixed issue with not all command file settings
//                                  used when opened from command line
//   1.1 Oct  7/2012 C. Van Lingen  <V1.20> Migrated to PdfSharp 1.32
//                                  Added use of CompatiblePdfReader based
//                                  on iTextSharp DLL
//                                  Added pagination and annotation
//   1.0 Jan  8/2008 C. Van Lingen  (V1.17) Replaced merge tool with PDF sharp
//                                  (handles up to version 1.6 PDF formats)
// =============================================================================
namespace PdfMerge.SplitMergeLib
{
    using System;
    using System.Windows.Forms;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.IO;

    public class PdfSharpSplitterMerger
    {
        private PdfDocument outputDocument = new PdfDocument();
        private PdfBookmark bm;
        private int lastAddedCount;
        private PdfDocument inputDocument;
        private int[] lastPageList;

        public PdfSharpSplitterMerger()
        {
            this.Subject = string.Empty;
            this.Author = Environment.UserName;
            this.Title = string.Empty;
            this.outputDocument = new PdfDocument();
            this.bm = new PdfBookmark(0);
            this.lastAddedCount = 0;
        }

        public string Subject { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int Add(string file_add)
        {
            // perform a garbage collection if processing large documents
            if (this.inputDocument != null && this.outputDocument.PageCount > 1000)
            {
                this.inputDocument = null;
                GC.Collect();
            }

            this.inputDocument = CompatiblePdfReader.Open(file_add, PdfDocumentOpenMode.Import);
            int count = this.inputDocument.PageCount;
            for (int idx = 0; idx < count; idx++)
            {
                this.outputDocument.AddPage(this.inputDocument.Pages[idx]);
            }

            // tbd - is this a good idea?
            if (this.inputDocument.Version > this.outputDocument.Version)
            {
                this.outputDocument.Version = this.inputDocument.Version;
            }

            this.lastPageList = null;

            this.lastAddedCount = count;
            return this.lastAddedCount;
        }

        public int Add(string file_add, int[] page_list)
        {
            // perform a garbage collection if processing large documents
            if (this.inputDocument != null && this.outputDocument.PageCount > 1000)
            {
                this.inputDocument = null;
                GC.Collect();
            }

            this.inputDocument = CompatiblePdfReader.Open(file_add, PdfDocumentOpenMode.Import);
            int count = this.inputDocument.PageCount;
            int added_count = 0;
            for (int idxInputPage = 0; idxInputPage < count; idxInputPage++)
            {
                for (int idxPageList = 0; idxPageList < page_list.Length; ++idxPageList)
                {
                    if (page_list[idxPageList] == idxInputPage)
                    {
                        this.outputDocument.AddPage(this.inputDocument.Pages[idxInputPage]);
                        ++added_count;
                    }
                }
            }

            // tbd - is this a good idea?
            if (this.inputDocument.Version > this.outputDocument.Version)
            {
                this.outputDocument.Version = this.inputDocument.Version;
            }

            this.lastPageList = page_list;
            if (added_count == count)
            {
                this.lastPageList = null;
            }

            this.lastAddedCount = added_count;
            return added_count;
        }

        public void Finish(string outfilename)
        {
            this.Finish(outfilename, null, false, 1, PaginationFormatting.PaginationFormats.PF_Page_1_of_N);
        }

        public void Finish(string outfilename, string annotate, bool numberPages, int startPageNum, PaginationFormatting.PaginationFormats paginationFormat)
        {
            this.outputDocument.Info.Author = this.Author;
            if (this.Subject.Length > 0)
            {
                this.outputDocument.Info.Subject = this.Subject;
            }

            if (this.Title.Length > 0)
            {
                this.outputDocument.Info.Title = this.Title;
            }

            PdfOutline outline = null;
            this.bm.WriteToDoc(ref outline, ref this.outputDocument, 0);

            #region Add Page Numbering

            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            XFont font = new XFont("Verdana", 9, XFontStyle.Bold, options);
            XStringFormat pageNumFormat = new XStringFormat();
            pageNumFormat.Alignment = XStringAlignment.Far;
            pageNumFormat.LineAlignment = XLineAlignment.Far;
            XStringFormat annotateFormat = new XStringFormat();
            annotateFormat.Alignment = XStringAlignment.Near;
            annotateFormat.LineAlignment = XLineAlignment.Far;
            XGraphics gfx;
            XRect box;

            int pageNumber = startPageNum;
            int pageCount = this.outputDocument.Pages.Count;
            foreach (PdfPage page in this.outputDocument.Pages)
            {
                // Get a graphics object for page
                gfx = XGraphics.FromPdfPage(page);

                if (numberPages == true)
                {
                    box = page.MediaBox.ToXRect();
                    box.Inflate(-30, -20);
                    string pageLabel = PaginationFormatting.GetFormattedPageString(
                        startPageNum,
                        pageNumber,
                        pageCount,
                        paginationFormat);

                    gfx.DrawString(
                      pageLabel,
                      font,
                      XBrushes.Black,
                      box,
                      pageNumFormat);
                }

                if (string.IsNullOrEmpty(annotate) == false)
                {
                    box = page.MediaBox.ToXRect();
                    box.Inflate(-30, -20);
                    gfx.DrawString(annotate, font, XBrushes.Black, box, annotateFormat);
                }

                ++pageNumber;
            }
            #endregion

            this.outputDocument.Save(outfilename);

            this.bm.Clear();
        }

        public int AddBookmarksFromFile(string rootTitle, int level, bool addBookmarksFromFiles, string warning_ref)
        {
            int bmCount = 0;

            if (this.inputDocument == null)
            {
                throw new Exception("Add document before adding bookmarks");
            }

            int page = this.outputDocument.PageCount - this.lastAddedCount + 1;

            string title = rootTitle;
            if (title != null)
            {
                if (title.Length == 0)
                {
                    title = null;
                }
            }

            try
            {
                // add bookmarks
                if (title != null)
                {
                    this.bm.AddBookmark(title, page, level);
                    ++bmCount;

                    if (addBookmarksFromFiles == true)
                    {
                        bmCount += this.DoAddBookmarksFromFile(ref this.bm, page, level + 1);
                    }
                }
                else
                {
                    if (addBookmarksFromFiles == true)
                    {
                        bmCount += this.DoAddBookmarksFromFile(ref this.bm, page, level); // level was 1??
                    }
                }

                // clean up and free memory
                if (addBookmarksFromFiles == true)
                {
                    if (page > 1000)
                    {
                        GC.Collect();
                    }
                }
            }
            catch (Exception err)
            {
                DialogResult res;
                if (err.Message.ToLower().StartsWith("warning") == true)
                {
                    res = MessageBox.Show(
                        err.Message + "\n\nOn :  " + warning_ref,
                        "Bookmark Warning",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    res = MessageBox.Show(
                        err.ToString() + "\n\nOn :  " + warning_ref,
                        "Bookmark Error",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);
                }

                if (res == DialogResult.Cancel)
                {
                    throw new Exception("Merge cancelled due to bookmark issue");
                }
            }

            return bmCount;
        }

        public int DoAddBookmarksFromFile(ref PdfBookmark bm, int page, int level)
        {
            // get the outline object
            PdfDictionary outline = (PdfDictionary)this.inputDocument.Internals.Catalog.Elements.GetObject("/Outlines");
            if (outline == null)
            {
                return 0;
            }

            PdfDictionary first = (PdfDictionary)outline.Elements.GetObject("/First");
            if (first == null)
            {
                return 0;
            }

            // add the bookmarks
            return this.AddSubBookmarksFromFile(ref bm, page, level, first);
        }

        public int AddSubBookmarksFromFile(ref PdfBookmark bm, int page, int level, PdfDictionary outline)
        {
            int bmCount = 0;

            PdfDictionary mark = outline;

            // get titles and pages of all bookmarks at this level
            while (mark != null)
            {
                int level_offset = 0;

                // get the title
                string title = mark.Elements.GetString("/Title");
                if (title == null)
                {
                    return bmCount;
                }

                // direct
                PdfObject dest = mark.Elements.GetObject("/Dest");

                // indirect
                if (dest == null)
                {
                    PdfDictionary @ref = (PdfDictionary)mark.Elements.GetObject("/A");
                    if (@ref != null)
                    {
                        // if null this is a blind bookmark
                        dest = (PdfArray)@ref.Elements.GetObject("/D");
                    }
                }

                // indirectly named (per 1.6 sample from acrobat website)
                // used for multiple bookmarks on same page
                if (dest == null)
                {
                    dest = this.GetNamedDestination(mark);
                }

                // add the bookmark. if it isn't a blind bookmark
                if (dest != null)
                {
                    PdfReference pref = null;
                    if (dest is PdfArray)
                    {
                        PdfArray pDest = (PdfArray)dest;
                        if (pDest.Elements[0] is PdfReference)
                        {
                            pref = (PdfReference)pDest.Elements[0];
                        }
                    }

                    if (pref != null)
                    {
                        // convert page to offset
                        int page_offset = -1;
                        for (int x = 0; x < this.inputDocument.Pages.Count; ++x)
                        {
                            PdfReference pageRef = (PdfReference)this.inputDocument.Pages.PagesArray.Elements[x];
                            if (pageRef == pref)
                            {
                                page_offset = x;
                                break;
                            }
                        }

                        // check if this page is being added or not
                        bool included = false;
                        if (this.lastPageList == null)
                        {
                            // null means all pages
                            included = true;
                        }
                        else
                        {
                            // check if page needs to be included
                            for (int x = 0; x < this.lastPageList.Length; ++x)
                            {
                                if (this.lastPageList[x] == page_offset)
                                {
                                    included = true;
                                    page_offset = x;
                                    break;
                                }
                            }
                        }

                        // to do - show a warning for bad bookmark?
                        // if (page_offset == -1)
                        //    throw new Exception(string.Format(
                        //        "Warning: Bookmark '{0}' refers to page object {1} which does not exist, bookmark will be ignored", title, pref.ObjectNumber));
                        if (page_offset != -1 && included == true)
                        {
                            bm.AddBookmark(title, page_offset + page, level);
                            ++bmCount;
                            level_offset = 1;
                        }
                    }
                }

                // if this bookmark has children, add them recursively
                PdfDictionary child = (PdfDictionary)mark.Elements.GetObject("/First");
                if (child != null)
                {
                    bmCount += this.AddSubBookmarksFromFile(ref bm, page, level + level_offset, child);
                }

                // get the next mark
                mark = (PdfDictionary)mark.Elements.GetObject("/Next");
            }

            return bmCount;
        }

        // indirectly named (per 1.6 sample from acrobat website)
        // used for multiple bookmarks on same page
        private PdfObject GetNamedDestination(PdfDictionary mark)
        {
            try
            {
                // get the name
                string name = mark.Elements.GetString("/Dest");
                if (name == null)
                {
                    return null;
                }

                // get destination
                PdfDictionary o1 = this.inputDocument.Internals.Catalog.Elements.GetDictionary("/Names");
                PdfDictionary o2 = o1.Elements.GetDictionary("/Dests");
                PdfArray o3 = o2.Elements.GetArray("/Kids");
                for (int ikid = 0; ikid < o3.Elements.Count; ++ikid)
                {
                    // single list
                    PdfDictionary o4 = (PdfDictionary)((PdfReference)o3.Elements[0]).Value;
                    PdfArray o5 = o4.Elements.GetArray("/Names");
                    if (o5 != null)
                    {
                        for (int iname = 0; iname < o5.Elements.Count; ++iname)
                        {
                            if (o5.Elements[iname].ToString() == name)
                            {
                                PdfDictionary o6 = (PdfDictionary)((PdfReference)o5.Elements[iname + 1]).Value;
                                return o6.Elements.GetObject("/D");
                            }
                        }
                    }

                    // double list
                    else
                    {
                        PdfArray o6 = o4.Elements.GetArray("/Kids");
                        for (int iname2 = 0; iname2 < o6.Elements.Count; ++iname2)
                        {
                            PdfDictionary o7 = (PdfDictionary)((PdfReference)o6.Elements[iname2]).Value;
                            PdfArray o8 = o7.Elements.GetArray("/Names");
                            for (int iname3 = 0; iname3 < o8.Elements.Count; ++iname3)
                            {
                                if (o8.Elements[iname3].ToString() == name)
                                {
                                    PdfDictionary o9 = (PdfDictionary)((PdfReference)o8.Elements[iname3 + 1]).Value;
                                    return o9.Elements.GetObject("/D");
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
