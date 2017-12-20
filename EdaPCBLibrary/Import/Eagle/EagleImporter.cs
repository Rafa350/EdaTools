namespace MikroPic.EdaTools.v1.Pcb.Import.Eagle {

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.IO;
    using System.Xml;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Collections;

    public sealed class EagleImporter : Importer {

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
        private Dictionary<string, Part> partDict = new Dictionary<string, Part>();

        public override Board LoadBoard(Stream stream) {

            XmlDocument doc = ReadXmlDocument(stream);

            Board board = new Board(); // boardBuilder.CreateBoard();

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

            foreach (XmlNode node in doc.SelectNodes("eagle/drawing/layers/layer")) {

                int layerNum = StrToInteger(GetAttribute(node, "number"));

                if (GetLayerId(layerNum) != LayerId.Unknown) {
                    if (!layerDict.ContainsKey(layerNum)) {

                        Layer layer = ParseLayerNode(node);
                        board.LayerStackup.AddLayer(layer);

                        layerDict.Add(layerNum, layer);
                    }
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

                string libraryName = GetAttribute(libraryNode, "name");

                foreach (XmlNode packageNode in libraryNode.SelectNodes("packages/package")) {

                    string packageName = GetAttribute(packageNode, "name");

                    ElementCollection elements = new ElementCollection();

                    foreach (XmlNode node in packageNode.ChildNodes) {
                        switch (node.Name) {
                            case "smd":
                                elements.Add(ParseSmdNode(node));
                                break;

                            case "pad":
                                elements.Add(ParsePadNode(node));
                                break;

                            case "text":
                                elements.Add(ParseTextNode(node));
                                break;

                            case "wire":
                                elements.Add(ParseWireNode(node));
                                break;

                            case "rectangle":
                                elements.Add(ParseRectangleNode(node));
                                break;

                            case "circle":
                                elements.Add(ParseCircleNode(node));
                                break;

                            case "polygon":
                                elements.Add(ParsePolygonNode(node));
                                break;

                            case "hole":
                                elements.Add(ParseHoleNode(node));
                                break;
                        }
                    }

                    string name = String.Format("{0}@{1}", packageName, libraryName);
                    Component component = new Component(name, elements);
                    board.Components.Add(component);
                    componentDict.Add(name, component);
                }
            }
        }

        private void CreateElements(XmlDocument doc, Board board) {

            foreach (XmlNode node in doc.SelectNodes("eagle/drawing/board/elements/element")) {
                Part part = ParseElementNode(node);
                board.AddPart(part);
                partDict.Add(part.Name, part);
            }
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

                        case "polygon": 
                            signal.Add(ParsePolygonNode(node));
                            break;

                        case "contactref":
                            signal.Add(ParseContactRefNode(node));
                            break;
                    }
                }

                board.AddSignal(signal);
            }
        }

        /*private void CreateVertexList(XmlNode polygonNode, RegionElement polygon) {

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
        }*/

        /// <summary>
        /// Procesa un node LAYER.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Layer' creat.</returns>
        /// 
        private Layer ParseLayerNode(XmlNode node) {

            string name = GetAttribute(node, "name");
            int layerNum = StrToInteger(GetAttribute(node, "number"));

            LayerId layerId = GetLayerId(layerNum);

            return new Layer(layerId, name, Colors.WhiteSmoke);
        }


        /// <summary>
        /// Procesa un node PAD.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ThPadElement' creat.</returns>
        /// 
        private Element ParsePadNode(XmlNode node) {

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
        /// Procesa un node SMD.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'SmdPadElement' creat.</returns>
        /// 
        private Element ParseSmdNode(XmlNode node) {

            string name = GetAttribute(node, "name");

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double width = StrToDouble(GetAttribute(node, "dx"));
            double height = StrToDouble(GetAttribute(node, "dy"));
            double rotate = StrToDouble(GetAttribute(node, "rot"));
            double roundnes = StrToDouble(GetAttribute(node, "roundness")) / 100;
            bool stop = StrToBoolean(GetAttribute(node, "stop"), true);
            bool cream = StrToBoolean(GetAttribute(node, "cream"), true);

            Layer layer = GetLayer(StrToInteger(GetAttribute(node, "layer")));

            return new SmdPadElement(name, new Point(x, y), layer, new Size(width, height), rotate, roundnes, stop, cream);
        }

        /// <summary>
        /// Procesa un node VIA.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ViaElement' creat.</returns>
        /// 
        private Element ParseViaNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double drill = StrToDouble(GetAttribute(node, "drill"));
            double size = StrToDouble(GetAttribute(node, "diameter"));

            List<Layer> layers = new List<Layer>();
            string extent = GetAttribute(node, "extent");
            string[] layerNames = extent.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string layerName in layerNames)
                layers.Add(GetLayer(Int32.Parse(layerName)));

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

        /// <summary>
        /// Procesa un node TEXT.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'TextElement' creat.</returns>
        /// 
        private Element ParseTextNode(XmlNode node) {

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

            Layer layer = GetLayer(StrToInteger(GetAttribute(node, "layer")));

            TextElement element = new TextElement(new Point(x, y), layer, rotate, height, TextElement.TextAlign.TopLeft);
            element.Name = name;
            element.Value = value;
            return element;
        }

        /// <summary>
        /// Procesa un node WIRE.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'LineElement' o ArcElement' creat.</returns>
        /// 
        private Element ParseWireNode(XmlNode node) {

            double x1 = StrToDouble(GetAttribute(node, "x1"));
            double y1 = StrToDouble(GetAttribute(node, "y1"));
            double x2 = StrToDouble(GetAttribute(node, "x2"));
            double y2 = StrToDouble(GetAttribute(node, "y2"));

            double angle = StrToDouble(GetAttribute(node, "curve"));
            LineElement.LineCapStyle lineCap = GetAttribute(node, "cap") == null ? LineElement.LineCapStyle.Round : LineElement.LineCapStyle.Flat;

            double thickness = StrToDouble(GetAttribute(node, "width"));

            Layer layer = GetLayer(StrToInteger(GetAttribute(node, "layer")));

            if (angle == 0)
                return new LineElement(new Point(x1, y1), new Point(x2, y2), layer, thickness, lineCap);
            else
                return new ArcElement(new Point(x1, y1), new Point(x2, y2), layer, thickness, angle, lineCap);
        }

        /// <summary>
        /// Procesa un node RECTANGLE.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RectangleElement' creat.</returns>
        /// 
        private Element ParseRectangleNode(XmlNode node) {

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

            Layer layer = GetLayer(StrToInteger(GetAttribute(node, "layer")));

            return new RectangleElement(new Point(x, y), layer, new Size(width, height), rotate, thickness);
        }

        /// <summary>
        /// Procesa un node CIRCLE.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'CircleElement' creat.</returns>
        /// 
        private Element ParseCircleNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));
            double thickness = StrToDouble(GetAttribute(node, "width"));
            double radius = StrToDouble(GetAttribute(node, "radius"));

            Layer layer = GetLayer(StrToInteger(GetAttribute(node, "layer")));

            return new CircleElement(new Point(x, y), layer, radius, thickness);
        }

        /// <summary>
        /// Procesa un node POLYGON.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RegionElement' creat.</returns>
        /// 
        private RegionElement ParsePolygonNode(XmlNode node) {

            Layer layer = GetLayer(StrToInteger(GetAttribute(node, "layer")));

            RegionElement region = new RegionElement();
            region.AddToLayer(layer);

            List<RegionElement.Segment> segments = new List<RegionElement.Segment>();
            foreach (XmlNode vertexNode in node.SelectNodes("vertex")) {

                double x = StrToDouble(GetAttribute(vertexNode, "x"));
                double y = StrToDouble(GetAttribute(vertexNode, "y"));
                double angle = StrToDouble(GetAttribute(vertexNode, "curve"));

                segments.Add(new RegionElement.Segment(new Point(x, y), angle));
            }

            return new RegionElement(layer,segments);
        }

        /// <summary>
        /// Procesa un node HOLE.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'HoleElement' creat.</returns>
        /// 
        private Element ParseHoleNode(XmlNode node) {

            double x = StrToDouble(GetAttribute(node, "x"));
            double y = StrToDouble(GetAttribute(node, "y"));

            double drill = StrToDouble(GetAttribute(node, "drill"));

            return new HoleElement(new Point(x, y), drill);
        }

        /// <summary>
        /// Procesa un node CONTACTREF.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Terminal' creat.</returns>
        /// 
        private Terminal ParseContactRefNode(XmlNode node) {

            string partName = GetAttribute(node, "element");
            string padName = GetAttribute(node, "pad");

            Part part = partDict[partName];

            return new Terminal(part, padName);
        }
        
        /// <summary>
        /// Procesa un node ELEMENT
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Part' creat.</returns>
        /// 
        private Part ParseElementNode(XmlNode node) {

            string name = GetAttribute(node, "name");
            string value = GetAttribute(node, "value");
            string componentKey = String.Format(
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
            part.Component = GetComponent(componentKey);

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
        /// Obte un objecte capa a partir del seu numero.
        /// </summary>
        /// <param name="layerNum">Numero de capa.</param>
        /// <returns>La capa.</returns>
        /// 
        private Layer GetLayer(int layerNum) {

            return layerDict[layerNum];
        }

        /// <summary>
        /// Obte el identificador de la capa a partir del seu numero.
        /// </summary>
        /// <param name="layerNum">Numero de la capa.</param>
        /// <returns>La capa.</returns>
        /// 
        private static LayerId GetLayerId(int layerNum) {

            switch (layerNum) {
                case 1:
                    return LayerId.Top;

                case 16:
                    return LayerId.Bottom;

                case 17:
                    return LayerId.Pads;

                case 18:
                    return LayerId.Vias;

                case 19:
                    return LayerId.Unrouted;

                case 20:
                    return LayerId.Profile;

                case 21:
                    return LayerId.TopPlace;

                case 22:
                    return LayerId.BottomPlace;

                case 25:
                    return LayerId.TopNames;

                case 26:
                    return LayerId.BottomNames;

                case 27:
                    return LayerId.TopValues;

                case 28:
                    return LayerId.BottomValues;

                case 29:
                    return LayerId.TopStop;

                case 30:
                    return LayerId.BottomStop;

                case 31:
                    return LayerId.TopCream;

                case 32:
                    return LayerId.BottomCream;

                case 35:
                    return LayerId.TopGlue;

                case 36:
                    return LayerId.BottomGlue;

                case 39:
                    return LayerId.TopKeepout;

                case 40:
                    return LayerId.BottomKeepout;

                case 41:
                    return LayerId.TopRestrict;

                case 42:
                    return LayerId.BottomRestrict;

                case 43:
                    return LayerId.ViaRestrict;

                case 44:
                    return LayerId.Drills;

                case 45:
                    return LayerId.Holes;

                case 51:
                    return LayerId.TopDocument;

                case 52:
                    return LayerId.BottomDocument;

                default:
                    return LayerId.Unknown;
            }           
        }

        /// <summary>
        /// Obte objecte Component a partir del seu nom.
        /// </summary>
        /// <param name="name">El nom del component.</param>
        /// <returns>El component.</returns>
        /// 
        private Component GetComponent(string name) {

            return componentDict[name];
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
