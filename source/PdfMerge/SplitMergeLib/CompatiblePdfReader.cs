// =============================================================================
// Work around for problem with irefstreams as posted here:
// http://forum.pdfsharp.net/viewtopic.php?f=2&t=693#p5855
// =============================================================================
// Uses version 4.1.6 of the iTextSharp library
// (http://itextsharp.svn.sourceforge.net/viewvc/itextsharp/tags/iTextSharp_4_1_6/)
// iTextSharp ncluded as an unmodified DLL used per the terms of the GNU LGPL and the Mozilla Public License.
// See the readme.doc file included with this package.
// =============================================================================

namespace PdfSharp.Pdf.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open
    /// </summary>
    public static class CompatiblePdfReader
    {
        /// <summary>
        /// uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open
        /// </summary>
        public static PdfDocument Open(string pdfPath, PdfDocumentOpenMode openmode)
        {
            using (FileStream fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
            {
                int len = (int)fileStream.Length;
                byte[] fileArray = new byte[len];
                fileStream.Read(fileArray, 0, len);
                fileStream.Close();

                return Open(fileArray, openmode);
            }
        }

        /// <summary>
        /// uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open
        /// </summary>
        public static PdfDocument Open(byte[] fileArray, PdfDocumentOpenMode openmode)
        {
            return Open(new MemoryStream(fileArray), openmode);
        }

        /// <summary>
        /// uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open
        /// </summary>
        public static PdfDocument Open(MemoryStream sourceStream, PdfDocumentOpenMode openmode)
        {
            PdfDocument outDoc = null;
            sourceStream.Position = 0;

            try
            {
                outDoc = PdfReader.Open(sourceStream, openmode);
            }
            catch (PdfSharp.Pdf.IO.PdfReaderException)
            {
                // workaround if pdfsharp doesn't support this pdf
                sourceStream.Position = 0;
                MemoryStream outputStream = new MemoryStream();
                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(sourceStream);
                iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(reader, outputStream);
                pdfStamper.FormFlattening = true;
                pdfStamper.Writer.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_4);
                pdfStamper.Writer.CloseStream = false;
                pdfStamper.Close();

                outDoc = PdfReader.Open(outputStream, openmode);
            }

            return outDoc;
        }
    }
}