namespace MikroPic.EdaTools.v1.Extractor {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
    using System.IO;

    class Program {

        static void Main(string[] args) {

            ShowCredits();

            if (args.Length == 0)
                ShowHelp();

            else {

                string inFileName = args[0];
                string outFileName = "";

                Board board = LoadBoard(inFileName);
                ExtractData(board, outFileName);
            }
        }

        private static void ShowCredits() {

        }

        private static void ShowHelp() {

        }

        private static Board LoadBoard(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                XmlBoardReader reader = new XmlBoardReader(stream);
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
