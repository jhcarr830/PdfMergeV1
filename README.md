# MergeTest2

Merge data onto existing individual pdf files, combining all output into a single pdf file.

**This is a Windows .NET 9 Console application**

Example use: Printing Mortgage Loan Documents.

**Configuration:**

File name: PdfMerge.cfg (must be present in working directory)

Example of configuration file contents:

```
PathToDataFiles=C:\Users\jhcarr\Projects\PdfMergeDataFiles\
PathToSourceFiles=C:\Users\jhcarr\Projects\PdfSourceFiles\
PathToOutputFiles=C:\Users\jhcarr\Projects\PdfOutputFiles\
PathToFontFiles=C:\Users\jhcarr\Projects\FontFiles\
PathToMergeFiles=C:\Users\jhcarr\Projects\MergeInfoFiles\
```

Note that there are no spaces in any line, and each path must end with a backslash

**Necessary Directories and Files**

The directories are specified in the configuration file, illustrated above.

- PathToDataFiles: Contains Document Set lists, in tab-delimited format, and the Merge Data for each document named in the document set (MergeData.tab)
- PathToSourceFiles: Contains PDF documents to be merged into (These are not modified by the program)
- PathToOutputFiles: Directory where intermediate and combined merged pdf files are generated. Note that intermediate files are erased after combining into the final file, named CombinedDocs.pdf
- PathToFontFiles: Contains ttf files. Necessary files are: arial.ttf, arialbd.ttf, arialbi.ttf, ariali.ttf, times.ttf, timesbd.ttf, timesbi.ttf, and timesi.ttf. Using other fonts would require different ttf files and the original source would need to be modified and recompiled.
- PathToMergeFiles: Contains Merge information for each document to be filled. This is a tab-delimited file, wherein each line contains the Field Name, Font Name, Font Size, Font Style, Page Number, X-Position, and Y-Position

---

##Publishing information from Copilot:

```
I have created a C# Console app with Visual Studio 2022. In the final release folder, the resulting executable file (MergeTest2.exe) exists along with various other files, including .dll, .json, .pdf. Can I create a single executable file, for distribution, without the need for these additional files?
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
