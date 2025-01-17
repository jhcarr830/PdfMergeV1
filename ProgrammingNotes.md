# Programming Notes

## Publishing information from Copilot:

```
I have created a C# Console app with Visual Studio 2022. In the final release folder, the resulting executable file (PdfMergeV1.exe) exists along with various other files, including .dll, .json, .pdf. Can I create a single executable file, for distribution, without the need for these additional files?
```

Yes, it's possible to create a single executable file for distribution. You can use the .NET Core single-file deployment option. Here's how you can do it:

Modify your project file (csproj): Open your .csproj file and add the following properties within a \<PropertyGroup\>:

xml:

```
<PublishSingleFile>true</PublishSingleFile>
<RuntimeIdentifier>win-x64</RuntimeIdentifier>
<SelfContained>true</SelfContained>
```

Publish your application: Use the dotnet publish command with the specified properties. You can run the following command in the terminal or command prompt:

sh

dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

This will generate a single executable file in the bin\Release\netcoreappX.X\win-x64\publish directory. Replace X.X with the version of .NET Core/5/6 you're using.

This method bundles everything your application needs into a single executable file, making it easier to distribute. However, keep in mind that the single executable might be larger in size compared to the original executable file. Let me know if you need further assistance!

## Example code using PdfSharp

```
// combine multiple pdfs into one pdf file:
using System;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

class Program
{
    static void Main(string[] args)
    {
        string[] files = Directory.GetFiles("path_to_your_pdf_files");
        PdfDocument outputDocument = new PdfDocument();

        foreach (string file in files)
        {
            PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
            foreach (PdfPage page in inputDocument.Pages)
            {
                outputDocument.AddPage(page);
            }
            inputDocument.Dispose();
        }

        outputDocument.Save("combined.pdf");
    }
}


//save merged pdf file as a new file:


using System;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

class Program
{
    static void Main(string[] args)
    {
        // Define the path to the PDF files and the output file
        string[] files = Directory.GetFiles("path_to_your_pdf_files");
        string outputFilePath = "path_to_save_combined/combined.pdf";

        // Create a new PDF document
        PdfDocument outputDocument = new PdfDocument();

        // Iterate through each PDF file and add their pages to the output document
        foreach (string file in files)
        {
            PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
            foreach (PdfPage page in inputDocument.Pages)
            {
                outputDocument.AddPage(page);
            }
            inputDocument.Dispose();
        }

        // Save the merged PDF document as a new file
        outputDocument.Save(outputFilePath);
    }
}
```
