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

Note that there are no spaces in any line, and each path must end with a backslash (for Windows)

**Necessary Directories and Files**

The directories are specified in the configuration file, illustrated above.

- PathToDataFiles: Contains Document Set lists, in tab-delimited format, and the Merge Data for each document named in the document set (**MergeData.tab**)
- PathToSourceFiles: Contains PDF documents to be merged into (These are not modified by the program)
- PathToOutputFiles: Directory where intermediate and combined merged pdf files are generated. Note that intermediate files are erased after combining into the final file, named CombinedDocs.pdf
- PathToFontFiles: Contains ttf files. Necessary files are: arial.ttf, arialbd.ttf, arialbi.ttf, ariali.ttf, times.ttf, timesbd.ttf, timesbi.ttf, and timesi.ttf. Using other fonts would require different ttf files and the original source would need to be modified and recompiled.
- PathToMergeFiles: Contains Merge information for each document to be filled. This is a tab-delimited file, wherein each line contains the Field Name, Font Name, Font Size, Font Style, Page Number, X-Position, and Y-Position

## Example input files:

Note that these files are plain ascii text files. When the lines in file contain more than one value, the values must be separated by a tab character (ascii value 9, or \t). To create these files, you must use a text editor that saves the tab as an individual character.

In these files, you may place comments by beginning the line with an octothorpe, also known as a hashtag or pound sign (#). There should be no leading white space in each line.

## MergeData File

Example MergeData file (MergeData.tab):

```
#FieldName\tFieldValue
Borrower1FullName\tJohn Schmidt
Borrower2FullName\tMary Schmidt
Borrower3FullName\tJose Moreno
Borrower4FullName\tRachel Smith
PropertyStreet\t95 Sweden Cir
PropertyCity\tSilverton
PropertyState\tOR
PropertyZip\t97381
LegalDescription1\tLot 13, Block 221
LegalDescription2\tas recorded on Page 231 in the Book of Plats
LegalDescription3\tin the County of Marion
LegalDescription4\tState of Oregon
LegalDescription5\tLegalDescription6\t
LoanNumber\t2025001
Principal\t350,000.00
InterestRate\t5.25
LoanTerm\t360
```

MergeData.tab (you may use a different name if you prefer) is used for retrieving data to be merged into every document. If a document references a field name that is not present in MergeData.tab, the program will abort with an error message. Any reference in any document is case sensitive; i.e., "LoanTerm" (in this example) will work, but "loanTerm", "loanterm", and "LOANTERM" will fail.

The error message will contain text similar to this:

```
"System.Collections.Generic.KeyNotFoundException: The given key 'loanTerm' was not present in the dictionary. ..."
```

## Document Merge File

This file is tab-delimited, each line containing 7 values: FieldName, FontName, FontSize, FontStyle, XPos, and YPos.

- FieldName is the name of the field to be merged into the specified location. It must match a field name in the MergeData.tab file, described above.
- FontName is the font to use. Valid values are "Arial" and "Times".
- FontSize is the point size of the field value that will print. To be readable, normal font sizes would be 10 points or 12 points. Larger font sizes would be used for headings, etc. Smaller font sizes may be disputed in court if the document is a legal contract. Attorney advice is recommended.
- FontStyle is R (Regular), B (Bold), I (Italic), and BI (Bold Italic).
- XPos is the X position (distance from left edge, in points) for the beginning of the value to be printed.
- YPos is the Y position (distance from the top edge, in points) for the beginning of the value to be printed.

Note that positions are always computed in points. There are 72 points per inch. For example, to space text a half inch apart, make the YPos of subsequent lines begin 36 points below the current one. For six lines per inch (standard typewriter spacing), use 12 points.

Example Document Merge file (LoanDataSheet.mrg):

```
#FieldName\tFontName\tFontSize\tFontStyle\tPage\tXPos\tYPos
Borrower1FullName\tArial\t12\tB\t1\t180\t93
Borrower2FullName\tArial\t12\tB\t1\t180\t105
Borrower3FullName\tArial\t12\tB\t1\t180\t117
Borrower4FullName\tArial\t12\tB\t1\t180\t129
PropertyStreet\tArial\t12\tI\t1\t180\t145
PropertyCity\tArial\t12\tR\t1\t180\t159
PropertyState\tArial\t12\tR\t1\t180\t175
PropertyZip\tArial\t12\tR\t1\t180\t190
LegalDescription1\tArial\t12\tBI\t1\t180\t216
LegalDescription2\tArial\t12\tBI\t1\t180\t232
Principal\tArial\t12\tR\t1\t180\t245
InterestRate\tArial\t12\tR\t1\t180\t258
LoanTerm\tArial\t12\tR\t1\t180\t271
```

## Document Set File

The Document Set File contains a list of documents to be printed. This file is NOT tab-delimited, because there is only one value per line; the name of the document. (Formerly there were additional values, no longer referenced.) The document name is not case-sensitive, because in windows, it is not required. Each document name in the Document Set must have 2 corresponding files; a .pdf file and a .mrg file. The .pdf file is the source document into which the data from MergeData.tab will be merged. The .mrg file is the list of fields to be merged into that document (described above as LoanDataSheet.mrg).

To print multiple copies of a file, simply add its name multiple times in the document set.

Example Document Set File (DocSet1.tab)

```
#DocumentName
COVER001A
LOANDATASHEET
CADOT_07_2021
MSNOTE_07_2021
#3DAYRTCB1
#3DAYRTCB2
EODLENDER
COVER001B
LOANDATASHEET
CADOT_07_2021
MSNOTE_07_2021
#3DAYRTCB1
#3DAYRTCB2
EODBORROWER
```

For further information, see the sample files supplied with the program.

This program is licensed under the MIT license. It is free to use, but may not be sold. Source code is found at https://github.com/jhcarr830/PdfMergeV1. The author is available for consultation.

```
January 20, 2025

James H Carr
Silverton, Oregon
jhcarr@gmail.com
jhcarr830@msn.com
818-854-2175
```
