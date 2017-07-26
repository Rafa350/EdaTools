namespace Eda.PCBViewer {

    using MikroPic.EdaTools.v1.Model;
    using MikroPic.EdaTools.v1.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    public sealed class BoardLoader {

        private sealed class HoleNodeInfo {

            public double X { get; set; }
            public double Y { get; set; }
            public double Drill { get; set; }
        }

        private sealed class ViaNodeInfo {

            public double X { get; set; }
            public double Y { get; set; }
            public double Size { get; set; }
            public double Drill { get; set; }
            public ViaElement.ViaShape Shape { get; set; }
            public Layer Upper { get; set; }
            public Layer Lower { get; set; }
        }

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
            public double Angle {get;set;}
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

        public Board Load(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                return Load(stream);
        }

        public Board Load(Stream stream) {

            XmlDocument doc = LoadXmlDocument(stream);

            Board board = boardBuilder.CreateBoard();

            CreateLayers(doc, board);
            CreateComponents(doc, board);
            CreateElements(doc, board);
            CreateSignals(doc, board);

            return board;
        }

        private XmlDocument LoadXmlDocument(Stream stream) {

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

                            case "hole": {
                                    HoleNodeInfo info = ParseHoleNode(node);
                                    component.Add(boardBuilder.CreateHole(new Point(info.X, info.Y), info.Drill));
                                }
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
                        case "via": {
                                ViaNodeInfo info = ParseViaNode(node);
                                signal.Add(boardBuilder.CreateVia(new Point(info.X, info.Y), info.Size, info.Drill, info.Shape, info.Upper, info.Lower));
                            }
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

        private PadNodeInfo ParsePadNode(XmlNode node) {

            string name = GetAttribute(node, "name");
            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double rotate = RotateToDouble(GetAttribute(node, "rot"));
            double drill = StrToDouble(GetAttribute(node, "drill"));
            string shape = GetAttribute(node, "shape");

            ThPadElement.ThPadShape padShape = ThPadElement.ThPadShape.Circular;
            switch (shape) {
                case "square":
                    padShape = ThPadElement.ThPadShape.Square;
                    break;

                case "octagon":
                    padShape = ThPadElement.ThPadShape.Octogonal;
                    break;

                case "long":
                    padShape = ThPadElement.ThPadShape.Oval;
                    break;
            }

            return new PadNodeInfo {
                Name = name,
                X = x,
                Y = y,
                Rotate = rotate,
                Shape = padShape,
                Drill = drill,
                Size = drill * 1.6
            };
        }

        private SmdNodeInfo ParseSmdNode(XmlNode node) {

            string name = GetAttribute(node, "name");
            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double width = StrToDouble(GetAttribute(node, "dx"));
            double height = StrToDouble(GetAttribute(node, "dy"));
            double rotate = RotateToDouble(GetAttribute(node, "rot"));
            double roundness = StrToDouble(GetAttribute(node, "roundness"));
            int layerNum = Int32.Parse(GetAttribute(node, "layer"));

            return new SmdNodeInfo {
                Name = name,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                Rotate = rotate,
                Radius = roundness / 100,
                Layer = layerDict[layerNum]
            };
        }

        private TextNodeInfo ParseTextNode(XmlNode node) {

            string name = null;
            string value = null;

            string s = node.InnerText;
            if (s.StartsWith(">"))
                name = s.Substring(1);
            else
                value = s;
            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double rotate = RotateToDouble(GetAttribute(node, "rot"));
            double height = StrToDouble(GetAttribute(node, "size"));
            int layerNum = Int32.Parse(GetAttribute(node, "layer"));

            return new TextNodeInfo {
                X = x,
                Y = y,
                Rotate = rotate,
                Height = height,
                Name = name,
                Value = value,
                Layer = layerDict[layerNum]
            };
        }

        private WireNodeInfo ParseWireNode(XmlNode node) {

            double x1 = StrToDouble(GetAttribute(node, "x1"));
            double y1 = StrToDouble(GetAttribute(node, "y1"));
            double x2 = StrToDouble(GetAttribute(node, "x2"));
            double y2 = StrToDouble(GetAttribute(node, "y2"));
            double curve = StrToDouble(GetAttribute(node, "curve"));
            string cap = GetAttribute(node, "cap");

            double thickness = StrToDouble(GetAttribute(node, "width"));
            int layerNum = Int32.Parse(GetAttribute(node, "layer"));

            return new WireNodeInfo {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                LineCap = cap == null ? LineElement.LineCapStyle.Round : LineElement.LineCapStyle.Flat,
                Thickness = thickness,
                Angle = curve,
                Layer = layerDict[layerNum]
            };
        }

        private RectangleNodeInfo ParseRectangleNode(XmlNode node) {

            double x1 = StrToDouble(GetAttribute(node, "x1"));
            double y1 = StrToDouble(GetAttribute(node, "y1"));
            double x2 = StrToDouble(GetAttribute(node, "x2"));
            double y2 = StrToDouble(GetAttribute(node, "y2"));
            double rotate = RotateToDouble(GetAttribute(node, "rot"));
            double thickness = StrToDouble(GetAttribute(node, "width"));
            int layerNum = Int32.Parse(GetAttribute(node, "layer"));

            return new RectangleNodeInfo {
                X = (x1 + x2) / 2,
                Y = (y1 + y2) / 2,
                Width = x2 - x1,
                Height = y2 - y1,
                Rotate = rotate,
                Thickness = thickness,
                Layer = layerDict[layerNum]
            };
        }

        private CircleNodeInfo ParseCircleNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double thickness = StrToDouble(GetAttribute(node, "width"));
            double radius = StrToDouble(GetAttribute(node, "radius"));
            int layerNum = Int32.Parse(GetAttribute(node, "layer"));

            return new CircleNodeInfo {
                X = x,
                Y = y,
                Thickness = thickness,
                Radius = radius,
                Layer = layerDict[layerNum]
            };
        }

        private PolygonNodeInfo ParsePolygonNode(XmlNode node) {

            double thickness = StrToDouble(GetAttribute(node, "width"));
            int layerNum = Int32.Parse(GetAttribute(node, "layer"));

            return new PolygonNodeInfo {
                Thickness = thickness,
                Layer = layerDict[layerNum]
            };
        }

        private VertexNodeInfo ParseVertexNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double curve = StrToDouble(GetAttribute(node, "curve"));

            return new VertexNodeInfo {
                X = x,
                Y = y,
                Angle = curve
            };
        }

        private HoleNodeInfo ParseHoleNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double drill = StrToDouble(GetAttribute(node, "drill"));

            return new HoleNodeInfo {
                X = x,
                Y = y,
                Drill = drill
            };
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
                rotate = RotateToDouble(rot);
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
            double rotate = RotateToDouble(GetAttribute(node, "rotate"));
            bool isVisible = GetAttribute(node, "display") != "off";

            return new Parameter(name, new Point(x, y), rotate, isVisible, value);
        }

        private ViaNodeInfo ParseViaNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double drill = StrToDouble(GetAttribute(node, "drill"));
            double diameter = StrToDouble(GetAttribute(node, "diameter"));
            string shape = GetAttribute(node, "shape");
            string extent = GetAttribute(node, "extent");
            string[] layers = extent.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int upperLayerNum = Int32.Parse(layers[0]);
            int lowerLayerNum = Int32.Parse(layers[1]);

            ViaElement.ViaShape viaShape = ViaElement.ViaShape.Circular;
            switch (shape) {
                case "square":
                    viaShape = ViaElement.ViaShape.Square;
                    break;

                case "octagon":
                    viaShape = ViaElement.ViaShape.Octogonal;
                    break;
            }

            return new ViaNodeInfo {
                X = x,
                Y = y,
                Drill = drill,
                Size = diameter,
                Shape = viaShape,
                Upper = layerDict[upperLayerNum],
                Lower = layerDict[lowerLayerNum],
            };
        }

        private static string GetAttribute(XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            return attribute == null ? null : attribute.Value;
        }

        private static double StrToDouble(string value) {

            if (String.IsNullOrEmpty(value))
                return 0;
            else
                return Double.Parse(value, CultureInfo.InvariantCulture);
        }

        private static double RotateToDouble(string value) {

            if (String.IsNullOrEmpty(value))
                return 0;
            else {
                return Double.Parse(value.Remove(0, value.StartsWith("M") ? 2 : 1), CultureInfo.InvariantCulture);
            }
        }
    }
}
