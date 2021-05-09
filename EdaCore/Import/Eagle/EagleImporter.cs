using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Net;

namespace MikroPic.EdaTools.v1.Core.Import.Eagle {

    /// <summary>
    /// Clase per importar una placa desde Eagle
    /// </summary>
    /// 
    public sealed class EagleImporter : IImporter {

        private const int topLayerNum = 1;
        private const int bottomLayerNum = 16;
        private const int padsLayerNum = 17;
        private const int viasLayerNum = 18;
        private const int drillsLayerNum = 44;
        private const int holesLayerNum = 45;

        private readonly Dictionary<string, Component> componentDict = new Dictionary<string, Component>();
        private readonly Dictionary<string, Signal> signalDict = new Dictionary<string, Signal>();
        private readonly Dictionary<Element, string> mapElementSignal = new Dictionary<Element, string>();

        private Board board;
        private XmlDocument doc;


        /// <inheritdoc/>
        /// 
        public Board ReadBoard(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {

                doc = ReadXmlDocument(stream);
                board = new Board();

                XmlNode layersNode = doc.SelectSingleNode("eagle/drawing/layers");
                IEnumerable<Layer> layers = ParseLayersNode(layersNode);
                board.AddLayers(layers);

                ProcessSignals();
                ProcessComponents();
                ProcessElements();

                return board;
            }
        }

        /// <inheritdoc/>
        /// 
        public Library ReadLibrary(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {

                doc = ReadXmlDocument(stream);

                XmlNode layersNode = doc.SelectSingleNode("eagle/drawing/layers");
                IEnumerable<Layer> layers = ParseLayersNode(layersNode);

                XmlNode packagesNode = doc.SelectSingleNode("eagle/drawing/library/packages");
                IEnumerable<Component> components = ParsePackagesNode(packagesNode);

                Library library = new Library("unnamed");
                library.AddComponents(components);
                return library;
            }
        }

        /// <inheritdoc/>
        /// 
        public Net ReadNet(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {

                doc = ReadXmlDocument(stream);

                XmlNode netsNode = doc.SelectSingleNode("eagle/drawing/schematic/sheets/sheet/nets");
                IEnumerable<NetSignal> signals = ParseNetsNode(netsNode);

                return new Net(signals);
            }
        }

        /// <summary>
        /// Carrega el document XML en format EAGLE
        /// </summary>
        /// <param name="stream">Stream d'entrada.</param>
        /// <returns>El document XML carregat.</returns>
        /// 
        private XmlDocument ReadXmlDocument(Stream stream) {

            XmlReaderSettings settings = new XmlReaderSettings {
                DtdProcessing = DtdProcessing.Ignore,
                CloseInput = true,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };

            XmlReader reader = XmlReader.Create(stream, settings);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return doc;
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
        /// Procesa els components
        /// </summary>
        /// 
        private void ProcessComponents() {

            // Procesa el tag <library>
            //
            foreach (XmlNode libraryNode in doc.SelectNodes("eagle/drawing/board/libraries/library")) {

                string libraryName = libraryNode.AttributeAsString("name");

                // Procesa el tag <package>
                //
                foreach (XmlNode packageNode in libraryNode.SelectNodes("packages/package")) {
                    Component component = ParsePackageNode(packageNode, libraryName);
                    board.AddComponent(component);
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

                string signalName = node.AttributeAsString("name");
                Signal signal = signalDict[signalName];

                foreach (XmlNode childNode in node.SelectNodes("contactref")) {
                    string partName = childNode.AttributeAsString("element");
                    string padName = childNode.AttributeAsString("pad");

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

                        if (element is IConectable conectable)
                            board.Connect(signal, conectable);
                    }
                }
            }
        }

        /// <summary>
        /// Procesa un node 'LAYERS'.
        /// </summary>
        /// <param name="layersNode">El node a procesar.</param>
        /// <returns>La llista de capes.</returns>
        /// 
        private IEnumerable<Layer> ParseLayersNode(XmlNode layersNode) {

            var layers = new List<Layer>();

            foreach (XmlNode layerNode in layersNode) {
                Layer layer = ParseLayerNode(layerNode);
                layers.Add(layer);
            }

            return layers;
        }

        /// <summary>
        /// Procesa un node 'LAYER'
        /// </summary>
        /// <param name="layerNode">El node a procesar.</param>
        /// <returns>L'objecte 'Layer' creat.</returns>
        /// 
        private Layer ParseLayerNode(XmlNode layerNode) {

            int layerNum = Int32.Parse(layerNode.Attributes["number"].Value);
            Layer layer = new Layer(GetLayerSide(layerNum), GetLayerTag(layerNum), GetLayerFunction(layerNum));
            //layerNames.Add(layerNum, layer.Name);

            return layer;
        }

        /// <summary>
        /// Procesa el node 'PACKAGES'
        /// </summary>
        /// <param name="packagesNode">El node a procesar.</param>
        /// <returns>La llista de components.</returns>
        /// 
        private IEnumerable<Component> ParsePackagesNode(XmlNode packagesNode) {

            var components = new List<Component>();
            foreach (XmlNode packageNode in packagesNode.ChildNodes) {
                Component component = ParsePackageNode(packageNode);
                components.Add(component);
            }

            return components;
        }

        /// <summary>
        /// Procesa un node 'PACKAGE'.
        /// </summary>
        /// <param name="packageNode">El node a procesar.</param>
        /// <returns>El component.</returns>
        /// 
        private Component ParsePackageNode(XmlNode packageNode, string libraryName = null) {

            string packageName = packageNode.AttributeAsString("name");
            string packageDescription = null;

            var elements = new List<Element>();
            foreach (XmlNode elementNode in packageNode.ChildNodes) {
                Element element = null;
                switch (elementNode.Name) {
                    case "description":
                        packageDescription = elementNode.InnerText;
                        break;

                    case "smd":
                        element = ParseSmdNode(elementNode);
                        break;

                    case "pad":
                        element = ParsePadNode(elementNode);
                        break;

                    case "text":
                        element = ParseTextNode(elementNode);
                        break;

                    case "wire":
                        element = ParseWireNode(elementNode);
                        break;

                    case "rectangle":
                        element = ParseRectangleNode(elementNode);
                        break;

                    case "circle":
                        element = ParseCircleNode(elementNode);
                        break;

                    case "polygon":
                        element = ParsePolygonNode(elementNode);
                        break;

                    case "hole":
                        element = ParseHoleNode(elementNode);
                        break;

                    default:
                        throw new InvalidOperationException(
                            String.Format("No se reconoce el tag '{0}'.", elementNode.Name));
                }
                if (element != null)
                    elements.Add(element);
            }

            string name = libraryName == null ? packageName : String.Format("{0}@{1}", packageName, libraryName);
            var component = new Component(name, elements);
            component.Description = packageDescription;
            return component;
        }

        /// <summary>
        /// Procesa el node 'ELEMENTS'.
        /// </summary>
        /// <param name="elementsNode">El node a procesar.</param>
        /// <returns>La llista d'elements.</returns>
        /// 
        private IEnumerable<Part> ParseElementsNode(XmlNode elementsNode) {

            var parts = new List<Part>();
            foreach (XmlNode elementNode in elementsNode.ChildNodes) {
                Part part = ParseElementNode(elementNode);
                parts.Add(part);
            }

            return parts;
        }

        /// <summary>
        /// Procesa un node 'SIGNAL'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte Signal creat.</returns>
        /// 
        private Signal ParseSignalNode(XmlNode node) {

            string signalName = node.AttributeAsString("name");

            return new Signal(signalName);
        }

        /// <summary>
        /// Procesa un node 'PAD'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ThPadElement' creat.</returns>
        /// 
        private Element ParsePadNode(XmlNode node) {

            string name = node.AttributeAsString("name");

            // Obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            Point position = new Point(x, y);

            // Obte l'angle de rotacio
            //
            Angle rotation = Angle.Zero;
            if (node.AttributeExists("rot"))
                rotation = ParseAngle(node.AttributeAsString("rot"));

            // Obte el tamany del forat
            //
            int drill = ParseNumber(node.AttributeAsString("drill"));

            // Obte el diametre
            //
            int size = (drill * 16) / 10;
            if (node.AttributeExists("diameter"))
                size = ParseNumber(node.AttributeAsString("diameter"));

            ThPadElement.ThPadShape shape = ThPadElement.ThPadShape.Circle;
            switch (node.AttributeAsString("shape")) {
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

            string padsLayerId = GetLayerName(padsLayerNum);
            string drillsLayerId = GetLayerName(drillsLayerNum);

            LayerSet layerSet = new LayerSet(padsLayerId, drillsLayerId, "Top.Copper", "Top.Stop", "Bottom.Copper", "Bottom.Stop");
            return new ThPadElement(name, layerSet, position, rotation, size, shape, drill);
        }

        /// <summary>
        /// Procesa un node 'SMD'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'SmdPadElement' creat.</returns>
        /// 
        private Element ParseSmdNode(XmlNode node) {

            string name = node.AttributeAsString("name");

            // Obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            Point position = new Point(x, y);

            // Obte el tamany
            //
            int width = ParseNumber(node.AttributeAsString("dx"));
            int height = ParseNumber(node.AttributeAsString("dy"));
            Size size = new Size(width, height);

            // Obte la rotacio
            //
            Angle rotation = Angle.Zero;
            if (node.AttributeExists("rot"))
                ParseAngle(node.AttributeAsString("rot"));

            // Obte el factor d'arrodoniment
            //
            Ratio roundness = Ratio.Zero;
            if (node.AttributeExists("roundness"))
                roundness = ParseRatio(node.AttributeAsString("roundness"));

            bool stop = node.AttributeAsBoolean("stop", true);
            bool cream = node.AttributeAsBoolean("cream", true);

            int layerNum = node.AttributeAsInteger("layer");
            LayerSet layerSet = new LayerSet(GetLayerName(layerNum));
            if (cream)
                layerSet = layerSet + "Top.Cream";
            if (stop)
                layerSet = layerSet + "Top.Stop";

            return new SmdPadElement(name, layerSet, position, size, rotation, roundness);
        }

        /// <summary>
        /// Procesa un node 'VIA'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ViaElement' creat.</returns>
        /// 
        private Element ParseViaNode(XmlNode node) {

            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            Point position = new Point(x, y);

            int drill = ParseNumber(node.AttributeAsString("drill"));

            int size = 0;
            if (node.AttributeExists("diameter"))
                size = ParseNumber(node.AttributeAsString("diameter"));

            string extent = node.AttributeAsString("extent");
            string[] layerNames = extent.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int[] layerNums = new int[layerNames.Length];
            for (int i = 0; i < layerNums.Length; i++)
                layerNums[i] = Int32.Parse(layerNames[i]);

            ViaElement.ViaShape shape = ViaElement.ViaShape.Circle;
            string shapeName = node.AttributeAsString("shape");
            switch (shapeName) {
                case "square":
                    shape = ViaElement.ViaShape.Square;
                    break;

                case "octagon":
                    shape = ViaElement.ViaShape.Octagon;
                    break;
            }

            string viasLayerId = GetLayerName(viasLayerNum);
            string drillsLayerId = GetLayerName(drillsLayerNum);

            LayerSet layerSet = new LayerSet(viasLayerId, drillsLayerId);
            foreach (int layerNum in layerNums) {
                layerSet = layerSet + GetLayerName(layerNum);
            }

            Element element = new ViaElement(layerSet, position, size, drill, shape);

            return element;
        }

        /// <summary>
        /// Procesa un node 'TEXT'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'TextElement' creat.</returns>
        /// 
        private Element ParseTextNode(XmlNode node) {

            string value = node.InnerText;

            // Obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            Point position = new Point(x, y);

            // Obte l'angle de rotacio
            //
            Angle rotation = Angle.Zero;
            if (node.AttributeExists("rot"))
                rotation = ParseAngle(node.AttributeAsString("rot"));

            int height = ParseNumber(node.AttributeAsString("size"));

            HorizontalTextAlign horizontalAlign = ParseHorizontalTextAlign(node.AttributeAsString("align"));
            VerticalTextAlign verticalAlign = ParseVerticalTextAlign(node.AttributeAsString("align"));

            int thickness = 100000;

            int layerNum = node.AttributeAsInteger("layer");
            LayerSet layerSet = new LayerSet(GetLayerName(layerNum));

            TextElement element = new TextElement(layerSet, position, rotation, height, thickness, horizontalAlign, verticalAlign);
            element.Value = value;

            return element;
        }

        /// <summary>
        /// Procesa un node 'WIRE'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'LineElement' o ArcElement' creat.</returns>
        /// 
        private Element ParseWireNode(XmlNode node) {

            int x1 = ParseNumber(node.AttributeAsString("x1"));
            int y1 = ParseNumber(node.AttributeAsString("y1"));
            Point p1 = new Point(x1, y1);

            int x2 = ParseNumber(node.AttributeAsString("x2"));
            int y2 = ParseNumber(node.AttributeAsString("y2"));
            Point p2 = new Point(x2, y2);

            Angle angle = Angle.Zero;
            if (node.AttributeExists("curve"))
                angle = ParseAngle(node.AttributeAsString("curve"));
            LineElement.CapStyle lineCap = node.AttributeAsString("cap") == null ? LineElement.CapStyle.Round : LineElement.CapStyle.Flat;
            int thickness = ParseNumber(node.AttributeAsString("width"));
            if (thickness == 0)
                thickness = 100000;

            int layerNum = node.AttributeAsInteger("layer");
            LayerSet layerSet = new LayerSet(GetLayerName(layerNum));

            Element element;
            if (angle.IsZero)
                element = new LineElement(layerSet, p1, p2, thickness, lineCap);
            else
                element = new ArcElement(layerSet, p1, p2, thickness, angle, lineCap);

            return element;
        }

        /// <summary>
        /// Procesa un node 'RECTANGLE'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RectangleElement' creat.</returns>
        /// 
        private Element ParseRectangleNode(XmlNode node) {

            // Obte la posicio i el tamany
            //
            int x1 = ParseNumber(node.AttributeAsString("x1"));
            int y1 = ParseNumber(node.AttributeAsString("y1"));
            int x2 = ParseNumber(node.AttributeAsString("x2"));
            int y2 = ParseNumber(node.AttributeAsString("y2"));
            Point position = new Point((x1 + x2) / 2, (y1 + y2) / 2);
            Size size = new Size(x2 - x1, y2 - y1);

            // Obte l'angle de rotacio
            //
            Angle rotation = Angle.Zero;
            if (node.AttributeExists("rot"))
                rotation = ParseAngle(node.AttributeAsString("rot"));

            // Obte l'amplada de linia
            //
            int thickness = 0;
            if (node.AttributeExists("width"))
                thickness = ParseNumber(node.AttributeAsString("width"));

            int layerNum = node.AttributeAsInteger("layer");
            LayerSet layerSet = new LayerSet(GetLayerName(layerNum));

            Element element = new RectangleElement(layerSet, position, size, Ratio.Zero, rotation, thickness, thickness == 0);

            return element;
        }

        /// <summary>
        /// Procesa un node 'CIRCLE'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'CircleElement' creat.</returns>
        /// 
        private Element ParseCircleNode(XmlNode node) {

            // obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            Point position = new Point(x, y);

            // Obte l'amplada de linia
            //
            int thickness = 0;
            if (node.AttributeExists("width"))
                thickness = ParseNumber(node.AttributeAsString("width"));

            // Obte el radi
            //
            int radius = ParseNumber(node.AttributeAsString("radius"));

            int layerNum = node.AttributeAsInteger("layer");
            LayerSet layerSet = new LayerSet(GetLayerName(layerNum));

            Element element = new CircleElement(layerSet, position, radius, thickness, thickness == 0);

            return element;
        }

        /// <summary>
        /// Procesa un node 'POLYGON'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RegionElement' creat.</returns>
        /// 
        private Element ParsePolygonNode(XmlNode node) {

            // Obte l'amplada de linia
            //
            int thickness = ParseNumber(node.AttributeAsString("width"));

            // Obte l'aillament
            //
            int clearance = 0;
            if (node.AttributeExists("isolate"))
                clearance = ParseNumber(node.AttributeAsString("isolate"));

            int layerNum = node.AttributeAsInteger("layer");
            LayerSet layerSet = new LayerSet(GetLayerName(layerNum));

            List<RegionElement.Segment> segments = new List<RegionElement.Segment>();
            foreach (XmlNode vertexNode in node.SelectNodes("vertex")) {

                // Obte la posicio
                //
                int x = ParseNumber(vertexNode.AttributeAsString("x"));
                int y = ParseNumber(vertexNode.AttributeAsString("y"));
                Point vertex = new Point(x, y);

                // Obte la curvatura
                //
                Angle angle = Angle.Zero;
                if (vertexNode.AttributeExists("curve"))
                    angle = ParseAngle(vertexNode.AttributeAsString("curve"));

                segments.Add(new RegionElement.Segment(vertex, angle));
            }

            Element element = new RegionElement(layerSet, thickness, true, clearance, segments);

            return element;
        }

        /// <summary>
        /// Procesa un node 'HOLE'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'HoleElement' creat.</returns>
        /// 
        private Element ParseHoleNode(XmlNode node) {

            // obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            Point position = new Point(x, y);

            // Obte el diametre del forat
            //
            int drill = ParseNumber(node.AttributeAsString("drill"));

            LayerSet layerSet = new LayerSet("Holes");
            HoleElement element = new HoleElement(layerSet, position, drill);

            return element;
        }

        /// <summary>
        /// Procesa un node 'ELEMENT'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Part' creat.</returns>
        /// 
        private Part ParseElementNode(XmlNode node) {

            string name = node.AttributeAsString("name");
            string value = node.AttributeAsString("value");
            string componentKey = String.Format(
                "{0}@{1}",
                node.AttributeAsString("package"),
                node.AttributeAsString("library"));

            // Obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            Point position = new Point(x, y);

            // Obte l'angle de rotacio i la cara
            //
            Angle rotation = Angle.Zero;
            bool flip = false;
            if (node.AttributeExists("rot")) {
                string rot = node.AttributeAsString("rot");
                if (rot.Contains("M"))
                    flip = true;

                rotation = ParseAngle(rot);
            }

            Part part = new Part(GetComponent(componentKey), name, position, rotation, flip);

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
                Point p = new Point(parameter.Position.X, parameter.Position.Y);

                Transformation t = new Transformation();
                t.Rotate(position, -rotation);
                t.Translate(new Point(-position.X, -position.Y));
                p = t.ApplyTo(p);

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
        /// Procesa un node 'ATTRIBUTE'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'PartAttribute' creat.</returns>
        /// 
        private PartAttribute ParseAttributeNode(XmlNode node) {

            string name = node.AttributeAsString("name");
            string value = node.AttributeAsString("value");

            PartAttribute attribute = new PartAttribute(name, value);

            attribute.IsVisible = node.AttributeAsString("display") != "off";

            // Obte la posicio
            //
            if (node.AttributeExists("x")) {
                int x = ParseNumber(node.AttributeAsString("x"));
                int y = ParseNumber(node.AttributeAsString("y"));
                attribute.Position = new Point(x, y);
            }

            // Obte l'angle de rotacio
            //
            if (node.AttributeExists("rot"))
                attribute.Rotation = ParseAngle(node.AttributeAsString("rot"));

            // Obte l'alçada de lletra
            //
            if (node.AttributeExists("size"))
                attribute.Height = ParseNumber(node.AttributeAsString("size"));

            // Obte l'aliniacio
            //
            if (node.AttributeExists("align")) {
                attribute.HorizontalAlign = ParseHorizontalTextAlign(node.AttributeAsString("align"));
                attribute.VerticalAlign = ParseVerticalTextAlign(node.AttributeAsString("align"));
            }
            else {
                attribute.HorizontalAlign = HorizontalTextAlign.Left;
                attribute.VerticalAlign = VerticalTextAlign.Bottom;
            }

            return attribute;
        }

        /// <summary>
        /// Procesa un node 'NETS'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>La coleccio d'objectes 'NetSignal' creada.</returns>
        /// 
        private IEnumerable<NetSignal> ParseNetsNode(XmlNode node) {

            // Procesa el tag <nets>
            //
            List<NetSignal> signals = new List<NetSignal>();
            foreach (XmlNode netNode in node.SelectNodes("net")) {
                NetSignal signal = ParseNetNode(netNode);
                signals.Add(signal);
            }

            return signals;
        }

        /// <summary>
        /// Procesa un node 'NET'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'NetElement' creat.</returns>
        /// 
        private NetSignal ParseNetNode(XmlNode node) {

            string netName = node.AttributeAsString("name");

            List<NetConnection> netConnections = new List<NetConnection>();
            foreach (XmlNode pinrefNode in node.SelectNodes("segment/pinref")) {
                IEnumerable<NetConnection> result = ParsePinrefNode(pinrefNode);
                if (result != null)
                    netConnections.AddRange(result);
            }

            return new NetSignal(netName, netConnections);
        }

        /// <summary>
        /// Procesa un node 'PINREF'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>La llista d'objectes 'NetConnection' creada.</returns>
        /// 
        private IEnumerable<NetConnection> ParsePinrefNode(XmlNode node) {

            string partName = node.AttributeAsString("part");
            string gateName = node.AttributeAsString("gate");
            string pinName = node.AttributeAsString("pin");

            string padNames = GetPadName(partName, gateName, pinName);
            if (padNames == null)
                return null;
            else {
                List<NetConnection> connections = new List<NetConnection>();
                foreach (string padName in padNames.Split(' ')) {
                    NetConnection connection = new NetConnection(partName, padName);
                    connections.Add(connection);
                }
                return connections;
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
            return Ratio.FromValue((int)(value * 10.0));
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
            return Angle.FromValue((int)(value * 100.0));
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

        /// <summary>
        /// Obte el nom d'una capa a partir del seu identificador numeric. Si la capa
        /// no existeix, la crea per futures referencies i l'afegeix a la placa.
        /// </summary>
        /// <param name="layerNum">Identificador de la capa.</param>
        /// <returns>El nom de la capa.</returns>
        /// 
        private static string GetLayerName(int layerNum) {

            return Layer.GetName(GetLayerSide(layerNum), GetLayerTag(layerNum));
        }

        /// <summary>
        /// Obte l'etiqueta de la capa.
        /// </summary>
        /// <param name="layerNum">Identificador de la capa.</param>
        /// <returns>L'etiqueta.</returns>
        /// 
        private static string GetLayerTag(int layerNum) {

            switch (layerNum) {
                case topLayerNum:
                case bottomLayerNum:
                    return "Copper";

                case padsLayerNum:
                    return "Pads";

                case viasLayerNum:
                    return "Vias";

                case 19:
                    return "Unrouted";

                case 20:
                    return "Profile";

                case 21:
                case 22:
                    return "Place";

                case 25:
                case 26:
                    return "Names";

                case 27:
                case 28:
                    return "Values";

                case 29:
                case 30:
                    return "Stop";

                case 31:
                case 32:
                    return "Cream";

                case 35:
                case 36:
                    return "Glue";

                case 39:
                case 40:
                    return "Keepout";

                case 41:
                case 42:
                    return "Restrict";

                case 43:
                    return "ViaRestrict";

                case drillsLayerNum:
                    return "Drills";

                case holesLayerNum:
                    return "Holes";

                case 51:
                case 52:
                    return "Document";

                default:
                    return "Unknown" + layerNum.ToString();
            }
        }

        /// <summary>
        /// Obte la cara a la que pertany la capa
        /// </summary>
        /// <param name="layerNum">Identificador de la capa.</param>
        /// <returns>La cara a la que pertany.</returns>
        /// 
        private static BoardSide GetLayerSide(int layerNum) {

            switch (layerNum) {
                case topLayerNum:
                case 21:
                case 25:
                case 27:
                case 29:
                case 31:
                case 35:
                case 39:
                case 41:
                case 51:
                    return BoardSide.Top;

                case bottomLayerNum:
                case 22:
                case 26:
                case 28:
                case 30:
                case 32:
                case 36:
                case 40:
                case 42:
                case 52:
                    return BoardSide.Bottom;

                default:
                    return BoardSide.None;
            }
        }

        /// <summary>
        /// Obte la funcio de la capa.
        /// </summary>
        /// <param name="layerNum">El identificador de la capa.</param>
        /// <returns>La funcio de la capa.</returns>
        /// 
        private static LayerFunction GetLayerFunction(int layerNum) {

            switch (layerNum) {
                case topLayerNum:
                case bottomLayerNum:
                    return LayerFunction.Signal;

                case 20:
                    return LayerFunction.Outline;

                case 21:
                case 22:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 35:
                case 36:
                case 39:
                case 40:
                case 41:
                case 42:
                case 51:
                case 52:
                    return LayerFunction.Design;

                default:
                    return LayerFunction.Unknown;
            }
        }

        private string GetPadName(string partName, string gateName, string pinName) {

            XmlNode partNode = doc.SelectSingleNode(
                String.Format("eagle/drawing/schematic/parts/part[@name='{0}']", partName));

            string libraryName = partNode.AttributeAsString("library");
            string deviceSetName = partNode.AttributeAsString("deviceset");
            string deviceName = partNode.AttributeAsString("device");

            XmlNode libraryNode = doc.SelectSingleNode(
                String.Format("eagle/drawing/schematic/libraries/library[@name='{0}']", libraryName));

            XmlNode deviceSetNode = libraryNode.SelectSingleNode(
                String.Format("devicesets/deviceset[@name='{0}']", deviceSetName));

            XmlNode deviceNode = deviceSetNode.SelectSingleNode(
                String.Format("devices/device[@name='{0}']", deviceName));

            XmlNode connectNode = deviceNode.SelectSingleNode(
                String.Format("connects/connect[@pin='{0}']", pinName));
            if (connectNode == null)
                return null;

            string padName = connectNode.AttributeAsString("pad");

            return padName;
        }
    }
}
