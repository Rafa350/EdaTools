namespace EdaDebugTest {

    using System;
    using System.IO;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Geometry.Polygons.Infrastructure;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.PanelElements;
    using MikroPic.EdaTools.v1.Pcb.Model.BoardElements;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;

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
            board.AddLayer(new Layer(Layer.ProfileName, BoardSide.Unknown, LayerFunction.Unknown, Color.Parse("255, 255, 255, 255")));



            List<Polygon> polygons = new List<Polygon>();
            foreach (var element in panel.Elements) {
                if (element is PlaceElement) {
                    PlaceElement place = (PlaceElement)element;
                    Polygon polygon = place.Board.GetOutlinePolygon();
                    polygons.Add(polygon);
                }
            }

            return board;
        }

        private static void MergeLayers(Board dstBoard, Board srcBoard) {

            // Genera les capes
            //
            foreach (Layer srcLayer in srcBoard.Layers) {
                if (dstBoard.GetLayer(srcLayer.Name, false) == null) {
                    Layer layer = new Layer(srcLayer.Name, srcLayer.Side, srcLayer.Function, srcLayer.Color, srcLayer.IsVisible);
                    dstBoard.AddLayer(layer);
                }
            }
        }
    }
}
