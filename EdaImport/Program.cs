using System;
using System.IO;
using MikroPic.EdaTools.v1.Core.Import.Eagle;
using MikroPic.EdaTools.v1.Core.Import.KiCad;
using MikroPic.EdaTools.v1.Core.Model.Board.IO;

namespace MikroPic.EdaTools.v1.Import {

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

            string sourceExt = Path.GetExtension(sourcePath);

            if (String.Compare(sourceExt, ".brd", true) == 0) {

                var importer = new EagleImporter();
                var board = importer.ReadBoard(sourcePath);

                var writer = new BoardStreamWriter(new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None));
                writer.Write(board);

                /*var boardWriter2 = new BoardWriter();
                boardWriter2.Write(
                    new FileStream(@"c:\temp\serialized.xml", FileMode.Create, FileAccess.Write, FileShare.None),
                    board);*/
            }

            else if (String.Compare(sourceExt, ".sch", true) == 0) {

                var importer = new EagleImporter();
                var net = importer.ReadNet(sourcePath);

                var writer = new NetStreamWriter(new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None));
                writer.Write(net);
            }

            else if (String.Compare(sourceExt, ".lbr", true) == 0) {

                var importer = new EagleImporter();
                var library = importer.ReadLibrary(sourcePath);

                var writer = new BoardStreamWriter(new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None));
                writer.Write(library);
            }

            else if (String.Compare(sourceExt, ".kicad_pcb", true) == 0) {

                var importer = new KiCadImporter();
                var board = importer.ReadBoard(sourcePath);

                var writer = new BoardStreamWriter(new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None));
                writer.Write(board);
            }
        }

        /// <summary>
        /// Mostra els credits del programa.
        /// </summary>
        /// 
        private static void ShowCredits() {

            string credits =
                "+--------------------------------------------------------------------\r\n" +
                "| EdaImport V2.1 - (c) 2019..2021 rsr.openware@gmail.com\r\n" +
                "+--------------------------------------------------------------------";

            Console.WriteLine(credits);
        }
    }
}
