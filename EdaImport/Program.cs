﻿namespace MikroPic.EdaTools.v1 {

    using MikroPic.EdaTools.v1.Pcb.Import;
    using MikroPic.EdaTools.v1.Pcb.Import.Eagle;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
    using MikroPic.EdaTools.v1.Cam;
    using System.IO;

    class Program {

        static void Main(string[] args) {

            Importer importer = new EagleImporter();
            Board board = importer.LoadBoard(@"c:\temp\board3.brd");

            XmlBoardWriter boardWriter = new XmlBoardWriter(
                new FileStream(@"c:\temp\board3.xml", FileMode.Create, FileAccess.Write, FileShare.None));
            boardWriter.Write(board);

            CAMGenerator camGenerator = new CAMGenerator();
            camGenerator.Generate(board, @"c:\temp\board3.cmp");
        }
    }
}
