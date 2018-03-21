rem @echo off

"..\..\MdXMLToolsV1\bin\x86\Release\MdXMLTool.exe" board3_PartList.xml ..\XSL\Export_eCircuits_BOM.xsl Production\eCircuits\eCircuits_BOM.csv
"..\..\MdXMLToolsV1\bin\x86\Release\MdXMLTool.exe" board3_PartList.xml ..\XSL\Export_eCircuits_CPL.xsl Production\eCircuits\eCircuits_CPL.csv

pause