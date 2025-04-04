﻿using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfMergeV1
{
   internal class Program
   {
      const int CONFIG_FILE_ERROR = 1;
      const int ARGUMENTS_ERROR = 2;
      const int MERGE_DATA_FILE_ERROR = 3;
      const int MERGE_DOCUMENT_DATA_ERROR = 4;
      const int MERGE_PRINT_ERROR = 5;
      static int Main(string[] args)
      {
         int iResult = Globals.ReadCfgFile();
         if (iResult == -1)
            return CONFIG_FILE_ERROR;
         GlobalFontSettings.FontResolver = new CustomFontResolver();
         if (args.Length < 3)
         {
            Console.WriteLine("\nUsage: PdfMergeV1.exe docsetFileName mergeDataFileName outputFileName [grid/nogrid]");
            Console.WriteLine("\nNote: Argument 4, [grid/nogrid] must be the word 'grid' if you want to\ndraw a grid on every document. If you omit the fourth argument or use\nany other word, no grid will be added.");
            return ARGUMENTS_ERROR;
         }
         //Console.WriteLine("Command Line Arguments:");
         //for (int i = 0; i < args.Length; i++)
         //{
         //    Console.WriteLine(args[i]);
         //}
         string docsetFileName = args[0];  // contains list of documents to be printed
         string mergedataFileName = args[1];  // contains the data to be merged into documents
         string outputFileName = args[2];  // name of output file
         bool drawGrid = false;
         if (args.Length > 3)
         {
            string sDrawGrid = args[3];
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
                        if (parts.Length > 1)
                           mergeDataDict.Add(parts[0], parts[1]);
                        else
                        {
                           Console.WriteLine($"Error in Merge Data File ({mergedataFileName})");
                           return MERGE_DATA_FILE_ERROR;
                        }
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
            int iMergePrintReturnValue = 0;
            iMergePrintReturnValue = MergePrintDocument(docsList[ixx] + ".pdf", docsList[ixx] + ".mrg", mergeDataDict, tempDocNumber, drawGrid);
            if (iMergePrintReturnValue != 0)
               return iMergePrintReturnValue;
            tempDocNumber++;
         }
         // merge all documents into one:
         if (File.Exists(Globals.PathToOutputFiles + outputFileName))
            File.Delete(Globals.PathToOutputFiles + outputFileName);
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
         outputDocument.Save(Globals.PathToOutputFiles + outputFileName);
         return 0;  // success
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

      public static int MergePrintDocument(string DocumentFileName, string MergeFileName, Dictionary<string, string> mergeDataDict, int tempDocNumber, bool drawGrid = false)
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
                     string[] parts = line.Split('\t');  // there must be 8 parts
                     if (parts.Length > 7)
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
                        mergeRec.Justify = parts[5];
                        if (double.TryParse(parts[6], out double xpos))
                           mergeRec.XPos = xpos;
                        else
                           mergeRec.XPos = 0;
                        if (double.TryParse(parts[7], out double ypos))
                           mergeRec.YPos = ypos;
                        else
                           mergeRec.YPos = 0;
                        mergeFieldList.Add(mergeRec);
                     }
                     else
                     {
                        Console.WriteLine($"Error in row {currRow} file {Globals.PathToMergeFiles + MergeFileName}");
                        return MERGE_DOCUMENT_DATA_ERROR;
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
               //Console.WriteLine($"{rec.FieldName}, {rec.FontName}, {rec.FontSize}, {rec.FontStyle}, {rec.Page}, {rec.Justify}, {rec.XPos}, {rec.YPos}, {mergeDataDict[rec.FieldName]}");
               if (mergeDataDict[rec.FieldName] != "")
               {
                  if (rec.Justify == "R")
                  {
                     double width = gfx.MeasureString(mergeDataDict[rec.FieldName], font).Width;
                     //This works: XRect rect = new XRect(rec.XPos - width, rec.YPos, width, 100);
                     XRect rect = new XRect(rec.XPos - width, rec.YPos, width, font.Height);
                     XTextFormatter tf = new XTextFormatter(gfx);
                     tf.Alignment = XParagraphAlignment.Right;
                     tf.DrawString(mergeDataDict[rec.FieldName], font, XBrushes.Black, rect, XStringFormats.TopLeft);
                  }
                  else
                     gfx.DrawString(mergeDataDict[rec.FieldName], font, XBrushes.Black, new XRect(rec.XPos, rec.YPos, pageWidth, pageHeight), XStringFormats.TopLeft);
                  gfx.Dispose();
                  //Console.WriteLine("===" + mergeDataDict[rec.FieldName] + "===");
               }
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
            return MERGE_PRINT_ERROR;
         }
         return 0;
      }

      public class DocumentMergeDataRecord
      {
         public string FieldName { get; set; } = "";
         public string FontName { get; set; } = "";
         public int FontSize { get; set; } = 12;
         public string FontStyle { get; set; } = "R";
         public int Page { get; set; }
         public string Justify { get; set; } = "L";
            public double XPos { get; set; } = 0;
         public double YPos { get; set; } = 0;
      }

   }
}

