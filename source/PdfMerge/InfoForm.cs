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
// File: InfoForm.cs
// Description: Form to view/enter PDF annotations
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
//   1.0 Aug 2/2008 C. Van Lingen  <V1.18> Initial Release
//=============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PdfMerge.SplitMergeLib;

namespace PdfMerge
{
    public partial class InfoForm : Form
    {
        private MergeListInfoDefn info;

        public InfoForm(ref MergeListInfoDefn _info)
        {
            info = _info;
            InitializeComponent();
            if (info.HasInfo == true)
            {
                textBoxTitle.Text = info.Info_Title;
                textBoxSubject.Text = info.Info_Subject;
                textBoxAuthor.Text = info.Info_Author;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            info.Info_Title = textBoxTitle.Text;
            info.Info_Subject = textBoxSubject.Text;
            info.Info_Author = textBoxAuthor.Text;

            info.HasInfo = false;
            if (info.Info_Title.Length > 0)
                info.HasInfo = true;
            if (info.Info_Subject.Length > 0)
                info.HasInfo = true;
            if (info.Info_Author.Length > 0)
                info.HasInfo = true;

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
