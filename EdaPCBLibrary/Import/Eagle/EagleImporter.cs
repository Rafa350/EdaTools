namespace MikroPic.EdaTools.v1.Pcb.Import.Eagle {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// Clase per importar una placa desde Eagle
    /// </summary>
    public sealed class EagleImporter : Importer {

        private readonly Dictionary<string, Block> componentDict = new Dictionary<string, Block>();
        private readonly Dictionary<string, Signal> signalDict = new Dictionary<string, Signal>();
        private readonly Dictionary<Element, string> mapElementSignal = new Dictionary<Element, string>();

        private Board board;
        private XmlDocument doc;

        /// <summary>
        /// Importa una placa en format EAGLE.
        /// </summary>
        /// <param name="stream">Stream d'entrada.</param>
        /// <returns>La placa.</returns>
        /// 
        public override Board LoadBoard(Stream stream) {

            doc = ReadXmlDocument(stream);
            board = new Board();

            ProcessLayers();
            ProcessSignals();
            ProcessBlocks();
            ProcessElements();

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
        /// <returns>El document XML carregat.</returns>
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

        /// <summary>
        /// Procesa les capes.
        /// </summary>
        /// 
        private void ProcessLayers() {

            // Recorre el node <layers> per crear les capes necesaries.
            //
            foreach (XmlNode layerNode in doc.SelectNodes("eagle/drawing/layers/layer")) {

                // Obte el identificador de la capa
                //
                int layerNum = GetAttributeInteger(layerNode, "number");
                string name = GetLayerName(layerNum);

                // Si la capa no existeix, la crea i l'afegeix a la placa.
                //
                if (board.GetLayer(name, false) == null) {
                    Layer layer = ParseLayerNode(layerNode);
                    board.AddLayer(layer);
                }
            }
        }

        /// <summary>
        /// Procesa les senyals
        /// </summary>
        /// 
        private void ProcessSignals() {

            // Recorre el node <signal> per crear les senyals necesaries
            //
            foreach (XmlNode signalNode in doc.SelectNodes("eagle/drawing/board/signals/signal")) {

                Signal signal = ParseSignalNode(signalNode);
                board.AddSignal(signal);
                signalDict.Add(signal.Name, signal);
            }
        }

        /// <summary>
        /// Procesa els blocs
        /// </summary>
        /// 
        private void ProcessBlocks() {

            // Procesa el tag <library>
            //
            foreach (XmlNode libraryNode in doc.SelectNodes("eagle/drawing/board/libraries/library")) {

                string libraryName = GetAttributeString(libraryNode, "name");

                // Procesa el tag <package>
                //
                foreach (XmlNode packageNode in libraryNode.SelectNodes("packages/package")) {

                    string packageName = GetAttributeString(packageNode, "name");

                    List<Element> elements = new List<Element>();
                    foreach (XmlNode childNode in packageNode.ChildNodes) {
                        Element element = null;
                        switch (childNode.Name) {
                            case "smd":
                                element = ParseSmdNode(childNode);
                                break;

                            case "pad":
                                element = ParsePadNode(childNode);
                                break;

                            case "text":
                                element = ParseTextNode(childNode);
                                break;

                            case "wire":
                                element = ParseWireNode(childNode);
                                break;

                            case "rectangle":
                                element = ParseRectangleNode(childNode);
                                break;

                            case "circle":
                                element = ParseCircleNode(childNode);
                                break;

                            case "polygon":
                                element = ParsePolygonNode(childNode);
                                break;

                            case "hole":
                                element = ParseHoleNode(childNode);
                                break;

                            case "description":
                                break;

                            default:
                                throw new InvalidOperationException(
                                    String.Format("No se reconoce el tag '{0}'.", childNode.Name));
                        }
                        if (element != null)
                            elements.Add(element);
                    }

                    string name = String.Format("{0}@{1}", packageName, libraryName);
                    Block component = new Block(name, elements);
                    board.AddBlock(component);
                    componentDict.Add(component.Name, component);
                }
            }
        }

        /// <summary>
        /// Procesa els elements.
        /// </summary>
        /// 
        private void ProcessElements() {

            // Procesa el tag <elements>
            //
            foreach (Part part in ParseElementsNode(doc.SelectSingleNode("eagle/drawing/board/elements"))) 
                board.AddPart(part);

            // Procesa el tag <plain>
            //
            foreach (XmlNode node in doc.SelectSingleNode("eagle/drawing/board/plain")) {

                Element element = null;
                switch (node.Name) {
                    case "wire":
                        element = ParseWireNode(node);
                        break;

                    case "rectangle":
                        element = ParseRectangleNode(node);
                        break;

                    case "text":
                        element = ParseTextNode(node);
                        break;

                    case "dimension":
                        break;

                    default:
                        throw new InvalidOperationException(
                            String.Format("No se reconoce el tag '{0}'.", node.Name));
                }
                if (element != null)
                    board.AddElement(element);
            }

            // Procesa el tag <signals>
            //
            foreach (XmlNode node in doc.SelectSingleNode("eagle/drawing/board/signals")) {

                string signalName = GetAttributeString(node, "name");
                Signal signal = signalDict[signalName];

                foreach (XmlNode childNode in node.SelectNodes("contactref")) {
                    string partName = GetAttributeString(childNode, "element");
                    string padName = GetAttributeString(childNode, "pad");

                    Part part = board.GetPart(partName, true);
                    foreach (PadElement pad in part.Pads) {
                        if (pad.Name == padName)
                            board.Connect(signal, pad, part);
                    }
                }

                foreach (XmlNode childNode in node.ChildNodes) {
                    Element element = null;
                    switch (childNode.Name) {
                        case "wire":
                            element = ParseWireNode(childNode);
                            break;

                        case "via":
                            element = ParseViaNode(childNode);
                            break;

                        case "polygon":
                            element = ParsePolygonNode(childNode);
                            break;

                        case "contactref":
                            break;

                        default:
                            throw new InvalidOperationException(
                                String.Format("No se reconoce el tag '{0}'.", childNode.Name));
                    }
                    if (element != null) {
                        board.AddElement(element);

                        IConectable conectable = element as IConectable;
                        if (conectable != null)
                            board.Connect(signal, conectable);
                    }
                }
            }
        }

        /// <summary>
        /// Procesa el node ELEMENTS.
        /// </summary>
        /// <param name="node">El element a procesar.</param>
        /// <returns>La llista d'elements.</returns>
        /// 
        private IEnumerable<Part> ParseElementsNode(XmlNode node) {

            List<Part> parts = new List<Part>();
            foreach (XmlNode childNode in node.ChildNodes) {
                Part part = ParseElementNode(childNode);
                parts.Add(part);
            }
            return parts;
        }

        /// <summary>
        /// Procesa un node SIGNAL
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte Signal creat.</returns>
        /// 
        private Signal ParseSignalNode(XmlNode node) {

            string signalName = GetAttributeString(node, "name");

            return new Signal(signalName);
        }

        /// <summary>
        /// Procesa un node LAYER.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Layer' creat.</returns>
        /// 
        private Layer ParseLayerNode(XmlNode node) {

            int layerNum = GetAttributeInteger(node, "number");
            string layerName = GetLayerName(layerNum);

            BoardSide side = BoardSide.Unknown;
            if (layerNum == 1) 
                side = BoardSide.Top;
            else if (layerNum > 1 && layerNum < 16) 
                side = BoardSide.Inner;
            else if (layerNum == 16)
                side = BoardSide.Bottom;

            LayerFunction function = LayerFunction.Unknown;
            if (layerNum >= 1 && layerNum <= 16)
                function = LayerFunction.Signal;

            Color color = GetLayerColor(layerNum);

            return new Layer(layerName, side, function, color);
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

            Element element = new ThPadElement(name, position, Angle.FromDegrees(rotate), size, shape, drill);

            board.Place(board.GetLayer(Layer.PadsName), element);
            board.Place(board.GetLayer(Layer.DrillsName), element);
            board.Place(board.GetLayer(Layer.TopName), element);
            board.Place(board.GetLayer(Layer.TopStopName), element);
            board.Place(board.GetLayer(Layer.BottomName), element);
            board.Place(board.GetLayer(Layer.BottomStopName), element);

            return element;
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

            int layerNum = GetAttributeInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            Element element = new SmdPadElement(name, position, size, Angle.FromDegrees(rotate), roundnes);
            board.Place(board.GetLayer(layerName), element);
            board.Place(board.GetLayer(Layer.PadsName), element);
            if (cream)
                board.Place(board.GetLayer(Layer.TopCreamName), element);
            if (stop)
                board.Place(board.GetLayer(Layer.TopStopName), element);

            return element;
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

            string extent = GetAttributeString(node, "extent");
            string[] layerNames = extent.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int[] layerNums = new int[layerNames.Length];
            for (int i = 0; i < layerNums.Length; i++)
                layerNums[i] = Int32.Parse(layerNames[i]);

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

            List<Layer> layers = new List<Layer>();
            Element element = new ViaElement(position, size, drill, shape);

            foreach(int layerNum in layerNums) {
                string layerName = GetLayerName(layerNum);
                board.Place(board.GetLayer(layerName), element);
            }
            board.Place(board.GetLayer(Layer.ViasName), element);
            board.Place(board.GetLayer(Layer.DrillsName), element);

            return element;
        }

        /// <summary>
        /// Procesa un node TEXT.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'TextElement' creat.</returns>
        /// 
        private Element ParseTextNode(XmlNode node) {

            string value = node.InnerText;
            if (value.StartsWith(">"))
                value = String.Format("$({0})", value.Substring(1));

            double x = GetAttributeDouble(node, "x");
            double y = GetAttributeDouble(node, "y");
            Point position = new Point(x, y);

            double rotate = GetAttributeDouble(node, "rot");
            double height = GetAttributeDouble(node, "size");

            int layerNum = GetAttributeInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            TextElement element = new TextElement(position, Angle.FromDegrees(rotate), height, TextElement.TextAlign.TopLeft);
            element.Value = value;

            board.Place(board.GetLayer(layerName), element);

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

            int layerNum = GetAttributeInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            Element element;
            if (angle == 0)
                element = new LineElement(p1, p2, thickness, lineCap);
            else
                element = new ArcElement(p1, p2, thickness, Angle.FromDegrees(angle), lineCap);

            board.Place(board.GetLayer(layerName), element);

            return element;
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
            Point position = new Point((x1 + x2) / 2.0, (y1 + y2) / 2.0);
            Size size = new Size(x2 - x1, y2 - y1);

            double rotation = GetAttributeDouble(node, "rot");
            double thickness = GetAttributeDouble(node, "width");

            int layerNum = GetAttributeInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            Element element = new RectangleElement(position, size, Angle.FromDegrees(rotation), thickness);

            board.Place(board.GetLayer(layerName), element);

            return element;
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

            int layerNum = GetAttributeInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            Element element = new CircleElement(position, radius, thickness);

            board.Place(board.GetLayer(layerName), element);

            return element;
        }

        /// <summary>
        /// Procesa un node POLYGON.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RegionElement' creat.</returns>
        /// 
        private Element ParsePolygonNode(XmlNode node) {

            double thickness = GetAttributeDouble(node, "width");

            int layerNum = GetAttributeInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            List<RegionElement.Segment> segments = new List<RegionElement.Segment>();
            foreach (XmlNode vertexNode in node.SelectNodes("vertex")) {

                double x = GetAttributeDouble(vertexNode, "x");
                double y = GetAttributeDouble(vertexNode, "y");
                Point vertex = new Point(x, y);

                double angle = GetAttributeDouble(vertexNode, "curve");

                segments.Add(new RegionElement.Segment(vertex, Angle.FromDegrees(angle)));
            }

            Element element = new RegionElement(thickness, segments);

            board.Place(board.GetLayer(layerName), element);

            return element;
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

            Element element = new HoleElement(position, drill);

            board.Place(board.GetLayer(Layer.HolesName), element);
            board.Place(board.GetLayer(Layer.TopName), element);
            board.Place(board.GetLayer(Layer.BottomName), element);

            return element;
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

            bool isFlipped = false;
            double rotation = 0;
            string rot = GetAttributeString(node, "rot");
            if (rot != null) {
                isFlipped = rot.IndexOf("M") != -1;

                rot = rot.Replace("M", null);
                rot = rot.Replace("R", null);
                rotation = Double.Parse(rot);
            }

            Part part = new Part(GetComponent(componentKey), name, position, Angle.FromDegrees(rotation), isFlipped);

            foreach (XmlNode attrNode in node.SelectNodes("attribute")) {

                PartAttribute parameter = ParseAttributeNode(attrNode);

                // Inicialitza els valor per defecte dels parametres NAME i VALUE
                //
                if (parameter.Name == "NAME")
                    parameter.Value = name;
                else if (parameter.Name == "VALUE")
                    parameter.Value = value;

                // Corrigeix perque siguin relatives al component
                //
                parameter.Position = new Point(parameter.Position.X - x, parameter.Position.Y - y);

                part.AddAttribute(parameter);
            }

            return part;
        }

        private PartAttribute ParseAttributeNode(XmlNode node) {

            string name = GetAttributeString(node, "name");
            string value = GetAttributeString(node, "value");
            double x = GetAttributeDouble(node, "x");
            double y = GetAttributeDouble(node, "y");
            double rotate = GetAttributeDouble(node, "rotate");
            bool isVisible = GetAttributeString(node, "display") != "off";

            return new PartAttribute(name, new Point(x, y), Angle.FromDegrees(rotate), isVisible, value);
        }

        /// <summary>
        /// Obte el nom de la capa a partir del seu numero.
        /// </summary>
        /// <param name="layerNum">Numero de la capa.</param>
        /// <returns>El nom de la capa.</returns>
        /// 
        private static string GetLayerName(int layerNum) {

            switch (layerNum) {
                case 1:
                    return Layer.TopName;

                case 16:
                    return Layer.BottomName;

                case 17:
                    return Layer.PadsName;

                case 18:
                    return Layer.ViasName;

                case 19:
                    return Layer.UnroutedName;

                case 20:
                    return Layer.ProfileName;

                case 21:
                    return Layer.TopPlaceName;

                case 22:
                    return Layer.BottomPlaceName;

                case 25:
                    return Layer.TopNamesName;

                case 26:
                    return Layer.BottomNamesName;

                case 27:
                    return Layer.TopValuesName;

                case 28:
                    return Layer.BottomValuesName;

                case 29:
                    return Layer.TopStopName;

                case 30:
                    return Layer.BottomStopName;

                case 31:
                    return Layer.TopCreamName;

                case 32:
                    return Layer.BottomCreamName;

                case 35:
                    return Layer.TopGlueName;

                case 36:
                    return Layer.BottomGlueName;

                case 39:
                    return Layer.TopKeepoutName;

                case 40:
                    return Layer.BottomKeepoutName;

                case 41:
                    return Layer.TopRestrictName;

                case 42:
                    return Layer.BottomRestrictName;

                case 43:
                    return Layer.ViaRestrictName;

                case 44:
                    return Layer.DrillsName;

                case 45:
                    return Layer.HolesName;

                case 51:
                    return Layer.TopDocumentName;

                case 52:
                    return Layer.BottomDocumentName;

                default:
                    return Layer.UnknownName;
            }
        }

        /// <summary>
        /// Obte el color de la capa a partir del seu numero.
        /// </summary>
        /// <param name="layerNum">El numero de la capa.</param>
        /// <returns>El color.</returns>
        /// 
        private Color GetLayerColor(int layerNum) {

            Color color = Colors.White;

            switch (layerNum) {
                case 1: // Top signal
                    color = Colors.Red;
                    color.ScA = 0.60f;
                    break;

                case 16: // Bottom signal
                    color = Colors.Blue;
                    color.ScA = 0.80f;
                    break;

                case 17: // Pads
                    color = Color.FromRgb(234, 161, 64);
                    break;

                case 18: // Vias
                    color = Colors.Green;
                    break;

                case 19: // Unrouted
                    color = Colors.Yellow;
                    break;

                case 21: // Top placement
                case 22: // Bottom placement
                    color =  Colors.LightGray;
                    color.ScA = 0.8f;
                    break;

                case 25:
                case 26:
                    color = Colors.LightGray;
                    break;

                case 31:
                case 32:
                    color = Colors.LightSeaGreen;
                    break;

                case 35: // Top glue
                case 36: // Bottom glue
                    color = Colors.LightSkyBlue;
                    break;

                case 39: // Top keepout
                case 40: // Bottom keepout
                    color = Colors.Cyan;
                    color.ScA = 0.40f;
                    break;

                case 41: // Top restrict
                case 42: // Bottom restrict
                case 43: // Via restrict
                    color = Colors.DarkViolet;
                    color.ScA = 0.40f;
                    break;

                case 45: // Holes
                    color = Colors.LightCoral;
                    break;

                case 51: // Top document
                case 52: // Bottom document
                    color = Color.FromRgb(160, 160, 160);
                    color.ScA = 0.80f;
                    break;
            }

            return color;
        }

        /// <summary>
        /// Obte objecte Component a partir del seu nom.
        /// </summary>
        /// <param name="name">El nom del component.</param>
        /// <returns>El component.</returns>
        /// 
        private Block GetComponent(string name) {

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
