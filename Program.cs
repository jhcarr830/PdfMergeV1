using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace MergeTest2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Globals.ReadCfgFile();
            GlobalFontSettings.FontResolver = new CustomFontResolver();
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
            string docsetFileName = args[0];  // contains list of documents to be printed
            string mergedataFileName = args[1];  // contains the data to be merged into documents
            bool drawGrid = false;
            if (args.Length > 2)
            {
                string sDrawGrid = args[2];
                if (sDrawGrid == "grid")
                    drawGrid = true;
            }
            // delete temp files in output directory (in case there are leftovers)
            var temporaryfiles = Directory.GetFiles(path: Globals.PathToOutputFiles, searchPattern: "Temp_*.pdf");
            foreach (string file in temporaryfiles)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            List<string> docsList = new List<string>();
            int currRow = 0;
            try
            {
                using (StreamReader sr = new StreamReader(Globals.PathToDataFiles + docsetFileName))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        currRow++;
                        if (line.Length > 0)
                        {
                            if (line.Substring(0, 1) != "#")
                            {
                                docsList.Add(line);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            // Get Merge Data and save it in a Dictionary object
            Dictionary<string, string> mergeDataDict = new Dictionary<string, string>();
            try
            {
                using (StreamReader sr = new StreamReader(Globals.PathToDataFiles + mergedataFileName))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            XFont font = new XFont("Arial", 12, XFontStyleEx.Regular);
            // Note: only 2 fonts are used in this program: Arial and Times

            int tempDocNumber = 1;
            // Print all documents in the document set in order.
            for (int ixx = 0; ixx < docsList.Count; ixx++)
            {
                MergePrintDocument(docsList[ixx] + ".pdf", docsList[ixx] + ".mrg", mergeDataDict, tempDocNumber, drawGrid);
                tempDocNumber++;
            }
            // merge all documents into one:
            if (File.Exists(Globals.PathToOutputFiles + "CombinedDocs.pdf"))
                File.Delete(Globals.PathToOutputFiles + "CombinedDocs.pdf");
            var intermediateFiles = Directory.GetFiles(Globals.PathToOutputFiles, "Temp_*.pdf");
            // sort the list
            Array.Sort(intermediateFiles);
            PdfDocument outputDocument = new PdfDocument();
            foreach (string file in intermediateFiles)
            {
                PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                foreach (PdfPage page in inputDocument.Pages)
                {
                    outputDocument.AddPage(page);
                }
                inputDocument.Dispose();
                if (File.Exists(file))
                    File.Delete(file);
            }
             outputDocument.Save(Globals.PathToOutputFiles + "CombinedDocs.pdf");
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
                var fontPath = Path.Combine(path1: Globals.PathToFontFiles, path2: faceName);
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

        public static void MergePrintDocument(string DocumentFileName, string MergeFileName, Dictionary<string, string> mergeDataDict, int tempDocNumber, bool drawGrid = false)
        {
            List<DocumentMergeDataRecord> mergeFieldList = new List<DocumentMergeDataRecord>();
            DocumentMergeDataRecord mergeRec;
            int currRow = 0;
            string intermediateDocName = "Temp_" + tempDocNumber.ToString("D5") + ".pdf";

            // The Merge file (*.mrg) contains the field names with font information
            // and location of each field to be printed in each document page
            try
            {
                // load Merge data into mergeFieldList
                using (StreamReader sr = new StreamReader(Globals.PathToMergeFiles + MergeFileName))
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
                                Console.WriteLine($"Error in row {currRow} file {Globals.PathToMergeFiles + MergeFileName}");
                            }
                        }
                    }
                }
                string inputPdfFile = Globals.PathToSourceFiles + DocumentFileName;
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
                    // handle multiple pages in input pdf file
                    page = pdfDocument.Pages[rec.Page - 1];
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

                if (drawGrid)
                {
                    font = new XFont("Arial", 10, XFontStyleEx.Regular);
                    XPen pen = new XPen(XColors.Pink, 1.0 / 36.0);
                    for (int iPage = 0; iPage < pdfDocument.Pages.Count; iPage++)
                    {
                        //page = pdfDocument.Pages[0];
                        page = pdfDocument.Pages[iPage];
                        gfx = XGraphics.FromPdfPage(page);
                        pageWidth = page.Width;
                        pageHeight = page.Height;
                        double x, y;
                        // Horizontal Lines
                        for (x = 0; x < pageWidth; x += 36.0)
                        {
                            for (y = 0; y < pageHeight; y += 36.0)
                            {
                                gfx.DrawLine(pen, x, y, pageWidth, y);
                                if (x == 0)
                                    gfx.DrawString(y.ToString(), font, XBrushes.Gray, new XRect(0, y, pageWidth, pageHeight), XStringFormats.TopLeft);
                            }
                        }
                        // Vertical Lines
                        for (y = 0; y < pageWidth; y += 36.0)
                        {
                            for (x = 0; x < pageHeight; x += 36.0)
                            {
                                gfx.DrawLine(pen, y, x, y, pageWidth);
                                if (y == 0 && x > 0)
                                    gfx.DrawString(x.ToString(), font, XBrushes.Gray, new XRect(x, 0, pageWidth, pageHeight), XStringFormats.TopLeft);
                            }
                        }
                    }
                }

                pdfDocument.Save($"{Globals.PathToOutputFiles}{intermediateDocName}");
                pdfDocument.Dispose();
                //Console.WriteLine($"============{DocumentFileName}================");
                //foreach (DocumentMergeDataRecord rec in mergeFieldList)
                //{
                //    Console.WriteLine($"{rec.FieldName}, {rec.FontName}, {rec.FontSize}, {rec.FontStyle}, {rec.Page}, {rec.XPos}, {rec.YPos}");
                //}
                //Console.WriteLine($"{intermediateDocName} is ready");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

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


