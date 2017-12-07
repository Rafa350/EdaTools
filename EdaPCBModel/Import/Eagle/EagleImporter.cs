namespace MikroPic.EdaTools.v1.Pcb.Import.Eagle {

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.IO;
    using System.Xml;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class EagleImporter: Importer {

        private sealed class SmdNodeInfo {

            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public double Radius { get; set; }
            public double Rotate { get; set; }
            public string Name { get; set; }
            public Layer Layer { get; set; }
        }

        private sealed class PadNodeInfo {

            public double X { get; set; }
            public double Y { get; set; }
            public double Rotate { get; set; }
            public double Size { get; set; }
            public double Drill { get; set; }
            public ThPadElement.ThPadShape Shape { get; set; }
            public string Name { get; set; }
        }

        private sealed class WireNodeInfo {

            public double X1 { get; set; }
            public double Y1 { get; set; }
            public double X2 { get; set; }
            public double Y2 { get; set; }
            public double Angle { get; set; }
            public double Thickness { get; set; }
            public LineElement.LineCapStyle LineCap { get; set; }
            public Layer Layer { get; set; }
        }

        private sealed class CircleNodeInfo {

            public double X { get; set; }
            public double Y { get; set; }
            public double Radius { get; set; }
            public double Thickness { get; set; }
            public Layer Layer { get; set; }
        }

        private sealed class RectangleNodeInfo {

            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public double Rotate { get; set; }
            public double Thickness { get; set; }
            public Layer Layer { get; set; }
        }

        private sealed class TextNodeInfo {

            public double X { get; set; }
            public double Y { get; set; }
            public double Rotate { get; set; }
            public double Height { get; set; }
            public TextElement.TextAlign Align { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public Layer Layer { get; set; }
        }

        private sealed class PolygonNodeInfo {

            public double Thickness { get; set; }
            public Layer Layer { get; set; }
        }

        private sealed class VertexNodeInfo {

            public double X { get; set; }
            public double Y { get; set; }
            public double Angle { get; set; }
        }

        private BoardBuilder boardBuilder = new BoardBuilder();
        private Dictionary<int, Layer> layerDict = new Dictionary<int, Layer>();
        private Dictionary<string, Component> componentDict = new Dictionary<string, Component>();

        public override Board LoadBoard(Stream stream) {

            XmlDocument doc = ReadXmlDocument(stream);

            Board board = boardBuilder.CreateBoard();

            CreateLayers(doc, board);
            CreateMeasures(doc, board);
            CreateComponents(doc, board);
            CreateElements(doc, board);
            CreateSignals(doc, board);

            return board;
        }

        public override Library LoadLibrary(Stream stream) {

            XmlDocument doc = ReadXmlDocument(stream);

            throw new NotImplementedException();
        }

        private XmlDocument ReadXmlDocument(Stream stream) {

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            XmlReader reader = XmlReader.Create(stream, settings);

            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            return doc;
        }

        private void CreateLayers(XmlDocument doc, Board board) {

            foreach (XmlNode layerNode in doc.SelectNodes("eagle/drawing/layers/layer")) {

                string name = layerNode.Attributes["name"].Value;
                int layerNum = Int32.Parse(layerNode.Attributes["number"].Value);

                bool ok = true;
                LayerId id = LayerId.UserDefined1;
                switch (layerNum) {
                    case 1:
                        id = LayerId.Top;
                        break;

                    case 16:
                        id = LayerId.Bottom;
                        break;

                    case 19:
                        id = LayerId.Unrouted;
                        break;

                    case 20:
                        id = LayerId.Profile;
                        break;

                    case 21:
                        id = LayerId.TopPlace;
                        break;

                    case 22:
                        id = LayerId.BottomPlace;
                        break;

                    case 25:
                        id = LayerId.TopNames;
                        break;

                    case 26:
                        id = LayerId.BottomNames;
                        break;

                    case 27:
                        id = LayerId.TopValues;
                        break;

                    case 28:
                        id = LayerId.BottomValues;
                        break;

                    case 29:
                        id = LayerId.TopStop;
                        break;

                    case 30:
                        id = LayerId.BottomStop;
                        break;

                    case 31:
                        id = LayerId.TopCream;
                        break;

                    case 32:
                        id = LayerId.BottomCream;
                        break;

                    case 35:
                        id = LayerId.TopGlue;
                        break;

                    case 36:
                        id = LayerId.BottomGlue;
                        break;

                    case 39:
                        id = LayerId.TopKeepout;
                        break;

                    case 40:
                        id = LayerId.BottomKeepout;
                        break;

                    case 41:
                        id = LayerId.TopRestrict;
                        break;

                    case 42:
                        id = LayerId.BottomRestrict;
                        break;

                    case 43:
                        id = LayerId.ViaRestrict;
                        break;

                    case 51:
                        id = LayerId.TopDocument;
                        break;

                    case 52:
                        id = LayerId.BottomDocument;
                        break;

                    default:
                        ok = false;
                        break;
                }

                if (ok) {
                    Layer layer = board.GetLayer(id);
                    if (layer == null) {
                        layer = new Layer(id, name, Colors.White, true);
                        board.AddLayer(layer);
                    }
                    layerDict.Add(layerNum, layer);
                }
            }
        }

        private void CreateMeasures(XmlDocument doc, Board board) {

            foreach (XmlNode node in doc.SelectNodes("eagle/drawing/board/plain/*")) {

                switch (node.Name) {
                    case "wire":
                        WireNodeInfo info = ParseWireNode(node);
                        if (info.Thickness == 0)
                            info.Thickness = 0.01;
                        if (info.Angle == 0)
                            board.AddElement(boardBuilder.CreateLine(new Point(info.X1, info.Y1), new Point(info.X2, info.Y2), info.Thickness, info.LineCap, info.Layer));
                        else
                            board.AddElement(boardBuilder.CreateArc(new Point(info.X1, info.Y1), new Point(info.X2, info.Y2), info.Thickness, info.LineCap, info.Angle, info.Layer));
                        break;
                }
            }
        }

        private void CreateComponents(XmlDocument doc, Board board) {

            foreach (XmlNode libraryNode in doc.SelectNodes("eagle/drawing/board/libraries/library")) {

                string libraryName = libraryNode.Attributes["name"].Value;

                foreach (XmlNode componentNode in libraryNode.SelectNodes("packages/package")) {

                    string componentId = String.Format(
                        "{0}@{1}",
                        componentNode.Attributes["name"].Value,
                        libraryName);

                    Component component = boardBuilder.CreateComponent(board, componentId);

                    foreach (XmlNode node in componentNode.ChildNodes) {

                        switch (node.Name) {

                            case "smd": {
                                    SmdNodeInfo info = ParseSmdNode(node);
                                    component.Add(boardBuilder.CreateSmdPad(new Point(info.X, info.Y), new Size(info.Width, info.Height), info.Radius, info.Rotate, info.Name, info.Layer));
                                }
                                break;

                            case "pad": {
                                    PadNodeInfo info = ParsePadNode(node);
                                    component.Add(boardBuilder.CreateThPad(new Point(info.X, info.Y), info.Rotate, info.Shape, info.Size, info.Drill, info.Name));
                                    break;
                                }

                            case "text": {
                                    TextNodeInfo info = ParseTextNode(node);
                                    component.Add(boardBuilder.CreateText(new Point(info.X, info.Y), info.Rotate, info.Height, info.Layer, info.Name, info.Value));
                                }
                                break;


                            case "wire": {
                                    WireNodeInfo info = ParseWireNode(node);
                                    if (info.Angle == 0)
                                        component.Add(boardBuilder.CreateLine(new Point(info.X1, info.Y1), new Point(info.X2, info.Y2), info.Thickness, info.LineCap, info.Layer));
                                    else
                                        component.Add(boardBuilder.CreateArc(new Point(info.X1, info.Y1), new Point(info.X2, info.Y2), info.Thickness, info.LineCap, info.Angle, info.Layer));
                                }
                                break;

                            case "rectangle": {
                                    RectangleNodeInfo info = ParseRectangleNode(node);
                                    component.Add(boardBuilder.CreateRectangle(new Point(info.X, info.Y), new Size(info.Width, info.Height), info.Rotate, info.Thickness, info.Layer));
                                }
                                break;

                            case "circle": {
                                    CircleNodeInfo info = ParseCircleNode(node);
                                    component.Add(boardBuilder.CreateCircle(new Point(info.X, info.Y), info.Radius, info.Thickness, info.Layer));
                                }
                                break;

                            case "polygon": {
                                    PolygonNodeInfo info = ParsePolygonNode(node);
                                    PolygonElement polygon = boardBuilder.CreatePolygon(new Point(0, 0), 0, info.Thickness, info.Layer);
                                    CreateVertexList(node, polygon);
                                    component.Add(polygon);
                                }
                                break;

                            case "hole":
                                component.Add(ParseHoleNode(node));
                                break;
                        }
                    }

                    componentDict.Add(componentId, component);
                }
            }
        }

        private void CreateElements(XmlDocument doc, Board board) {

            foreach (XmlNode node in doc.SelectNodes("eagle/drawing/board/elements/element"))
                board.AddPart(ParseElementNode(node));
        }

        private void CreateSignals(XmlDocument doc, Board board) {

            foreach (XmlNode signalNode in doc.SelectNodes("eagle/drawing/board/signals/signal")) {

                string signalName = GetAttribute(signalNode, "name");

                Signal signal = new Signal();
                signal.Name = signalName;

                foreach (XmlNode node in signalNode.ChildNodes) {

                    switch (node.Name) {
                        case "via": 
                            signal.Add(ParseViaNode(node));
                            break;

                        case "wire": {
                                WireNodeInfo info = ParseWireNode(node);
                                if (info.Angle == 0)
                                    signal.Add(boardBuilder.CreateLine(new Point(info.X1, info.Y1), new Point(info.X2, info.Y2), info.Thickness, info.LineCap, info.Layer));
                                else
                                    signal.Add(boardBuilder.CreateArc(new Point(info.X1, info.Y1), new Point(info.X2, info.Y2), info.Thickness, info.LineCap, info.Angle, info.Layer));
                            }
                            break;
                    }
                }

                board.AddSignal(signal);
            }
        }

        private void CreateVertexList(XmlNode polygonNode, PolygonElement polygon) {

            bool first = true;
            foreach (XmlNode vertexNode in polygonNode.SelectNodes("vertex")) {

                VertexNodeInfo info = ParseVertexNode(vertexNode);

                if (first) {
                    polygon.Position = new Point(info.X, info.Y);
                    first = false;
                }
                else {
                    if (info.Angle == 0)
                        polygon.AddLine(new Point(info.X, info.Y));
                    else
                        polygon.AddArc(new Point(info.X, info.Y), info.Angle);
                }
            }
        }

        /// <summary>
        /// Procesa un node de tipus PAD.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>Informacio recolectada del node.</returns>
        /// 
        private PadNodeInfo ParsePadNode(XmlNode node) {

            PadNodeInfo info = new PadNodeInfo();

            info.Name = GetAttribute(node, "name");

            info.X = StrToDouble(GetAttribute(node, "x"));
            info.Y = StrToDouble(GetAttribute(node, "y"));
            info.Rotate = StrToDouble(GetAttribute(node, "rot"));

            info.Drill = StrToDouble(GetAttribute(node, "drill"));
            info.Size = info.Drill * 1.6;

            info.Shape = ThPadElement.ThPadShape.Circular;
            switch (GetAttribute(node, "shape")) {
                case "square":
                    info.Shape = ThPadElement.ThPadShape.Square;
                    break;

                case "octagon":
                    info.Shape = ThPadElement.ThPadShape.Octogonal;
                    break;

                case "long":
                    info.Shape = ThPadElement.ThPadShape.Oval;
                    break;
            }

            return info;
        }

        /// <summary>
        /// Procesa un node de tipus SMD.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>Informacio recolectada del node.</returns>
        /// 
        private SmdNodeInfo ParseSmdNode(XmlNode node) {

            SmdNodeInfo info = new SmdNodeInfo();

            info.Name = GetAttribute(node, "name");

            info.X = StrToDouble(GetAttribute(node, "x"));
            info.Y = StrToDouble(GetAttribute(node, "y"));
            info.Width = StrToDouble(GetAttribute(node, "dx"));
            info.Height = StrToDouble(GetAttribute(node, "dy"));
            info.Rotate = StrToDouble(GetAttribute(node, "rot"));
            info.Radius = StrToDouble(GetAttribute(node, "roundness")) / 100;

            info.Layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            return info;
        }

        /// <summary>
        /// Procesa un doce de tipus TEXT.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>La informacio recopilada.</returns>
        /// 
        private TextNodeInfo ParseTextNode(XmlNode node) {

            TextNodeInfo info = new TextNodeInfo();

            string s = node.InnerText;
            if (s.StartsWith(">")) {
                info.Name = s.Substring(1);
                info.Value = null;
            }
            else {
                info.Name = null;
                info.Value = s;
            }

            info.X = StrToDouble(GetAttribute(node, "x"));
            info.Y = StrToDouble(GetAttribute(node, "y"));
            info.Rotate = StrToDouble(GetAttribute(node, "rot"));
            info.Height = StrToDouble(GetAttribute(node, "size"));
            info.Layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            return info;
        }

        /// <summary>
        /// Procesa un node WIRE.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>La informacio recopilada.</returns>
        /// 
        private WireNodeInfo ParseWireNode(XmlNode node) {

            WireNodeInfo info = new WireNodeInfo();

            info.X1 = StrToDouble(GetAttribute(node, "x1"));
            info.Y1 = StrToDouble(GetAttribute(node, "y1"));
            info.X2 = StrToDouble(GetAttribute(node, "x2"));
            info.Y2 = StrToDouble(GetAttribute(node, "y2"));

            info.Angle = StrToDouble(GetAttribute(node, "curve"));
            info.LineCap = GetAttribute(node, "cap") == null ? LineElement.LineCapStyle.Round : LineElement.LineCapStyle.Flat;
            info.Thickness = StrToDouble(GetAttribute(node, "width"));

            info.Layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            return info;
        }

        /// <summary>
        /// Procesa un node de tipus RECTANGLE
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>La informacio recopilada.</returns>
        /// 
        private RectangleNodeInfo ParseRectangleNode(XmlNode node) {

            RectangleNodeInfo info = new RectangleNodeInfo();

            double x1 = StrToDouble(GetAttribute(node, "x1"));
            double y1 = StrToDouble(GetAttribute(node, "y1"));
            double x2 = StrToDouble(GetAttribute(node, "x2"));
            double y2 = StrToDouble(GetAttribute(node, "y2"));
            info.X = (x1 + x2) / 2;
            info.Y = (y1 + y2) / 2;
            info.Width = x2 - x1;
            info.Height = y2 - y1;

            info.Rotate = StrToDouble(GetAttribute(node, "rot"));
            info.Thickness = StrToDouble(GetAttribute(node, "width"));

            info.Layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            return info;
        }

        /// <summary>
        /// Procesa un node de tipus CIRCLE
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>La informacio recopilada.</returns>
        /// 
        private CircleNodeInfo ParseCircleNode(XmlNode node) {

            CircleNodeInfo info = new CircleNodeInfo();

            info.X = StrToDouble(GetAttribute(node, "x"));
            info.Y = StrToDouble(GetAttribute(node, "y"));
            info.Thickness = StrToDouble(GetAttribute(node, "width"));
            info.Radius = StrToDouble(GetAttribute(node, "radius"));

            info.Layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            return info;
        }

        /// <summary>
        /// Procesa un node POLYGON.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>La informacio recopilada.</returns>
        /// 
        private PolygonNodeInfo ParsePolygonNode(XmlNode node) {

            PolygonNodeInfo info = new PolygonNodeInfo();

            info.Thickness = 0; //  StrToDouble(GetAttribute(node, "width"));
            info.Layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            return info;
        }

        /// <summary>
        /// Procesa un node de tipus VERTEX.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>La informacio recopilada.</returns>
        /// 
        private VertexNodeInfo ParseVertexNode(XmlNode node) {

            VertexNodeInfo info = new VertexNodeInfo();

            info.X = StrToDouble(GetAttribute(node, "x"));
            info.Y = StrToDouble(GetAttribute(node, "y"));
            info.Angle = StrToDouble(GetAttribute(node, "curve"));

            return info;
        }

        /// <summary>
        /// Procesa un node de tipus HOLE.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>Un element HOLE.</returns>
        /// 
        private HoleElement ParseHoleNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));

            double drill = StrToDouble(GetAttribute(node, "drill"));

            return new HoleElement(new Point(x, y), drill);
        }

        private Part ParseElementNode(XmlNode node) {

            string name = GetAttribute(node, "name");
            string value = GetAttribute(node, "value");
            string packageName = String.Format(
                "{0}@{1}",
                GetAttribute(node, "package"),
                GetAttribute(node, "library"));
            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));

            bool mirror = false;
            double rotate = 0;
            string rot = GetAttribute(node, "rot");
            if (rot != null) {
                rotate = StrToDouble(rot);
                mirror = rot.StartsWith("M");
            }

            Part part = new Part();
            part.Name = name;
            part.Position = new Point(x, y);
            part.Rotate = rotate;
            part.IsMirror = mirror;
            part.Component = componentDict[packageName];

            foreach (XmlNode attrNode in node.SelectNodes("attribute")) {

                Parameter parameter = ParseAttributeNode(attrNode);

                if (parameter.Name == "NAME")
                    parameter.Value = name;
                else if (parameter.Name == "VALUE")
                    parameter.Value = value;

                // Corrigeix perque siguin relatives al component
                //
                parameter.Position = new Point(parameter.Position.X - x, parameter.Position.Y - y);

                part.AddParameter(parameter);
            }

            return part;
        }

        private Parameter ParseAttributeNode(XmlNode node) {

            string name = GetAttribute(node, "name");
            string value = GetAttribute(node, "value");
            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double rotate = StrToDouble(GetAttribute(node, "rotate"));
            bool isVisible = GetAttribute(node, "display") != "off";

            return new Parameter(name, new Point(x, y), rotate, isVisible, value);
        }

        private ViaElement ParseViaNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double drill = StrToDouble(GetAttribute(node, "drill"));
            double size = StrToDouble(GetAttribute(node, "diameter"));

            LayerSet layers = new LayerSet();
            string extent = GetAttribute(node, "extent");
            string[] layerNames = extent.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string layerName in layerNames)
                layers.Add(layerDict[Int32.Parse(layerName)]);

            ViaElement.ViaShape shape = ViaElement.ViaShape.Circular;
            string shapeName = GetAttribute(node, "shape");
            switch (shapeName) {
                case "square":
                    shape = ViaElement.ViaShape.Square;
                    break;

                case "octagon":
                    shape = ViaElement.ViaShape.Octogonal;
                    break;
            }

            return new ViaElement(new Point(x, y), size, drill, shape, layers);
        }

        private static string GetAttribute(XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            return attribute == null ? null : attribute.Value;
        }

        private static double StrToDouble(string value) {

            if (String.IsNullOrEmpty(value))
                return 0;
            else {
                int i = 0;
                while (i < value.Length) {
                    if (Char.IsLetter(value[i]))
                        i++;
                    else
                        return Double.Parse(value.Substring(i), CultureInfo.InvariantCulture);
                }
                return 0;
            }
        }

        private static int StrToInteger(string value) {

            if (String.IsNullOrEmpty(value))
                return 0;
            else 
                return Int32.Parse(value);
        }
    }
}
