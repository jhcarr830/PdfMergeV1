using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace MergeTest2
{
    internal class Program
    {
        const string PATH_TO_DATA_FILES = @"C:\Users\jhcarr\Projects\";
        const string PATH_TO_SOURCE_FILES = @"C:\Users\jhcarr\Projects\PdfSourceFiles\";
        const string PATH_TO_OUTPUT_FILES = @"C:\Users\jhcarr\Projects\PdfOutputFiles\";
        const string PATH_TO_FONT_FILES = @"C:\Users\jhcarr\Projects\FontFiles\";
        const string PATH_TO_MERGE_FILES = @"C:\Users\jhcarr\Projects\MergeInfoFiles\";
        static void Main(string[] args)
        {
            GlobalFontSettings.FontResolver = new CustomFontResolver();
            //Console.WriteLine("Hello, World!");
            if (args.Length == 0)
            {
                Console.WriteLine("no args");
                return;
            }
            Console.WriteLine("Command Line Arguments:");
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine(args[i]);
            }
            string docsetFileName = args[0];
            string mergedataFileName = args[1];
            List<string> docsList = new List<string>();
            //DocsetRecord doc;
            int currRow = 0;
            try
            {
                using (StreamReader sr = new StreamReader(PATH_TO_DATA_FILES + docsetFileName))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        currRow++;
                        //Console.WriteLine(line);
                        if (line.Length > 0)
                        {
                            if (line.Substring(0, 1) != "#")
                            {
                                docsList.Add(line);
                            }
                        }
                        //string[] parts = line.Split('\t');
                        //if (parts.Length > 2)
                        //{
                        //    if (parts[0].Substring(0, 1) != "#")  // skip comments
                        //    {
                        //        doc = new DocsetRecord();
                        //        if (int.TryParse(parts[0], out int docnum))
                        //            doc.DocumentNumber = docnum;
                        //        else
                        //            Console.WriteLine($"Row {currRow}: Unable to convert DocumentNumber to integer");
                        //        doc.DocumentName = parts[1];
                        //        if (int.TryParse(parts[2], out int num))
                        //            doc.NumCopies = num;
                        //        else
                        //            Console.WriteLine($"Row {currRow}: Unable to convert NumCopies to integer");
                        //        docsList.Add(doc);
                        //        Console.WriteLine($"Document: {doc.DocumentName}");
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //for (int ixx = 0; ixx < docsList.Count; ixx++)
            //{
            //    Console.Write(docsList[ixx].DocumentName + " - ");
            //    Console.WriteLine(docsList[ixx].NumCopies);
            //}

            // Get Merge Data and save it in a Dictionary
            Dictionary<string, string> mergeDataDict = new Dictionary<string, string>();
            try
            {
                using (StreamReader sr = new StreamReader(PATH_TO_MERGE_FILES + mergedataFileName))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (line.Substring(0, 1) != "#")  // skip comments
                            {
                                string[] parts = line.Split('\t');
                                mergeDataDict.Add(parts[0], parts[1]);
                            }
                        }
                    }
                    //Console.WriteLine("***mergeDataDict***");
                    //foreach (KeyValuePair<string, string> entry in mergeDataDict)
                    //    Console.WriteLine($"Key: {entry.Key}, Value: {entry.Value}");
                    //Console.WriteLine(">>>>" + mergeDataDict["Borrower1FullName"] + "<<<<");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            XFont font = new XFont("Arial", 12, XFontStyleEx.Regular);
            //XFont font = new XFont("Times", 12, XFontStyleEx.Regular);

            int tempDocNumber = 1;
            // Print all documents in the document set in order.
            //Console.WriteLine("Printing all Documents in order");
            for (int ixx = 0; ixx < docsList.Count; ixx++)
            {
                //Console.WriteLine($"{docsList[ixx].DocumentName} Copies: {docsList[ixx].NumCopies}");
                //for (int iyy = 0; iyy < (docsList[ixx].NumCopies); iyy++)
                //{
                //Console.WriteLine($"{docsList[ixx].DocumentName} #{iyy + 1}");
                MergePrintDocument(docsList[ixx] + ".pdf", docsList[ixx] + ".mrg", mergeDataDict, tempDocNumber);
                tempDocNumber++;
                //}
            }
        }

        public class CustomFontResolver : IFontResolver
        {
            public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
                {
                    if (isBold && isItalic)
                    {
                        return new FontResolverInfo("arialbi.ttf");
                    }
                    else if (isBold)
                    {
                        return new FontResolverInfo("arialbd.ttf");
                    }
                    else if (isItalic)
                    {
                        return new FontResolverInfo("ariali.ttf");
                    }
                    else
                    {
                        return new FontResolverInfo("arial.ttf");
                    }
                }
                if (familyName.Equals("Times", StringComparison.OrdinalIgnoreCase))
                {
                    if (isBold && isItalic)
                    {
                        return new FontResolverInfo("timesbi.ttf");
                    }
                    else if (isBold)
                    {
                        return new FontResolverInfo("timesbd.ttf");
                    }
                    else if (isItalic)
                    {
                        return new FontResolverInfo("timesi.ttf");
                    }
                    else
                    {
                        return new FontResolverInfo("times.ttf");
                    }
                }
                // default to arial regular
                return new FontResolverInfo("arial.ttf");
            }
            public byte[] GetFont(string faceName)
            {
                var fontPath = Path.Combine(PATH_TO_FONT_FILES, faceName);
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        using (var fs = File.OpenRead(fontPath))
                        {
                            fs.CopyTo(ms);
                            ms.Position = 0;
                            return ms.ToArray();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw new Exception($"Font file not found: {fontPath}");
                    }
                }
            }
        }

        public static void MergePrintDocument(string DocumentFileName, string MergeFileName, Dictionary<string, string> mergeDataDict, int tempDocNumber)
        {
            //Console.WriteLine($">>>{DocumentFileName} {MergeFileName}<<<");
            List<DocumentMergeDataRecord> mergeFieldList = new List<DocumentMergeDataRecord>();
            DocumentMergeDataRecord mergeRec;
            int currRow = 0;
            string intermediateDocName = "Temp_" + tempDocNumber.ToString("D5") + ".pdf";

            // The Merge file (*.mrg) contains the field names with font information
            // and location of each field to be printed in each document page
            try
            {
                // load Merge data into mergeFieldList
                using (StreamReader sr = new StreamReader(PATH_TO_MERGE_FILES + MergeFileName))
                {
                    string? line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        currRow++;
                        if (line.Substring(0, 1) != "#")  // skip comments
                        {
                            string[] parts = line.Split('\t');  // there must be 7 parts
                            if (parts.Length > 6)
                            {
                                mergeRec = new DocumentMergeDataRecord();
                                mergeRec.FieldName = parts[0];
                                mergeRec.FontName = parts[1];
                                if (int.TryParse(parts[2], out int fontsize))
                                    mergeRec.FontSize = fontsize;
                                else
                                    mergeRec.FontSize = 12;
                                mergeRec.FontStyle = parts[3];
                                if (int.TryParse(parts[4], out int pagenum))
                                    mergeRec.Page = pagenum;
                                else
                                    mergeRec.Page = 1;
                                if (double.TryParse(parts[5], out double xpos))
                                    mergeRec.XPos = xpos;
                                else
                                    mergeRec.XPos = 0;
                                if (double.TryParse(parts[6], out double ypos))
                                    mergeRec.YPos = ypos;
                                else
                                    mergeRec.YPos = 0;
                                mergeFieldList.Add(mergeRec);
                            }
                            else
                            {
                                Console.WriteLine($"Error in row {currRow} file {PATH_TO_MERGE_FILES + MergeFileName}");
                            }
                        }
                    }
                }
                string inputPdfFile = PATH_TO_SOURCE_FILES + DocumentFileName;
                PdfDocument pdfDocument;
                PdfPage page;
                XGraphics gfx;
                XFont font;
                XFontStyleEx fontStyle;
                XUnitPt pageWidth, pageHeight;

                // merge and print current document (inputPdfFile) to <intermediateDocName>
                pdfDocument = PdfReader.Open(inputPdfFile, PdfDocumentOpenMode.Modify);

                foreach (DocumentMergeDataRecord rec in mergeFieldList)
                {
                    // todo: handle multiple pages in input pdf file
                    page = pdfDocument.Pages[0];
                    gfx = XGraphics.FromPdfPage(page);
                    switch (rec.FontStyle)
                    {
                        case "Regular":
                            fontStyle = XFontStyleEx.Regular;
                            break;
                        case "BoldItalic":
                            fontStyle = XFontStyleEx.BoldItalic;
                            break;
                        case "Bold":
                            fontStyle = XFontStyleEx.Bold;
                            break;
                        case "Italic":
                            fontStyle = XFontStyleEx.Italic;
                            break;
                        default:
                            fontStyle = XFontStyleEx.Regular;
                            break;
                    }
                    font = new XFont(rec.FontName, rec.FontSize, fontStyle);
                    pageWidth = page.Width;
                    pageHeight = page.Height;
                    gfx.DrawString(mergeDataDict[rec.FieldName], font, XBrushes.Black, new XRect(rec.XPos, rec.YPos, pageWidth, pageHeight), XStringFormats.TopLeft);
                    gfx.Dispose();
                    //Console.WriteLine("===" + mergeDataDict[rec.FieldName] + "===");
                }
                
                //Console.WriteLine($"Saving page to {PATH_TO_OUTPUT_FILES}{intermediateDocName}");
                pdfDocument.Save($"{PATH_TO_OUTPUT_FILES}{intermediateDocName}");
                pdfDocument.Dispose();
                //Console.WriteLine($"============{DocumentFileName}================");
                //foreach (DocumentMergeDataRecord rec in mergeFieldList)
                //{
                //    Console.WriteLine($"{rec.FieldName}, {rec.FontName}, {rec.FontSize}, {rec.FontStyle}, {rec.Page}, {rec.XPos}, {rec.YPos}");
                //}
                Console.WriteLine($"{intermediateDocName} is ready");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        //public class DocsetRecord
        //{
        //    public int DocumentNumber = 0;
        //    public string DocumentName = "";
        //    public int NumCopies = 0;
        //}

        public class DocumentMergeDataRecord
        {
            public string FieldName { get; set; } = "";
            public string FontName { get; set; } = "";
            public int FontSize { get; set; } = 12;
            public string FontStyle { get; set; } = "R";
            public int Page { get; set; }
            public double XPos { get; set; } = 0;
            public double YPos { get; set; } = 0;
        }

    }
}



// combine multiple pdfs into one pdf file:
//using System;
//using System.IO;
//using PdfSharp.Pdf;
//using PdfSharp.Pdf.IO;

//class Program
//{
//    static void Main(string[] args)
//    {
//        string[] files = Directory.GetFiles("path_to_your_pdf_files");
//        PdfDocument outputDocument = new PdfDocument();

//        foreach (string file in files)
//        {
//            PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
//            foreach (PdfPage page in inputDocument.Pages)
//            {
//                outputDocument.AddPage(page);
//            }
//            inputDocument.Dispose();
//        }

//        outputDocument.Save("combined.pdf");
//    }
//}


//save merged pdf file as a new file:


//using System;
//using System.IO;
//using PdfSharp.Pdf;
//using PdfSharp.Pdf.IO;

//class Program
//{
//    static void Main(string[] args)
//    {
//        // Define the path to the PDF files and the output file
//        string[] files = Directory.GetFiles("path_to_your_pdf_files");
//        string outputFilePath = "path_to_save_combined/combined.pdf";

//        // Create a new PDF document
//        PdfDocument outputDocument = new PdfDocument();

//        // Iterate through each PDF file and add their pages to the output document
//        foreach (string file in files)
//        {
//            PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
//            foreach (PdfPage page in inputDocument.Pages)
//            {
//                outputDocument.AddPage(page);
//            }
//            inputDocument.Dispose();
//        }

//        // Save the merged PDF document as a new file
//        outputDocument.Save(outputFilePath);
//    }
//}


