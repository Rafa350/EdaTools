namespace MikroPic.EdaTools.v1.Converter {

    using System;
    using System.IO;
    using MikroPic.EdaTools.v1.Core.Import;
    using MikroPic.EdaTools.v1.Core.Import.Eagle;
    using MikroPic.EdaTools.v1.Core.Import.KiCad;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;
    using MikroPic.EdaTools.v1.Core.Model.Net;

    class Program {

        static void Main(string[] args) {

            ShowCredits();

            bool verbose = false;
            string sourcePath = args[0];
            string targetPath = args[1];

            foreach (var arg in args) {

                if (arg == "/v")
                    verbose = true;
            }

            if (verbose) {
                Console.WriteLine("| Source path : {0}", sourcePath);
                Console.WriteLine("| Target path : {0}", targetPath);
                Console.WriteLine("+--------------------------------------------------------------------");
                Console.WriteLine();
            }

            if (String.Compare(Path.GetExtension(sourcePath), ".brd", true) == 0) {

                Importer importer = new EagleImporter();
                Board board = importer.ReadBoard(sourcePath);

                BoardStreamWriter boardWriter = new BoardStreamWriter(
                    new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None));
                boardWriter.Write(board);
            }

            else if (String.Compare(Path.GetExtension(sourcePath), ".sch", true) == 0) {

                Importer importer = new EagleImporter();
                Net net = importer.ReadNet(sourcePath);

                NetStreamWriter netWriter = new NetStreamWriter(
                    new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None));
                netWriter.Write(net);
            }

            else if (String.Compare(Path.GetExtension(sourcePath), ".lbr", true) == 0) {

                Importer importer = new EagleImporter();
                Library library = importer.ReadLibrary(sourcePath);

                BoardStreamWriter boardWriter = new BoardStreamWriter(
                    new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None));
                boardWriter.Write(library);
            }

            else if (String.Compare(Path.GetExtension(sourcePath), ".kicad_pcb", true) == 0) {

                Importer importer = new KiCadImporter();
                Board board = importer.ReadBoard(sourcePath);

                BoardStreamWriter boardWriter = new BoardStreamWriter(
                    new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None));
                boardWriter.Write(board);
            }
        }

        /// <summary>
        /// Mostra els credits del programa.
        /// </summary>
        /// 
        private static void ShowCredits() {

            string credits =
                "+--------------------------------------------------------------------\r\n" +
                "| EdaConverter V1.0 - (c) 2019 rsr.openware@gmail.com\r\n" +
                "+--------------------------------------------------------------------";

            Console.WriteLine(credits);
        }
    }
}
