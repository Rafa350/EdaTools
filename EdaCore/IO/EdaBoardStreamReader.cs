using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.IO;

namespace MikroPic.EdaTools.v1.Core.IO {

    /// <summary>
    /// Clase per la lectura de plaques des d'un stream
    /// </summary>
    /// 
    public sealed class EdaBoardStreamReader {

        private struct PadInfo {
            public string Name;
            public string SignalName;
        }

        private static readonly XmlSchemaSet _schemas;

        private readonly XmlReaderAdapter _rd;
        private Dictionary<Tuple<IEdaConectable, EdaPart>, string> _elementSignal = new Dictionary<Tuple<IEdaConectable, EdaPart>, string>();
        private Dictionary<string, EdaComponent> _components = new Dictionary<string, EdaComponent>();
        private int _version;

        /// <summary>
        /// Constructor estatic de la clase
        /// </summary>
        /// 
        static EdaBoardStreamReader() {

            _schemas = new XmlSchemaSet();

            string schemaResourceName = "MikroPic.EdaTools.v1.Core.IO.Schemas.EdaBoardDocument.xsd";
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", schemaResourceName));
            XmlSchema schema = XmlSchema.Read(resourceStream, null);
            _schemas.Add(schema);

            _schemas.Compile();
        }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="stream">Stream de lectura.</param>
        /// 
        public EdaBoardStreamReader(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("El stream no es de lectura.");

            var settings = new XmlReaderSettings();
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.CloseInput = false;
            settings.Schemas = _schemas;
            settings.ValidationType = _schemas == null ? ValidationType.None : ValidationType.Schema;
            settings.ConformanceLevel = ConformanceLevel.Document;

            XmlReader reader = XmlReader.Create(stream, settings);
            _rd = new XmlReaderAdapter(reader);
        }

        /// <summary>
        /// Llegeix una placa.
        /// </summary>
        /// <returns>La placa.</returns>
        /// 
        public EdaBoard ReadBoard() {

            _rd.NextTag();
            return ParseBoardDocumentNode();
        }

        /// <summary>
        /// Llegeix una llibraria
        /// </summary>
        /// <returns>La llibraria.</returns>
        /// 
        public EdaLibrary ReadLibrary() {

            _rd.NextTag();
            return ParseLibraryDocumentNode();
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// <returns>La placa.</returns>
        /// 
        private EdaBoard ParseBoardDocumentNode() {

            if (!_rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            string documentType = _rd.AttributeAsString("documentType");
            if (documentType != "board")
                throw new InvalidDataException("No es un documentdo de tipo 'board'");

            _version = _rd.AttributeAsInteger("version");

            _rd.NextTag();
            EdaBoard board = ParseBoardNode();

            _rd.NextTag();
            if (!_rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");

            return board;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// <returns>La llibreria.</returns>
        /// 
        private EdaLibrary ParseLibraryDocumentNode() {

            if (!_rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            string documentType = _rd.AttributeAsString("documentType");
            if (documentType != "componentLibrary")
                throw new InvalidDataException("No es un documentdo de tipo 'library'");

            _version = _rd.AttributeAsInteger("version");

            _rd.NextTag();
            EdaLibrary library = ParseLibraryNode();

            _rd.NextTag();
            if (!_rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");

            return library;
        }

        /// <summary>
        /// Procesa el node 'board'
        /// </summary>
        /// <returns>La placa.</returns>
        /// 
        private EdaBoard ParseBoardNode() {

            if (!_rd.IsStartTag("board"))
                throw new InvalidDataException("Se esperaba <board>");

            EdaPoint position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            EdaAngle rotation = EdaParser.ParseAngle(_rd.AttributeAsString("rotation"));

            _rd.NextTag();

            IEnumerable<EdaLayer> layers = null;
            if (_rd.TagName == "layers") {
                layers = ParseLayersNode();
                _rd.NextTag();
            }

            IEnumerable<EdaSignal> signals = null;
            if (_rd.TagName == "signals") {
                signals = ParseSignalsNode();
                _rd.NextTag();
            }

            IEnumerable<EdaComponent> components = null;
            if (_rd.TagName == "components") {
                components = ParseComponentsNode();
                _rd.NextTag();
            }

            IEnumerable<EdaPart> parts = null;
            if (_rd.TagName == "parts") {
                parts = ParsePartsNode();
                _rd.NextTag();
            }

            IEnumerable<EdaElement> elements = null;
            if (_rd.TagName == "elements") {
                elements = ParseBoardElementsNode();
                _rd.NextTag();
            }

            // Crea la placa.
            //
            var board = new EdaBoard();
            board.Position = position;
            board.Rotation = rotation;
            if (layers != null)
                board.AddLayers(layers);
            if (signals != null)
                board.AddSignals(signals);
            if (components != null)
                board.AddComponents(components);
            if (parts != null)
                board.AddParts(parts);
            if (elements != null)
                board.AddElements(elements);
            foreach (var elementSignal in _elementSignal) {
                IEdaConectable element = elementSignal.Key.Item1;
                EdaPart part = elementSignal.Key.Item2;
                EdaSignal signal = board.GetSignal(elementSignal.Value);
                board.Connect(signal, element, part);
            }

            if (!_rd.IsEndTag("board"))
                throw new InvalidDataException("Se esperaba </board>");

            return board;
        }

        /// <summary>
        /// Procesa el node 'board'
        /// </summary>
        /// <returns>La llibraria.</returns>
        /// 
        private EdaLibrary ParseLibraryNode() {

            if (!_rd.IsStartTag("library"))
                throw new InvalidDataException("Se esperaba <library>");

            string name = _rd.AttributeAsString("name");

            EdaLibrary library = new EdaLibrary(name);

            _rd.NextTag();
            if (_rd.TagName == "components") {
                library.AddComponents(ParseComponentsNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("library"))
                throw new InvalidDataException("Se esperaba </library>");

            return library;
        }


        /// <summary>
        /// Procesa el node 'layers'.
        /// </summary>
        /// <returns>La llista d'objectes 'Layer' obtinguda.</returns>
        /// 
        private IEnumerable<EdaLayer> ParseLayersNode() {

            if (!_rd.IsStartTag("layers"))
                throw new InvalidDataException("Se esperaba <layers>");

            var layers = new List<EdaLayer>();

            _rd.NextTag();
            while (_rd.IsStartTag("layer")) {
                layers.Add(ParseLayerNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("layers"))
                throw new InvalidDataException("Se esperaba </layers>");

            return layers;
        }

        /// <summary>
        /// Procesa el node 'layer'.
        /// </summary>
        /// <returns>L'objecte 'Layer' obtingut.</returns>
        /// 
        private EdaLayer ParseLayerNode() {

            if (!_rd.IsStartTag("layer"))
                throw new InvalidDataException("Se esperaba <layer>");

            string id = _rd.AttributeAsString("id");
            BoardSide side = _rd.AttributeAsEnum<BoardSide>("side", BoardSide.None);
            LayerFunction function = _rd.AttributeAsEnum<LayerFunction>("function", LayerFunction.Unknown);

            _rd.NextTag();
            if (!_rd.IsEndTag("layer"))
                throw new InvalidDataException("Se esperaba </layer>");

            return new EdaLayer(EdaLayerId.Parse(id), side, function);
        }

        /// <summary>
        /// Procesa el node 'signals'.
        /// </summary>
        /// <returns>La llista de senyals.</returns>
        /// 
        private IEnumerable<EdaSignal> ParseSignalsNode() {

            if (!_rd.IsStartTag("signals"))
                throw new InvalidDataException("Se esperaba <signals>");

            var signals = new List<EdaSignal>();

            _rd.NextTag();
            while (_rd.IsStartTag("signal")) {
                signals.Add(ParseSignalNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("signals"))
                throw new InvalidDataException("Se esperaba </signals>");

            return signals;
        }

        /// <summary>
        /// Procesa el node 'signal'
        /// </summary>
        /// <returns>L'objecte 'Signal' obtingut.</returns>
        /// 
        private EdaSignal ParseSignalNode() {

            if (!_rd.IsStartTag("signal"))
                throw new InvalidDataException("Se esperaba <signal>");

            var name = _rd.AttributeAsString("name");
            var clearance = EdaParser.ParseScalar(_rd.AttributeAsString("clearance"));

            _rd.NextTag();
            if (!_rd.IsEndTag("signal"))
                throw new InvalidDataException("Se esperaba </signal>");

            return new EdaSignal {
                Name = name,
                Clearance = clearance
            };
        }

        /// <summary>
        /// Procesa el node 'components'
        /// </summary>
        /// <returns>La llista d'objectes 'Component' obtinguda.</returns>
        /// 
        private IEnumerable<EdaComponent> ParseComponentsNode() {

            if (!_rd.IsStartTag("components"))
                throw new InvalidDataException("Se esperaba <components>");

            var components = new List<EdaComponent>();

            _rd.NextTag();
            while (_rd.IsStartTag("component")) {
                components.Add(ParseComponentNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("components"))
                throw new InvalidDataException("Se esperaba </components>");

            return components;
        }

        /// <summary>
        /// Procesa el node 'component'.
        /// </summary>
        /// <returns>L'objecte 'Component' obtingut.</returns>
        /// 
        private EdaComponent ParseComponentNode() {

            if (!_rd.IsStartTag("component"))
                throw new InvalidDataException("Se esperaba <component>");

            string name = _rd.AttributeAsString("name");

            var component = new EdaComponent();
            component.Name = name;

            RegisterComponent(component);

            _rd.NextTag();
            component.AddElements(ParseComponentElementsNode());
            _rd.NextTag();

            if (!_rd.IsEndTag("component"))
                throw new InvalidDataException("Se esperaba </component>");

            return component;
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <returns>La llista d'objectres 'Element' obtinguda.</returns>
        /// 
        private IEnumerable<EdaElement> ParseComponentElementsNode() {

            if (!_rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            var elements = new List<EdaElement>();

            _rd.NextTag();
            while (_rd.IsStart) {
                switch (_rd.TagName) {
                    case "line":
                        elements.Add(ParseLineNode());
                        break;

                    case "arc":
                        elements.Add(ParseArcNode());
                        break;

                    case "rectangle":
                        elements.Add(ParseRectangleNode());
                        break;

                    case "circle":
                        elements.Add(ParseCircleNode());
                        break;

                    case "region":
                        elements.Add(ParseRegionNode());
                        break;

                    case "tpad":
                        elements.Add(ParseTPadNode());
                        break;

                    case "spad":
                        elements.Add(ParseSPadNode());
                        break;

                    case "text":
                        elements.Add(ParseTextNode());
                        break;

                    case "circleHole":
                        elements.Add(ParseCircleHoleNode());
                        break;

                    case "lineHole":
                        elements.Add(ParseLineHoleNode());
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <line>, <arc>, <rectangle>, <circle>, <tpad>, <spad>, <via>, <text>, <region> <circleHole> o <lineHole>");
                }
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("elements"))
                throw new InvalidDataException("Se esperaba </elements>");

            return elements;
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <returns>La llista d'objectes 'Element' obtinguda.</returns>
        /// 
        private IEnumerable<EdaElement> ParseBoardElementsNode() {

            if (!_rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            var elements = new List<EdaElement>();

            _rd.NextTag();
            while (_rd.IsStart) {
                switch (_rd.TagName) {
                    case "line":
                        elements.Add(ParseLineNode());
                        break;

                    case "arc":
                        elements.Add(ParseArcNode());
                        break;

                    case "rectangle":
                        elements.Add(ParseRectangleNode());
                        break;

                    case "circle":
                        elements.Add(ParseCircleNode());
                        break;

                    case "region":
                        elements.Add(ParseRegionNode());
                        break;

                    case "tpad":
                        elements.Add(ParseTPadNode());
                        break;

                    case "spad":
                        elements.Add(ParseSPadNode());
                        break;

                    case "via":
                        elements.Add(ParseViaNode());
                        break;

                    case "text":
                        elements.Add(ParseTextNode());
                        break;

                    case "circleHole":
                        elements.Add(ParseCircleHoleNode());
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <line>, <arc>, <rectangle>, <circle>, <tpad>, <spad>, <via>, <text>, <region> o <hole>");
                }
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("elements"))
                throw new InvalidDataException("Se esperaba </elements>");

            return elements;
        }

        /// <summary>
        /// Procesa un node 'parts'.
        /// </summary>
        /// <returns>La llista d'objectes 'Part' obtinguts.</returns>
        /// 
        private IEnumerable<EdaPart> ParsePartsNode() {

            if (!_rd.IsStartTag("parts"))
                throw new InvalidDataException("Se esperaba <parts>");

            List<EdaPart> parts = new List<EdaPart>();

            _rd.NextTag();
            while (_rd.IsStartTag("part")) {
                parts.Add(ParsePartNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("parts"))
                throw new InvalidDataException("Se esperaba </parts>");

            return parts;
        }

        /// <summary>
        /// Procesa un node 'part'.
        /// </summary>
        /// <returns>L'objecte 'Part' obtingut.</returns>
        /// 
        private EdaPart ParsePartNode() {

            if (!_rd.IsStartTag("part"))
                throw new InvalidDataException("Se esperaba <part>");

            string name = _rd.AttributeAsString("name");
            EdaPoint position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            EdaAngle rotation = EdaParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            PartSide side = _rd.AttributeAsEnum("side", PartSide.Top);
            string componentName = _rd.AttributeAsString("component");
            var component = GetComponent(componentName);

            EdaPart part = new EdaPart {
                Component = component,
                Name = name,
                Position = position,
                Rotation = rotation,
                Side = side
            };

            _rd.NextTag();
            while (_rd.IsStart) {
                switch (_rd.TagName) {
                    case "attributes":
                        part.AddAttributes(ParsePartAttributesNode());
                        break;

                    case "pads":
                        foreach (var padInfo in ParsePartPadsNode()) {
                            EdaPadBaseElement pad = part.GetPad(padInfo.Name);
                            _elementSignal.Add(new Tuple<IEdaConectable, EdaPart>(pad, part), padInfo.SignalName);
                        }
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <pads> o <attributes>");
                }
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("part"))
                throw new InvalidDataException("Se esperaba </part>");

            return part;
        }

        /// <summary>
        /// Procesa el node 'attributes'
        /// </summary>
        /// 
        private IEnumerable<EdaPartAttribute> ParsePartAttributesNode() {

            if (!_rd.IsStartTag("attributes"))
                throw new InvalidDataException("Se esperaba <attributes>");

            var attributes = new List<EdaPartAttribute>();

            _rd.NextTag();
            while (_rd.IsStartTag("attribute")) {
                attributes.Add(ParsePartAttributeNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("attributes"))
                throw new InvalidDataException("Se esperaba </attributes>");

            return attributes;
        }

        /// <summary>
        /// Procesa un node 'attribute'
        /// </summary>
        /// <returns>L'objecte 'partAttribute' obtingut.</returns>
        /// 
        private EdaPartAttribute ParsePartAttributeNode() {

            if (!_rd.IsStartTag("attribute"))
                throw new InvalidDataException("Se esperaba <attribute>");

            string name = _rd.AttributeAsString("name");
            string value = _rd.AttributeAsString("value");
            bool visible = _rd.AttributeAsBoolean("visible", true);

            var attribute = new EdaPartAttribute(name, value, visible);

            if (_rd.AttributeExists("position"))
                attribute.Position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));

            if (_rd.AttributeExists("rotation"))
                attribute.Rotation = EdaParser.ParseAngle(_rd.AttributeAsString("rotation"));

            if (_rd.AttributeExists("height"))
                attribute.Height = EdaParser.ParseScalar(_rd.AttributeAsString("height"));

            if (_rd.AttributeExists("horizontalAlign"))
                attribute.HorizontalAlign = _rd.AttributeAsEnum("horizontalAlign", HorizontalTextAlign.Left);

            if (_rd.AttributeExists("verticalAlign"))
                attribute.VerticalAlign = _rd.AttributeAsEnum("verticalAlign", VerticalTextAlign.Bottom);

            _rd.NextTag();
            if (!_rd.IsEndTag("attribute"))
                throw new InvalidDataException("Se esperaba </attribute>");

            return attribute;
        }

        /// <summary>
        /// Procesa el node 'pads'
        /// </summary>
        /// <returns>La llista d'objectes 'PadInfo' obtinguda.</returns>
        /// 
        private IEnumerable<PadInfo> ParsePartPadsNode() {

            if (!_rd.IsStartTag("pads"))
                throw new InvalidDataException("Se esperaba <pads>");

            var pads = new List<PadInfo>();

            _rd.NextTag();
            while (_rd.IsStartTag("pad")) {
                pads.Add(ParsePartPadNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("pads"))
                throw new InvalidDataException("Se esperaba </pads>");

            return pads;
        }

        /// <summary>
        /// Procesa un node 'pad'.
        /// </summary>
        /// <returns>L'objecte 'PadInfo' obtingut.</returns>
        /// 
        private PadInfo ParsePartPadNode() {

            if (!_rd.IsStartTag("pad"))
                throw new InvalidDataException("Se esperaba <pad>");

            string padName = _rd.AttributeAsString("name");
            string signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("pad"))
                throw new InvalidDataException("Se esperaba </pad>");

            PadInfo padInfo = new PadInfo {
                Name = padName,
                SignalName = signalName
            };

            return padInfo;
        }

        /// <summary>
        /// Procesa el node 'line'
        /// </summary>
        /// 
        private EdaElement ParseLineNode() {

            if (!_rd.IsStartTag("line"))
                throw new InvalidDataException("Se esperaba <line>");

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layer"));
            var startPosition = EdaParser.ParsePoint(_rd.AttributeAsString("startPosition"));
            var endPosition = EdaParser.ParsePoint(_rd.AttributeAsString("endPosition"));
            var thickness = EdaParser.ParseScalar(_rd.AttributeAsString("thickness", "0"));
            var lineCap = _rd.AttributeAsEnum<EdaLineCap>("lineCap", EdaLineCap.Round);
            var signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("line"))
                throw new InvalidDataException("Se esperaba </line>");

            var element = new EdaLineElement {
                LayerSet = layerSet,
                StartPosition = startPosition,
                EndPosition = endPosition,
                Thickness = thickness,
                LineCap = lineCap
            };

            if (signalName != null)
                _elementSignal.Add(new Tuple<IEdaConectable, EdaPart>(element, null), signalName);

            return element;
        }

        /// <summary>
        /// Procesa un node 'arc'.
        /// </summary>
        /// <returns>L'objecte 'ArcElement' obtingut.</returns>
        /// 
        private EdaElement ParseArcNode() {

            if (!_rd.IsStartTag("arc"))
                throw new InvalidDataException("Se esperaba <arc>");

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layer"));
            var startPosition = EdaParser.ParsePoint(_rd.AttributeAsString("startPosition"));
            var endPosition = EdaParser.ParsePoint(_rd.AttributeAsString("endPosition"));
            var thickness = EdaParser.ParseScalar(_rd.AttributeAsString("thickness"));
            var angle = EdaParser.ParseAngle(_rd.AttributeAsString("angle"));
            var lineCap = _rd.AttributeAsEnum<EdaLineCap>("lineCap", EdaLineCap.Round);
            var signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("arc"))
                throw new InvalidDataException("Se esperaba </arc>");

            var element = new EdaArcElement {
                LayerSet = layerSet,
                StartPosition = startPosition,
                EndPosition = endPosition,
                Thickness = thickness,
                Angle = angle,
                LineCap = lineCap
            };

            if (signalName != null)
                _elementSignal.Add(new Tuple<IEdaConectable, EdaPart>(element, null), signalName);

            return element;
        }

        /// <summary>
        /// Procesa un node 'rectangle'
        /// </summary>
        /// <returns>L'objecte 'RectangleElement' obtingut.</returns>
        /// 
        private EdaElement ParseRectangleNode() {

            if (!_rd.IsStartTag("rectangle"))
                throw new InvalidDataException("Se esperaba <rectangle>");

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layer"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var size = EdaParser.ParseSize(_rd.AttributeAsString("size"));
            var rotation = EdaParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            var thickness = EdaParser.ParseScalar(_rd.AttributeAsString("thickness", "0"));
            var roundness = EdaParser.ParseRatio(_rd.AttributeAsString("roundness", "0"));

            _rd.NextTag();
            if (!_rd.IsEndTag("rectangle"))
                throw new InvalidDataException("Se esperaba </rectangle>");

            var element = new EdaRectangleElement {
                LayerSet = layerSet,
                Position = position,
                Size = size,
                CornerRatio = roundness,
                Rotation = rotation,
                Thickness = thickness
            };

            return element;
        }

        /// <summary>
        /// Procesa un node 'circle'
        /// </summary>
        /// <returns>L'objecte 'CircleElement' obtingut.</returns>
        /// 
        private EdaElement ParseCircleNode() {

            if (!_rd.IsStartTag("circle"))
                throw new InvalidDataException("Se esperaba <circle>");

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layer"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var radius = EdaParser.ParseScalar(_rd.AttributeAsString("radius"));
            var thickness = EdaParser.ParseScalar(_rd.AttributeAsString("thickness", "0"));
            var filled = _rd.AttributeAsBoolean("filled");

            _rd.NextTag();
            if (!_rd.IsEndTag("circle"))
                throw new InvalidDataException("Se esperaba </circle>");

            var element = new EdaCircleElement {
                LayerSet = layerSet,
                Position = position,
                Radius = radius,
                Thickness = thickness,
                Filled = filled
            };

            return element;
        }

        /// <summary>
        /// Procesa un node 'region'
        /// </summary>
        /// <returns>L'objecte 'RegionElement' obtingut.</returns>
        /// 
        private EdaElement ParseRegionNode() {

            if (!_rd.IsStartTag("region"))
                throw new InvalidDataException("Se esperaba <region>");

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layer"));
            var thickness = _rd.AttributeExists("thickness") ?
                EdaParser.ParseScalar(_rd.AttributeAsString("thickness")) :
                0;
            var clearance = EdaParser.ParseScalar(_rd.AttributeAsString("clearance", "0"));
            var priority = _rd.AttributeAsInteger("priority");
            var signalName = _rd.AttributeAsString("signal");

            var element = new EdaRegionElement {
                LayerSet = layerSet,
                Thickness = thickness,
                Clearance = clearance,
                ThermalThickness = 400000,
                ThermalClearance = 200000,
                Priority = priority
            };

            if (signalName != null)
                _elementSignal.Add(new Tuple<IEdaConectable, EdaPart>(element, null), signalName);

            _rd.NextTag();
            List<EdaArcPoint> vertices = null;
            while (_rd.IsStartTag("segment")) {
                if (vertices == null)
                    vertices = new List<EdaArcPoint>();
                vertices.Add(ParseRegionSegmentNode());
                _rd.NextTag();
            }
            element.Vertices = vertices;

            if (!_rd.IsEndTag("region"))
                throw new InvalidDataException("Se esperaba </region>");

            return element;
        }

        /// <summary>
        /// Procesa un node 'segment'
        /// </summary>
        /// <returns>L'objecte 'ArcPoint' obtingut.</returns>
        /// 
        private EdaArcPoint ParseRegionSegmentNode() {

            if (!_rd.IsStartTag("segment"))
                throw new InvalidDataException("Se esperaba <segment>");

            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var angle = EdaParser.ParseAngle(_rd.AttributeAsString("angle", "0"));

            _rd.NextTag();
            if (!_rd.IsEndTag("segment"))
                throw new InvalidDataException("Se esperaba </segment>");

            return new EdaArcPoint(position, angle);
        }

        /// <summary>
        /// Procesa un node tpad
        /// </summary>
        /// <returns>L'objecte 'TPadElement' obtingut</returns>
        /// 
        private EdaElement ParseTPadNode() {

            if (!_rd.IsStartTag("tpad"))
                throw new InvalidDataException("Se esperaba <tpad>");

            var name = _rd.AttributeAsString("name");
            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var topSize = EdaParser.ParseSize(_rd.AttributeAsString("topSize"));
            var innerSize = _rd.AttributeExists("innerSize") ? EdaParser.ParseSize(_rd.AttributeAsString("innerSize")) : topSize;
            var bottomSize = _rd.AttributeExists("bottomSize") ? EdaParser.ParseSize(_rd.AttributeAsString("bottomSize")) : topSize;
            var cornerRatio = EdaParser.ParseRatio(_rd.AttributeAsString("cornerRatio", "0"));
            var cornerShape = _rd.AttributeAsEnum("cornerShape", EdaThtPadElement.CornerShapeType.Round);
            var rotation = EdaParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            var drill = EdaParser.ParseScalar(_rd.AttributeAsString("drill"));
            var signalName = _rd.AttributeAsString("signal");
            var clearance = EdaParser.ParseScalar(_rd.AttributeAsString("clearance", "0"));
            var maskClearance = EdaParser.ParseScalar(_rd.AttributeAsString("maskClearance", "0"));

            _rd.NextTag();
            if (!_rd.IsEndTag("tpad"))
                throw new InvalidDataException("Se esperaba </tpad>");

            var element = new EdaThtPadElement {
                Name = name,
                LayerSet = layerSet,
                Position = position,
                CornerRatio = cornerRatio,
                CornerShape = cornerShape,
                Rotation = rotation,
                TopSize = topSize,
                InnerSize = innerSize,
                BottomSize = bottomSize,
                DrillDiameter = drill,
                Clearance = clearance,
                MaskClearance = maskClearance
            };

            if (signalName != null)
                _elementSignal.Add(new Tuple<IEdaConectable, EdaPart>(element, null), signalName);

            return element;
        }

        /// <summary>
        /// Procesa un node tpad
        /// </summary>
        /// <returns>L'objecte 'SPadElement' obtingut.</returns>
        /// 
        private EdaElement ParseSPadNode() {

            if (!_rd.IsStartTag("spad"))
                throw new InvalidDataException("Se esperaba <spad>");

            var name = _rd.AttributeAsString("name");
            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var size = EdaParser.ParseSize(_rd.AttributeAsString("size"));
            var rotation = EdaParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            var cornerRatio = EdaParser.ParseRatio(_rd.AttributeAsString("cornerRatio", "0"));
            var cornerShape = _rd.AttributeAsEnum("cornerShape", EdaSmtPadElement.CornerShapeType.Round);
            var signalName = _rd.AttributeAsString("signal");
            var clearance = EdaParser.ParseScalar(_rd.AttributeAsString("clearance", "0"));
            var maskClearance = EdaParser.ParseScalar(_rd.AttributeAsString("maskClearance", "0"));
            var pasteReductionRatio = EdaParser.ParseRatio(_rd.AttributeAsString("pasteReductionRatio", "0"));

            _rd.NextTag();
            if (!_rd.IsEndTag("spad"))
                throw new InvalidDataException("Se esperaba </spad>");

            var element = new EdaSmtPadElement {
                Name = name,
                LayerSet = layerSet,
                Position = position,
                Size = size,
                Rotation = rotation,
                CornerRatio = cornerRatio,
                CornerShape = cornerShape,
                Clearance = clearance,
                MaskClearance = maskClearance,
                MaskEnabled = true,
                PasteReductionRatio = pasteReductionRatio,
                PasteEnabled = true
            };

            if (signalName != null)
                _elementSignal.Add(new Tuple<IEdaConectable, EdaPart>(element, null), signalName);

            return element;
        }

        /// <summary>
        /// Procesa un node 'via'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private EdaElement ParseViaNode() {

            if (!_rd.IsStartTag("via"))
                throw new InvalidDataException("Se esperaba <via>");

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var outerSize = EdaParser.ParseScalar(_rd.AttributeAsString("outerSize"));
            var innerSize = _rd.AttributeExists("innerSize") ?
                EdaParser.ParseScalar(_rd.AttributeAsString("innerSize")) :
                outerSize;
            var drill = EdaParser.ParseScalar(_rd.AttributeAsString("drill"));
            var signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("via"))
                throw new InvalidDataException("Se esperaba </via>");

            var element = new EdaViaElement {
                LayerSet = layerSet,
                Position = position,
                OuterSize = outerSize,
                InnerSize = innerSize,
                DrillDiameter = drill
            };

            if (signalName != null)
                _elementSignal.Add(new Tuple<IEdaConectable, EdaPart>(element, null), signalName);

            return element;
        }

        /// <summary>
        /// Procesa un node 'text'
        /// </summary>
        /// <returns>L'objecte 'TextElement' obtingut.</returns>
        /// 
        private EdaElement ParseTextNode() {

            if (!_rd.IsStartTag("text"))
                throw new InvalidDataException("Se esperaba <text>");

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layer"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var rotation = EdaParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            var height = EdaParser.ParseScalar(_rd.AttributeAsString("height"));
            var horizontalAlign = _rd.AttributeAsEnum("horizontalAlign", HorizontalTextAlign.Left);
            var verticalAlign = _rd.AttributeAsEnum("verticalAlign", VerticalTextAlign.Bottom);
            var thickness = EdaParser.ParseScalar(_rd.AttributeAsString("thickness"));
            var value = _rd.AttributeAsString("value");

            _rd.NextTag();
            if (!_rd.IsEndTag("text"))
                throw new InvalidDataException("Se esperaba </text>");

            var element = new EdaTextElement {
                LayerSet = layerSet,
                Position = position,
                Rotation = rotation,
                Height = height,
                Thickness = thickness,
                HorizontalAlign = horizontalAlign,
                VerticalAlign = verticalAlign,
                Value = value
            };

            return element;
        }

        /// <summary>
        /// Procesa un node 'circleHole'
        /// </summary>
        /// <returns>L'element.</returns>
        /// <exception cref="InvalidDataException">Dades incorrectes.</exception>
        /// 
        private EdaElement ParseCircleHoleNode() {

            if (!_rd.IsStartTag("circleHole"))
                throw new InvalidDataException("Se esperaba <circleHole>");

            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var diameter = EdaParser.ParseScalar(_rd.AttributeAsString("diameter"));
            var platted = _rd.AttributeAsBoolean("platted");

            _rd.NextTag();
            if (!_rd.IsEndTag("circleHole"))
                throw new InvalidDataException("Se esperaba </circleHole>");

            var element = new EdaCircularHoleElement {
                Position = position,
                Diameter = diameter,
                Platted = platted
            };

            return element;
        }

        /// <summary>
        /// Procesa un n ode 'lineHole'
        /// </summary>
        /// <returns>L'element.</returns>
        /// <exception cref="InvalidDataException">Dades incorrecters.</exception>
        /// 
        private EdaElement ParseLineHoleNode() {

            if (!_rd.IsStartTag("lineHole"))
                throw new InvalidDataException("Se esperaba <lineHole>");

            var startPosition = EdaParser.ParsePoint(_rd.AttributeAsString("startPosition"));
            var endPosition = EdaParser.ParsePoint(_rd.AttributeAsString("endPosition"));
            var diameter = EdaParser.ParseScalar(_rd.AttributeAsString("diameter"));
            var platted = _rd.AttributeAsBoolean("platted");

            _rd.NextTag();
            if (!_rd.IsEndTag("lineHole"))
                throw new InvalidDataException("Se esperaba </lineHole>");

            var element = new EdaLinearHoleElement {
                StartPosition = startPosition,
                EndPosition = endPosition,
                Diameter = diameter,
                Platted = platted
            };

            return element;
        }

        private void RegisterComponent(EdaComponent component) {

            _components.Add(component.Name, component);
        }

        private EdaComponent GetComponent(string name) {

            return _components[name];
        }
    }
}
