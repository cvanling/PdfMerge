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
// File: PdfBookmark.cs
// Description: Class for adding PDF bookmarks
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
//   1.0 Jan  1/2008 C. Van Lingen  (V17) Replaced merge tool with PDF sharp 
//                                  (handles up to version 1.6 PDF formats)
//=============================================================================
using System;
using System.Collections;
using System.IO;

namespace PdfMerge.SplitMergeLib
{
	/// <summary>
	/// Multi-level class to contain a bookmark tree list
	/// </summary>
	public class PdfBookmark
	{
		protected static int outline_obj;
		protected static int obj_count;

		protected int level;
		protected int PageIndex;
		protected string Title;
		protected Stream PdfStream;

		protected PdfBookmark Parent;
		protected PdfBookmark Prev;
		protected PdfBookmark Next;

		protected ArrayList BookmarkObjects;

		public PdfBookmark(int create_at_level)
		{
			level = create_at_level;
			BookmarkObjects = new ArrayList();
			if (level==0) 
				obj_count=0;
			else 
				++obj_count;
		}

		public void Clear()
		{
			if (level !=0)
				throw new Exception("Clear cannot be called at sublevel of bookmark class");
			BookmarkObjects.Clear();
			BookmarkObjects = new ArrayList();
			obj_count=0;
		}

		public int GetCount()
		{
			return obj_count;
		}

		public int AddBookmark(string Title, int page, int add_at_level)
		{
			if (add_at_level==level) 
			{
				// add to this object
				BookmarkObjects.Add(new PdfBookmark(level+1));
				PdfBookmark mark = BookmarkObjects[BookmarkObjects.Count-1] as PdfBookmark;
				mark.PageIndex=page-1;
				mark.Title=Title.Replace(">","");
				mark.Title=mark.Title.Replace("(","");
				mark.Title=mark.Title.Replace(")","");
				mark.Parent=this;
				if (BookmarkObjects.Count>1)
					mark.Prev=BookmarkObjects[BookmarkObjects.Count-2] as PdfBookmark;
				if (mark.Prev != null)
					mark.Prev.Next=mark;
			}
			else 
			{
				// add to a sub object
				if (BookmarkObjects.Count==0) 
					throw new Exception("Bookmark hierarchy is invalid");
				PdfBookmark mark = BookmarkObjects[BookmarkObjects.Count-1] as PdfBookmark;
				mark.AddBookmark(Title,page, add_at_level);
			}
			return GetCount();
		}

        public void WriteToDoc(ref PdfSharp.Pdf.PdfOutline outline, ref PdfSharp.Pdf.PdfDocument pdfDoc, int startPage)
        {
            PdfSharp.Pdf.PdfOutline sub = null;
            if (this.Title != null)
            {
                if (outline == null)
                    sub = pdfDoc.Outlines.Add(this.Title, pdfDoc.Pages[startPage + this.PageIndex]);
                else
                    sub = outline.Outlines.Add(this.Title, pdfDoc.Pages[startPage + this.PageIndex]);
            }

            // command all subobjects to write as well
            foreach (PdfBookmark bm in this.BookmarkObjects)
                bm.WriteToDoc(ref sub, ref pdfDoc, startPage);
        }

	}
}
