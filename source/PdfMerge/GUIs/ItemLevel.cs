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
// File: ItemLevel.cs
// Description: Item level for interactive merge definition
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
//   1.0 Dec 22/2017 C. Van Lingen  <V2.00> Initial release
// =============================================================================
namespace PdfMerge
{
    using System.Drawing;
    using System.Windows.Forms;

    public class ItemLevel
    {
        internal ItemLevel(Color color, int levelIndex)
        {
            this.Style = new DataGridViewCellStyle();

            this.Style.BackColor = color;
            this.Style.SelectionBackColor = color;
            this.Style.SelectionForeColor = Color.Black;

            this.Index = levelIndex;
        }

        public DataGridViewCellStyle Style { get; set; }

        public int Index { get; set; }

        internal string GetItemString(int rowindex)
        {
            string s = "..................................";
            int ndots = this.Index;
            if (ndots > 7)
            {
                ndots = 7;
            }

            return s.Substring(0, ndots) + (rowindex + 1).ToString();
        }
    }
}
