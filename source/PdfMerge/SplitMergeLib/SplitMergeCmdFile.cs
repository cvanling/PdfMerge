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
// File: SplitMergeCmdFile.cs
// Description: Top level class for merge/split from a command file
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
//   1.5 Jan 27/2018 C. Van Lingen  <V2.01> Improvements to page list input
//                                  Added code to create file for unit testing
//
//   1.4 Dec 20/2017 C. Van Lingen  <V2.00> Migrated to PdfSharp 1.50 beta 4c
//                                  Automatically rebuild filenames if possible
//                                  when files are moved to a different folder
//
//   1.3 Oct  7/2012 C. Van Lingen  <V1.20> Migrated to PdfSharp 1.32
//                                  Added use of CompatiblePdfReader based
//                                  on iTextSharp DLL
//                                  Added pagination and annotation
//
//   1.2 Jul 25/2008 C. Van Lingen  <V1.18> Added XML command file support to allow
//                                  operation with unicode strings
//
//   1.1 Jan  1/2008 C. Van Lingen  (V1.17) Replaced merge tool with PDF sharp
//                                  (handles up to version 1.6 PDF formats)
//
//   1.0 Oct 17/2006 C. Van Lingen  Added use of PdfTk to preprocess for multiple version tags (V1.14)
//                                  (Previous fix was not adequate)
// =============================================================================

namespace PdfMerge.SplitMergeLib
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;
    using System.Text;

    /// <summary>
    /// *********************************************************************************************
    /// This class reads data from a text command file with the following format:
    ///
    /// [path of pdf file];[page range];[include/exclude];[bookmark];[level]
    ///
    /// An text command file example:
    /// d:\shell.pdf;1-4;include;TITLE
    /// d:\shell.pdf;5;exclude
    /// d:\Compliance.pdf;all;include;COMPLIANCE;1
    ///
    /// Notes:
    /// 1) The page range can be a single page number, an n-n range, or all.
    ///
    /// 2) Use include to import the bookmarks, exclude to not import bookmarks from
    /// the file.
    ///
    /// 3) The bookmark field is optional.  If present a bookmark will be added at the first page of the
    /// file being added.
    ///
    /// 4) The level field is optional.  This is used to arrange bookmarks in a hierarchy.  An exception
    /// will result if a level is skipped.
    ///
    /// *********************************************************************************************
    /// Alternatively this class will read from an XML command file with the following format:
    /// <?xml version="1.0" encoding="utf-16"?>
    /// <merge>
    ///    <file>
    ///        <path>d:\shell.pdf</path>
    ///        <pages>1-4</pages>
    ///        <bookmark>TITLE</bookmark>
    ///    </file>
    ///    <file exclude="1">
    ///        <path>d:\shell.pdf</path>
    ///        <pages>5</pages>
    ///    </file>
    ///    <file>
    ///        <path>d:\Compliance.pdf</path>
    ///        <bookmark>COMPLIANCE</bookmark>
    ///        <level>1</level>
    ///    </file>
    /// </merge>
    ///
    /// Notes:
    /// 1) The pages tag is optional if missing all pages are included. Page range can be a single page number,
    /// an n-n range, or all.
    ///
    /// 2) The bookmark tag is optional.  If present a bookmark will be added at the first page of the
    /// file being added.
    ///
    /// 3) The level tag is optional.  This is used to arrange bookmarks in a hierarchy.  An exception
    /// will result if a level is skipped.
    ///
    /// 4) The exclude attribute on the file tag is optional. If present and equal to 1 then the
    /// bookmarks from that file will not be imported.
    /// *********************************************************************************************
    /// </summary>
    public class SplitMergeCmdFile : System.ICloneable
    {
        public SplitMergeCmdFile()
        {
            this.MergeListFileArray = new List<MergeListFiles>();
            this.MergeListInfo = new MergeListInfoDefn();
        }

        public List<MergeListFiles> MergeListFileArray { get; set; }

        public MergeListInfoDefn MergeListInfo { get; set; }

        public static bool Compare(SplitMergeCmdFile m1, SplitMergeCmdFile m2)
        {
            if (MergeListInfoDefn.Compare(m1.MergeListInfo, m2.MergeListInfo) == false)
            {
                return false;
            }

            if (m1.MergeListFileArray.Count != m2.MergeListFileArray.Count)
            {
                return false;
            }

            for (int iFile = 0; iFile < m1.MergeListFileArray.Count; ++iFile)
            {
                if (MergeListFiles.Compare(m1.MergeListFileArray[iFile], m2.MergeListFileArray[iFile]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public string DoSplitMerge(string commandFilename, string outputFilename, ListBox lbStat, int startPageNumber, bool numberPages, string annotation, PaginationFormatting.PaginationFormats paginationFormat, bool createTestInfo = false)
        {
            PdfSharpSplitterMerger psm = null;
            int page = 1;
            int bmCount = 0;

            if (commandFilename != null)
            {
                try
                {
                    int fileMissingCount = 0;
                    if (commandFilename.ToLower().EndsWith(".xml"))
                    {
                        fileMissingCount = this.ReadXmlCommandFile(commandFilename);
                    }
                    else
                    {
                        fileMissingCount = this.ReadAsciiCommandFile(commandFilename);
                    }

                    if (fileMissingCount > 0)
                    {
                        this.ResolvePathsIfFoldersMoved(commandFilename, fileMissingCount);
                    }
                }
                catch (Exception err)
                {
                    return err.Message;
                }
            }

            string line = string.Empty;
            try
            {
                psm = new PdfSharpSplitterMerger();

                if (this.MergeListInfo.HasInfo == true)
                {
                    psm.Title = this.MergeListInfo.InfoTitle;
                    psm.Subject = this.MergeListInfo.InfoSubject;
                    psm.Author = this.MergeListInfo.InfoAuthor;
                }

                if (commandFilename != null)
                {
                    // get options from command file
                    numberPages = this.MergeListInfo.NumberPages;
                    annotation = this.MergeListInfo.Annotation;
                    paginationFormat = this.MergeListInfo.PaginationFormat;
                    startPageNumber = this.MergeListInfo.StartPage;
                }

                foreach (MergeListFiles merge in this.MergeListFileArray)
                {
                    line = merge.Descriptor;

                    string infile = merge.Path;
                    string pagelst = merge.Pages;

                    string file_add = infile;
                    if (lbStat != null)
                    {
                        lbStat.Items.Add(line);
                        lbStat.TopIndex = lbStat.Items.Count - 1;
                        lbStat.Refresh();
                    }

                    List<int> ps = this.ExtractPageList(pagelst);

                    // merge the input file
                    if (ps.Count > 0)
                    {
                        page += psm.Add(file_add, ps.ToArray());
                    }
                    else
                    {
                        page += psm.Add(file_add);
                    }

                    // add bookmarks
                    string rootTitle = merge.Bookmark;
                    int level = merge.Level;
                    bmCount += psm.AddBookmarksFromFile(rootTitle, level, merge.Include, line);
                }

                // this writes out the merged files
                psm.Finish(outputFilename, annotation, numberPages, startPageNumber, paginationFormat);

                if (createTestInfo)
                {
                    SortedDictionary<string, string> report = new SortedDictionary<string, string>();
                    report.Add("MergeListFileArrayCount", this.MergeListFileArray.Count.ToString());
                    report.Add("PageCount", (page-1).ToString());
                    report.Add("BookMarkCount", bmCount.ToString());
                    StringBuilder info = new StringBuilder();
                    foreach (KeyValuePair<string, string> kvp in report)
                    {
                        info.AppendFormat("{0}={1}\r\n", kvp.Key, kvp.Value);
                    }

                    File.WriteAllText(outputFilename + ".info", info.ToString());
                }
            }
            catch (Exception err)
            {
                psm = null;

                string retval;
                if (line != null)
                {
                    if (line.Length > 0)
                    {
                        retval = err.Message + "\nOn Line:\n" + line;
                    }
                    else
                    {
                        retval = err.Message;
                    }
                }
                else
                {
                    retval = err.Message;
                }

                return retval;
            }

            psm = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // return empty string indicating no error
            return string.Empty;
        }

        public void Load(string commandFilename)
        {
            try
            {
                int fileMissingCount = 0;
                if (commandFilename.ToLower().EndsWith(".xml"))
                {
                    fileMissingCount = this.ReadXmlCommandFile(commandFilename);
                }
                else
                {
                    fileMissingCount = this.ReadAsciiCommandFile(commandFilename);
                }

                if (fileMissingCount > 0)
                {
                    this.ResolvePathsIfFoldersMoved(commandFilename, fileMissingCount);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void Save(string commandFilename)
        {
            try
            {
                if (commandFilename.ToLower().EndsWith(".xml"))
                {
                    this.SaveXmlCommandFile(commandFilename);
                }
                else
                {
                    this.SaveAsciiCommandFile(commandFilename);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public object Clone()
        {
            SplitMergeCmdFile clone = new SplitMergeCmdFile();
            clone.MergeListInfo = (MergeListInfoDefn)this.MergeListInfo.Clone();
            clone.MergeListFileArray = new List<MergeListFiles>();
            foreach (var mergeListFile in this.MergeListFileArray)
            {
                clone.MergeListFileArray.Add((MergeListFiles)mergeListFile.Clone());
            }

            return clone;
        }

        private void SaveAsciiCommandFile(string filename)
        {
            if (this.MergeListFileArray.Count < 1)
            {
                return;
            }

            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (MergeListFiles mergeItem in this.MergeListFileArray)
                {
                    sw.WriteLine(mergeItem.Descriptor);
                }

                if (this.MergeListInfo.HasInfo == true)
                {
                    sw.WriteLine(this.MergeListInfo.Descriptor);
                }
            }
        }

        private int ReadAsciiCommandFile(string filename)
        {
            int fileMissingCount = 0;

            this.MergeListFileArray = new List<MergeListFiles>();
            this.MergeListInfo = new MergeListInfoDefn();

            string line = string.Empty;

            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] args = line.Split(new char[] { ';', '\r', '\n' });
                        if (args[0].ToLower() == "[info]")
                        {
                            this.MergeListInfo.HasInfo = true;
                            if (args.Length > 1)
                            {
                                this.MergeListInfo.InfoTitle = args[1];
                            }

                            if (args.Length > 2)
                            {
                                this.MergeListInfo.InfoSubject = args[2];
                            }

                            if (args.Length > 3)
                            {
                                this.MergeListInfo.InfoAuthor = args[3];
                            }
                        }
                        else
                        {
                            MergeListFiles mergeElement = new MergeListFiles();
                            mergeElement.Path = args[0];
                            mergeElement.Pages = args[1];
                            if (args.Length > 2)
                            {
                                if (args[2].ToUpper() == "EXCLUDE")
                                {
                                    mergeElement.Include = false;
                                }
                            }

                            if (args.Length > 3)
                            {
                                mergeElement.Bookmark = args[3];
                            }

                            if (args.Length > 4)
                            {
                                mergeElement.Level = int.Parse(args[4]);
                            }

                            this.MergeListFileArray.Add(mergeElement);
                            if (File.Exists(mergeElement.Path) == false)
                            {
                                ++fileMissingCount;
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                if (line != null)
                {
                    if (line.Length > 0)
                    {
                        throw new Exception(err.Message + "\nOn Line:\n" + line);
                    }
                }

                throw err;
            }

            return fileMissingCount;
        }

        private void SaveXmlCommandFile(string filename)
        {
            if (this.MergeListFileArray.Count < 1)
            {
                return;
            }

            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(filename, new System.Text.UnicodeEncoding());
            writer.Formatting = Formatting.Indented;

            writer.WriteStartDocument();

            writer.WriteStartElement("merge");

            // Write sub-elements
            foreach (MergeListFiles mergeItem in this.MergeListFileArray)
            {
                writer.WriteStartElement("file");
                if (mergeItem.Include == false)
                {
                    writer.WriteAttributeString("exclude", "1");
                }

                writer.WriteElementString("path", mergeItem.Path);
                writer.WriteElementString("pages", mergeItem.Pages);
                if (mergeItem.Bookmark != null)
                {
                    writer.WriteElementString("bookmark", mergeItem.Bookmark);
                }

                if (mergeItem.Level > 0)
                {
                    writer.WriteElementString("level", XmlConvert.ToString(mergeItem.Level));
                }

                writer.WriteEndElement();
            }

            #region write info and options
            if (this.MergeListInfo.HasInfo == true)
            {
                writer.WriteStartElement("info");
                if (this.MergeListInfo.InfoAuthor.Length > 0)
                {
                    writer.WriteElementString("author", this.MergeListInfo.InfoAuthor);
                }

                if (this.MergeListInfo.InfoSubject.Length > 0)
                {
                    writer.WriteElementString("subject", this.MergeListInfo.InfoSubject);
                }

                if (this.MergeListInfo.InfoTitle.Length > 0)
                {
                    writer.WriteElementString("title", this.MergeListInfo.InfoTitle);
                }

                writer.WriteEndElement();
            }

            writer.WriteStartElement("options");
            if (string.IsNullOrEmpty(this.MergeListInfo.OutFilename) == false)
            {
                writer.WriteElementString("outfile", this.MergeListInfo.OutFilename);
            }

            if (string.IsNullOrEmpty(this.MergeListInfo.Annotation) == false)
            {
                writer.WriteElementString("annotation", this.MergeListInfo.Annotation);
            }

            if (this.MergeListInfo.NumberPages == true)
            {
                writer.WriteElementString("startpage", this.MergeListInfo.StartPage.ToString());
            }

            writer.WriteElementString("paginationformat", ((int)this.MergeListInfo.PaginationFormat).ToString());

            writer.WriteEndElement();
            #endregion

            writer.WriteFullEndElement();

            writer.Close();
        }

        private int ReadXmlCommandFile(string filename)
        {
            string planFilePath = Path.GetDirectoryName(filename);

            this.MergeListFileArray = new List<MergeListFiles>();
            this.MergeListInfo = new MergeListInfoDefn();

            MergeListFiles mergeElement = null;

            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(filename);

            int fileNotFoundCount = 0;

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name.ToLower())
                    {
                        case "file":
                            if (mergeElement != null)
                            {
                                this.MergeListFileArray.Add(mergeElement);
                            }

                            mergeElement = new MergeListFiles();
                            object exclude = reader.GetAttribute("exclude");
                            if (exclude != null)
                            {
                                if (exclude.ToString() == "1")
                                {
                                    mergeElement.Include = false;
                                }
                            }

                            break;
                        case "path":
                            mergeElement.Path = reader.ReadElementContentAsString();

                            // resolve paths, if files were moved
                            if (File.Exists(mergeElement.Path) == false)
                            {
                                ++fileNotFoundCount;
                            }

                            break;
                        case "pages":
                            mergeElement.Pages = reader.ReadElementContentAsString();
                            break;
                        case "bookmark":
                            mergeElement.Bookmark = reader.ReadElementContentAsString();
                            break;
                        case "level":
                            mergeElement.Level = reader.ReadElementContentAsInt();
                            break;
                        case "info":
                            this.MergeListInfo.HasInfo = true;
                            break;
                        case "author":
                            this.MergeListInfo.HasInfo = true;
                            this.MergeListInfo.InfoAuthor = reader.ReadElementContentAsString();
                            break;
                        case "title":
                            this.MergeListInfo.HasInfo = true;
                            this.MergeListInfo.InfoTitle = reader.ReadElementContentAsString();
                            break;
                        case "subject":
                            this.MergeListInfo.HasInfo = true;
                            this.MergeListInfo.InfoSubject = reader.ReadElementContentAsString();
                            break;
                        case "annotation":
                            this.MergeListInfo.Annotation = reader.ReadElementContentAsString();
                            break;
                        case "outfile":
                            this.MergeListInfo.OutFilename = reader.ReadElementContentAsString();
                            break;
                        case "startpage":
                            this.MergeListInfo.StartPage = int.Parse(reader.ReadElementContentAsString());
                            this.MergeListInfo.NumberPages = true;
                            break;
                        case "paginationformat":
                            try
                            {
                                this.MergeListInfo.PaginationFormat = (PaginationFormatting.PaginationFormats)int.Parse(reader.ReadElementContentAsString());
                            }
                            catch
                            {
                                throw new Exception("Invalid value for pagination format in command file");
                            }

                            break;
                    }
                }
            }

            if (mergeElement != null)
            {
                this.MergeListFileArray.Add(mergeElement);
            }

            reader.Close();

            return fileNotFoundCount;
        }

        private void ResolvePathsIfFoldersMoved(string cmdFileName, int missingCount)
        {
            // first get a list of folders used in the plan
            List<string> planFolders = new List<string>();
            foreach (MergeListFiles mergeElement in this.MergeListFileArray)
            {
                string folder = Path.GetDirectoryName(mergeElement.Path);
                if (planFolders.Contains(folder) == false)
                {
                    planFolders.Add(folder);
                }
            }

            if (planFolders.Count < 1)
            {
                return;
            }

            // find the common path
            string commonPath = FindCommonDirectoryPath.FindCommonPath(planFolders);

            string findPath = Path.GetDirectoryName(cmdFileName);
            while (true)
            {
                if (this.CheckMissing(commonPath, findPath) == 0)
                {
                    break;
                }

                if (Directory.GetParent(findPath) == null)
                {
                    return;
                }

                findPath = Directory.GetParent(findPath).FullName;
            }

            // all found - go ahead and adjust
            this.AdjustFolders(commonPath, findPath);
        }

        private int CheckMissing(string oldCommonFolder, string newCommonFolder)
        {
            int countMissing = 0;
            foreach (MergeListFiles mergeElement in this.MergeListFileArray)
            {
                string newFileName = mergeElement.Path.Replace(oldCommonFolder, newCommonFolder);
                if (File.Exists(newFileName) == false && File.Exists(mergeElement.Path) == false)
                {
                    ++countMissing;
                }
            }

            return countMissing;
        }

        private void AdjustFolders(string oldCommonFolder, string newCommonFolder)
        {
            foreach (MergeListFiles mergeElement in this.MergeListFileArray)
            {
                mergeElement.Path = mergeElement.Path.Replace(oldCommonFolder, newCommonFolder);
            }

            this.MergeListInfo.OutFilename = this.MergeListInfo.OutFilename.Replace(oldCommonFolder, newCommonFolder);
        }

        private List<int> ExtractPageList(string pageListStr)
        {
            List<int> pageListAry = new List<int>();

            // first split to sections based on ; or , separators
            string[] pageSections = pageListStr.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pageSection in pageSections)
            {
                // build the page array
                string[] pagearr = pageSection.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int start = 0;
                int end = 0;
                try
                {
                    start = int.Parse(pagearr[0]);
                    end = start;
                    if (pagearr.Length > 1)
                    {
                        end = int.Parse(pagearr[1]);
                    }

                    if (end > 0)
                    {
                        for (int x = start; x <= end; ++x)
                        {
                            pageListAry.Add(x - 1);
                        }
                    }
                }
                catch
                {
                }
            }

            return pageListAry;
        }
    }
}
