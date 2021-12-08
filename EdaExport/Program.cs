using System;
using System.IO;
using MikroPic.EdaTools.v1.Core.Export.KiCad;
using MikroPic.EdaTools.v1.Core.Model.Board.IO;

namespace MikroPic.EdaTools.v1.Export {

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

            string targetExt = Path.GetExtension(targetPath);

            if (String.Compare(targetExt, ".pretty", true) == 0) {

                using (var stream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    var reader = new LibraryStreamReader(stream);
                    var library = reader.Read();

                    var exporter = new KiCadExporter();
                    exporter.WriteLibrary(targetPath, library);
                }
            }
        }

        /// <summary>
        /// Mostra els credits del programa.
        /// </summary>
        /// 
        private static void ShowCredits() {

            string credits =
                "+--------------------------------------------------------------------\r\n" +
                "| EdaExport V2.1 - (c) 2019..2021 rsr.openware@gmail.com\r\n" +
                "+--------------------------------------------------------------------";

            Console.WriteLine(credits);
        }

    }
}