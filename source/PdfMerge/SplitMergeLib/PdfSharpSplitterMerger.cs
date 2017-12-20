//=============================================================================
// Project: PdfMerge - An Open Source Pdf Splitter/Merger with bookmark 
// importing. 
//
// Uses PdfSharp library (http://www.pdfsharp.net).
//
// Also uses version 4.1.6 of the iTextSharp library 
// (http://itextsharp.svn.sourceforge.net/viewvc/itextsharp/tags/iTextSharp_4_1_6/)
// iTextSharp is included as an unmodified DLL used per the terms of the GNU LGPL and the Mozilla Public License.  
// See the readme.doc file included with this package.
//=============================================================================
// File: PdfSharpSplitterMerger.cs
// Description: Class to wrap PDFSharp functions for merging
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
//=============================================================================
//
// Revision History:
//
//   1.1 Oct  7/2012 C. Van Lingen  <V1.20> Migrated to PdfSharp 1.32
//                                  Added use of CompatiblePdfReader based
//                                  on iTextSharp DLL
//                                  Added pagination and annotation
//   1.0 Jan  8/2008 C. Van Lingen  (V1.17) Replaced merge tool with PDF sharp 
//                                  (handles up to version 1.6 PDF formats)
//=============================================================================
using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Windows.Forms;

using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf.Advanced;

namespace PdfMerge.SplitMergeLib
{
    public class PdfSharpSplitterMerger
    {
        public string subject;
        public string title;
        public string author;
        public PdfDocument outputDocument = new PdfDocument();
        private PdfBookmark bm;
        private int LastAddedCount;

        public PdfSharpSplitterMerger()
        {
            subject = "";
            author = Environment.UserName;
            title = "";
            outputDocument = new PdfDocument();
            bm = new PdfBookmark(0);
            LastAddedCount = 0;
        }

        PdfDocument inputDocument;
        public int Add(string file_add)
        {
            // perform a garbage collection if processing large documents
            if (inputDocument != null && outputDocument.PageCount > 1000)
            {
                inputDocument = null;
                GC.Collect();
            }

            inputDocument = CompatiblePdfReader.Open(file_add, PdfDocumentOpenMode.Import);
            int count = inputDocument.PageCount;
            for (int idx = 0; idx < count; idx++)
                outputDocument.AddPage(inputDocument.Pages[idx]);

            // tbd - is this a good idea?
            if (inputDocument.Version > outputDocument.Version)
                outputDocument.Version = inputDocument.Version;

            last_page_list = null;

            LastAddedCount = count;
            return LastAddedCount;
        }

        private int[] last_page_list;

        public int Add(string file_add, int[] page_list)
        {
            // perform a garbage collection if processing large documents
            if (inputDocument != null && outputDocument.PageCount > 1000)
            {
                inputDocument = null;
                GC.Collect();
            }

            inputDocument = CompatiblePdfReader.Open(file_add, PdfDocumentOpenMode.Import);
            int count = inputDocument.PageCount;
            int added_count = 0;
            for (int idxInputPage = 0; idxInputPage < count; idxInputPage++)
            {
                for (int idxPageList = 0; idxPageList < page_list.Length; ++idxPageList)
                {
                    if (page_list[idxPageList] == idxInputPage)
                    {
                        outputDocument.AddPage(inputDocument.Pages[idxInputPage]);
                        ++added_count;
                    }
                }
            }

            // tbd - is this a good idea?
            if (inputDocument.Version > outputDocument.Version)
                outputDocument.Version = inputDocument.Version;

            last_page_list = page_list;
            if (added_count == count)
                last_page_list = null;

            LastAddedCount = added_count;
            return added_count;
        }

        public void Finish(string outfilename)
        {
            Finish(outfilename, null, false, 1);
        }

        public void Finish(string outfilename, string Annotate, bool NumberPages, int StartPageNum)
        {
            outputDocument.Info.Author = author;
            if (subject.Length > 0)
                outputDocument.Info.Subject = subject;
            if (title.Length > 0)
                outputDocument.Info.Title = title;

            PdfOutline outline = null;
            bm.WriteToDoc(ref outline, ref outputDocument, 0);

            #region Add Page Numbering
            XFont font = new XFont("Verdana", 9, XFontStyle.Bold);
            XStringFormat PageNumFormat = new XStringFormat();
            PageNumFormat.Alignment = XStringAlignment.Far;
            PageNumFormat.LineAlignment = XLineAlignment.Far;
            XStringFormat AnnotateFormat = new XStringFormat();
            AnnotateFormat.Alignment = XStringAlignment.Near;
            AnnotateFormat.LineAlignment = XLineAlignment.Far;
            XGraphics gfx;
            XRect box;

            int PageNumber = StartPageNum;
            int PageCount = outputDocument.Pages.Count;
            foreach (PdfPage page in outputDocument.Pages)
            {
                // Get a graphics object for page
                gfx = XGraphics.FromPdfPage(page);

                if (NumberPages == true)
                {
                    box = page.MediaBox.ToXRect();
                    box.Inflate(-30, -20);
                    if (StartPageNum == 1)
                    {
                        gfx.DrawString(String.Format("Page {0} of {1}", PageNumber, PageCount),
                          font, XBrushes.Black, box, PageNumFormat);
                    }
                    else
                    {
                        gfx.DrawString(String.Format("Page {0}", PageNumber),
                          font, XBrushes.Black, box, PageNumFormat);
                    }
                }

                if (String.IsNullOrEmpty(Annotate) == false)
                {
                    box = page.MediaBox.ToXRect();
                    box.Inflate(-30, -20);
                    gfx.DrawString(Annotate, font, XBrushes.Black, box, AnnotateFormat);
                }

                ++PageNumber;

            }
            #endregion

            outputDocument.Save(outfilename);

            bm.Clear();
        }

        public void AddBookmarksFromFile(string RootTitle, int level, bool AddBookmarksFromFiles, string warning_ref)
        {
            if (inputDocument == null)
                throw new Exception("Add document before adding bookmarks");

            int page = outputDocument.PageCount - LastAddedCount + 1;

            string Title = RootTitle;
            if (Title != null)
                if (Title.Length == 0)
                    Title = null;

            try
            {
                // add bookmarks
                if (Title != null)
                {
                    bm.AddBookmark(Title, page, level);
                    if (AddBookmarksFromFiles == true)
                        DoAddBookmarksFromFile(ref bm, page, level + 1);
                }
                else
                {
                    if (AddBookmarksFromFiles == true)
                        DoAddBookmarksFromFile(ref bm, page, level); // level was 1??
                }

                // clean up and free memory
                if (AddBookmarksFromFiles == true)
                {
                    if (page > 1000)
                        GC.Collect();
                }
            }
            catch (Exception err)
            {
                DialogResult res;
                if (err.Message.ToLower().StartsWith("warning") == true)
                    res = MessageBox.Show(err.Message + "\n\nOn :  " + warning_ref,
                        "Bookmark Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                else
                    res = MessageBox.Show(err.ToString() + "\n\nOn :  " + warning_ref,
                        "Bookmark Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Cancel)
                    throw (new Exception("Merge cancelled due to bookmark issue"));
            }

        }

        public void DoAddBookmarksFromFile(ref PdfBookmark bm, int page, int level)
        {
            // get the outline object
            PdfDictionary outline = (PdfDictionary)inputDocument.Internals.Catalog.Elements.GetObject("/Outlines");
            if (outline == null)
                return;

            PdfDictionary first = (PdfDictionary)outline.Elements.GetObject("/First");
            if (first == null)
                return;

            // add the bookmarks
            AddSubBookmarksFromFile(ref bm, page, level, first);
        }

        public void AddSubBookmarksFromFile(ref PdfBookmark bm, int page, int level, PdfDictionary outline)
        {
            PdfDictionary mark = outline;

            // get titles and pages of all bookmarks at this level
            while (mark != null)
            {
                int level_offset = 0;

                // get the title
                string title = mark.Elements.GetString("/Title");
                if (title == null)
                    return;

                // direct
                PdfObject Dest = mark.Elements.GetObject("/Dest");

                // indirect
                if (Dest == null)
                {
                    PdfDictionary Ref = (PdfDictionary)mark.Elements.GetObject("/A");
                    if (Ref != null) // if null this is a blind bookmark
                        Dest = (PdfArray)Ref.Elements.GetObject("/D");
                }

                // indirectly named (per 1.6 sample from acrobat website)
                // used for multiple bookmarks on same page
                if (Dest == null)
                    Dest = GetNamedDestination(mark);

                // add the bookmark. if it isn't a blind bookmark
                if (Dest != null)
                {
                    PdfReference pref = null;
                    if (Dest is PdfArray)
                    {
                        PdfArray pDest = (PdfArray)Dest;
                        if (pDest.Elements[0] is PdfReference)
                            pref = (PdfReference)pDest.Elements[0];
                    }

                    if (pref != null)
                    {
                        // convert page to offset
                        int page_offset = -1;
                        for (int x = 0; x < inputDocument.Pages.Count; ++x)
                        {
                            PdfReference PageRef = (PdfReference)inputDocument.Pages.PagesArray.Elements[x];
                            if (PageRef == pref)
                            {
                                page_offset = x;
                                break;
                            }
                        }

                        // check if this page is being added or not
                        bool Included = false;
                        if (last_page_list == null) // null means all pages
                        {
                            Included = true;
                        }
                        else // check if page needs to be included
                        {
                            for (int x = 0; x < last_page_list.Length; ++x)
                            {
                                if (last_page_list[x] == page_offset)
                                {
                                    Included = true;
                                    page_offset = x;
                                    break;
                                }
                            }
                        }

                        // to do - show a warning for bad bookmark?
                        //if (page_offset == -1)
                        //    throw new Exception(string.Format(
                        //        "Warning: Bookmark '{0}' refers to page object {1} which does not exist, bookmark will be ignored", title, pref.ObjectNumber));

                        if (page_offset != -1 && Included == true)
                        {
                            bm.AddBookmark(title, page_offset + page, level);
                            level_offset = 1;
                        }
                    }
                }

                // if this bookmark has children, add them recursively
                PdfDictionary child = (PdfDictionary)mark.Elements.GetObject("/First");
                if (child != null)
                    AddSubBookmarksFromFile(ref bm, page, level + level_offset, child);

                // get the next mark
                mark = (PdfDictionary)mark.Elements.GetObject("/Next");
            }
        }

        // indirectly named (per 1.6 sample from acrobat website)
        // used for multiple bookmarks on same page
        PdfObject GetNamedDestination(PdfDictionary mark)
        {
            try
            {
                // get the name
                string name = mark.Elements.GetString("/Dest");
                if (name == null)
                    return null;

                // get destination
                PdfDictionary O1 = inputDocument.Internals.Catalog.Elements.GetDictionary("/Names");
                PdfDictionary O2 = O1.Elements.GetDictionary("/Dests");
                PdfArray O3 = O2.Elements.GetArray("/Kids");
                for (int ikid = 0; ikid < O3.Elements.Count; ++ikid)
                {
                    // single list
                    PdfDictionary O4 = (PdfDictionary)((PdfReference)O3.Elements[0]).Value;
                    PdfArray O5 = O4.Elements.GetArray("/Names");
                    if (O5 != null)
                    {
                        for (int iname = 0; iname < O5.Elements.Count; ++iname)
                            if (O5.Elements[iname].ToString() == name)
                            {
                                PdfDictionary O6 = (PdfDictionary)((PdfReference)O5.Elements[iname + 1]).Value;
                                return O6.Elements.GetObject("/D");
                            }
                    }
                    // double list
                    else
                    {
                        PdfArray O6 = O4.Elements.GetArray("/Kids");
                        for (int iname2 = 0; iname2 < O6.Elements.Count; ++iname2)
                        {
                            PdfDictionary O7 = (PdfDictionary)((PdfReference)O6.Elements[iname2]).Value;
                            PdfArray O8 = O7.Elements.GetArray("/Names");
                            for (int iname3 = 0; iname3 < O8.Elements.Count; ++iname3)
                                if (O8.Elements[iname3].ToString() == name)
                                {
                                    PdfDictionary O9 = (PdfDictionary)((PdfReference)O8.Elements[iname3 + 1]).Value;
                                    return O9.Elements.GetObject("/D");
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
