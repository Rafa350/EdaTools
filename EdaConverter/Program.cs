﻿namespace MikroPic.EdaTools.v1.Converter {

    using MikroPic.EdaTools.v1.Core.Import;
    using MikroPic.EdaTools.v1.Core.Import.Eagle;
    using MikroPic.EdaTools.v1.Core.Model;
    using MikroPic.EdaTools.v1.Core.Model.IO;
    using System.IO;

    class Program {

        static void Main(string[] args) {

            Importer importer = new EagleImporter();
            Board board = importer.Read(args[0]);

            BoardStreamWriter boardWriter = new BoardStreamWriter(
                new FileStream(args[1], FileMode.Create, FileAccess.Write, FileShare.None));
            boardWriter.Write(board);
        }
    }
}

