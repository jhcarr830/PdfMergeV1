# PdfMergeV1

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

- PathToDataFiles: Contains Document Set lists, in tab-delimited format, and the Merge Data for each document named in the document set (**MergeData.tab**)
- PathToSourceFiles: Contains PDF documents to be merged into (These are not modified by the program)
- PathToOutputFiles: Directory where intermediate and combined merged pdf files are generated. Note that intermediate files are erased after combining into the final file, named CombinedDocs.pdf
- PathToFontFiles: Contains ttf files. Necessary files are: arial.ttf, arialbd.ttf, arialbi.ttf, ariali.ttf, times.ttf, timesbd.ttf, timesbi.ttf, and timesi.ttf. Using other fonts would require different ttf files and the original source would need to be modified and recompiled.
- PathToMergeFiles: Contains Merge information for each document to be filled. This is a tab-delimited file, wherein each line contains the Field Name, Font Name, Font Size, Font Style, Page Number, X-Position, and Y-Position
