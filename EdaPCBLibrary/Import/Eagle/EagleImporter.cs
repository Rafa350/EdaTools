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
                        board.AddElement(ParseWireNode(node));
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

                            case "smd":
                                component.Add(ParseSmdNode(node));
                                break;

                            case "pad": 
                                component.Add(ParsePadNode(node));
                                break;

                            case "text":
                                component.Add(ParseTextNode(node));
                                break;


                            case "wire":
                                component.Add(ParseWireNode(node));
                                break;

                            case "rectangle": 
                                component.Add(ParseRectangleNode(node));
                                break;
                                
                            case "circle": 
                                component.Add(ParseCircleNode(node));
                                break;

                            case "polygon": {
                                    PolygonNodeInfo info = ParsePolygonNode(node);
                                    RegionElement polygon = boardBuilder.CreatePolygon(new Point(0, 0), 0, info.Thickness, info.Layer);
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

                        case "wire":
                            signal.Add(ParseWireNode(node));
                            break;

                        case "polygon": {
                                PolygonNodeInfo info = ParsePolygonNode(node);
                                RegionElement polygon = boardBuilder.CreatePolygon(new Point(0, 0), 0, info.Thickness, info.Layer);
                                CreateVertexList(node, polygon);
                                signal.Add(polygon);
                            }
                            break;
                    }
                }

                board.AddSignal(signal);
            }
        }

        private void CreateVertexList(XmlNode polygonNode, RegionElement polygon) {

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
        /// Procesa un node de tipus PAD i crea l'element corresponent.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element creat.</returns>
        /// 
        private ElementBase ParsePadNode(XmlNode node) {

            string name = GetAttribute(node, "name");

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double rotate = StrToDouble(GetAttribute(node, "rot"));

            double drill = StrToDouble(GetAttribute(node, "drill"));
            double size = drill * 1.6;

            ThPadElement.ThPadShape shape = ThPadElement.ThPadShape.Circular;
            switch (GetAttribute(node, "shape")) {
                case "square":
                    shape = ThPadElement.ThPadShape.Square;
                    break;

                case "octagon":
                    shape = ThPadElement.ThPadShape.Octogonal;
                    break;

                case "long":
                    shape = ThPadElement.ThPadShape.Oval;
                    break;
            }

            return new ThPadElement(name, new Point(x, y), rotate, size, shape, drill);
        }

        /// <summary>
        /// Procesa un node de tipus SMD i crea l'element corresponent.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element creat.</returns>
        /// 
        private ElementBase ParseSmdNode(XmlNode node) {

            string name = GetAttribute(node, "name");

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double width = StrToDouble(GetAttribute(node, "dx"));
            double height = StrToDouble(GetAttribute(node, "dy"));
            double rotate = StrToDouble(GetAttribute(node, "rot"));
            double roundnes = StrToDouble(GetAttribute(node, "roundness")) / 100;
            bool stop = StrToBoolean(GetAttribute(node, "stop"), true);
            bool cream = StrToBoolean(GetAttribute(node, "cream"), true);

            Layer layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            return new SmdPadElement(name, new Point(x, y), layer, new Size(width, height), rotate, roundnes, stop, cream);
        }

        /// <summary>
        /// Procesa un doce de tipus TEXT.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>La informacio recopilada.</returns>
        /// 
        private ElementBase ParseTextNode(XmlNode node) {

            string name = null;
            string value = null;

            string s = node.InnerText;
            if (s.StartsWith(">")) 
                name = s.Substring(1);
            else 
                value = s;

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double rotate = StrToDouble(GetAttribute(node, "rot"));
            double height = StrToDouble(GetAttribute(node, "size"));

            Layer layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            TextElement element = new TextElement(new Point(x, y), layer, rotate, height, TextElement.TextAlign.TopLeft);
            element.Name = name;
            element.Value = value;
            return element;
        }

        /// <summary>
        /// Procesa un node WIRE i crea l'element corresponent.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element creat.</returns>
        /// 
        private ElementBase ParseWireNode(XmlNode node) {

            double x1 = StrToDouble(GetAttribute(node, "x1"));
            double y1 = StrToDouble(GetAttribute(node, "y1"));
            double x2 = StrToDouble(GetAttribute(node, "x2"));
            double y2 = StrToDouble(GetAttribute(node, "y2"));

            double angle = StrToDouble(GetAttribute(node, "curve"));
            LineElement.LineCapStyle lineCap = GetAttribute(node, "cap") == null ? LineElement.LineCapStyle.Round : LineElement.LineCapStyle.Flat;

            double thickness = StrToDouble(GetAttribute(node, "width"));

            Layer layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            if (angle == 0) 
                return new LineElement(new Point(x1, y1), layer, new Point(x2, y2), thickness, lineCap);
            else 
                return new ArcElement(new Point(x1, y1), layer, new Point(x2, y2), thickness, angle, lineCap);
        }

        /// <summary>
        /// Procesa un node de tipus RECTANGLE i crea el element corresponent.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element creat.</returns>
        /// 
        private ElementBase ParseRectangleNode(XmlNode node) {

            double x1 = StrToDouble(GetAttribute(node, "x1"));
            double y1 = StrToDouble(GetAttribute(node, "y1"));
            double x2 = StrToDouble(GetAttribute(node, "x2"));
            double y2 = StrToDouble(GetAttribute(node, "y2"));
            double x = (x1 + x2) / 2;
            double y = (y1 + y2) / 2;
            double width = x2 - x1;
            double height = y2 - y1;

            double rotate = StrToDouble(GetAttribute(node, "rot"));
            double thickness = StrToDouble(GetAttribute(node, "width"));

            Layer layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            return new RectangleElement(new Point(x, y), layer, new Size(width, height), rotate, thickness);
        }

        /// <summary>
        /// Procesa un node de tipus CIRCLE i crea el element corresponent.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element creat.</returns>
        /// 
        private ElementBase ParseCircleNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double thickness = StrToDouble(GetAttribute(node, "width"));
            double radius = StrToDouble(GetAttribute(node, "radius"));

            Layer layer = layerDict[StrToInteger(GetAttribute(node, "layer"))];

            return new CircleElement(new Point(x, y), layer, radius, thickness);
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
        /// Procesa un node de tipus HOLE i crea l'element correspoonent.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element creat.</returns>
        /// 
        private ElementBase ParseHoleNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));

            double drill = StrToDouble(GetAttribute(node, "drill"));

            return new HoleElement(new Point(x, y), drill);
        }

        /// <summary>
        /// Procesa un node de tipus ELEMENT i crea un objecte Part
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte Part creat.</returns>
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

        /// <summary>
        /// Procesa un n ode de tipus VIA i crea l'element corresponent.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element creat.</returns>
        /// 
        private ElementBase ParseViaNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double drill = StrToDouble(GetAttribute(node, "drill"));
            double size = StrToDouble(GetAttribute(node, "diameter"));

            List<Layer> layers = new List<Layer>();
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

            return new ViaElement(new Point(x, y), layers, size, drill, shape);
        }

        private static string GetAttribute(XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            return attribute == null ? null : attribute.Value;
        }

        private static double StrToDouble(string value, double derfValue = 0) {

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

        private static int StrToInteger(string value, int defValue = 0) {

            if (String.IsNullOrEmpty(value))
                return defValue;
            else 
                return Int32.Parse(value);
        }

        private static bool StrToBoolean(string value, bool defValue = false) {

            if (String.IsNullOrEmpty(value))
                return defValue;
            else
                return 
                    String.Compare(value, "yes", true) == 0 || 
                    String.Compare(value, "true", true) == 0;
        }
    }
}
