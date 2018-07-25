rem @echo off

rem Genera els fitxers de produccio PCB
"..\bin\x86\Release\EdaCAMTool.exe" board3.xml

rem Genera els fitxers de fabricacio P&P
"..\bin\x86\Release\EdaExtractor.exe" board3.xml
"..\..\XMLTools\bin\x86\Release\MdXMLTool.exe" board3_PartList.xml ..\XSL\Export_eCircuits_BOM.xsl Production\eCircuits\board3_BOM.csv
"..\..\XMLTools\bin\x86\Release\MdXMLTool.exe" board3_PartList.xml ..\XSL\Export_eCircuits_CPL.xsl Production\eCircuits\board3_CPL.csv

pause