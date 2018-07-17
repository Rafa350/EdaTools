namespace MikroPic.EdaTools.v1.Pcb.Import.Eagle {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        public override Board Read(Stream stream) {

            doc = ReadXmlDocument(stream);
            board = new Board();

            ProcessLayers();
            ProcessSignals();
            ProcessBlocks();
            ProcessElements();

            return board;
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
                int layerNum = GetAttributeAsInteger(layerNode, "number");
                string name = GetLayerName(layerNum);

                // Si la capa no existeix, la crea i l'afegeix a la placa.
                //
                if (board.GetLayer(name, false) == null) {
                    Layer layer = ParseLayerNode(layerNode);
                    board.AddLayer(layer);
                }
            }

            // Crera els parells de capes
            //
            board.DefinePair(board.GetLayer(Layer.TopName), board.GetLayer(Layer.BottomName));
            board.DefinePair(board.GetLayer(Layer.TopPlaceName), board.GetLayer(Layer.BottomPlaceName));
            board.DefinePair(board.GetLayer(Layer.TopNamesName), board.GetLayer(Layer.BottomNamesName));
            board.DefinePair(board.GetLayer(Layer.TopValuesName), board.GetLayer(Layer.BottomValuesName));
            board.DefinePair(board.GetLayer(Layer.TopStopName), board.GetLayer(Layer.BottomStopName));
            board.DefinePair(board.GetLayer(Layer.TopCreamName), board.GetLayer(Layer.BottomCreamName));
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

                string libraryName = GetAttributeAsString(libraryNode, "name");

                // Procesa el tag <package>
                //
                foreach (XmlNode packageNode in libraryNode.SelectNodes("packages/package")) {

                    string packageName = GetAttributeAsString(packageNode, "name");

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

                string signalName = GetAttributeAsString(node, "name");
                Signal signal = signalDict[signalName];

                foreach (XmlNode childNode in node.SelectNodes("contactref")) {
                    string partName = GetAttributeAsString(childNode, "element");
                    string padName = GetAttributeAsString(childNode, "pad");

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

            string signalName = GetAttributeAsString(node, "name");

            return new Signal(signalName);
        }

        /// <summary>
        /// Procesa un node LAYER.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Layer' creat.</returns>
        /// 
        private Layer ParseLayerNode(XmlNode node) {

            int layerNum = GetAttributeAsInteger(node, "number");
            string layerName = GetLayerName(layerNum);

            BoardSide side = BoardSide.Unknown;
            if ((layerNum == 1) || layerName.Contains("Top"))
                side = BoardSide.Top;
            else if (layerNum > 1 && layerNum < 16) 
                side = BoardSide.Inner;
            else if ((layerNum == 16) || layerName.Contains("Bottom"))
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

            string name = GetAttributeAsString(node, "name");

            // Obte la posicio
            //
            int x = ParseNumber(GetAttributeAsString(node, "x"));
            int y = ParseNumber(GetAttributeAsString(node, "y"));
            PointInt position = new PointInt(x, y);

            // Obte l'angle de rotacio
            //
            Angle rotation = Angle.Zero;
            if (AttributeExists(node, "rot"))
                rotation = ParseAngle(GetAttributeAsString(node, "rot"));

            // Obte el tamany del forat
            //
            int drill = ParseNumber(GetAttributeAsString(node, "drill"));
            int size = (drill * 16) / 10;

            ThPadElement.ThPadShape shape = ThPadElement.ThPadShape.Circular;
            switch (GetAttributeAsString(node, "shape")) {
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

            Element element = new ThPadElement(name, position, rotation, size, shape, drill);

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

            string name = GetAttributeAsString(node, "name");

            // Obte la posicio
            //
            int x = ParseNumber(GetAttributeAsString(node, "x"));
            int y = ParseNumber(GetAttributeAsString(node, "y"));
            PointInt position = new PointInt(x, y);

            // Obte el tamany
            //
            int width = ParseNumber(GetAttributeAsString(node, "dx"));
            int height = ParseNumber(GetAttributeAsString(node, "dy"));
            SizeInt size = new SizeInt(width, height);

            // Obte la rotacio
            //
            Angle rotation = Angle.Zero;
            if (AttributeExists(node, "rot"))
                ParseAngle(GetAttributeAsString(node, "rot"));

            // Obte el factor d'arrodoniment
            //
            Ratio roundness = Ratio.Zero;
            if (AttributeExists(node, "roundness"))
                roundness = ParseRatio(GetAttributeAsString(node, "roundness"));

            bool stop = GetAttributeAsBoolean(node, "stop", true);
            bool cream = GetAttributeAsBoolean(node, "cream", true);

            int layerNum = GetAttributeAsInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            Element element = new SmdPadElement(name, position, size, rotation, roundness);
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

            int x = ParseNumber(GetAttributeAsString(node, "x"));
            int y = ParseNumber(GetAttributeAsString(node, "y"));
            PointInt position = new PointInt(x, y);

            int drill = ParseNumber(GetAttributeAsString(node, "drill"));

            int size = 0;
            if (AttributeExists(node, "diameter"))
                size = ParseNumber(GetAttributeAsString(node, "diameter"));

            string extent = GetAttributeAsString(node, "extent");
            string[] layerNames = extent.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int[] layerNums = new int[layerNames.Length];
            for (int i = 0; i < layerNums.Length; i++)
                layerNums[i] = Int32.Parse(layerNames[i]);

            ViaElement.ViaShape shape = ViaElement.ViaShape.Circular;
            string shapeName = GetAttributeAsString(node, "shape");
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

            // Obte la posicio
            //
            int x = ParseNumber(GetAttributeAsString(node, "x"));
            int y = ParseNumber(GetAttributeAsString(node, "y"));
            PointInt position = new PointInt(x, y);

            // Obte l'angle de rotacio
            //
            Angle rotation = Angle.Zero;
            if (AttributeExists(node, "rot"))
                rotation = ParseAngle(GetAttributeAsString(node, "rot"));

            int height = ParseNumber(GetAttributeAsString(node, "size"));

            TextAlign align = ParseTextAlign(GetAttributeAsString(node, "align"));

            int thickness = 100000;

            int layerNum = GetAttributeAsInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            TextElement element = new TextElement(position, rotation, height, thickness, align);
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

            int x1 = ParseNumber(GetAttributeAsString(node, "x1"));
            int y1 = ParseNumber(GetAttributeAsString(node, "y1"));
            PointInt p1 = new PointInt(x1, y1);

            int x2 = ParseNumber(GetAttributeAsString(node, "x2"));
            int y2 = ParseNumber(GetAttributeAsString(node, "y2"));
            PointInt p2 = new PointInt(x2, y2);

            Angle angle = Angle.Zero;
            if (AttributeExists(node, "curve"))
                angle = ParseAngle(GetAttributeAsString(node, "curve"));
            LineElement.LineCapStyle lineCap = GetAttributeAsString(node, "cap") == null ? LineElement.LineCapStyle.Round : LineElement.LineCapStyle.Flat;
            int thickness = ParseNumber(GetAttributeAsString(node, "width"));
            if (thickness == 0)
                thickness = 100000;

            int layerNum = GetAttributeAsInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            Element element;
            if (angle.IsZero)
                element = new LineElement(p1, p2, thickness, lineCap);
            else
                element = new ArcElement(p1, p2, thickness, angle, lineCap);

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

            // Obte la posicio i el tamany
            //
            int x1 = ParseNumber(GetAttributeAsString(node, "x1"));
            int y1 = ParseNumber(GetAttributeAsString(node, "y1"));
            int x2 = ParseNumber(GetAttributeAsString(node, "x2"));
            int y2 = ParseNumber(GetAttributeAsString(node, "y2"));
            PointInt position = new PointInt((x1 + x2) / 2, (y1 + y2) / 2);
            SizeInt size = new SizeInt(x2 - x1, y2 - y1);

            // Obte l'angle de rotacio
            //
            Angle rotation = Angle.Zero;
            if (AttributeExists(node, "rot"))
                rotation = ParseAngle(GetAttributeAsString(node, "rot"));

            // Obte l'amplada de linia
            //
            int thickness = 0;
            if (AttributeExists(node, "width"))
                thickness = ParseNumber(GetAttributeAsString(node, "width"));

            int layerNum = GetAttributeAsInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            Element element = new RectangleElement(position, size, rotation, thickness);

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

            // obte la posicio
            //
            int x = ParseNumber(GetAttributeAsString(node, "x"));
            int y = ParseNumber(GetAttributeAsString(node, "y"));
            PointInt position = new PointInt(x, y);

            // Obte l'amplada de linia
            //
            int thickness = 0;
            if (AttributeExists(node, "width"))
                thickness = ParseNumber(GetAttributeAsString(node, "width"));

            // Obte el radi
            //
            int radius = ParseNumber(GetAttributeAsString(node, "radius"));

            int layerNum = GetAttributeAsInteger(node, "layer");
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

            // Obte l'amplada de linia
            //
            int thickness = ParseNumber(GetAttributeAsString(node, "width"));

            int layerNum = GetAttributeAsInteger(node, "layer");
            string layerName = GetLayerName(layerNum);

            List<RegionElement.Segment> segments = new List<RegionElement.Segment>();
            foreach (XmlNode vertexNode in node.SelectNodes("vertex")) {

                // Obte la posicio
                //
                int x = ParseNumber(GetAttributeAsString(vertexNode, "x"));
                int y = ParseNumber(GetAttributeAsString(vertexNode, "y"));
                PointInt vertex = new PointInt(x, y);

                // Obte la curvatura
                //
                Angle angle = Angle.Zero;
                if (AttributeExists(vertexNode, "curve"))
                    angle = ParseAngle(GetAttributeAsString(vertexNode, "curve"));

                segments.Add(new RegionElement.Segment(vertex, angle));
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

            // obte la posicio
            //
            int x = ParseNumber(GetAttributeAsString(node, "x"));
            int y = ParseNumber(GetAttributeAsString(node, "y"));
            PointInt position = new PointInt(x, y);

            // Obte el diametre del forat
            //
            int drill = ParseNumber(GetAttributeAsString(node, "drill"));

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

            string name = GetAttributeAsString(node, "name");
            string value = GetAttributeAsString(node, "value");
            string componentKey = String.Format(
                "{0}@{1}",
                GetAttributeAsString(node, "package"),
                GetAttributeAsString(node, "library"));

            // Obte la posicio
            //
            int x = ParseNumber(GetAttributeAsString(node, "x"));
            int y = ParseNumber(GetAttributeAsString(node, "y"));
            PointInt position = new PointInt(x, y);

            // Obte l'angle de rotacio
            //
            Angle rotation = Angle.Zero;

            // obte la cara de la placa
            //
            BoardSide side = BoardSide.Top;

            if (AttributeExists(node, "rot")) {

                string rot = GetAttributeAsString(node, "rot");

                int i;
                for (i = 0; i < rot.Length; i++) {
                    if (rot[i] == 'M')
                       side = BoardSide.Bottom;

                    else if (Char.IsDigit(rot[i]))
                        break;
                }

                rotation = ParseAngle(rot.Substring(i));
            }

            Part part = new Part(GetComponent(componentKey), name, position, rotation, side);


            foreach (XmlNode attrNode in node.SelectNodes("attribute")) {

                PartAttribute parameter = ParseAttributeNode(attrNode);

                // Inicialitza els valor per defecte dels parametres NAME i VALUE
                //
                if (parameter.Name == "NAME")
                    parameter.Value = name;
                else if (parameter.Name == "VALUE")
                    parameter.Value = value;

                // Corrigeix perque la posicio sigui relativa al component
                //
                System.Windows.Point p = new System.Windows.Point(parameter.Position.X, parameter.Position.Y);

                Matrix m = new Matrix();
                m.RotateAt(-rotation.Degrees / 100, position.X, position.Y);
                m.Translate(-position.X, -position.Y);
                p = m.Transform(p);

                parameter.Position = new PointInt((int)p.X, (int)p.Y);
                parameter.Rotation = parameter.Rotation - rotation;

                part.AddAttribute(parameter);
            }

            return part;
        }

        /// <summary>
        /// Procesa un node ATTRIBUTE
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'PartAttribute' creat.</returns>
        /// 
        private PartAttribute ParseAttributeNode(XmlNode node) {

            string name = GetAttributeAsString(node, "name");
            string value = GetAttributeAsString(node, "value");

            PartAttribute attribute = new PartAttribute(name, value);

            attribute.IsVisible = GetAttributeAsString(node, "display") != "off";

            // Obte la posicio
            //
            if (AttributeExists(node, "x")) {
                int x = ParseNumber(GetAttributeAsString(node, "x"));
                int y = ParseNumber(GetAttributeAsString(node, "y"));
                attribute.Position = new PointInt(x, y);
            }

            // Obte l'angle de rotacio
            //
            if (AttributeExists(node, "rot"))
                attribute.Rotation = ParseAngle(GetAttributeAsString(node, "rot"));

            // Obte l'alçada de lletra
            //
            if (AttributeExists(node, "size"))
                attribute.Height = ParseNumber(GetAttributeAsString(node, "size"));

            // Obte l'aliniacio
            //
            if (AttributeExists(node, "align"))
                attribute.Align = ParseTextAlign(GetAttributeAsString(node, "align"));

            return attribute;
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

        /// <summary>
        /// Converteix un text a int
        /// </summary>
        /// <param name="s">El text a convertir,</param>
        /// <returns>El valor obtingut.</returns>
        /// 
        private static int ParseNumber(string s) {

            double value = XmlConvert.ToDouble(s);
            return (int)(value * 1000000.0);
        }

        /// <summary>
        /// Converteix un text a Ratio
        /// </summary>
        /// <param name="s">El text a convertir.</param>
        /// <returns>El valor obtingut.</returns>
        /// 
        private static Ratio ParseRatio(string s) {

            double value = XmlConvert.ToDouble(s);
            return Ratio.FromPercent((int)(value * 10.0));
        }

        /// <summary>
        /// Converteix un text a Angle
        /// </summary>
        /// <param name="s">El text a convertir.</param>
        /// <returns>El valor aobtingut.</returns>
        /// 
        private static Angle ParseAngle(string s) {

            int index = 0;
            if (!Char.IsDigit(s[index]))
                index++;
            if (!Char.IsDigit(s[index]))
                index++;

            double value = XmlConvert.ToDouble(s.Substring(index));
            return Angle.FromDegrees((int)(value * 100.0));
        }

        /// <summary>
        /// Converteix un text a TextAlign
        /// </summary>
        /// <param name="s">El text a convertir.</param>
        /// <returns>El valor obtingut.</returns>
        /// 
        private static TextAlign ParseTextAlign(string s) {

            switch (s) {
                case "top-left":
                    return TextAlign.TopLeft;

                case "top-center":
                    return TextAlign.TopCenter;

                case "top-right":
                    return TextAlign.TopRight;

                case "center-left":
                    return TextAlign.MiddleLeft;

                case "center-center":
                    return TextAlign.MiddleCenter;

                case "center-right":
                    return TextAlign.MiddleRight;

                case "bottom-left":
                    return TextAlign.BottomLeft;

                case "bottom-center":
                    return TextAlign.BottomCenter;

                case "bottom-right":
                    return TextAlign.BottomRight;

                default:
                    return TextAlign.BottomLeft;
            }
        }

        private static bool AttributeExists(XmlNode node, string name) {

            return node.Attributes[name] != null;
        }

        private static string GetAttributeAsString(XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            return attribute == null ? null : attribute.Value;
        }

        private static double GetAttributeDouble(XmlNode node, string name, int defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else {
                string value = attribute.Value;
                int start = 0;
                if (value.IndexOf("R") != -1)
                    start++;
                return XmlConvert.ToDouble(value.Substring(start));
            }
        }

        private static int GetAttributeAsInteger(XmlNode node, string name, int defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else 
                return XmlConvert.ToInt32(attribute.Value);
        }

        private static bool GetAttributeAsBoolean(XmlNode node, string name, bool defValue = false) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else
                return
                   String.Compare(attribute.Value, "yes", true) == 0 ||
                   String.Compare(attribute.Value, "true", true) == 0;
        }
    }
}
