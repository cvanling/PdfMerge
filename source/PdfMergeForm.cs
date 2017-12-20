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
// File: PdfMergeForm.cs
// Description: Main form for interactive merge definition
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
//   1.3 Oct 21/2012 C. Van Lingen  <V21> Added right click context menu for
//                                  copying filenames to bookmarks and duplicating
//                                  lines.  Added tooltips.
//   1.2 Oct  7/2012 C. Van Lingen  <V20> Migrated to PdfSharp 1.32
//                                  Added use of CompatiblePdfReader based
//                                  on iTextSharp DLL
//                                  Added pagination and annotation
//   1.1 Mar 15/2009 C. Van Lingen  <V19> Migrated to PdfSharp 1.20, Various GUI updates
//   1.0 Jul 29/2008 C. Van Lingen  <V18> Initial Release
//=============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Microsoft.Win32;
using JWC;

using PdfMerge.SplitMergeLib;


namespace PdfMerge
{

    public partial class PdfMergeForm : Form
    {
        SplitMergeCmdFile merger = new SplitMergeCmdFile();
        protected MruStripMenuInline mruMenu;
        string CurrentFile = "";
        List<ItemLevel> rowcolors = new List<ItemLevel>();

        public PdfMergeForm()
        {
            InitializeComponent();
            dataGrid.AllowDrop = true;

            mruMenu = new MruStripMenuInline(fileToolStripMenuItem1, recentToolStripMenuItem, new MruStripMenu.ClickedHandler(OnMruFile), Program.mruRegKey);

            // initialize item level array;
            rowcolors.Add(new ItemLevel(Color.White, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightGray, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightCoral, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightCyan, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightBlue, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LemonChiffon, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightPink, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightGreen, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightPink, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightSalmon, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightSeaGreen, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightSkyBlue, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightSlateGray, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightSteelBlue, rowcolors.Count));
            rowcolors.Add(new ItemLevel(Color.LightYellow, rowcolors.Count));

            while (rowcolors.Count < 21)
                rowcolors.Add(new ItemLevel(Color.LightCyan, rowcolors.Count));
        }

        private void OnMruFile(int number, String filename)
        {
            // open the selected command file
            if (File.Exists(filename) == false)
                return;
            merger.Load(filename);
            CurrentFile = filename;
            UpdateGrid();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            merger.MergeListFileArray = new List<MergeListFiles>();
            CurrentFile = "";
            UpdateGrid();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show a file open dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Command Files (*.xml;*.lst)|*.xml;*.lst|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            dlg.FileName = CurrentFile;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                merger.Load(dlg.FileName);
                mruMenu.AddFile(dlg.FileName);
                CurrentFile = dlg.FileName;
            }
            dlg.Dispose();
            UpdateGrid();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentFile == "")
                SaveAsToolStripMenuItem_Click(null, null);
            UpdateFromGrid();
            merger.Save(CurrentFile);
            UpdateGrid();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateFromGrid();

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Command Files (*.xml;*.lst)|*.xml;*.lst|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            dlg.FileName = CurrentFile;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                merger.Save(dlg.FileName);
                mruMenu.AddFile(dlg.FileName);
                CurrentFile = dlg.FileName;
            }
            dlg.Dispose();

            UpdateGrid();
        }

        private void UpdateGrid()
        {
            dataGrid.Rows.Clear();
            int ItemIndex = 0;

            foreach (MergeListFiles mergeitem in merger.MergeListFileArray)
            {
                ++ItemIndex;
                int r = dataGrid.Rows.Add();

                dataGrid.Rows[r].Cells[0].Value = rowcolors[mergeitem.Level].GetItemString(r);
                dataGrid.Rows[r].Cells[0].Style = rowcolors[mergeitem.Level].style;

                dataGrid.Rows[r].Cells[1].Value = mergeitem.Path;

                dataGrid.Rows[r].Cells[2].Value = mergeitem.Pages;

                if (mergeitem.Bookmark != null)
                    dataGrid.Rows[r].Cells[3].Value = mergeitem.Bookmark;

                DataGridViewCheckBoxCell check = (DataGridViewCheckBoxCell)dataGrid.Rows[r].Cells[4];
                check.Value = mergeitem.Include;

                DataGridViewComboBoxCell combocell = (DataGridViewComboBoxCell)dataGrid.Rows[r].Cells[5];
                combocell.Items.Add("Root");
                for (int x = 1; x < 20; ++x)
                    combocell.Items.Add("Level " + x.ToString());
                combocell.Value = combocell.Items[mergeitem.Level];
            }

            this.Text = "PdfMerge - " + CurrentFile;

            outBox.Text = merger.MergeListInfo.OutFilename;
            checkBoxNumberPages.Checked = merger.MergeListInfo.NumberPages;
            textBoxStartPage.Text = merger.MergeListInfo.StartPage.ToString();
            if (string.IsNullOrEmpty(merger.MergeListInfo.Annotation) == false)
                textBoxAnnotate.Text = merger.MergeListInfo.Annotation;
            else
                textBoxAnnotate.Text = "";
        }

        private void buttonPickFile_Click(object sender, EventArgs e)
        {
            UpdateFromGrid();

            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.DefaultExt = "pdf";
            openFileDlg.Filter = "PDF Documents (*.pdf)|*.pdf";
            openFileDlg.Multiselect = true;
            openFileDlg.ShowReadOnly = true;
            openFileDlg.Title = "Add PDF Files ...";
            openFileDlg.FilterIndex = 1;

            if (openFileDlg.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string file in openFileDlg.FileNames)
                {
                    MergeListFiles MergeListItem = new MergeListFiles();
                    if (merger.MergeListFileArray.Count > 0)
                        MergeListItem.Level = merger.MergeListFileArray[merger.MergeListFileArray.Count - 1].Level;
                    MergeListItem.Path = file;
                    merger.MergeListFileArray.Add(MergeListItem);
                }
            }
            UpdateGrid();
        }

        private void Merge_Click(object sender, EventArgs e)
        {
            UpdateFromGrid();

            int StartPageNum = 1;
            try
            {
                StartPageNum = int.Parse(textBoxStartPage.Text);
            }
            catch
            {
            }

            try
            {
                status.Visible = true;
                status.Show();
                status.Items.Clear();

                while (true)
                {
                    string err;
                    SplitMergeCmdFile splitmerge = new SplitMergeCmdFile();
                    err = merger.DoSplitMerge(null,
                        outBox.Text,
                        status,
                        StartPageNum,
                        checkBoxNumberPages.Checked,
                        textBoxAnnotate.Text);
                    if (err.Length == 0)
                        break;
                    DialogResult res = MessageBox.Show(err, "Error", MessageBoxButtons.RetryCancel);
                    if (res != DialogResult.Retry)
                        break;
                }
            }
            finally
            {
                status.Hide();
                status.Visible = false;
            }
        }

        private void ViewLast_Click(object sender, EventArgs e)
        {
            string v = Program.GetViewer();
            if (v.Length == 0)
                System.Diagnostics.Process.Start("\"" + outBox.Text + "\"");
            else
                System.Diagnostics.Process.Start(v, "\"" + outBox.Text + "\"");
        }

        private void UpdateFromGrid()
        {
            dataGrid.EndEdit();
            merger.MergeListFileArray = new List<MergeListFiles>();
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                MergeListFiles MergeItem = new MergeListFiles();

                MergeItem.Path = row.Cells[1].Value.ToString();

                MergeItem.Pages = row.Cells[2].Value.ToString();

                if (row.Cells[3].Value != null)
                    if (row.Cells[3].Value.ToString().Length > 0)
                        MergeItem.Bookmark = row.Cells[3].Value.ToString();

                DataGridViewCheckBoxCell check = (DataGridViewCheckBoxCell)row.Cells[4];
                if ((bool)check.Value == false)
                    MergeItem.Include = false;

                DataGridViewComboBoxCell combo = (DataGridViewComboBoxCell)row.Cells[5];
                MergeItem.Level = combo.Items.IndexOf(row.Cells[5].Value);

                merger.MergeListFileArray.Add(MergeItem);
            }

            merger.MergeListInfo.OutFilename = outBox.Text;
            merger.MergeListInfo.NumberPages = checkBoxNumberPages.Checked;
            try
            {
                merger.MergeListInfo.StartPage = int.Parse(textBoxStartPage.Text);
            }
            catch
            {
                merger.MergeListInfo.StartPage = 1;
            }
            merger.MergeListInfo.Annotation = textBoxAnnotate.Text;

        }

        private void PdfViewerSelect_Click(object sender, EventArgs e)
        {
            Program.PdfViewerSelect();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = Application.ProductName + "\nVersion:" + Application.ProductVersion;
            MessageBox.Show(this, s, "About PDF Merge", MessageBoxButtons.OK
                , MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private List<int> GetSelected()
        {
            List<int> Selected = new List<int>();
            foreach (DataGridViewRow row in dataGrid.SelectedRows)
                Selected.Add(row.Index);
            Selected.Sort();
            return Selected;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 0)
                return;

            UpdateFromGrid();

            // get selected
            List<int> Selected = GetSelected();

            // delete from bottom
            for (int sel_ind = Selected.Count - 1; sel_ind >= 0; --sel_ind)
            {
                int irow = Selected[sel_ind];
                merger.MergeListFileArray.RemoveAt(irow);
            }

            UpdateGrid();

            dataGrid.ClearSelection();
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 0)
                return;

            UpdateFromGrid();

            // get selected
            List<int> Selected = GetSelected();

            // ignore if first row selected
            if (Selected[0] == 0)
                return;

            // move up from top
            for (int sel_ind = 0; sel_ind < Selected.Count; ++sel_ind)
            {
                int irow = Selected[sel_ind];
                MergeListFiles SelectedItem = (MergeListFiles)merger.MergeListFileArray[irow].Clone();
                merger.MergeListFileArray.RemoveAt(irow);
                merger.MergeListFileArray.Insert(irow - 1, SelectedItem);
            }

            UpdateGrid();

            // restore selections
            dataGrid.ClearSelection();
            foreach (int irow in Selected)
                dataGrid.Rows[irow - 1].Selected = true;
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 0)
                return;

            UpdateFromGrid();

            // get selected
            List<int> Selected = GetSelected();

            // ignore if last row selected
            if (Selected[Selected.Count - 1] >= (dataGrid.Rows.Count - 1))
                return;

            // move up from bottom
            for (int sel_ind = Selected.Count - 1; sel_ind >= 0; --sel_ind)
            {
                int irow = Selected[sel_ind];
                MergeListFiles SelectedItem = (MergeListFiles)merger.MergeListFileArray[irow].Clone();
                merger.MergeListFileArray.RemoveAt(irow);
                merger.MergeListFileArray.Insert(irow + 1, SelectedItem);
            }

            UpdateGrid();

            // restore selections
            dataGrid.ClearSelection();
            foreach (int irow in Selected)
                dataGrid.Rows[irow + 1].Selected = true;
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 0)
                return;

            UpdateFromGrid();

            // get selected
            List<int> Selected = GetSelected();

            // ignore if last row selected
            //if (Selected[Selected.Count - 1] >= (dataGrid.Rows.Count - 1))
            //    return;

            string v = Program.GetViewer();

            // move up from bottom
            for (int sel_ind = Selected.Count - 1; sel_ind >= 0; --sel_ind)
            {
                int irow = Selected[sel_ind];
                MergeListFiles SelectedItem = (MergeListFiles)merger.MergeListFileArray[irow].Clone();

                string fn = SelectedItem.Path;
                if (v.Length == 0)
                {
                    System.Diagnostics.Process.Start("\"" + fn + "\"");
                }
                else
                {
                    System.Diagnostics.Process.Start(v, "\"" + fn + "\"");
                }
            }

        }

        private void PdfMergeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mruMenu.SaveToRegistry();
        }

        private void buttonSaveTo_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.FileName = outBox.Text;
            dlg.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                outBox.Text = dlg.FileName;
            }
            dlg.Dispose();
        }

        private void dataGrid_DragDrop(object sender, DragEventArgs e)
        {
            UpdateFromGrid();

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            foreach (string fn in files)
            {
                if (fn.ToLower().EndsWith(".pdf") == true)
                {
                    MergeListFiles MergeListItem = new MergeListFiles();
                    if (merger.MergeListFileArray.Count > 0)
                        MergeListItem.Level = merger.MergeListFileArray[merger.MergeListFileArray.Count - 1].Level;
                    MergeListItem.Path = fn;
                    merger.MergeListFileArray.Add(MergeListItem);
                }
            }
            UpdateGrid();
        }

        private void dataGrid_DragOver(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            bool Allow = false;
            foreach (string fn in files)
                if (fn.ToLower().EndsWith(".pdf") == true)
                    Allow = true;
            if (e.Data.GetData(DataFormats.FileDrop) != null && Allow == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateGrid();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // restore selections
            dataGrid.ClearSelection();
            foreach (DataGridViewRow row in dataGrid.Rows)
                row.Selected = true;
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://pdfmerge.wiki.sourceforge.net/");
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoForm infodlg = new InfoForm(ref merger.MergeListInfo);
            infodlg.ShowDialog();
        }

        private void dataGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGrid.CurrentCell.ColumnIndex == 5)
            {
                int r = dataGrid.CurrentRow.Index;
                DataGridViewComboBoxCell combo = (DataGridViewComboBoxCell)dataGrid.Rows[r].Cells[5];
                int Level = combo.Items.IndexOf(dataGrid.Rows[r].Cells[5].Value);
                dataGrid.Rows[r].Cells[0].Style = rowcolors[Level].style;
                dataGrid.Rows[r].Cells[0].Value = rowcolors[Level].GetItemString(r);
            }
        }

        private void copyFilenamesToBookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                try
                {
                    string path = row.Cells[1].Value.ToString();
                    string bookmark = Path.GetFileNameWithoutExtension(path);
                    row.Cells[3].Value = bookmark;
                }
                catch
                {
                }
            }
        }

        #region Right click context menu in grid
        private void DuplicateRow(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if (row.Selected == true)
                {
                    try
                    {
                        int sel = row.Index;
                        UpdateFromGrid();
                        MergeListFiles MergeListItem = merger.MergeListFileArray[row.Index];
                        merger.MergeListFileArray.Add(MergeListItem);
                        UpdateGrid();
                        dataGrid.Rows[sel].Selected = true;
                        return;
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void CopyFileNameToBookMark(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if (row.Selected == true)
                {
                    try
                    {
                        string path = row.Cells[1].Value.ToString();
                        string bookmark = Path.GetFileNameWithoutExtension(path);
                        row.Cells[3].Value = bookmark;
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void dataGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitTestInfo = dataGrid.HitTest(e.X, e.Y);
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell && hitTestInfo.ColumnIndex != 3)
                {
                    int rowindex = hitTestInfo.RowIndex;
                    foreach (DataGridViewRow row in dataGrid.Rows)
                        row.Selected = row.Index == rowindex;

                    ContextMenu cm = new ContextMenu();
                    cm.MenuItems.Add("Duplicate Row at End", new EventHandler(DuplicateRow));
                    cm.MenuItems.Add("Copy Filename to Bookmark", new EventHandler(CopyFileNameToBookMark));
                    cm.Show(dataGrid, new Point(e.X, e.Y));
                }
            }
        }
        #endregion

    }
        
            

    internal class ItemLevel
    {
        internal ItemLevel(Color color,int _index)
        {
            style.BackColor = color;
            style.SelectionBackColor = color;
            style.SelectionForeColor = Color.Black;
            index = _index;
        }

        internal DataGridViewCellStyle style = new DataGridViewCellStyle();
        int index=0;

        internal string GetItemString(int rowindex)
        {
                string s = "..................................";
                int ndots = index;
                if (ndots > 7)
                    ndots = 7;
                return s.Substring(0, ndots) + (rowindex + 1).ToString();
        }
    }

}
