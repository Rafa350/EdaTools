# EdaTools


Tools and utilities for PCB design.
!!!Very very early version. Runs ok for my works.


EdaImport
-Convert EAGLE v7 BRD file to EdaTools board file (.XBRD)
-Convert EAGLE v7 LBR file to EdaTools library file (.XLIB)
-Convert KiCad pcb file to EdaTools board file (.XBRD)

EdaExport
-Convert EdaTools board file (.XBRD) to EAGLE or KiCAD
-Convert EdaTools library file (.XLIB) to KiCAD footprint library

EdaExtractor.
-Extract information from board (BOM, Centroids, etc)

EdaPanelizer.
-Merge any number of boards to create a single board panel.

EdaCAMTool.
-Create production files from CAM project file (.XCAM).
-Generate gerber X3 image files.
-Generate gerber X3 drill and route files.
-Generate IPC-D356 netlist file.
-Generate IPC-2581C file.
-Compliant with Ucamco Gerber File Format Specification (www.ucamco.com).
-Passed Gerber test with "https://gerber-viewer.ucamco.com/".
-Passed IPC2581C test with 3D PCBA Viewer.

Notes.
-IPC2581C preliminar version no usable.
