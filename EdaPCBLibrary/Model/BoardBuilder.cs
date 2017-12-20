namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Collections;

    public sealed class BoardBuilder {

        private Board board;

        public Board CreateBoard() {

            board = new Board();

            Color topColor = Colors.Red; 
            Color bottomColor = Colors.Blue;
            Color innerColor = Colors.Yellow;
            Color namesColor = Colors.White;
            Color valuesColor = Colors.White;
            Color stopColor = Colors.DarkGreen;
            Color creamColor = Colors.DarkSalmon;
            Color glueColor = Colors.DarkTurquoise;
            Color documentColor = Colors.LightGray;
            Color restrictColor = Color.FromArgb(64, Colors.DarkViolet.R, Colors.DarkViolet.G, Colors.DarkViolet.B) ;
            Color keepoutColor = Color.FromArgb(64, Colors.Cyan.R, Colors.Cyan.G, Colors.Cyan.B);
            Color padColor = Colors.DarkGoldenrod;
            Color viaColor = Colors.Green;
            Color holeColor = Colors.LightCoral;
            Color measuresColor = Colors.Yellow;

            // Defineix les capes conductores
            //
            /*Layer bottomLayer = new Layer(LayerId.Bottom, "Bottom", bottomColor, true, true);
            board.Layers.Add(bottomLayer);
            board.Layers(new Layer(LayerId.Top, "Top", topColor, true, bottomLayer));
            
            board.Layer(new Layer(LayerId.Inner2, "Inner2", innerColor, true));
            board.Layer(new Layer(LayerId.Inner3, "Inner3", innerColor, true));
            board.Layer(new Layer(LayerId.Inner4, "Inner4", innerColor, true));
            board.Layer(new Layer(LayerId.Inner5, "Inner5", innerColor, true));
            board.Layer(new Layer(LayerId.Inner6, "Inner6", innerColor, true));
            board.Layer(new Layer(LayerId.Inner7, "Inner7", innerColor, true));
            board.Layer(new Layer(LayerId.Inner8, "Inner8", innerColor, true));
            board.Layer(new Layer(LayerId.Inner9, "Inner9", innerColor, true));
            board.Layer(new Layer(LayerId.Inner10, "Inner10", innerColor, true));
            board.Layer(new Layer(LayerId.Inner11, "Inner11", innerColor, true));
            board.Layer(new Layer(LayerId.Inner12, "Inner12", innerColor, true));
            board.Layer(new Layer(LayerId.Inner13, "Inner13", innerColor, true));
            board.Layer(new Layer(LayerId.Inner14, "Inner14", innerColor, true));
            board.Layer(new Layer(LayerId.Inner15, "Inner15", innerColor, true));

            board.Layer(new Layer(LayerId.Unrouted, "Unrouted", Colors.Salmon, true));

            // Defineix les caper de vies, pads, holes, etc
            //
            board.Layer(new Layer(LayerId.Vias, "Vias", viaColor, true));
            board.Layer(new Layer(LayerId.Pads, "Pads", padColor, true));
            board.Layer(new Layer(LayerId.Holes, "Holes", holeColor, true));
            board.Layer(new Layer(LayerId.Drills, "Drills", holeColor, true));

            // Defineix la capa de mesures
            //
            board.Layer(new Layer(LayerId.Profile, "Profile", measuresColor, true));

            // Defineix les capes de documentacio
            //
            Layer bottomNames = new Layer(LayerId.BottomNames, "BottomNames", namesColor, true, true);
            board.Layer(bottomNames);
            board.Layer(new Layer(LayerId.TopNames, "TopNames", namesColor, true, bottomNames));

            Layer bottomValues = new Layer(LayerId.BottomValues, "BottomValues", valuesColor, false, true);
            board.Layer(bottomValues);
            board.Layer(new Layer(LayerId.TopValues, "TopValues", valuesColor, false, bottomValues));

            Layer bottomDocument = new Layer(LayerId.BottomDocument, "BottomDocument", documentColor, true, true);
            board.Layer(bottomDocument);
            board.Layer(new Layer(LayerId.TopDocument, "TopDocument", documentColor, true, bottomDocument));

            Layer bottomPlace = new Layer(LayerId.BottomPlace, "BottomPlace", documentColor, true, true);
            board.Layer(bottomPlace);
            board.Layer(new Layer(LayerId.TopPlace, "TopPlace", documentColor, true, bottomPlace));

            // Defineix les capes de fabricacio
            //
            board.Layer(new Layer(LayerId.TopGlue, "TopGlue", glueColor, true));
            board.Layer(new Layer(LayerId.BottomGlue, "BottomGlue", glueColor, true, true));

            board.AddLayer(new Layer(LayerId.TopCream, "TopCream", creamColor, true));
            board.AddLayer(new Layer(LayerId.BottomCream, "BottomCream", creamColor, true));

            board.AddLayer(new Layer(LayerId.TopStop, "TopStop", stopColor, true));
            board.AddLayer(new Layer(LayerId.BottomStop, "BottomStop", stopColor, true, true));

            // Defineix les capes de restriccions
            //
            board.AddLayer(new Layer(LayerId.BottomRestrict, "BottomRestrict", restrictColor, true, true));
            board.AddLayer(new Layer(LayerId.TopRestrict, "TopRestrict", restrictColor, true));

            board.AddLayer(new Layer(LayerId.ViaRestrict, "ViaRestrict", restrictColor, true));

            board.Layers.Add(new Layer(LayerId.BottomKeepout, "BottomKeepout", keepoutColor, true, true));
            board.Layers.Add(new Layer(LayerId.TopKeepout, "TopKeepout", keepoutColor, true));
            */
            return board;
        }
    
        public Board Board {
            get {
                return board;
            }
        }
    }
}
