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
// File: PdfMergeForm.cs
// Description: Main form for interactive merge definition
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
//   1.4 Dec 20/2017 C. Van Lingen  <V2.00> Migrated to PdfSharp 1.50 beta 4c
//                                  Automatically rebuild filenames if possible
//                                  when files are moved to a different folder.
//                                  Fixed error handling when using View button
//                                  and file(s) are not found.
//                                  Added support for pagination formats.
//                                  Changed new command to clear out all fields.
//   1.3 Oct 21/2012 C. Van Lingen  <V1.21> Added right click context menu for
//                                  copying filenames to bookmarks and duplicating
//                                  lines.  Added tooltips.
//   1.2 Oct  7/2012 C. Van Lingen  <V1.20> Migrated to PdfSharp 1.32
//                                  Added use of CompatiblePdfReader based
//                                  on iTextSharp DLL
//                                  Added pagination and annotation
//   1.1 Mar 15/2009 C. Van Lingen  <V1.19> Migrated to PdfSharp 1.20, Various GUI updates
//   1.0 Jul 29/2008 C. Van Lingen  <V1.18> Initial Release
// =============================================================================
namespace PdfMerge
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using JWC;
    using PdfMerge.SplitMergeLib;

    public partial class PdfMergeForm : Form
    {
        private SplitMergeCmdFile merger = new SplitMergeCmdFile();
        private MruStripMenuInline mruMenu;
        private string currentFile = string.Empty;
        private List<ItemLevel> rowcolors = new List<ItemLevel>();

        public PdfMergeForm(string cmdFile, string outFile)
        {
            this.InitializeComponent();
            this.dataGrid.AllowDrop = true;

            this.mruMenu = new MruStripMenuInline(this.fileToolStripMenuItem1, this.recentToolStripMenuItem, new MruStripMenu.ClickedHandler(this.OnMruFile), Program.MruRegKey);

            // initialize item level array;
            this.rowcolors.Add(new ItemLevel(Color.White, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightGray, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightCoral, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightCyan, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightBlue, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LemonChiffon, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightPink, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightGreen, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightPink, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightSalmon, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightSeaGreen, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightSkyBlue, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightSlateGray, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightSteelBlue, this.rowcolors.Count));
            this.rowcolors.Add(new ItemLevel(Color.LightYellow, this.rowcolors.Count));

            while (this.rowcolors.Count < 21)
            {
                this.rowcolors.Add(new ItemLevel(Color.LightCyan, this.rowcolors.Count));
            }

            this.comboBoxPageNumFormat.Items.Clear();
            var values = Enum.GetValues(typeof(PaginationFormatting.PaginationFormats));
            foreach (var fmt in values)
            {
                string s = fmt.ToString();
                s = s.Replace("PF", string.Empty);
                s = s.Replace("_", " ");
                s = s.Replace("fwdslash", "/");
                this.comboBoxPageNumFormat.Items.Add(s);
            }

            this.PaginationFormat = PaginationFormatting.PaginationFormats.PF_Page_1_of_N;

            if (outFile != string.Empty)
            {
                this.outBox.Text = outFile;
            }

            if (cmdFile != string.Empty)
            {
                try
                {
                    this.merger.Load(cmdFile);
                    this.currentFile = cmdFile;
                    this.UpdateGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "File Load Error", MessageBoxButtons.OK);
                }
            }
        }

        private PaginationFormatting.PaginationFormats PaginationFormat
        {
            get
            {
                return (PaginationFormatting.PaginationFormats)this.comboBoxPageNumFormat.SelectedIndex;
            }

            set
            {
                this.comboBoxPageNumFormat.SelectedIndex = (int)value;
            }
        }

        private void OnMruFile(int number, string filename)
        {
            // open the selected command file
            if (File.Exists(filename) == false)
            {
                return;
            }

            this.merger.Load(filename);
            this.currentFile = filename;
            this.UpdateGrid();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.merger.MergeListFileArray = new List<MergeListFiles>();
            this.currentFile = string.Empty;
            this.merger = new SplitMergeCmdFile();
            this.UpdateGrid();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show a file open dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Command Files (*.xml;*.lst)|*.xml;*.lst|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            dlg.FileName = this.currentFile;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.merger.Load(dlg.FileName);
                this.mruMenu.AddFile(dlg.FileName);
                this.currentFile = dlg.FileName;
            }

            dlg.Dispose();
            this.UpdateGrid();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.currentFile == string.Empty)
            {
                this.SaveAsToolStripMenuItem_Click(null, null);
            }

            this.UpdateFromGrid();
            this.merger.Save(this.currentFile);
            this.UpdateGrid();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UpdateFromGrid();

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Command Files (*.xml;*.lst)|*.xml;*.lst|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            dlg.FileName = this.currentFile;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.merger.Save(dlg.FileName);
                this.mruMenu.AddFile(dlg.FileName);
                this.currentFile = dlg.FileName;
            }

            dlg.Dispose();

            this.UpdateGrid();
        }

        private void UpdateGrid()
        {
            this.dataGrid.Rows.Clear();
            int itemIndex = 0;

            foreach (MergeListFiles mergeitem in this.merger.MergeListFileArray)
            {
                ++itemIndex;
                int r = this.dataGrid.Rows.Add();

                this.dataGrid.Rows[r].Cells[0].Value = this.rowcolors[mergeitem.Level].GetItemString(r);
                this.dataGrid.Rows[r].Cells[0].Style = this.rowcolors[mergeitem.Level].Style;

                this.dataGrid.Rows[r].Cells[1].Value = mergeitem.Path;

                this.dataGrid.Rows[r].Cells[2].Value = mergeitem.Pages;

                if (mergeitem.Bookmark != null)
                {
                    this.dataGrid.Rows[r].Cells[3].Value = mergeitem.Bookmark;
                }

                DataGridViewCheckBoxCell check = (DataGridViewCheckBoxCell)this.dataGrid.Rows[r].Cells[4];
                check.Value = mergeitem.Include;

                DataGridViewComboBoxCell combocell = (DataGridViewComboBoxCell)this.dataGrid.Rows[r].Cells[5];
                combocell.Items.Add("Root");
                for (int x = 1; x < 20; ++x)
                {
                    combocell.Items.Add("Level " + x.ToString());
                }

                combocell.Value = combocell.Items[mergeitem.Level];
            }

            this.Text = "PdfMerge - " + this.currentFile;

            this.outBox.Text = this.merger.MergeListInfo.OutFilename;
            this.checkBoxNumberPages.Checked = this.merger.MergeListInfo.NumberPages;
            this.textBoxStartPage.Text = this.merger.MergeListInfo.StartPage.ToString();
            if (string.IsNullOrEmpty(this.merger.MergeListInfo.Annotation) == false)
            {
                this.textBoxAnnotate.Text = this.merger.MergeListInfo.Annotation;
            }
            else
            {
                this.textBoxAnnotate.Text = string.Empty;
            }

            this.PaginationFormat = this.merger.MergeListInfo.PaginationFormat;
        }

        private void ButtonPickFile_Click(object sender, EventArgs e)
        {
            this.UpdateFromGrid();

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
                    MergeListFiles mergeListItem = new MergeListFiles();
                    if (this.merger.MergeListFileArray.Count > 0)
                    {
                        mergeListItem.Level = this.merger.MergeListFileArray[this.merger.MergeListFileArray.Count - 1].Level;
                    }

                    mergeListItem.Path = file;
                    this.merger.MergeListFileArray.Add(mergeListItem);
                }
            }

            this.UpdateGrid();
        }

        private void Merge_Click(object sender, EventArgs e)
        {
            this.UpdateFromGrid();

            int startPageNum = 1;
            try
            {
                startPageNum = int.Parse(this.textBoxStartPage.Text);
            }
            catch
            {
            }

            try
            {
                this.status.Visible = true;
                this.status.Show();
                this.status.Items.Clear();

                while (true)
                {
                    string err;
                    SplitMergeCmdFile splitmerge = new SplitMergeCmdFile();
                    err = this.merger.DoSplitMerge(
                        null,
                        this.outBox.Text,
                        this.status,
                        startPageNum,
                        this.checkBoxNumberPages.Checked,
                        this.textBoxAnnotate.Text,
                        this.PaginationFormat);
                    if (err.Length == 0)
                    {
                        break;
                    }

                    DialogResult res = MessageBox.Show(err, "Error", MessageBoxButtons.RetryCancel);
                    if (res != DialogResult.Retry)
                    {
                        break;
                    }
                }
            }
            finally
            {
                this.status.Hide();
                this.status.Visible = false;
            }
        }

        private void ViewLast_Click(object sender, EventArgs e)
        {
            string v = Program.GetViewer();
            if (v.Length == 0)
            {
                System.Diagnostics.Process.Start("\"" + this.outBox.Text + "\"");
            }
            else
            {
                System.Diagnostics.Process.Start(v, "\"" + this.outBox.Text + "\"");
            }
        }

        private void UpdateFromGrid()
        {
            this.dataGrid.EndEdit();
            this.merger.MergeListFileArray = new List<MergeListFiles>();
            foreach (DataGridViewRow row in this.dataGrid.Rows)
            {
                MergeListFiles mergeItem = new MergeListFiles();

                mergeItem.Path = row.Cells[1].Value.ToString();

                mergeItem.Pages = row.Cells[2].Value.ToString();

                if (row.Cells[3].Value != null)
                {
                    if (row.Cells[3].Value.ToString().Length > 0)
                    {
                        mergeItem.Bookmark = row.Cells[3].Value.ToString();
                    }
                }

                DataGridViewCheckBoxCell check = (DataGridViewCheckBoxCell)row.Cells[4];
                if ((bool)check.Value == false)
                {
                    mergeItem.Include = false;
                }

                DataGridViewComboBoxCell combo = (DataGridViewComboBoxCell)row.Cells[5];
                mergeItem.Level = combo.Items.IndexOf(row.Cells[5].Value);

                this.merger.MergeListFileArray.Add(mergeItem);
            }

            this.merger.MergeListInfo.OutFilename = this.outBox.Text;
            this.merger.MergeListInfo.NumberPages = this.checkBoxNumberPages.Checked;
            try
            {
                this.merger.MergeListInfo.StartPage = int.Parse(this.textBoxStartPage.Text);
            }
            catch
            {
                this.merger.MergeListInfo.StartPage = 1;
            }

            this.merger.MergeListInfo.Annotation = this.textBoxAnnotate.Text;

            this.merger.MergeListInfo.PaginationFormat = this.PaginationFormat;
        }

        private void PdfViewerSelect_Click(object sender, EventArgs e)
        {
            Program.PdfViewerSelect();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = Application.ProductName + "\nVersion:" + Application.ProductVersion;
            MessageBox.Show(
                this,
                s,
                "About PDF Merge",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private List<int> GetSelected()
        {
            List<int> selected = new List<int>();
            foreach (DataGridViewRow row in this.dataGrid.SelectedRows)
            {
                selected.Add(row.Index);
            }

            selected.Sort();
            return selected;
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            if (this.dataGrid.SelectedRows.Count == 0)
            {
                return;
            }

            this.UpdateFromGrid();

            // get selected
            List<int> selected = this.GetSelected();

            // delete from bottom
            for (int sel_ind = selected.Count - 1; sel_ind >= 0; --sel_ind)
            {
                int irow = selected[sel_ind];
                this.merger.MergeListFileArray.RemoveAt(irow);
            }

            this.UpdateGrid();

            this.dataGrid.ClearSelection();
        }

        private void ButtonUp_Click(object sender, EventArgs e)
        {
            if (this.dataGrid.SelectedRows.Count == 0)
            {
                return;
            }

            this.UpdateFromGrid();

            // get selected
            List<int> selected = this.GetSelected();

            // ignore if first row selected
            if (selected[0] == 0)
            {
                return;
            }

            // move up from top
            for (int sel_ind = 0; sel_ind < selected.Count; ++sel_ind)
            {
                int irow = selected[sel_ind];
                MergeListFiles selectedItem = (MergeListFiles)this.merger.MergeListFileArray[irow].Clone();
                this.merger.MergeListFileArray.RemoveAt(irow);
                this.merger.MergeListFileArray.Insert(irow - 1, selectedItem);
            }

            this.UpdateGrid();

            // restore selections
            this.dataGrid.ClearSelection();
            foreach (int irow in selected)
            {
                this.dataGrid.Rows[irow - 1].Selected = true;
            }
        }

        private void ButtonDown_Click(object sender, EventArgs e)
        {
            if (this.dataGrid.SelectedRows.Count == 0)
            {
                return;
            }

            this.UpdateFromGrid();

            // get selected
            List<int> selected = this.GetSelected();

            // ignore if last row selected
            if (selected[selected.Count - 1] >= (this.dataGrid.Rows.Count - 1))
            {
                return;
            }

            // move up from bottom
            for (int sel_ind = selected.Count - 1; sel_ind >= 0; --sel_ind)
            {
                int irow = selected[sel_ind];
                MergeListFiles selectedItem = (MergeListFiles)this.merger.MergeListFileArray[irow].Clone();
                this.merger.MergeListFileArray.RemoveAt(irow);
                this.merger.MergeListFileArray.Insert(irow + 1, selectedItem);
            }

            this.UpdateGrid();

            // restore selections
            this.dataGrid.ClearSelection();
            foreach (int irow in selected)
            {
                this.dataGrid.Rows[irow + 1].Selected = true;
            }
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (this.dataGrid.SelectedRows.Count == 0)
            {
                return;
            }

            this.UpdateFromGrid();

            // get selected
            List<int> selected = this.GetSelected();

            // ignore if last row selected
            // if (Selected[Selected.Count - 1] >= (dataGrid.Rows.Count - 1))
            //    return;
            string v = Program.GetViewer();

            List<string> notFound = new List<string>();

            // move up from bottom
            for (int sel_ind = selected.Count - 1; sel_ind >= 0; --sel_ind)
            {
                int irow = selected[sel_ind];
                MergeListFiles selectedItem = (MergeListFiles)this.merger.MergeListFileArray[irow].Clone();

                string fn = selectedItem.Path;

                if (File.Exists(fn) == false)
                {
                    notFound.Add(fn);
                    continue;
                }

                if (v.Length == 0)
                {
                    System.Diagnostics.Process.Start("\"" + fn + "\"");
                }
                else
                {
                    System.Diagnostics.Process.Start(v, "\"" + fn + "\"");
                }
            }

            if (notFound.Count > 0)
            {
                string err = string.Format("{0} file(s) were not found:", notFound.Count);
                foreach (string fn in notFound)
                {
                    err += "\n" + fn;
                }

                MessageBox.Show(err, "Error", MessageBoxButtons.OK);
            }
        }

        private void PdfMergeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.mruMenu.SaveToRegistry();
        }

        private void ButtonSaveTo_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.FileName = this.outBox.Text;
            dlg.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.outBox.Text = dlg.FileName;
            }

            dlg.Dispose();
        }

        private void DataGrid_DragDrop(object sender, DragEventArgs e)
        {
            this.UpdateFromGrid();

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            foreach (string fn in files)
            {
                if (fn.ToLower().EndsWith(".pdf") == true)
                {
                    MergeListFiles mergeListItem = new MergeListFiles();
                    if (this.merger.MergeListFileArray.Count > 0)
                    {
                        mergeListItem.Level = this.merger.MergeListFileArray[this.merger.MergeListFileArray.Count - 1].Level;
                    }

                    mergeListItem.Path = fn;
                    this.merger.MergeListFileArray.Add(mergeListItem);
                }
            }

            this.UpdateGrid();
        }

        private void DataGrid_DragOver(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            bool allow = false;
            foreach (string fn in files)
            {
                if (fn.ToLower().EndsWith(".pdf") == true)
                {
                    allow = true;
                }
            }

            if (e.Data.GetData(DataFormats.FileDrop) != null && allow == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UpdateGrid();
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // restore selections
            this.dataGrid.ClearSelection();
            foreach (DataGridViewRow row in this.dataGrid.Rows)
            {
                row.Selected = true;
            }
        }

        private void ExitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string v = Program.GetViewer();
            string fn = Path.Combine(Application.StartupPath, "readme.pdf");

            if (v.Length == 0)
            {
                System.Diagnostics.Process.Start("\"" + fn + "\"");
            }
            else
            {
                System.Diagnostics.Process.Start(v, "\"" + fn + "\"");
            }
        }

        private void CustomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoForm infodlg = new InfoForm(this.merger.MergeListInfo);
            infodlg.ShowDialog();
        }

        private void DataGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dataGrid.CurrentCell.ColumnIndex == 5)
            {
                int r = this.dataGrid.CurrentRow.Index;
                DataGridViewComboBoxCell combo = (DataGridViewComboBoxCell)this.dataGrid.Rows[r].Cells[5];
                int level = combo.Items.IndexOf(this.dataGrid.Rows[r].Cells[5].Value);
                this.dataGrid.Rows[r].Cells[0].Style = this.rowcolors[level].Style;
                this.dataGrid.Rows[r].Cells[0].Value = this.rowcolors[level].GetItemString(r);
            }
        }

        private void CopyFilenamesToBookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.dataGrid.Rows)
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
            foreach (DataGridViewRow row in this.dataGrid.Rows)
            {
                if (row.Selected == true)
                {
                    try
                    {
                        int sel = row.Index;
                        this.UpdateFromGrid();
                        MergeListFiles mergeListItem = this.merger.MergeListFileArray[row.Index];
                        this.merger.MergeListFileArray.Add(mergeListItem);
                        this.UpdateGrid();
                        this.dataGrid.Rows[sel].Selected = true;
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
            foreach (DataGridViewRow row in this.dataGrid.Rows)
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

        private void DataGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitTestInfo = this.dataGrid.HitTest(e.X, e.Y);
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell && hitTestInfo.ColumnIndex != 3)
                {
                    int rowindex = hitTestInfo.RowIndex;
                    foreach (DataGridViewRow row in this.dataGrid.Rows)
                    {
                        row.Selected = row.Index == rowindex;
                    }

                    ContextMenu cm = new ContextMenu();
                    cm.MenuItems.Add("Duplicate Row at End", new EventHandler(this.DuplicateRow));
                    cm.MenuItems.Add("Copy Filename to Bookmark", new EventHandler(this.CopyFileNameToBookMark));
                    cm.Show(this.dataGrid, new Point(e.X, e.Y));
                }
            }
        }
        #endregion
    }
}
