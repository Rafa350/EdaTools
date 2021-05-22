﻿namespace MikroPic.EdaTools.v1.Extractor {

    using System.IO;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;

    class Program {

        static void Main(string[] args) {

            ShowCredits();

            if (args.Length == 0)
                ShowHelp();

            else {

                string inputFileName = Path.GetFullPath(args[0]);
                string folder = Path.GetDirectoryName(inputFileName);
                string name = Path.GetFileNameWithoutExtension(inputFileName);

                string outputFileName = string.Format(@"{0}\{1}_PartList.xml", folder, name);

                Board board = LoadBoard(inputFileName);
                ExtractData(board, outputFileName);
            }
        }

        private static void ShowCredits() {

        }

        private static void ShowHelp() {

        }

        private static Board LoadBoard(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                BoardStreamReader reader = new BoardStreamReader(stream);
                return reader.Read();
            }
        }

        private static void ExtractData(Board board, string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream)) {
                    PartExtractor extractor = new PartExtractor(board);
                    extractor.Extract(writer);
                }
            }
        }
    }
}
