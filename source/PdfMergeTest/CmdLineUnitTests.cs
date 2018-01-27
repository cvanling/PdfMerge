using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace PdfMergeTest
{
    [TestClass]
    public class CmdLineUnitTests
    {
        [TestMethod]
        public void MergeTestCase1_XML()
        {
            string planfile = @"testfiles\mergetestcase1.xml";
            string outfile = "mergeoutput1.pdf";
            string[] args = new string[] { "pdfmerge.exe", planfile, outfile, "/CREATETESTINFO" };
            File.Delete(outfile);
            bool showGui;
            string resCmdFile;
            string resOutfile;
            string err = PdfMerge.Program.ProcessCommandLine(args, out showGui, out resCmdFile, out resOutfile);

            bool pass1 = File.Exists(outfile);
            Assert.IsTrue(pass1, "outfile should exist : " + planfile);

            bool pass2 = err == string.Empty;
            Assert.IsTrue(pass2, "no error : " + planfile);

            SortedDictionary<string, string> testInfo = ReadTestOutput(outfile);
            Assert.IsTrue(testInfo["MergeListFileArrayCount"] == "4", "MergeListFileArrayCount == 4");
            Assert.IsTrue(testInfo["PageCount"] == "7", "PageCount == 7");
            Assert.IsTrue(testInfo["BookMarkCount"] == "8", "BookMarkCount == 8");
        }

        [TestMethod]
        public void MergeTestCase2_XML()
        {
            string planfile = @"testfiles\mergetestcase2.xml";
            string outfile = "mergeoutput2.pdf";
            string[] args = new string[] { "pdfmerge.exe", planfile, outfile, "/CREATETESTINFO" };
            File.Delete(outfile);
            bool showGui;
            string resCmdFile;
            string resOutfile;
            string err = PdfMerge.Program.ProcessCommandLine(args, out showGui, out resCmdFile, out resOutfile);

            bool pass1 = File.Exists(outfile);
            Assert.IsTrue(pass1, "outfile should exist : " + planfile);

            bool pass2 = err == string.Empty;
            Assert.IsTrue(pass2, "no error : " + planfile);

            SortedDictionary<string, string> testInfo = ReadTestOutput(outfile);
            Assert.IsTrue(testInfo["MergeListFileArrayCount"] == "4", "MergeListFileArrayCount == 4");
            Assert.IsTrue(testInfo["PageCount"] == "6", "PageCount == 6");
            Assert.IsTrue(testInfo["BookMarkCount"] == "7", "BookMarkCount == 7");
        }

        [TestMethod]
        public void MergeTestCase3_XML()
        {
            string planfile = @"testfiles\mergetestcase3.xml";
            string outfile = "mergeoutput3.pdf";
            string[] args = new string[] { "pdfmerge.exe", planfile, outfile, "/CREATETESTINFO" };
            File.Delete(outfile);
            bool showGui;
            string resCmdFile;
            string resOutfile;
            string err = PdfMerge.Program.ProcessCommandLine(args, out showGui, out resCmdFile, out resOutfile);

            bool pass1 = File.Exists(outfile);
            Assert.IsTrue(pass1, "outfile should exist : " + planfile);

            bool pass2 = err == string.Empty;
            Assert.IsTrue(pass2, "no error : " + planfile);

            SortedDictionary<string, string> testInfo = ReadTestOutput(outfile);
            Assert.IsTrue(testInfo["MergeListFileArrayCount"] == "4", "MergeListFileArrayCount == 4");
            Assert.IsTrue(testInfo["PageCount"] == "7", "PageCount == 7");
            Assert.IsTrue(testInfo["BookMarkCount"] == "8", "BookMarkCount == 8");
        }

        [TestMethod]
        public void Unicode_XML()
        {
            string planfile = @"testfiles\unicodetest.xml";
            string outfile = "mergeoutputunicode.pdf";
            string[] args = new string[] { "pdfmerge.exe", planfile, outfile, "/CREATETESTINFO" };
            File.Delete(outfile);
            bool showGui;
            string resCmdFile;
            string resOutfile;
            string err = PdfMerge.Program.ProcessCommandLine(args, out showGui, out resCmdFile, out resOutfile);

            bool pass1 = File.Exists(outfile);
            Assert.IsTrue(pass1, "outfile should exist : " + planfile);

            bool pass2 = err == string.Empty;
            Assert.IsTrue(pass2, "no error : " + planfile);

            SortedDictionary<string, string> testInfo = ReadTestOutput(outfile);
            Assert.IsTrue(testInfo["MergeListFileArrayCount"] == "5", "MergeListFileArrayCount == 5");
            Assert.IsTrue(testInfo["PageCount"] == "14", "PageCount == 14");
            Assert.IsTrue(testInfo["BookMarkCount"] == "17", "BookMarkCount == 17");
        }

        [TestMethod]
        public void BadFileCase()
        {
            string planfile = @"testfiles\badfilecase.xml";
            string outfile = "mergeoutput.pdf";
            string[] args = new string[] { "pdfmerge.exe", planfile, outfile };
            File.Delete(outfile);
            bool showGui;
            string resCmdFile;
            string resOutfile;
            string err = PdfMerge.Program.ProcessCommandLine(args, out showGui, out resCmdFile, out resOutfile);

            bool pass1 = !File.Exists(outfile);
            Assert.IsTrue(pass1, "outfile should not exist : " + planfile);

            bool pass2 = err.Contains("Could not find file");
            Assert.IsTrue(pass2, "wrong error : " + err);
        }

        private SortedDictionary<string,string> ReadTestOutput(string filename)
        {
            SortedDictionary<string, string> res = new SortedDictionary<string, string>();
            string[] lines = File.ReadAllLines(filename + ".info");
            foreach (string line in lines )
            {
                string[] parts = line.Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    res.Add(parts[0], parts[1]);
                }                   
            }
            return res;
        }
        

    }
}
