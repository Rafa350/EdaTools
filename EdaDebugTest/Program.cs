namespace EdaDebugTest {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
    using MikroPic.EdaTools.v1.Pcb.Model.PanelElements;
    using System.IO;

    class Program {

        static void Main(string[] args) {

            Panel panel = LoadPanel(@"..\..\..\..\Data\Panel3.XPNL");
            Board board = GenerateBoard(panel);
            SaveBoard(board, @"..\..\..\..\Data\Panel3.XBRD");
        }

        private static Panel LoadPanel(string fileName) {

            string folder = Path.GetDirectoryName(fileName);

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                PanelStreamReader reader = new PanelStreamReader(stream);

                Panel panel = reader.Read();

                foreach (var element in panel.Elements) {
                    if (element is PlaceElement) {
                        PlaceElement place = (PlaceElement)element;
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

        private static Board GenerateBoard(Panel panel) {

            Board board = new Board();

            Panelizer panelizer = new Panelizer(board);
            panelizer.Panelize(panel);

            return board;
        }
    }
}
