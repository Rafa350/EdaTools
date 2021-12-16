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
    public sealed class EagleImporter : IEdaImporter {

        private const int topLayerNum = 1;
        private const int bottomLayerNum = 16;
        private const int padsLayerNum = 17;
        private const int viasLayerNum = 18;
        private const int drillsLayerNum = 44;
        private const int holesLayerNum = 45;

        private readonly Dictionary<string, EdaComponent> componentDict = new Dictionary<string, EdaComponent>();
        private readonly Dictionary<string, EdaSignal> signalDict = new Dictionary<string, EdaSignal>();
        private readonly Dictionary<EdaElement, string> mapElementSignal = new Dictionary<EdaElement, string>();

        private EdaBoard board;
        private XmlDocument doc;


        /// <inheritdoc/>
        /// 
        public EdaBoard ReadBoard(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {

                doc = ReadXmlDocument(stream);
                board = new EdaBoard();

                XmlNode layersNode = doc.SelectSingleNode("eagle/drawing/layers");
                IEnumerable<EdaLayer> layers = ParseLayersNode(layersNode);
                board.AddLayers(layers);

                ProcessSignals();
                ProcessComponents();
                ProcessElements();

                return board;
            }
        }

        /// <inheritdoc/>
        /// 
        public EdaLibrary ReadLibrary(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {

                doc = ReadXmlDocument(stream);

                XmlNode layersNode = doc.SelectSingleNode("eagle/drawing/layers");
                IEnumerable<EdaLayer> layers = ParseLayersNode(layersNode);

                XmlNode packagesNode = doc.SelectSingleNode("eagle/drawing/library/packages");
                IEnumerable<EdaComponent> components = ParsePackagesNode(packagesNode);

                EdaLibrary library = new EdaLibrary("unnamed");
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

            var settings = new XmlReaderSettings {
                DtdProcessing = DtdProcessing.Ignore,
                CloseInput = true,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };

            var reader = XmlReader.Create(stream, settings);
            var doc = new XmlDocument();
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

                EdaSignal signal = ParseSignalNode(signalNode);
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
                    var component = ParsePackageNode(packageNode, libraryName);
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
            foreach (EdaPart part in ParseElementsNode(doc.SelectSingleNode("eagle/drawing/board/elements")))
                board.AddPart(part);

            // Procesa el tag <plain>
            //
            foreach (XmlNode node in doc.SelectSingleNode("eagle/drawing/board/plain")) {

                EdaElement element = null;
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
                EdaSignal signal = signalDict[signalName];

                foreach (XmlNode childNode in node.SelectNodes("contactref")) {
                    string partName = childNode.AttributeAsString("element");
                    string padName = childNode.AttributeAsString("pad");

                    EdaPart part = board.GetPart(partName, true);
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
                    EdaElement element = null;
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

                        if (element is IEdaConectable conectable)
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
        private IEnumerable<EdaLayer> ParseLayersNode(XmlNode layersNode) {

            var layers = new List<EdaLayer>();

            foreach (XmlNode layerNode in layersNode) {
                EdaLayer layer = ParseLayerNode(layerNode);
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
        private EdaLayer ParseLayerNode(XmlNode layerNode) {

            int layerNum = Int32.Parse(layerNode.Attributes["number"].Value);
            return new EdaLayer(GetLayerId(layerNum), GetLayerSide(layerNum), GetLayerFunction(layerNum));
        }

        /// <summary>
        /// Procesa el node 'PACKAGES'
        /// </summary>
        /// <param name="packagesNode">El node a procesar.</param>
        /// <returns>La llista de components.</returns>
        /// 
        private IEnumerable<EdaComponent> ParsePackagesNode(XmlNode packagesNode) {

            var components = new List<EdaComponent>();
            foreach (XmlNode packageNode in packagesNode.ChildNodes) {
                var component = ParsePackageNode(packageNode);
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
        private EdaComponent ParsePackageNode(XmlNode packageNode, string libraryName = null) {

            string packageName = packageNode.AttributeAsString("name");
            string packageDescription = null;

            var elements = new List<EdaElement>();
            foreach (XmlNode elementNode in packageNode.ChildNodes) {
                EdaElement element = null;
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
            
            var component = new EdaComponent();
            component.Name = name;
            component.Description = packageDescription;
            component.AddElements(elements);
            
            return component;
        }

        /// <summary>
        /// Procesa el node 'ELEMENTS'.
        /// </summary>
        /// <param name="elementsNode">El node a procesar.</param>
        /// <returns>La llista d'elements.</returns>
        /// 
        private IEnumerable<EdaPart> ParseElementsNode(XmlNode elementsNode) {

            var parts = new List<EdaPart>();
            foreach (XmlNode elementNode in elementsNode.ChildNodes) {
                EdaPart part = ParseElementNode(elementNode);
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
        private EdaSignal ParseSignalNode(XmlNode node) {

            var name = node.AttributeAsString("name");
            var clearance = 150000;

            return new EdaSignal {
                Name = name,
                Clearance = clearance
            };
        }

        /// <summary>
        /// Procesa un node 'PAD'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ThPadElement' creat.</returns>
        /// 
        private EdaElement ParsePadNode(XmlNode node) {

            // Obte el nom
            //
            string name = node.AttributeAsString("name");

            // Obte el conjunt de capes
            //
            //LayerSet layerSet = ParseLayer(node.AttributeAsString("layers"));
            EdaLayerSet layerSet = new EdaLayerSet();
            layerSet.Add(EdaLayerId.TopCopper);
            layerSet.Add(EdaLayerId.BottomCopper);
            layerSet.Add(EdaLayerId.TopStop);
            layerSet.Add(EdaLayerId.BottomStop);
            layerSet.Add(EdaLayerId.Drills);

            // Obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            EdaPoint position = new EdaPoint(x, y);

            // Obte l'angle de rotacio
            //
            EdaAngle rotation = EdaAngle.Zero;
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

            return new ThPadElement {
                Name = name,
                LayerSet = layerSet,
                Position = position,
                Rotation = rotation,
                TopSize = size,
                InnerSize = size,
                BottomSize = size,
                Shape = shape,
                Drill = drill
            };
        }

        /// <summary>
        /// Procesa un node 'SMD'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'SmdPadElement' creat.</returns>
        /// 
        private EdaElement ParseSmdNode(XmlNode node) {

            string name = node.AttributeAsString("name");

            // Obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            EdaPoint position = new EdaPoint(x, y);

            // Obte el tamany
            //
            int width = ParseNumber(node.AttributeAsString("dx"));
            int height = ParseNumber(node.AttributeAsString("dy"));
            EdaSize size = new EdaSize(width, height);

            // Obte la rotacio
            //
            EdaAngle rotation = EdaAngle.Zero;
            if (node.AttributeExists("rot"))
                ParseAngle(node.AttributeAsString("rot"));

            // Obte el factor d'arrodoniment
            //
            EdaRatio roundness = EdaRatio.Zero;
            if (node.AttributeExists("roundness"))
                roundness = ParseRatio(node.AttributeAsString("roundness"));

            int layerNum = node.AttributeAsInteger("layer");
            var layerSet = new EdaLayerSet(GetLayerId(layerNum));

            return new SmdPadElement {
                Name = name,
                LayerSet = layerSet,
                Position = position,
                Size = size,
                Rotation = rotation,
                Roundness = roundness
            };
        }

        /// <summary>
        /// Procesa un node 'VIA'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ViaElement' creat.</returns>
        /// 
        private EdaElement ParseViaNode(XmlNode node) {

            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            EdaPoint position = new EdaPoint(x, y);

            int drill = ParseNumber(node.AttributeAsString("drill"));

            int size = 0;
            if (node.AttributeExists("diameter"))
                size = ParseNumber(node.AttributeAsString("diameter"));

            string extent = node.AttributeAsString("extent");
            string[] layerNames = extent.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            EdaLayerId topLayerId = GetLayerId(Int32.Parse(layerNames[0]));
            EdaLayerId bottomLayerId = GetLayerId(Int32.Parse(layerNames[1]));

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

            return new ViaElement { 
                LayerSet = new EdaLayerSet(topLayerId, bottomLayerId), 
                Position = position, 
                OuterSize = size, 
                InnerSize = size,
                Drill = drill, 
                Shape = shape 
            };
        }

        /// <summary>
        /// Procesa un node 'TEXT'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'TextElement' creat.</returns>
        /// 
        private EdaElement ParseTextNode(XmlNode node) {

            string value = node.InnerText;
            if (value.StartsWith('>'))
                value = String.Format("{{{0}}}", value.Substring(1));

            // Obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            EdaPoint position = new EdaPoint(x, y);

            // Obte l'angle de rotacio
            //
            EdaAngle rotation = EdaAngle.Zero;
            if (node.AttributeExists("rot"))
                rotation = ParseAngle(node.AttributeAsString("rot"));

            int height = ParseNumber(node.AttributeAsString("size"));

            HorizontalTextAlign horizontalAlign = ParseHorizontalTextAlign(node.AttributeAsString("align"));
            VerticalTextAlign verticalAlign = ParseVerticalTextAlign(node.AttributeAsString("align"));

            int thickness = 100000;

            int layerNum = node.AttributeAsInteger("layer");
            var layerSet = new EdaLayerSet(GetLayerId(layerNum));

            var element = new TextElement {
                LayerSet = layerSet,
                Position = position,
                Rotation = rotation,
                Height = height,
                Thickness = thickness,
                HorizontalAlign = horizontalAlign,
                VerticalAlign = verticalAlign
            };
            element.Value = value;

            return element;
        }

        /// <summary>
        /// Procesa un node 'WIRE'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'LineElement' o ArcElement' creat.</returns>
        /// 
        private EdaElement ParseWireNode(XmlNode node) {

            int x1 = ParseNumber(node.AttributeAsString("x1"));
            int y1 = ParseNumber(node.AttributeAsString("y1"));
            EdaPoint p1 = new EdaPoint(x1, y1);

            int x2 = ParseNumber(node.AttributeAsString("x2"));
            int y2 = ParseNumber(node.AttributeAsString("y2"));
            EdaPoint p2 = new EdaPoint(x2, y2);

            EdaAngle angle = EdaAngle.Zero;
            if (node.AttributeExists("curve"))
                angle = ParseAngle(node.AttributeAsString("curve"));
            LineElement.CapStyle lineCap = node.AttributeAsString("cap") == null ? LineElement.CapStyle.Round : LineElement.CapStyle.Flat;
            int thickness = ParseNumber(node.AttributeAsString("width"));
            if (thickness == 0)
                thickness = 100000;

            int layerNum = node.AttributeAsInteger("layer");
            var layerId = GetLayerId(layerNum);

            EdaElement element;
            if (angle.IsZero)
                element = new LineElement {
                    StartPosition = p1,
                    EndPosition = p2,
                    Thickness = thickness,
                    LineCap = lineCap
                };
            else
                element = new ArcElement {
                    StartPosition = p1,
                    EndPosition = p2,
                    Thickness = thickness,
                    Angle = angle,
                    LineCap = lineCap
                };
            element.LayerSet.Add(layerId);

            return element;
        }

        /// <summary>
        /// Procesa un node 'RECTANGLE'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RectangleElement' creat.</returns>
        /// 
        private EdaElement ParseRectangleNode(XmlNode node) {

            // Obte la posicio i el tamany
            //
            int x1 = ParseNumber(node.AttributeAsString("x1"));
            int y1 = ParseNumber(node.AttributeAsString("y1"));
            int x2 = ParseNumber(node.AttributeAsString("x2"));
            int y2 = ParseNumber(node.AttributeAsString("y2"));
            var position = new EdaPoint((x1 + x2) / 2, (y1 + y2) / 2);
            var size = new EdaSize(x2 - x1, y2 - y1);

            // Obte l'angle de rotacio
            //
            var rotation = EdaAngle.Zero;
            if (node.AttributeExists("rot"))
                rotation = ParseAngle(node.AttributeAsString("rot"));

            // Obte l'amplada de linia
            //
            int thickness = 0;
            if (node.AttributeExists("width"))
                thickness = ParseNumber(node.AttributeAsString("width"));

            var layerNum = node.AttributeAsInteger("layer");
            var layerSet = new EdaLayerSet(GetLayerId(layerNum));

            return new RectangleElement {
                LayerSet = layerSet,
                Position = position,
                Size = size,
                Roundness = EdaRatio.Zero,
                Rotation = rotation,
                Thickness = thickness,
                Filled = thickness == 0
            };
        }

        /// <summary>
        /// Procesa un node 'CIRCLE'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'CircleElement' creat.</returns>
        /// 
        private EdaElement ParseCircleNode(XmlNode node) {

            // obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            var position = new EdaPoint(x, y);

            // Obte l'amplada de linia
            //
            int thickness = 0;
            if (node.AttributeExists("width"))
                thickness = ParseNumber(node.AttributeAsString("width"));

            // Obte el radi
            //
            int radius = ParseNumber(node.AttributeAsString("radius"));

            int layerNum = node.AttributeAsInteger("layer");
            var layerSet = new EdaLayerSet(GetLayerId(layerNum));

            return new CircleElement {
                LayerSet = layerSet,
                Position = position,
                Radius = radius,
                Thickness = thickness,
                Filled = thickness == 0
            };
        }

        /// <summary>
        /// Procesa un node 'POLYGON'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RegionElement' creat.</returns>
        /// 
        private EdaElement ParsePolygonNode(XmlNode node) {

            // Obte l'amplada de linia
            //
            int thickness = ParseNumber(node.AttributeAsString("width"));

            // Obte l'aillament
            //
            int clearance = 0;
            if (node.AttributeExists("isolate"))
                clearance = ParseNumber(node.AttributeAsString("isolate"));

            int layerNum = node.AttributeAsInteger("layer");
            var layerId = GetLayerId(layerNum);
            var layerSet = new EdaLayerSet(layerId);

            var segments = new List<EdaArcPoint>();
            foreach (XmlNode vertexNode in node.SelectNodes("vertex")) {

                // Obte la posicio
                //
                int x = ParseNumber(vertexNode.AttributeAsString("x"));
                int y = ParseNumber(vertexNode.AttributeAsString("y"));
                EdaPoint vertex = new EdaPoint(x, y);

                // Obte la curvatura
                //
                EdaAngle angle = EdaAngle.Zero;
                if (vertexNode.AttributeExists("curve"))
                    angle = ParseAngle(vertexNode.AttributeAsString("curve"));

                segments.Add(new EdaArcPoint(vertex, angle));
            }

            var element = new RegionElement {
                LayerSet = layerSet,
                Thickness = thickness,
                Filled = true,
                Clearance = clearance,
                Segments = segments
            };
            return element;
        }

        /// <summary>
        /// Procesa un node 'HOLE'.
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'HoleElement' creat.</returns>
        /// 
        private EdaElement ParseHoleNode(XmlNode node) {

            // obte la posicio
            //
            int x = ParseNumber(node.AttributeAsString("x"));
            int y = ParseNumber(node.AttributeAsString("y"));
            var position = new EdaPoint(x, y);

            // Obte el diametre del forat
            //
            int drill = ParseNumber(node.AttributeAsString("drill"));

            var element = new HoleElement {
                Position = position,
                Drill = drill
            };
            element.LayerSet.Add(EdaLayerId.Holes);
            return element;
        }

        /// <summary>
        /// Procesa un node 'ELEMENT'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Part' creat.</returns>
        /// 
        private EdaPart ParseElementNode(XmlNode node) {

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
            var position = new EdaPoint(x, y);

            // Obte l'angle de rotacio i la cara
            //
            EdaAngle rotation = EdaAngle.Zero;
            PartSide side = PartSide.Top;
            if (node.AttributeExists("rot")) {
                string rot = node.AttributeAsString("rot");
                if (rot.Contains('M'))
                    side = PartSide.Bottom;

                rotation = ParseAngle(rot);
            }

            var part = new EdaPart {
                Component = GetComponent(componentKey),
                Name = name,
                Position = position,
                Rotation = rotation,
                Side = side
            };

            bool hasNameAttribute = false;
            bool hasValueAttribute = false;
            foreach (XmlNode attrNode in node.SelectNodes("attribute")) {

                EdaPartAttribute parameter = ParseAttributeNode(attrNode);

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
                EdaPoint p = new EdaPoint(parameter.Position.X, parameter.Position.Y);

                Transformation t = new Transformation();
                t.Rotate(position, -rotation);
                t.Translate(new EdaPoint(-position.X, -position.Y));
                p = t.ApplyTo(p);

                parameter.Position = new EdaPoint((int)p.X, (int)p.Y);
                parameter.Rotation = parameter.Rotation - rotation;

                part.AddAttribute(parameter);
            }

            if (!hasNameAttribute)
                part.AddAttribute(new EdaPartAttribute("NAME", name, true));

            if (!hasValueAttribute)
                part.AddAttribute(new EdaPartAttribute("VALUE", value, true));

            return part;
        }

        /// <summary>
        /// Procesa un node 'ATTRIBUTE'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'PartAttribute' creat.</returns>
        /// 
        private EdaPartAttribute ParseAttributeNode(XmlNode node) {

            string name = node.AttributeAsString("name");
            string value = node.AttributeAsString("value");

            EdaPartAttribute attribute = new EdaPartAttribute(name, value);

            attribute.IsVisible = node.AttributeAsString("display") != "off";

            // Obte la posicio
            //
            if (node.AttributeExists("x")) {
                int x = ParseNumber(node.AttributeAsString("x"));
                int y = ParseNumber(node.AttributeAsString("y"));
                attribute.Position = new EdaPoint(x, y);
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
        private EdaComponent GetComponent(string name) {

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
        private static EdaRatio ParseRatio(string s) {

            double value = XmlConvert.ToDouble(s);
            return EdaRatio.FromValue((int)(value * 10.0));
        }

        /// <summary>
        /// Converteix un text a Angle.
        /// </summary>
        /// <param name="s">El text a convertir.</param>
        /// <returns>El valor obtingut.</returns>
        /// <remarks>L'angle pot portar un prefix, que cal descartar.</remarks>
        /// 
        private static EdaAngle ParseAngle(string s) {

            int index = 0;
            if (Char.IsLetter(s[index]))
                index++;
            if (Char.IsLetter(s[index]))
                index++;

            double value = XmlConvert.ToDouble(s.Substring(index));
            return EdaAngle.FromValue((int)(value * 100.0));
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
        /// Obte l'etiqueta de la capa.
        /// </summary>
        /// <param name="layerNum">Identificador de la capa.</param>
        /// <returns>L'etiqueta.</returns>
        /// 
        private static EdaLayerId GetLayerId(int layerNum) {

            switch (layerNum) {
                case topLayerNum:
                    return EdaLayerId.TopCopper;

                case bottomLayerNum:
                    return EdaLayerId.BottomCopper;

                case padsLayerNum:
                    return EdaLayerId.Pads;

                case viasLayerNum:
                    return EdaLayerId.Vias;

                case 19:
                    return EdaLayerId.Unrouted;

                case 20:
                    return EdaLayerId.Profile;

                case 21:
                    return EdaLayerId.TopPlace;
                case 22:
                    return EdaLayerId.BottomPlace;

                case 25:
                    return EdaLayerId.TopNames;
                case 26:
                    return EdaLayerId.BottomNames;

                case 27:
                    return EdaLayerId.TopValues;
                case 28:
                    return EdaLayerId.BottomValues;

                case 29:
                    return EdaLayerId.TopStop;
                case 30:
                    return EdaLayerId.BottomStop;

                case 31:
                    return EdaLayerId.TopCream;
                case 32:
                    return EdaLayerId.BottomCream;

                case 35:
                    return EdaLayerId.TopGlue;
                case 36:
                    return EdaLayerId.BottomGlue;

                case 39:
                    return EdaLayerId.TopKeepout;
                case 40:
                    return EdaLayerId.BottomKeepout;

                case 41:
                    return EdaLayerId.TopRestrict;
                case 42:
                    return EdaLayerId.BottomRestrict;

                case 43:
                    return EdaLayerId.ViaRestrict;

                case drillsLayerNum:
                    return EdaLayerId.Drills;

                case holesLayerNum:
                    return EdaLayerId.Holes;

                case 51:
                    return EdaLayerId.TopDocument;
                case 52:
                    return EdaLayerId.BottomDocument;

                default:
                    return EdaLayerId.Get(String.Format("Unknown{0}", layerNum));
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
