namespace MikroPic.EdaTools.v1.Panelizer {

    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using MikroPic.EdaTools.v1.Panel.Model.IO;
    using System.IO;

    class Program {

        static void Main(string[] args) {

            ShowCredits();

            if (args.Length == 0)
                ShowHelp();

            else {

                string projectFileName = Path.GetFullPath(args[0]);
                string boardFileName = Path.ChangeExtension(projectFileName, ".xbrd");

                Project panel = LoadPanel(projectFileName);
                Board board = GenerateBoard(panel);
                SaveBoard(board, boardFileName);
            }
        }

        private static Project LoadPanel(string fileName) {

            string folder = Path.GetDirectoryName(fileName);

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                ProjectStreamReader reader = new ProjectStreamReader(stream);

                Project panel = reader.Read();

                foreach (var element in panel.Items) {
                    if (element is PcbItem) {
                        PcbItem place = (PcbItem)element;
                        place.FileName = Path.Combine(folder, place.FileName);
                    }
                }

                return panel;
            }
        }

        private static void SaveBoard(Board board, string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                BoardStreamWriter writer = new BoardStreamWriter(stream);
                writer.Write(board);
            }
        }

        private static Board GenerateBoard(Project panel) {

            Board board = new Board();

            Panelizer panelizer = new Panelizer(board);
            panelizer.Panelize(panel);

            return board;
        }

        private static void ShowCredits() {

        }

        private static void  ShowHelp() {

        }
    }
}
