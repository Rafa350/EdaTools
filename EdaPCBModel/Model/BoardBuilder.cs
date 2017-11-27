namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

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
            Layer bottomLayer = new Layer(LayerId.Bottom, "Bottom", bottomColor, true, true);
            board.AddLayer(bottomLayer);
            board.AddLayer(new Layer(LayerId.Top, "Top", topColor, true, bottomLayer));
            
            board.AddLayer(new Layer(LayerId.Inner2, "Inner2", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner3, "Inner3", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner4, "Inner4", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner5, "Inner5", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner6, "Inner6", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner7, "Inner7", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner8, "Inner8", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner9, "Inner9", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner10, "Inner10", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner11, "Inner11", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner12, "Inner12", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner13, "Inner13", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner14, "Inner14", innerColor, true));
            board.AddLayer(new Layer(LayerId.Inner15, "Inner15", innerColor, true));

            board.AddLayer(new Layer(LayerId.Unrouted, "Unrouted", Colors.Salmon, true));

            // Defineix les caper de vies, pads, holes, etc
            //
            board.AddLayer(new Layer(LayerId.Vias, "Vias", viaColor, true));
            board.AddLayer(new Layer(LayerId.Pads, "Pads", padColor, true));
            board.AddLayer(new Layer(LayerId.Holes, "Holes", holeColor, true));

            // Defineix la capa de mesures
            //
            board.AddLayer(new Layer(LayerId.Measures, "Measures", measuresColor, true));

            // Defineix les capes de documentacio
            //
            Layer bottomNames = new Layer(LayerId.BottomNames, "BottomNames", namesColor, true, true);
            board.AddLayer(bottomNames);
            board.AddLayer(new Layer(LayerId.TopNames, "TopNames", namesColor, true, bottomNames));

            Layer bottomValues = new Layer(LayerId.BottomValues, "BottomValues", valuesColor, false, true);
            board.AddLayer(bottomValues);
            board.AddLayer(new Layer(LayerId.TopValues, "TopValues", valuesColor, false, bottomValues));

            Layer bottomDocument = new Layer(LayerId.BottomDocument, "BottomDocument", documentColor, true, true);
            board.AddLayer(bottomDocument);
            board.AddLayer(new Layer(LayerId.TopDocument, "TopDocument", documentColor, true, bottomDocument));

            Layer bottomPlace = new Layer(LayerId.BottomPlace, "BottomPlace", documentColor, true, true);
            board.AddLayer(bottomPlace);
            board.AddLayer(new Layer(LayerId.TopPlace, "TopPlace", documentColor, true, bottomPlace));

            // Defineix les capes de fabricacio
            //
            board.AddLayer(new Layer(LayerId.TopGlue, "TopGlue", glueColor, true));
            board.AddLayer(new Layer(LayerId.BottomGlue, "BottomGlue", glueColor, true, true));

            board.AddLayer(new Layer(LayerId.TopCream, "TopCream", creamColor, true));
            board.AddLayer(new Layer(LayerId.BottomCream, "BottomCream", creamColor, true));

            board.AddLayer(new Layer(LayerId.TopStop, "TopStop", stopColor, true));
            board.AddLayer(new Layer(LayerId.BottomStop, "BottomStop", stopColor, true, true));

            // Defineix les capes de restriccions
            //
            board.AddLayer(new Layer(LayerId.BottomRestrict, "BottomRestrict", restrictColor, true, true));
            board.AddLayer(new Layer(LayerId.TopRestrict, "TopRestrict", restrictColor, true));

            board.AddLayer(new Layer(LayerId.ViaRestrict, "ViaRestrict", restrictColor, true));

            board.AddLayer(new Layer(LayerId.BottomKeepout, "BottomKeepout", keepoutColor, true, true));
            board.AddLayer(new Layer(LayerId.TopKeepout, "TopKeepout", keepoutColor, true));

            return board;
        }

        public Component CreateComponent(Board board, string name) {

            if (board == null)
                throw new ArgumentNullException("board");

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Component component = new Component();
            board.AddComponent(component);

            component.Name = name;

            return component;
        }

        public Part CreatePart(string name, Point position, bool mirror, Component component) {

            Part part = new Part();
            part.Name = name;
            part.Position = position;
            part.IsMirror = mirror;
            part.Component = component;

            board.AddPart(part);

            return part;
        }

        public Signal CreateSignal(string name) {

            Signal signal = new Signal();
            
            board.AddSignal(signal);

            return signal;
        }

        /// <summary>
        /// Crea una via.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany de la corona.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// <param name="shape">Formade la corona.</param>
        /// <param name="upper">Capa superior.</param>
        /// <param name="lower">Capa inferior.</param>
        /// <returns>El objecte Via creat.</returns>
        /// 
        public ViaElement CreateVia(Point position, double size, double drill, ViaElement.ViaShape shape, Layer upper, Layer lower) {

            if (upper == null)
                throw new ArgumentNullException("upper");

            if (lower == null)
                throw new ArgumentNullException("lower");

            ViaElement via = new ViaElement();
            via.Position = position;
            via.Size = size;
            via.Drill = drill;
            via.Shape = shape;
            via.Layer = board.GetLayer(LayerId.Vias);
            via.Upper = upper;
            via.Lower = lower;

            return via;
        }

        public ThPadElement CreateThPad(Point position, double rotate, ThPadElement.ThPadShape shape, double size, double drill, string name) {

            ThPadElement pad = new ThPadElement();
            pad.Position = position;
            pad.Rotate = rotate;
            pad.Shape = shape;
            pad.Size = size;
            pad.Drill = drill;
            pad.Name = name;
            pad.Layer = board.GetLayer(LayerId.Pads);

            return pad;
        }

        public SmdPadElement CreateSmdPad(Point position, Size size, double radius, double rotate, string name, Layer layer) {

            SmdPadElement pad = new SmdPadElement();
            pad.Position = position;
            pad.Size = size;
            pad.Roundnes = radius;
            pad.Rotate = rotate;
            pad.Name = name;
            pad.Layer = layer;

            return pad;
        }

        public ArcElement CreateArc(Point startPosition, Point endPosition, double thickness, LineElement.LineCapStyle lineCap, double angle, Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            ArcElement arc = new ArcElement();
            arc.StartPosition = startPosition;
            arc.EndPosition = endPosition;
            arc.Thickness = thickness;
            arc.LineCap = lineCap;
            arc.Angle = angle;
            arc.Layer = layer;

            return arc;
        }

        public CircleElement CreateCircle(Point position, double radius, double thickness, Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            CircleElement circle = new CircleElement();
            circle.Position = position;
            circle.Radius = radius;
            circle.Thickness = thickness;
            circle.Layer = layer;

            return circle;
        }

        public LineElement CreateLine(Point startPosition, Point endPosition, double thickness, LineElement.LineCapStyle lineCap, Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            LineElement line = new LineElement();
            line.StartPosition = startPosition;
            line.EndPosition = endPosition;
            line.Thickness = thickness;
            line.LineCap = lineCap;
            line.Layer = layer;

            return line;
        }
       
        public RectangleElement CreateRectangle(Point position, Size size, double rotate, double thickness, Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            RectangleElement rectangle = new RectangleElement();
            rectangle.Position = position;
            rectangle.Size = size;
            rectangle.Rotate = rotate;
            rectangle.Thickness = thickness;
            rectangle.Layer = layer;

            return rectangle;
        }

        public PolygonElement CreatePolygon(Point position, double rotate, double thickness, Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            PolygonElement polygon = new PolygonElement();
            polygon.Position = position;
            polygon.Thickness = thickness;
            polygon.Layer = layer;

            return polygon;
        }

        public TextElement CreateText(Point position, double rotate, double height, Layer layer, string name, string value) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            TextElement text = new TextElement();
            text.Position = position;
            text.Rotate = rotate;
            text.Height = height;
            text.Layer = layer;
            text.Name = name;
            text.Value = value;

            return text;
        }

        public HoleElement CreateHole(Point position, double drill) {

            HoleElement hole = new HoleElement();
            hole.Position = position;
            hole.Drill = drill;
            hole.Layer = board.GetLayer(LayerId.Holes);

            return hole;
        }

        public Board Board {
            get {
                return board;
            }
        }
    }
}
