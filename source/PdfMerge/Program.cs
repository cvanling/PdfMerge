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
// File: Program.cs
// Description: PdfMerge top level class
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
//   1.1 Jul 30/2008 C. Van Lingen  (V18) Added XML command file support and improved
//                                  GUI
//   1.0 Jan  1/2008 C. Van Lingen  (V17) Initial Release 
//
//=============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

using PdfMerge.SplitMergeLib;

namespace PdfMerge
{
    class Program
    {
        public static string mruRegKey = "SOFTWARE\\PDFMERGE\\PdfMerge";

        [STAThread]
        static void Main()
        {
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 2)
            {
            DoAgain1:
                SplitMergeCmdFile splitmerge = new SplitMergeCmdFile();
                string cmdfile = arguments[1];
                string outfile = arguments[2];
                String err = splitmerge.DoSplitMerge(cmdfile, outfile);
                if (err.Length > 0)
                {
                    DialogResult res = MessageBox.Show(err, "Error", MessageBoxButtons.RetryCancel);
                    if (res == DialogResult.Retry)
                        goto DoAgain1;
                }
                if (arguments.Length == 4)
                {
                    if (err.Length == 0 && arguments[3].ToUpper() == "/SHOWPDF")
                    {
                        string v = GetViewer();
                        if (v.Length == 0)
                        {
                            System.Diagnostics.Process.Start("\"" + outfile + "\"");
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(v, "\"" + outfile + "\"");
                        }
                    }
                }
                Application.Exit();
                return;
            }
            PdfMergeForm merge = new PdfMergeForm();
            Application.Run(merge);
        }

        public static string GetViewer()
        {
            string retval = LoadFromRegistry("PdfViewer");
            if (retval == null)
                retval = "";
            return retval;
        }

        public static void PdfViewerSelect()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Title = "Select Acrobat Viewer";
            openFileDlg.InitialDirectory = "c:\\program files";
            openFileDlg.Filter = "Acrobat Viewer files (*.exe)|*.exe";
            openFileDlg.FilterIndex = 1;

            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                string s = openFileDlg.FileName;
                SaveToRegistry("PdfViewer", openFileDlg.FileName);
            }
        }

        public static void SaveToRegistry(string key, string value)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(mruRegKey);
            if (regKey != null)
            {
                regKey.SetValue(key,value);
                regKey.Close();
            }
        }

        public static string LoadFromRegistry(string key)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(mruRegKey);
            string result = null;
            if (regKey != null)
            {
                result = (string)regKey.GetValue(key, "");
                regKey.Close();
            }
            if (result != null)
                if (result.Length == 0)
                    result = null;
            return result;
        }


    }
}
