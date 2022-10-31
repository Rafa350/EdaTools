﻿using System.IO;
using MikroPic.EdaTools.v1.Core.IO;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Extractor {

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

                EdaBoard board = LoadBoard(inputFileName);
                ExtractData(board, outputFileName);
            }
        }

        private static void ShowCredits() {

        }

        private static void ShowHelp() {

        }

        private static EdaBoard LoadBoard(string fileName) {

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                var reader = new EdaBoardStreamReader(stream);
                return reader.ReadBoard();
            }
        }

        private static void ExtractData(EdaBoard board, string fileName) {

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (var writer = new StreamWriter(stream)) {
                    var extractor = new PartExtractor(board);
                    extractor.Extract(writer);
                }
            }
        }
    }
}
