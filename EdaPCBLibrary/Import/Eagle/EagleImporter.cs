namespace MikroPic.EdaTools.v1.Pcb.Import.Eagle {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Media;
    using System.Xml;

    using Color = MikroPic.EdaTools.v1.Geometry.Color;
    using SysColor = System.Windows.Media.Color;
    using SysPoint = System.Windows.Point;

    /// <summary>
    /// Clase per importar una placa desde Eagle
    /// </summary>
    /// 
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
                LayerId layerId = GetLayerId(layerNum);

                // Si la capa no existeix, la crea i l'afegeix a la placa.
                //
                if (board.GetLayer(layerId, false) == null) {
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

                    case "circle":
                        element = ParseCircleNode(node);
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
                            try {
                                board.Connect(signal, pad, part);
                            }
                            catch {
                                throw new InvalidOperationException(
                                    String.Format("El pad '{0}' del elemento '{1}', ya esta conectado a la señal '{2}'",
                                    pad.Name, part.Name, signal.Name));
                            }
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
            LayerId layerId = GetLayerId(layerNum);
            LayerFunction function = LayerFunction.Unknown;
            if (layerNum >= 1 && layerNum <= 16)
                function = LayerFunction.Signal;
            else if (layerNum == 20)
                function = LayerFunction.Outline;

            return new Layer(layerId, function);
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
            Point position = new Point(x, y);

            // Obte l'angle de rotacio
            //
            Angle rotation = Angle.Zero;
            if (AttributeExists(node, "rot"))
                rotation = ParseAngle(GetAttributeAsString(node, "rot"));

            // Obte el tamany del forat
            //
            int drill = ParseNumber(GetAttributeAsString(node, "drill"));

            // Obte el diametre
            //
            int size = (drill * 16) / 10;
            if (AttributeExists(node, "diameter"))
                size = ParseNumber(GetAttributeAsString(node, "diameter"));

            ThPadElement.ThPadShape shape = ThPadElement.ThPadShape.Circle;
            switch (GetAttributeAsString(node, "shape")) {
                case "square":
                    shape = ThPadElement.ThPadShape.Square;
                    break;

                case "octagon":
                    shape = ThPadElement.ThPadShape.Octagon;
                    break;

                case "long":
                    shape = ThPadElement.ThPadShape.Oval;
                    break;
            }

            LayerSet layerSet = new LayerSet(Layer.PadsId, Layer.DrillsId, Layer.TopId, Layer.TopStopId, Layer.BottomId, Layer.BottomStopId);
            return new ThPadElement(name, layerSet, position, rotation, size, shape, drill);
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
            Point position = new Point(x, y);

            // Obte el tamany
            //
            int width = ParseNumber(GetAttributeAsString(node, "dx"));
            int height = ParseNumber(GetAttributeAsString(node, "dy"));
            Size size = new Size(width, height);

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
            LayerId layerId = GetLayerId(layerNum);
            LayerSet layerSet = new LayerSet(layerId);
            if (cream) 
                layerSet = layerSet + Layer.TopCreamId;
            if (stop) 
                layerSet = layerSet + Layer.TopStopId;

            return new SmdPadElement(name, layerSet, position, size, rotation, roundness);
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
            Point position = new Point(x, y);

            int drill = ParseNumber(GetAttributeAsString(node, "drill"));

            int size = 0;
            if (AttributeExists(node, "diameter"))
                size = ParseNumber(GetAttributeAsString(node, "diameter"));

            string extent = GetAttributeAsString(node, "extent");
            string[] layerNames = extent.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int[] layerNums = new int[layerNames.Length];
            for (int i = 0; i < layerNums.Length; i++)
                layerNums[i] = Int32.Parse(layerNames[i]);

            ViaElement.ViaShape shape = ViaElement.ViaShape.Circle;
            string shapeName = GetAttributeAsString(node, "shape");
            switch (shapeName) {
                case "square":
                    shape = ViaElement.ViaShape.Square;
                    break;

                case "octagon":
                    shape = ViaElement.ViaShape.Octagon;
                    break;
            }

            LayerSet layerSet = new LayerSet(Layer.ViasId, Layer.DrillsId);
            foreach (int layerNum in layerNums) {
                LayerId layerId = GetLayerId(layerNum);
                layerSet = layerSet + layerId;
            }

            Element element = new ViaElement(layerSet, position, size, drill, shape);

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
            Point position = new Point(x, y);

            // Obte l'angle de rotacio
            //
            Angle rotation = Angle.Zero;
            if (AttributeExists(node, "rot"))
                rotation = ParseAngle(GetAttributeAsString(node, "rot"));

            int height = ParseNumber(GetAttributeAsString(node, "size"));

            HorizontalTextAlign horizontalAlign = ParseHorizontalTextAlign(GetAttributeAsString(node, "align"));
            VerticalTextAlign verticalAlign = ParseVerticalTextAlign(GetAttributeAsString(node, "align"));

            int thickness = 100000;

            int layerNum = GetAttributeAsInteger(node, "layer");
            LayerId layerId = GetLayerId(layerNum);
            LayerSet layerSet = new LayerSet(layerId);

            TextElement element = new TextElement(layerSet, position, rotation, height, thickness, horizontalAlign, verticalAlign);
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

            int x1 = ParseNumber(GetAttributeAsString(node, "x1"));
            int y1 = ParseNumber(GetAttributeAsString(node, "y1"));
            Point p1 = new Point(x1, y1);

            int x2 = ParseNumber(GetAttributeAsString(node, "x2"));
            int y2 = ParseNumber(GetAttributeAsString(node, "y2"));
            Point p2 = new Point(x2, y2);

            Angle angle = Angle.Zero;
            if (AttributeExists(node, "curve"))
                angle = ParseAngle(GetAttributeAsString(node, "curve"));
            LineElement.LineCapStyle lineCap = GetAttributeAsString(node, "cap") == null ? LineElement.LineCapStyle.Round : LineElement.LineCapStyle.Flat;
            int thickness = ParseNumber(GetAttributeAsString(node, "width"));
            if (thickness == 0)
                thickness = 100000;

            int layerNum = GetAttributeAsInteger(node, "layer");
            LayerId layerId = GetLayerId(layerNum);
            LayerSet layerSet = new LayerSet(layerId);

            Element element;
            if (angle.IsZero)
                element = new LineElement(layerSet, p1, p2, thickness, lineCap);
            else
                element = new ArcElement(layerSet, p1, p2, thickness, angle, lineCap);

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
            Point position = new Point((x1 + x2) / 2, (y1 + y2) / 2);
            Size size = new Size(x2 - x1, y2 - y1);

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
            LayerId layerId = GetLayerId(layerNum);
            LayerSet layerSet = new LayerSet(layerId);

            Element element = new RectangleElement(layerSet, position, size, Ratio.Zero, rotation, thickness, thickness == 0);

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
            Point position = new Point(x, y);

            // Obte l'amplada de linia
            //
            int thickness = 0;
            if (AttributeExists(node, "width"))
                thickness = ParseNumber(GetAttributeAsString(node, "width"));

            // Obte el radi
            //
            int radius = ParseNumber(GetAttributeAsString(node, "radius"));

            int layerNum = GetAttributeAsInteger(node, "layer");
            LayerId layerId = GetLayerId(layerNum);
            LayerSet layerSet = new LayerSet(layerId);

            Element element = new CircleElement(layerSet, position, radius, thickness, thickness == 0);

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

            // Obte l'aillament
            //
            int clearance = 0;
            if (AttributeExists(node, "isolate"))
                clearance = ParseNumber(GetAttributeAsString(node, "isolate"));

            int layerNum = GetAttributeAsInteger(node, "layer");
            LayerId layerId = GetLayerId(layerNum);
            LayerSet layerSet = new LayerSet(layerId);

            List<RegionElement.Segment> segments = new List<RegionElement.Segment>();
            foreach (XmlNode vertexNode in node.SelectNodes("vertex")) {

                // Obte la posicio
                //
                int x = ParseNumber(GetAttributeAsString(vertexNode, "x"));
                int y = ParseNumber(GetAttributeAsString(vertexNode, "y"));
                Point vertex = new Point(x, y);

                // Obte la curvatura
                //
                Angle angle = Angle.Zero;
                if (AttributeExists(vertexNode, "curve"))
                    angle = ParseAngle(GetAttributeAsString(vertexNode, "curve"));

                segments.Add(new RegionElement.Segment(vertex, angle));
            }

            Element element = new RegionElement(layerSet, thickness, true, clearance, segments);

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
            Point position = new Point(x, y);

            // Obte el diametre del forat
            //
            int drill = ParseNumber(GetAttributeAsString(node, "drill"));

            LayerSet layerSet = new LayerSet(Layer.HolesId, Layer.TopId, Layer.BottomId);
            HoleElement element = new HoleElement(layerSet, position, drill);

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
            Point position = new Point(x, y);

            // Obte l'angle de rotacio i la cara
            //
            Angle rotation = Angle.Zero;
            BoardSide side = BoardSide.Top;
            if (AttributeExists(node, "rot")) {
                string rot = GetAttributeAsString(node, "rot");
                if (rot.Contains("M"))
                   side = BoardSide.Bottom;

                rotation = ParseAngle(rot);
            }

            Part part = new Part(GetComponent(componentKey), name, position, rotation, side);

            bool hasNameAttribute = false;
            bool hasValueAttribute = false;
            foreach (XmlNode attrNode in node.SelectNodes("attribute")) {

                PartAttribute parameter = ParseAttributeNode(attrNode);

                // Inicialitza els valor per defecte dels parametres NAME i VALUE
                //
                if (parameter.Name == "NAME") {
                    parameter.Value = name;
                    hasNameAttribute = true;
                }
                else if (parameter.Name == "VALUE") {
                    parameter.Value = value;
                    hasValueAttribute = true;
                }

                // Corrigeix perque la posicio sigui relativa al component
                //
                SysPoint p = new SysPoint(parameter.Position.X, parameter.Position.Y);

                Matrix m = new Matrix();
                m.RotateAt(-rotation.Degrees / 100, position.X, position.Y);
                m.Translate(-position.X, -position.Y);
                p = m.Transform(p);

                parameter.Position = new Point((int)p.X, (int)p.Y);
                parameter.Rotation = parameter.Rotation - rotation;

                part.AddAttribute(parameter);
            }

            if (!hasNameAttribute)
                part.AddAttribute(new PartAttribute("NAME", name, true));
            
            if (!hasValueAttribute) 
                part.AddAttribute(new PartAttribute("VALUE", value, true));

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
                attribute.Position = new Point(x, y);
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
            if (AttributeExists(node, "align")) {
                attribute.HorizontalAlign = ParseHorizontalTextAlign(GetAttributeAsString(node, "align"));
                attribute.VerticalAlign = ParseVerticalTextAlign(GetAttributeAsString(node, "align"));
            }
            else {
                attribute.HorizontalAlign = HorizontalTextAlign.Left;
                attribute.VerticalAlign = VerticalTextAlign.Bottom;
            }

            return attribute;
        }

        /// <summary>
        /// Obte el identificador de la capa a partir del seu numero.
        /// </summary>
        /// <param name="layerNum">Numero de la capa.</param>
        /// <returns>El identificador de la capa.</returns>
        /// 
        private static LayerId GetLayerId(int layerNum) {

            switch (layerNum) {
                case 1:
                    return Layer.TopId;

                case 16:
                    return Layer.BottomId;

                case 17:
                    return Layer.PadsId;

                case 18:
                    return Layer.ViasId;

                case 19:
                    return Layer.UnroutedId;

                case 20:
                    return Layer.ProfileId;

                case 21:
                    return Layer.TopPlaceId;

                case 22:
                    return Layer.BottomPlaceId;

                case 25:
                    return Layer.TopNamesId;

                case 26:
                    return Layer.BottomNamesId;

                case 27:
                    return Layer.TopValuesId;

                case 28:
                    return Layer.BottomValuesId;

                case 29:
                    return Layer.TopStopId;

                case 30:
                    return Layer.BottomStopId;

                case 31:
                    return Layer.TopCreamId;

                case 32:
                    return Layer.BottomCreamId;

                case 35:
                    return Layer.TopGlueId;

                case 36:
                    return Layer.BottomGlueId;

                case 39:
                    return Layer.TopKeepoutId;

                case 40:
                    return Layer.BottomKeepoutId;

                case 41:
                    return Layer.TopRestrictId;

                case 42:
                    return Layer.BottomRestrictId;

                case 43:
                    return Layer.ViaRestrictId;

                case 44:
                    return Layer.DrillsId;

                case 45:
                    return Layer.HolesId;

                case 51:
                    return Layer.TopDocumentId;

                case 52:
                    return Layer.BottomDocumentId;

                default:
                    return Layer.UnknownId;
            }
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
        /// Converteix un text a Angle.
        /// </summary>
        /// <param name="s">El text a convertir.</param>
        /// <returns>El valor obtingut.</returns>
        /// <remarks>L'angle pot portar un prefix, que cal descartar.</remarks>
        /// 
        private static Angle ParseAngle(string s) {

            int index = 0;
            if (Char.IsLetter(s[index]))
                index++;
            if (Char.IsLetter(s[index]))
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
        private static HorizontalTextAlign ParseHorizontalTextAlign(string s) {

            switch (s) {
                case "top-left":
                case "center-left":
                case "bottom-left":
                default:
                    return HorizontalTextAlign.Left;

                case "top-center":
                case "center-center":
                case "bottom-center":
                    return HorizontalTextAlign.Center;

                case "top-right":
                case "center-right":
                case "bottom-right":
                    return HorizontalTextAlign.Right;
            }
        }

        /// <summary>
        /// Converteix un text a TextAlign
        /// </summary>
        /// <param name="s">El text a convertir.</param>
        /// <returns>El valor obtingut.</returns>
        /// 
        private static VerticalTextAlign ParseVerticalTextAlign(string s) {

            switch (s) {
                case "top-left":
                case "top-center":
                case "top-right":
                    return VerticalTextAlign.Top;

                case "center-left":
                case "center-center":
                case "center-right":
                    return VerticalTextAlign.Middle;

                case "bottom-left":
                case "bottom-center":
                case "bottom-right":
                default:
                    return VerticalTextAlign.Bottom;
            }
        }

        private static bool AttributeExists(XmlNode node, string name) {

            return node.Attributes[name] != null;
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="name">El nom del atribut.</param>
        /// <returns>El valor obtingut.</returns>
        /// 
        private static string GetAttributeAsString(XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            return attribute == null ? null : attribute.Value;
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defValue">El valor per defecte..</param>
        /// <returns>El seu valor com a double.</returns>
        /// 
        private static double GetAttributeDouble(XmlNode node, string name, int defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else
                return XmlConvert.ToDouble(attribute.Value);
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defValue">El valor per defecte.</param>
        /// <returns>El valor com integer.</returns>
        /// 
        private static int GetAttributeAsInteger(XmlNode node, string name, int defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else 
                return XmlConvert.ToInt32(attribute.Value);
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defValue">El valor per defecte.</param>
        /// <returns>El valor com boolean.</returns>
        /// 
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
