﻿namespace MikroPic.EdaTools.v1.Pcb.Import.Eagle {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    public sealed class EagleImporter : Importer {

        private Dictionary<int, Layer> layerDict = new Dictionary<int, Layer>();
        private Dictionary<string, Component> componentDict = new Dictionary<string, Component>();
        private Dictionary<string, Part> partDict = new Dictionary<string, Part>();

        public override Board LoadBoard(Stream stream) {

            XmlDocument doc = ReadXmlDocument(stream);

            Board board = new Board();

            CreateLayers(doc, board);
            CreateMeasures(doc, board);
            CreateComponents(doc, board);
            CreateParts(doc, board);
            CreateSignals(doc, board);

            return board;
        }

        public override Library LoadLibrary(Stream stream) {

            XmlDocument doc = ReadXmlDocument(stream);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Carrega el document XML en format EAGLE
        /// </summary>
        /// <param name="stream">Stream d'entrada.</param>
        /// <returns>El document XML carregqat.</returns>
        /// 
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

                int layerNum = GetAttributeInteger(node, "number");

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

                string libraryName = GetAttributeString(libraryNode, "name");

                foreach (XmlNode packageNode in libraryNode.SelectNodes("packages/package")) {

                    string packageName = GetAttributeString(packageNode, "name");

                    List<Element> elements = new List<Element>();

                    foreach (XmlNode node in packageNode.ChildNodes) {
                        Element element = null;
                        switch (node.Name) {
                            case "smd":
                                element = ParseSmdNode(node);
                                break;

                            case "pad":
                                element = ParsePadNode(node);
                                break;

                            case "text":
                                element = ParseTextNode(node);
                                break;

                            case "wire":
                                element = ParseWireNode(node);
                                break;

                            case "rectangle":
                                element = ParseRectangleNode(node);
                                break;

                            case "circle":
                                element = ParseCircleNode(node);
                                break;

                            case "polygon":
                                element = ParsePolygonNode(node);
                                break;

                            case "hole":
                                element = ParseHoleNode(node);
                                break;

                            case "description":
                                break;

                            default:
                                throw new InvalidOperationException(
                                    String.Format("No se reconoce el tag '{0}'.", node.Name));
                        }

                        if (element != null)
                            elements.Add(element);
                    }

                    string name = String.Format("{0}@{1}", packageName, libraryName);
                    Component component = new Component(name, elements);
                    board.AddComponent(component);
                    componentDict.Add(name, component);
                }
            }
        }

        private void CreateParts(XmlDocument doc, Board board) {

            foreach (XmlNode node in doc.SelectNodes("eagle/drawing/board/elements/element")) {
                Part part = ParseElementNode(node);
                board.AddPart(part);
                partDict.Add(part.Name, part);
            }
        }

        private void CreateSignals(XmlDocument doc, Board board) {

            foreach (XmlNode signalNode in doc.SelectNodes("eagle/drawing/board/signals/signal")) {

                string signalName = GetAttributeString(signalNode, "name");

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

                        default:
                            throw new InvalidOperationException(
                                String.Format("No se reconoce el tag '{0}'.", node.Name));
                    }
                }

                board.AddSignal(signal);
            }
        }

        /// <summary>
        /// Procesa un node LAYER.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Layer' creat.</returns>
        /// 
        private Layer ParseLayerNode(XmlNode node) {

            string name = GetAttributeString(node, "name");
            int layerNum = GetAttributeInteger(node, "number");

            LayerId layerId = GetLayerId(layerNum);
            Color color = GetLayerColor(layerNum);

            return new Layer(layerId, name, color);
        }


        /// <summary>
        /// Procesa un node PAD.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ThPadElement' creat.</returns>
        /// 
        private Element ParsePadNode(XmlNode node) {

            string name = GetAttributeString(node, "name");

            double x = GetAttributeDouble(node, "x");
            double y = GetAttributeDouble(node, "y");
            Point position = new Point(x, y);

            double rotate = GetAttributeDouble(node, "rot");

            double drill = GetAttributeDouble(node, "drill");
            double size = drill * 1.6;

            ThPadElement.ThPadShape shape = ThPadElement.ThPadShape.Circular;
            switch (GetAttributeString(node, "shape")) {
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

            return new ThPadElement(name, position, rotate, size, shape, drill);
        }

        /// <summary>
        /// Procesa un node SMD.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'SmdPadElement' creat.</returns>
        /// 
        private Element ParseSmdNode(XmlNode node) {

            string name = GetAttributeString(node, "name");

            double x = GetAttributeDouble(node, "x");
            double y = GetAttributeDouble(node, "y");
            Point position = new Point(x, y);

            double width = GetAttributeDouble(node, "dx");
            double height = GetAttributeDouble(node, "dy");
            Size size = new Size(width, height);

            double rotate = GetAttributeDouble(node, "rot");
            double roundnes = GetAttributeDouble(node, "roundness") / 100;
            bool stop = GetAttributeBoolean(node, "stop", true);
            bool cream = GetAttributeBoolean(node, "cream", true);

            Layer layer = GetLayer(GetAttributeInteger(node, "layer"));

            return new SmdPadElement(name, position, layer, size, rotate, roundnes, stop, cream);
        }

        /// <summary>
        /// Procesa un node VIA.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ViaElement' creat.</returns>
        /// 
        private Element ParseViaNode(XmlNode node) {

            double x = GetAttributeDouble(node, "x");
            double y = GetAttributeDouble(node, "y");
            Point position = new Point(x, y);

            double drill = GetAttributeDouble(node, "drill");
            double size = GetAttributeDouble(node, "diameter");

            List<Layer> layers = new List<Layer>();
            string extent = GetAttributeString(node, "extent");
            string[] layerNames = extent.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string layerName in layerNames)
                layers.Add(GetLayer(Int32.Parse(layerName)));

            ViaElement.ViaShape shape = ViaElement.ViaShape.Circular;
            string shapeName = GetAttributeString(node, "shape");
            switch (shapeName) {
                case "square":
                    shape = ViaElement.ViaShape.Square;
                    break;

                case "octagon":
                    shape = ViaElement.ViaShape.Octogonal;
                    break;
            }

            return new ViaElement(position, layers, size, drill, shape);
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

            double x = GetAttributeDouble(node, "x");
            double y = GetAttributeDouble(node, "y");
            Point position = new Point(x, y);

            double rotate = GetAttributeDouble(node, "rot");
            double height = GetAttributeDouble(node, "size");

            Layer layer = GetLayer(GetAttributeInteger(node, "layer"));

            TextElement element = new TextElement(position, layer, rotate, height, TextElement.TextAlign.TopLeft);
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

            double x1 = GetAttributeDouble(node, "x1");
            double y1 = GetAttributeDouble(node, "y1");
            Point p1 = new Point(x1, y1);

            double x2 = GetAttributeDouble(node, "x2");
            double y2 = GetAttributeDouble(node, "y2");
            Point p2 = new Point(x2, y2);

            double angle = GetAttributeDouble(node, "curve");
            LineElement.LineCapStyle lineCap = GetAttributeString(node, "cap") == null ? LineElement.LineCapStyle.Round : LineElement.LineCapStyle.Flat;
            double thickness = GetAttributeDouble(node, "width");

            Layer layer = GetLayer(GetAttributeInteger(node, "layer"));

            if (angle == 0)
                return new LineElement(p1, p2, layer, thickness, lineCap);
            else
                return new ArcElement(p1, p2, layer, thickness, angle, lineCap);
        }

        /// <summary>
        /// Procesa un node RECTANGLE.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RectangleElement' creat.</returns>
        /// 
        private Element ParseRectangleNode(XmlNode node) {

            double x1 = GetAttributeDouble(node, "x1");
            double y1 = GetAttributeDouble(node, "y1");
            double x2 = GetAttributeDouble(node, "x2");
            double y2 = GetAttributeDouble(node, "y2");
            Point position = new Point((x1 + x2) / 2, (y1 + y2) / 2);
            Size size = new Size(x2 - x1, y2 - y1);

            double rotate = GetAttributeDouble(node, "rot");
            double thickness = GetAttributeDouble(node, "width");

            Layer layer = GetLayer(GetAttributeInteger(node, "layer"));

            return new RectangleElement(position, layer, size, rotate, thickness);
        }

        /// <summary>
        /// Procesa un node CIRCLE.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'CircleElement' creat.</returns>
        /// 
        private Element ParseCircleNode(XmlNode node) {

            double x = GetAttributeDouble(node, "x");
            double y = GetAttributeDouble(node, "y");
            Point position = new Point(x, y);

            double thickness = GetAttributeDouble(node, "width");
            double radius = GetAttributeDouble(node, "radius");

            Layer layer = GetLayer(GetAttributeInteger(node, "layer"));

            return new CircleElement(position, layer, radius, thickness);
        }

        /// <summary>
        /// Procesa un node POLYGON.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RegionElement' creat.</returns>
        /// 
        private Element ParsePolygonNode(XmlNode node) {

            Layer layer = GetLayer(GetAttributeInteger(node, "layer"));

            RegionElement region = new RegionElement();
            region.AddToLayer(layer);

            List<RegionElement.Segment> segments = new List<RegionElement.Segment>();
            foreach (XmlNode vertexNode in node.SelectNodes("vertex")) {

                double x = GetAttributeDouble(vertexNode, "x");
                double y = GetAttributeDouble(vertexNode, "y");
                Point vertex = new Point(x, y);

                double angle = GetAttributeDouble(vertexNode, "curve");

                segments.Add(new RegionElement.Segment(vertex, angle));
            }

            return new RegionElement(layer, segments);
        }

        /// <summary>
        /// Procesa un node HOLE.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'HoleElement' creat.</returns>
        /// 
        private Element ParseHoleNode(XmlNode node) {

            double x = GetAttributeDouble(node, "x");
            double y = GetAttributeDouble(node, "y");
            Point position = new Point(x, y);

            double drill = GetAttributeDouble(node, "drill");

            return new HoleElement(position, drill);
        }

        /// <summary>
        /// Procesa un node CONTACTREF.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Terminal' creat.</returns>
        /// 
        private Terminal ParseContactRefNode(XmlNode node) {

            string partName = GetAttributeString(node, "element");
            string padName = GetAttributeString(node, "pad");

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

            string name = GetAttributeString(node, "name");
            string value = GetAttributeString(node, "value");
            string componentKey = String.Format(
                "{0}@{1}",
                GetAttributeString(node, "package"),
                GetAttributeString(node, "library"));

            double x = GetAttributeDouble(node, "x");
            double y = GetAttributeDouble(node, "y");
            Point position = new Point(x, y);

            bool mirror = false;
            double rotate = 0;
            string rot = GetAttributeString(node, "rot");
            if (rot != null) {
                mirror = rot.IndexOf("M") != -1;

                rot = rot.Replace("M", null);
                rot = rot.Replace("R", null);
                rotate = Double.Parse(rot);
            }

            Part part = new Part();
            part.Name = name;
            part.Position = position;
            part.Rotation = rotate;
            part.IsFlipped = mirror;
            part.Component = GetComponent(componentKey);

            foreach (XmlNode attrNode in node.SelectNodes("attribute")) {

                Parameter parameter = ParseAttributeNode(attrNode);

                if (parameter.Name == "NAME")
                    parameter.Value = name;
                else if (parameter.Name == "VALUE")
                    parameter.Value = value;

                // Corrigeix perque siguin relatives al component
                //
                parameter.Position = new System.Windows.Point(parameter.Position.X - x, parameter.Position.Y - y);

                part.AddParameter(parameter);
            }

            return part;
        }

        private Parameter ParseAttributeNode(XmlNode node) {

            string name = GetAttributeString(node, "name");
            string value = GetAttributeString(node, "value");
            double x = GetAttributeDouble(node, "x");
            double y = GetAttributeDouble(node, "y");
            double rotate = GetAttributeDouble(node, "rotate");
            bool isVisible = GetAttributeString(node, "display") != "off";

            return new Parameter(name, new System.Windows.Point(x, y), rotate, isVisible, value);
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
        /// Obte el color de la capa a partir del seu numero.
        /// </summary>
        /// <param name="layerNum">El numero de la capa.</param>
        /// <returns>El color.</returns>
        /// 
        private Color GetLayerColor(int layerNum) {

            switch (layerNum) {
                case 1: // Top signal
                    return Colors.Red;

                case 16: // Bottom signal
                    return Colors.Blue;

                case 17: // Through hole pads
                    return Colors.DarkGoldenrod;

                case 18: // Vias
                    return Colors.Green;

                case 21: // Top placement
                case 22: // Bottom placement
                    return Colors.LightGray;

                case 25:
                case 26:
                    return Colors.LightGray;

                case 31:
                case 32:
                    return Colors.LightSeaGreen;

                case 35: // Top glue
                case 36: // Bottom glue
                    return Colors.LightSkyBlue;

                case 39: // Top keepout
                case 40: // Bottom keepout
                    return Color.FromArgb(64, Colors.Cyan.R, Colors.Cyan.G, Colors.Cyan.B);

                case 41: // Top restrict
                case 42: // Bottom restrict
                case 43: // Via restrict
                    return Color.FromArgb(64, Colors.DarkViolet.R, Colors.DarkViolet.G, Colors.DarkViolet.B);

                case 45: // Holes
                    return Colors.LightCoral;

                case 51: // Top document
                case 52: // Bottom document
                    return Colors.LightGray;

                default:
                    return Colors.White;
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

        private static string GetAttributeString(XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            return attribute == null ? null : attribute.Value;
        }

        private static double GetAttributeDouble(XmlNode node, string name, double derfValue = 0) {

            string value = GetAttributeString(node, name);
            if (String.IsNullOrEmpty(value))
                return 0;
            else {
                int start = 0;
                if (value.IndexOf("R") != -1)
                    start++;
                return Double.Parse(value.Substring(start), CultureInfo.InvariantCulture);
            }
        }

        private static int GetAttributeInteger(XmlNode node, string name, int defValue = 0) {

            string value = GetAttributeString(node, name);
            if (String.IsNullOrEmpty(value))
                return defValue;
            else
                return Int32.Parse(value);
        }

        private static bool GetAttributeBoolean(XmlNode node, string name, bool defValue = false) {

            string value = GetAttributeString(node, name);
            if (String.IsNullOrEmpty(value))
                return defValue;
            else
                return
                    String.Compare(value, "yes", true) == 0 ||
                    String.Compare(value, "true", true) == 0;
        }
    }
}
