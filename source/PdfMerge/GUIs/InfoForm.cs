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
// File: InfoForm.cs
// Description: Form to view/enter PDF annotations
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
//   1.0 Aug 2/2008 C. Van Lingen  <V1.18> Initial Release
// =============================================================================
namespace PdfMerge
{
    using System;
    using System.Windows.Forms;
    using PdfMerge.SplitMergeLib;

    public partial class InfoForm : Form
    {
        private MergeListInfoDefn info;

        public InfoForm(MergeListInfoDefn info)
        {
            this.info = info;
            this.InitializeComponent();
            if (this.info.HasInfo == true)
            {
                this.textBoxTitle.Text = this.info.InfoTitle;
                this.textBoxSubject.Text = this.info.InfoSubject;
                this.textBoxAuthor.Text = this.info.InfoAuthor;
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.info.InfoTitle = this.textBoxTitle.Text;
            this.info.InfoSubject = this.textBoxSubject.Text;
            this.info.InfoAuthor = this.textBoxAuthor.Text;

            this.info.HasInfo = false;
            if (this.info.InfoTitle.Length > 0)
            {
                this.info.HasInfo = true;
            }

            if (this.info.InfoSubject.Length > 0)
            {
                this.info.HasInfo = true;
            }

            if (this.info.InfoAuthor.Length > 0)
            {
                this.info.HasInfo = true;
            }

            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
