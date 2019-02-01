﻿namespace MikroPic.EdaTools.v1.Converter {

    using MikroPic.EdaTools.v1.Core.Import;
    using MikroPic.EdaTools.v1.Core.Import.Eagle;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;
    using System;
    using System.IO;

    class Program {

        static void Main(string[] args) {

            ShowCredits();

            bool verbose = false;
            string sourcePath = args[0];

            foreach (var arg in args) {

                if (arg == "/v")
                    verbose = true;
            }

            if (verbose) {
                Console.WriteLine("| Source path : {0}", sourcePath);
                Console.WriteLine("+--------------------------------------------------------------------");
                Console.WriteLine();
            }

            Importer importer = new EagleImporter();
            Board board = importer.Read(sourcePath);

            BoardStreamWriter boardWriter = new BoardStreamWriter(
                new FileStream(args[1], FileMode.Create, FileAccess.Write, FileShare.None));
            boardWriter.Write(board);
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
