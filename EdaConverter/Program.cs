﻿namespace MikroPic.EdaTools.v1.Converter {

    using MikroPic.EdaTools.v1.Pcb.Import;
    using MikroPic.EdaTools.v1.Pcb.Import.Eagle;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
    using System.IO;

    class Program {

        static void Main(string[] args) {

            Importer importer = new EagleImporter();
            Board board = importer.Read(args[0]);

            XmlBoardWriter boardWriter = new XmlBoardWriter(
                new FileStream(args[1], FileMode.Create, FileAccess.Write, FileShare.None));
            boardWriter.Write(board);
        }
    }
}

