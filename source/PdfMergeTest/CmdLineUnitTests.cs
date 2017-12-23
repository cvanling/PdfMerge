using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

namespace PdfMergeTest
{
    [TestClass]
    public class CmdLineUnitTests
    {
        [TestMethod]
        public void MergeTestCase1_XML()
        {
            string planfile = @"testfiles\mergetestcase1.xml";
            string outfile = "mergeoutput.pdf";
            string[] args = new string[] { "pdfmerge.exe", planfile, outfile };
            File.Delete(outfile);
            bool showGui;
            string resCmdFile;
            string resOutfile;
            string err = PdfMerge.Program.ProcessCommandLine(args, out showGui, out resCmdFile, out resOutfile);

            bool pass1 = File.Exists(outfile);
            Assert.IsTrue(pass1, "outfile should exist : " + planfile);

            bool pass2 = err == string.Empty;
            Assert.IsTrue(pass2, "no error : " + planfile);
        }

        [TestMethod]
        public void Unicode_XML()
        {
            string planfile = @"testfiles\unicodetest.xml";
            string outfile = "mergeoutputunicode.pdf";
            string[] args = new string[] { "pdfmerge.exe", planfile, outfile, "/showgui" };
            File.Delete(outfile);
            bool showGui;
            string resCmdFile;
            string resOutfile;
            string err = PdfMerge.Program.ProcessCommandLine(args, out showGui, out resCmdFile, out resOutfile);

            bool pass1 = File.Exists(outfile);
            Assert.IsTrue(pass1, "outfile should exist : " + planfile);

            bool pass2 = err == string.Empty;
            Assert.IsTrue(pass2, "no error : " + planfile);
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

    }
}
