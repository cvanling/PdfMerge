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
// File: SplitMergeCmdFile.cs
// Description: Top level class for merge/split from a command file
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
//   1.3 Oct  7/2012 C. Van Lingen  <V20> Migrated to PdfSharp 1.32
//                                  Added use of CompatiblePdfReader based
//                                  on iTextSharp DLL
//                                  Added pagination and annotation
//   1.2 Jul 25/2008 C. Van Lingen  <V18> Added XML command file support to allow
//                                  operation with unicode strings
//
//   1.1 Jan  1/2008 C. Van Lingen  (V17) Replaced merge tool with PDF sharp 
//                                  (handles up to version 1.6 PDF formats)
//
//   1.0 Oct 17/2006 C. Van Lingen  Added use of PdfTk to preprocess for multiple version tags (V14)
//                                  (Previous fix was not adequate)
//=============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace PdfMerge.SplitMergeLib
{
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
	public class SplitMergeCmdFile
	{
        public List<MergeListFiles> MergeListFileArray = new List<MergeListFiles>();
        public MergeListInfoDefn MergeListInfo = new MergeListInfoDefn();

        public SplitMergeCmdFile()
		{
		}

        public string DoSplitMerge(string CommandFilename, string OutputFilename)
        {
            return DoSplitMerge(CommandFilename, OutputFilename, null, 1, false, null);
        }

        public string DoSplitMerge(string OutputFilename, ListBox lbStat)
        {
            return DoSplitMerge(null, OutputFilename, lbStat, 1, false, null);
        }

        public string DoSplitMerge(string CommandFilename, string OutputFilename, ListBox lbStat, int StartPageNumber, bool NumberPages, string Annotation)
        {
            PdfSharpSplitterMerger psm = null;

            if (CommandFilename != null)
            {
                try
                {
                    if (CommandFilename.ToLower().EndsWith(".xml"))
                        ReadXmlCommandFile(CommandFilename);
                    else
                        ReadAsciiCommandFile(CommandFilename);
                }
                catch (Exception err)
                {
                    return err.Message;
                }
            }

            string line = "";
            try
            {
                psm = new PdfSharpSplitterMerger();

                int page = 1;

                if (MergeListInfo.HasInfo == true)
                {
                    psm.title = MergeListInfo.Info_Title;
                    psm.subject = MergeListInfo.Info_Subject;
                    psm.author = MergeListInfo.Info_Author;
                }

                foreach (MergeListFiles merge in MergeListFileArray)
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

                        // build the page array
                        string[] pagearr = pagelst.Split(new Char[] { '-' });
                        int start = 0;
                        int end = 0;
                        ArrayList ps = new ArrayList();
                        try
                        {
                            start = int.Parse(pagearr[0]);
                            end = start;
                            if (pagearr.Length > 1)
                                end = int.Parse(pagearr[1]);
                            if (end > 0)
                                for (int x = start; x <= end; ++x)
                                    ps.Add(x - 1);
                        }
                        catch
                        {
                        }

                        // merge the input file
                        if (ps.Count > 0)
                            page += psm.Add(file_add, ps.ToArray(typeof(int)) as int[]);
                        else
                            page += psm.Add(file_add);

                        // add bookmarks
                        string RootTitle = merge.Bookmark;
                        int level = merge.Level;
                        psm.AddBookmarksFromFile(RootTitle, level, merge.Include, line);                
                }
                // this writes out the merged files
                psm.Finish(OutputFilename, Annotation, NumberPages, StartPageNumber);

            }
            catch (Exception err)
            {
                psm = null;

                String retval;
                if (line != null)
                    if (line.Length > 0)
                        retval = err.Message + "\nOn Line:\n" + line;
                    else
                        retval = err.Message;
                else
                    retval = err.Message;
                return retval;
            }

            psm = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // return empty string indicating no error
            return "";
        }

        public void Load(string CommandFilename)
        {
            try
            {
                if (CommandFilename.ToLower().EndsWith(".xml"))
                    ReadXmlCommandFile(CommandFilename);
                else
                    ReadAsciiCommandFile(CommandFilename);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void Save(string CommandFilename)
        {
            try
            {
                if (CommandFilename.ToLower().EndsWith(".xml"))
                    SaveXmlCommandFile(CommandFilename);
                else
                    SaveAsciiCommandFile(CommandFilename);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void SaveAsciiCommandFile(string filename)
        {
            if (MergeListFileArray.Count < 1)
                return;

            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (MergeListFiles MergeItem in MergeListFileArray)
                {
                    sw.WriteLine(MergeItem.Descriptor);
                }
                if (MergeListInfo.HasInfo == true)
                    sw.WriteLine(MergeListInfo.Descriptor);
            }
        }

        private void ReadAsciiCommandFile(string filename)
        {
            MergeListFileArray = new List<MergeListFiles>();
            MergeListInfo = new MergeListInfoDefn();

            string line = "";

            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    while ((line = sr.ReadLine()) != null)
                    {

                        string[] args = line.Split(new Char[] { ';', '\r', '\n' });
                        if (args[0].ToLower() == "[info]")
                        {
                            MergeListInfo.HasInfo = true;
                            if (args.Length > 1)
                                MergeListInfo.Info_Title = args[1];
                            if (args.Length > 2)
                                MergeListInfo.Info_Subject = args[2];
                            if (args.Length > 3)
                                MergeListInfo.Info_Author = args[3];
                        }
                        else
                        {
                            MergeListFiles MergeElement = new MergeListFiles();
                            MergeElement.Path = args[0];
                            MergeElement.Pages = args[1];
                            if (args.Length > 2)
                                if (args[2].ToUpper() == "EXCLUDE")
                                    MergeElement.Include = false;
                            if (args.Length > 3)
                                MergeElement.Bookmark = args[3];
                            if (args.Length > 4)
                                MergeElement.Level = int.Parse(args[4]);
                            MergeListFileArray.Add(MergeElement);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                if (line != null)
                    if (line.Length > 0)
                        throw new Exception(err.Message + "\nOn Line:\n" + line);
                throw err;
            }
        }

        private void SaveXmlCommandFile(string filename)
        {
            if (MergeListFileArray.Count < 1)
                return;

            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(filename, new System.Text.UnicodeEncoding());
            writer.Formatting = Formatting.Indented;

            writer.WriteStartDocument();
            
            writer.WriteStartElement("merge");

            //Write sub-elements
            foreach (MergeListFiles MergeItem in MergeListFileArray)
            {
                writer.WriteStartElement("file");
                if (MergeItem.Include == false)
                    writer.WriteAttributeString("exclude", "1");
                writer.WriteElementString("path", MergeItem.Path);
                writer.WriteElementString("pages", MergeItem.Pages);
                if (MergeItem.Bookmark != null)
                {
                    writer.WriteElementString("bookmark", MergeItem.Bookmark);
                    if (MergeItem.Level > 0)
                        writer.WriteElementString("level", XmlConvert.ToString(MergeItem.Level));
                }
                writer.WriteEndElement();
            }

            #region write info and options
            if (MergeListInfo.HasInfo == true)
            {
                writer.WriteStartElement("info");
                if (MergeListInfo.Info_Author.Length > 0)
                    writer.WriteElementString("author", MergeListInfo.Info_Author);
                if (MergeListInfo.Info_Subject.Length > 0)
                    writer.WriteElementString("subject", MergeListInfo.Info_Subject);
                if (MergeListInfo.Info_Title.Length > 0)
                    writer.WriteElementString("title", MergeListInfo.Info_Title);
                writer.WriteEndElement();
            }
            writer.WriteStartElement("options");
            if (string.IsNullOrEmpty(MergeListInfo.OutFilename) == false)
                writer.WriteElementString("outfile", MergeListInfo.OutFilename);
            if (string.IsNullOrEmpty(MergeListInfo.Annotation) == false)
                writer.WriteElementString("annotation", MergeListInfo.Annotation);
            if (MergeListInfo.NumberPages == true)
                writer.WriteElementString("startpage", MergeListInfo.StartPage.ToString());
            writer.WriteEndElement();
            #endregion

            writer.WriteFullEndElement();

            writer.Close();  
        }

        private void ReadXmlCommandFile(string filename)
        {
            MergeListFileArray = new List<MergeListFiles>();
            MergeListInfo = new MergeListInfoDefn();

            MergeListFiles MergeElement = null;

            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(filename);

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name.ToLower())
                    {
                        case "file":
                            if (MergeElement != null)
                                MergeListFileArray.Add(MergeElement);
                            MergeElement = new MergeListFiles();
                            object exclude = reader.GetAttribute("exclude");
                            if (exclude != null)
                                if (exclude.ToString() == "1")
                                    MergeElement.Include = false;
                            break;
                        case "path":
                            MergeElement.Path = reader.ReadElementContentAsString();
                            break;
                        case "pages":
                            MergeElement.Pages = reader.ReadElementContentAsString();
                            break;
                        case "bookmark":
                            MergeElement.Bookmark = reader.ReadElementContentAsString();
                            break;
                        case "level":
                            MergeElement.Level = reader.ReadElementContentAsInt();
                            break;
                        case "info":
                            MergeListInfo.HasInfo = true;
                            break;
                        case "author":
                            MergeListInfo.HasInfo = true;
                            MergeListInfo.Info_Author = reader.ReadElementContentAsString();
                            break;
                        case "title":
                            MergeListInfo.HasInfo = true;
                            MergeListInfo.Info_Title = reader.ReadElementContentAsString();
                            break;
                        case "subject":
                            MergeListInfo.HasInfo = true;
                            MergeListInfo.Info_Subject = reader.ReadElementContentAsString();
                            break;
                        case "annotation":
                            MergeListInfo.Annotation = reader.ReadElementContentAsString();
                            break;
                        case "outfile":
                            MergeListInfo.OutFilename = reader.ReadElementContentAsString();
                            break;
                        case "startpage":
                            MergeListInfo.StartPage = int.Parse(reader.ReadElementContentAsString());
                            MergeListInfo.NumberPages = true;
                            break;
                    }
                }
            }
            if (MergeElement != null)
                MergeListFileArray.Add(MergeElement);
            reader.Close();
        }

	}

    public class MergeListInfoDefn
    {
        public bool HasInfo = false;
        public string Info_Author, Info_Title, Info_Subject;

        public string OutFilename="merged.pdf";
        public string Annotation="";
        public bool NumberPages=false;
        public int StartPage=1;

        public MergeListInfoDefn()
        {
            HasInfo = false;
            Info_Title = "";
            Info_Subject = "";
            Info_Author = "";
        }

        /// <summary>
        /// Ascii representation
        /// </summary>
        public string Descriptor
        {
            get
            {
                string s = "[info];";
                if (Info_Title.Length > 0)
                {
                    s += Info_Title;
                    if (Info_Subject.Length > 0)
                    {
                        s += ";" + Info_Subject;
                        if (Info_Author.Length > 0)
                        {
                            s += ";" + Info_Author;
                        }

                    }
                }
                return s;
            }
        }
    }

    public class MergeListFiles : ICloneable
    {
        public string Path;
        public string Pages;
        public string Bookmark;
        public int Level;
        public bool Include;

        public Object Clone()
        {
            MergeListFiles clone = new MergeListFiles();
            clone.Path = Path;
            clone.Pages = Pages;
            clone.Bookmark = Bookmark;
            clone.Level = Level;
            clone.Include = Include;
            return clone;
        }

        public MergeListFiles()
        {
            Path = null;
            Pages = "all";
            Bookmark = null;
            Level = 0;
            Include = true;
        }

        /// <summary>
        /// Description of current operation for GUI display
        /// </summary>
        public string Descriptor
        {
            get
            {
                string s = "";
                // [path of pdf file];[page range];[import];[bookmark];[level]
                s += Path + ";";
                s += Pages + ";";
                if (Include == true)
                    s += "include";
                else
                    s += "exclude";
                if (Bookmark != null)
                {
                    s += ";" + Bookmark;
                    if (Level > 0)
                        s += ";" + Level.ToString();
                }
                return s;
            }
        }
    }

}
