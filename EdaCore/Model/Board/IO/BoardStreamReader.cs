namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

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

    /// <summary>
    /// Clase per la lectura de plaques des d'un stream
    /// </summary>
    /// 
    public sealed class BoardStreamReader {

        private struct PadInfo {
            public string Name;
            public string SignalName;
        }

        private static readonly XmlSchemaSet schemas;

        private readonly XmlReaderAdapter _rd;
        private Board _board;
        private int _version;

        /// <summary>
        /// Constructor estatic de la clase
        /// </summary>
        /// 
        static BoardStreamReader() {

            schemas = new XmlSchemaSet();

            string schemaResourceName = "MikroPic.EdaTools.v1.Core.Model.Board.IO.Schemas.XBRD.xsd";
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", schemaResourceName));
            XmlSchema schema = XmlSchema.Read(resourceStream, null);
            schemas.Add(schema);

            schemas.Compile();
        }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="stream">Stream de lectura.</param>
        /// 
        public BoardStreamReader(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("El stream no es de lectura.");

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.CloseInput = false;
            settings.Schemas = schemas;
            settings.ValidationType = schemas == null ? ValidationType.None : ValidationType.Schema;
            settings.ConformanceLevel = ConformanceLevel.Document;

            XmlReader reader = XmlReader.Create(stream, settings);
            _rd = new XmlReaderAdapter(reader);
        }

        /// <summary>
        /// Llegeix una placa.
        /// </summary>
        /// <returns>La placa.</returns>
        /// 
        public Board Read() {

            _board = new Board();

            _rd.NextTag();
            ParseDocumentNode();

            return _board;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// 
        private void ParseDocumentNode() {

            if (!_rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            _rd.NextTag();
            ParseBoardNode();

            _rd.NextTag();
            if (!_rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");
        }

        /// <summary>
        /// Procesa el node 'board'
        /// </summary>
        /// 
        private void ParseBoardNode() {

            if (!_rd.IsStartTag("board"))
                throw new InvalidDataException("Se esperaba <board>");

            _version = _rd.AttributeAsInteger("version");

            _rd.NextTag();
            if (_rd.TagName == "layers") {
                _board.AddLayers(ParseLayersNode());
                _rd.NextTag();
            }
            if (_rd.TagName == "signals") {
                _board.AddSignals(ParseSignalsNode());
                _rd.NextTag();
            }
            if (_rd.TagName == "components") {
                _board.AddComponents(ParseComponentsNode());
                _rd.NextTag();
            }
            if (_rd.TagName == "parts") {
                _board.AddParts(ParsePartsNode());
                _rd.NextTag();
            }
            if (_rd.TagName == "elements") {
                _board.AddElements(ParseBoardElementsNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("board"))
                throw new InvalidDataException("Se esperaba </board>");
        }

        /// <summary>
        /// Procesa el node 'layers'.
        /// </summary>
        /// <returns>La llista d'objectes 'Layer' obtinguda.</returns>
        /// 
        private IEnumerable<Layer> ParseLayersNode() {

            if (!_rd.IsStartTag("layers"))
                throw new InvalidDataException("Se esperaba <layers>");

            List<Layer> layers = new List<Layer>();

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
        private Layer ParseLayerNode() {

            if (!_rd.IsStartTag("layer"))
                throw new InvalidDataException("Se esperaba <layer>");

            BoardSide side = _rd.AttributeAsEnum<BoardSide>("side", BoardSide.None);
            string tag = _rd.AttributeAsString("tag");
            LayerFunction function = _rd.AttributeAsEnum<LayerFunction>("function", LayerFunction.Unknown);

            _rd.NextTag();
            if (!_rd.IsEndTag("layer"))
                throw new InvalidDataException("Se esperaba </layer>");

            Layer layer = new Layer(side, tag, function);
            return layer;
        }

        /// <summary>
        /// Procesa el node 'signals'.
        /// </summary>
        /// 
        private IEnumerable<Signal> ParseSignalsNode() {

            if (!_rd.IsStartTag("signals"))
                throw new InvalidDataException("Se esperaba <signals>");

            List<Signal> signals = new List<Signal>();

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
        private Signal ParseSignalNode() {

            if (!_rd.IsStartTag("signal"))
                throw new InvalidDataException("Se esperaba <signal>");

            string name = _rd.AttributeAsString("name");

            _rd.NextTag();
            if (!_rd.IsEndTag("signal"))
                throw new InvalidDataException("Se esperaba </signal>");

            Signal signal = new Signal(name);
            return signal;
        }

        /// <summary>
        /// Procesa el node 'components'
        /// </summary>
        /// <returns>La llista d'objectes 'Component' obtinguda.</returns>
        /// 
        private IEnumerable<Component> ParseComponentsNode() {

            if (!_rd.IsStartTag("components"))
                throw new InvalidDataException("Se esperaba <components>");

            List<Component> blocks = new List<Component>();

            _rd.NextTag();
            while (_rd.IsStartTag("component")) {
                blocks.Add(ParseComponentNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("components"))
                throw new InvalidDataException("Se esperaba </components>");

            return blocks;
        }

        /// <summary>
        /// Procesa el node 'component'.
        /// </summary>
        /// <returns>L'objecte 'Component' obtingut.</returns>
        /// 
        private Component ParseComponentNode() {

            if (!_rd.IsStartTag("component"))
                throw new InvalidDataException("Se esperaba <component>");

            string name = _rd.AttributeAsString("name");

            Component block = new Component(name);

            _rd.NextTag();
            block.AddElements(ParseComponentElementsNode());
            _rd.NextTag();

            if (!_rd.IsEndTag("component"))
                throw new InvalidDataException("Se esperaba </component>");

            return block;
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <returns>La llista d'objectres 'Element' obtinguda.</returns>
        /// 
        private IEnumerable<Element> ParseComponentElementsNode() {

            if (!_rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            List<Element> elements = new List<Element>();

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

                    case "hole":
                        elements.Add(ParseHoleNode());
                        break;

                    case "text":
                        elements.Add(ParseTextNode());
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
        /// Procesa el node 'elements'
        /// </summary>
        /// <returns>La llista d'objectes 'Element' obtinguda.</returns>
        /// 
        private IEnumerable<Element> ParseBoardElementsNode() {

            if (!_rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            List<Element> elements = new List<Element>();

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

                    case "hole":
                        elements.Add(ParseHoleNode());
                        break;

                    case "text":
                        elements.Add(ParseTextNode());
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
        private IEnumerable<Part> ParsePartsNode() {

            if (!_rd.IsStartTag("parts"))
                throw new InvalidDataException("Se esperaba <parts>");

            List<Part> parts = new List<Part>();

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
        private Part ParsePartNode() {

            if (!_rd.IsStartTag("part"))
                throw new InvalidDataException("Se esperaba <part>");

            string name = _rd.AttributeAsString("name");
            Point position = XmlTypeParser.ParsePoint(_rd.AttributeAsString("position"));
            Angle rotation = XmlTypeParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            bool flip = _rd.AttributeAsBoolean("flip", false);
            string blockName = _rd.AttributeAsString("component");

            Component block = _board.GetComponent(blockName);
            Part part = new Part(block, name, position, rotation, flip);

            _rd.NextTag();
            while (_rd.IsStart) {
                switch (_rd.TagName) {
                    case "attributes":
                        part.AddAttributes(ParsePartAttributesNode());
                        break;

                    case "pads":
                        foreach (var padInfo in ParsePartPadsNode()) {
                            PadElement pad = part.GetPad(padInfo.Name);
                            Signal signal = _board.GetSignal(padInfo.SignalName);
                            _board.Connect(signal, pad, part);
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
        private IEnumerable<PartAttribute> ParsePartAttributesNode() {

            if (!_rd.IsStartTag("attributes"))
                throw new InvalidDataException("Se esperaba <attributes>");

            List<PartAttribute> attributes = new List<PartAttribute>();

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
        private PartAttribute ParsePartAttributeNode() {

            if (!_rd.IsStartTag("attribute"))
                throw new InvalidDataException("Se esperaba <attribute>");

            string name = _rd.AttributeAsString("name");
            string value = _rd.AttributeAsString("value");
            bool visible = _rd.AttributeAsBoolean("visible", true);

            var attribute = new PartAttribute(name, value, visible);

            if (_rd.AttributeExists("position"))
                attribute.Position = XmlTypeParser.ParsePoint(_rd.AttributeAsString("position"));

            if (_rd.AttributeExists("rotation"))
                attribute.Rotation = XmlTypeParser.ParseAngle(_rd.AttributeAsString("rotation"));

            if (_rd.AttributeExists("height"))
                attribute.Height = XmlTypeParser.ParseNumber(_rd.AttributeAsString("height"));

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

            List<PadInfo> pads = new List<PadInfo>();

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
        private LineElement ParseLineNode() {

            if (!_rd.IsStartTag("line"))
                throw new InvalidDataException("Se esperaba <line>");

            LayerSet layerSet = LayerSet.Parse(_rd.AttributeAsString("layers"));
            Point startPosition = XmlTypeParser.ParsePoint(_rd.AttributeAsString("startPosition"));
            Point endPosition = XmlTypeParser.ParsePoint(_rd.AttributeAsString("endPosition"));
            int thickness = XmlTypeParser.ParseNumber(_rd.AttributeAsString("thickness", "0"));
            LineElement.CapStyle lineCap = _rd.AttributeAsEnum<LineElement.CapStyle>("lineCap", LineElement.CapStyle.Round);
            string signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("line"))
                throw new InvalidDataException("Se esperaba </line>");

            LineElement line = new LineElement(layerSet, startPosition, endPosition, thickness, lineCap);
            if (signalName != null) {
                Signal signal = _board.GetSignal(signalName);
                _board.Connect(signal, line);
            }

            return line;
        }

        /// <summary>
        /// Procesa un node 'arc'.
        /// </summary>
        /// <returns>L'objecte 'ArcElement' obtingut.</returns>
        /// 
        private ArcElement ParseArcNode() {

            if (!_rd.IsStartTag("arc"))
                throw new InvalidDataException("Se esperaba <arc>");

            LayerSet layerSet = LayerSet.Parse(_rd.AttributeAsString("layers"));
            Point startPosition = XmlTypeParser.ParsePoint(_rd.AttributeAsString("startPosition"));
            Point endPosition = XmlTypeParser.ParsePoint(_rd.AttributeAsString("endPosition"));
            int thickness = XmlTypeParser.ParseNumber(_rd.AttributeAsString("thickness"));
            Angle angle = XmlTypeParser.ParseAngle(_rd.AttributeAsString("angle"));
            LineElement.CapStyle lineCap = _rd.AttributeAsEnum<LineElement.CapStyle>("lineCap", LineElement.CapStyle.Round);
            string signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("arc"))
                throw new InvalidDataException("Se esperaba </arc>");

            ArcElement arc = new ArcElement(layerSet, startPosition, endPosition, thickness, angle, lineCap);
            if (signalName != null) {
                Signal signal = _board.GetSignal(signalName);
                _board.Connect(signal, arc);
            }

            return arc;
        }

        /// <summary>
        /// Procesa un node 'rectangle'
        /// </summary>
        /// <returns>L'objecte 'RectangleElement' obtingut.</returns>
        /// 
        private RectangleElement ParseRectangleNode() {

            if (!_rd.IsStartTag("rectangle"))
                throw new InvalidDataException("Se esperaba <rectangle>");

            LayerSet layerSet = LayerSet.Parse(_rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(_rd.AttributeAsString("position"));
            Size size = XmlTypeParser.ParseSize(_rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            int thickness = XmlTypeParser.ParseNumber(_rd.AttributeAsString("thickness", "0"));
            Ratio roundness = XmlTypeParser.ParseRatio(_rd.AttributeAsString("roundness", "0"));
            bool filled = _rd.AttributeAsBoolean("filled", thickness == 0);

            _rd.NextTag();
            if (!_rd.IsEndTag("rectangle"))
                throw new InvalidDataException("Se esperaba </rectangle>");

            RectangleElement rectangle = new RectangleElement(layerSet, position, size, roundness, rotation, thickness, filled);

            return rectangle;
        }

        /// <summary>
        /// Procesa un node 'circle'
        /// </summary>
        /// <returns>L'objecte 'CircleElement' obtingut.</returns>
        /// 
        private CircleElement ParseCircleNode() {

            if (!_rd.IsStartTag("circle"))
                throw new InvalidDataException("Se esperaba <circle>");

            LayerSet layerSet = LayerSet.Parse(_rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(_rd.AttributeAsString("position"));
            int radius = XmlTypeParser.ParseNumber(_rd.AttributeAsString("radius"));
            int thickness = XmlTypeParser.ParseNumber(_rd.AttributeAsString("thickness", "0"));
            bool filled = _rd.AttributeAsBoolean("filled", thickness == 0);

            _rd.NextTag();
            if (!_rd.IsEndTag("circle"))
                throw new InvalidDataException("Se esperaba </circle>");

            CircleElement circle = new CircleElement(layerSet, position, radius, thickness, filled);

            return circle;
        }

        /// <summary>
        /// Procesa un node 'region'
        /// </summary>
        /// <returns>L'objecte 'RegionElement' obtingut.</returns>
        /// 
        private RegionElement ParseRegionNode() {

            if (!_rd.IsStartTag("region"))
                throw new InvalidDataException("Se esperaba <region>");

            LayerSet layerSet = LayerSet.Parse(_rd.AttributeAsString("layers"));
            int thickness = _rd.AttributeExists("thickness") ?
                XmlTypeParser.ParseNumber(_rd.AttributeAsString("thickness")) :
                0;
            bool filled = _rd.AttributeAsBoolean("filled", thickness == 0);
            int clearance = XmlTypeParser.ParseNumber(_rd.AttributeAsString("clearance", "0"));
            string signalName = _rd.AttributeAsString("signal");

            RegionElement region = new RegionElement(layerSet, thickness, filled, clearance);
            if (signalName != null) {
                Signal signal = _board.GetSignal(signalName);
                _board.Connect(signal, region);
            }

            _rd.NextTag();
            while (_rd.IsStartTag("segment")) {
                region.Add(ParseRegionSegmentNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("region"))
                throw new InvalidDataException("Se esperaba </region>");

            return region;
        }

        /// <summary>
        /// Procesa un node 'segment'
        /// </summary>
        /// <returns>L'objecte 'RegionElement.Segment' obtingut.</returns>
        /// 
        private RegionElement.Segment ParseRegionSegmentNode() {

            if (!_rd.IsStartTag("segment"))
                throw new InvalidDataException("Se esperaba <segment>");

            Point position = XmlTypeParser.ParsePoint(_rd.AttributeAsString("position"));
            Angle angle = XmlTypeParser.ParseAngle(_rd.AttributeAsString("angle", "0"));

            _rd.NextTag();
            if (!_rd.IsEndTag("segment"))
                throw new InvalidDataException("Se esperaba </segment>");

            RegionElement.Segment segment = new RegionElement.Segment(position, angle);

            return segment;
        }

        /// <summary>
        /// Procesa un node tpad
        /// </summary>
        /// <returns>L'objecte 'TPadElement' obtingut</returns>
        /// 
        private ThPadElement ParseTPadNode() {

            if (!_rd.IsStartTag("tpad"))
                throw new InvalidDataException("Se esperaba <tpad>");

            string name = _rd.AttributeAsString("name");
            LayerSet layerSet = LayerSet.Parse(_rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(_rd.AttributeAsString("position"));
            int size = XmlTypeParser.ParseNumber(_rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            int drill = XmlTypeParser.ParseNumber(_rd.AttributeAsString("drill"));
            ThPadElement.ThPadShape shape = _rd.AttributeAsEnum<ThPadElement.ThPadShape>("shape", ThPadElement.ThPadShape.Circle);
            string signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("tpad"))
                throw new InvalidDataException("Se esperaba </tpad>");

            ThPadElement pad = new ThPadElement(name, layerSet, position, rotation, size, shape, drill);
            if (signalName != null) {
                Signal signal = _board.GetSignal(signalName);
                _board.Connect(signal, pad);
            }

            return pad;
        }

        /// <summary>
        /// Procesa un node tpad
        /// </summary>
        /// <returns>L'objecte 'SPadElement' obtingut.</returns>
        /// 
        private SmdPadElement ParseSPadNode() {

            if (!_rd.IsStartTag("spad"))
                throw new InvalidDataException("Se esperaba <spad>");

            string name = _rd.AttributeAsString("name");
            LayerSet layerSet = LayerSet.Parse(_rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(_rd.AttributeAsString("position"));
            Size size = XmlTypeParser.ParseSize(_rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            Ratio roundness = XmlTypeParser.ParseRatio(_rd.AttributeAsString("roundness", "0"));
            string signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("spad"))
                throw new InvalidDataException("Se esperaba </spad>");

            SmdPadElement pad = new SmdPadElement(name, layerSet, position, size, rotation, roundness);
            if (signalName != null) {
                Signal signal = _board.GetSignal(signalName);
                _board.Connect(signal, pad);
            }

            return pad;
        }

        /// <summary>
        /// Procesa un node 'via'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private ViaElement ParseViaNode() {

            if (!_rd.IsStartTag("via"))
                throw new InvalidDataException("Se esperaba <via>");

            LayerSet layerSet = LayerSet.Parse(_rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(_rd.AttributeAsString("position"));
            int outerSize = XmlTypeParser.ParseNumber(_rd.AttributeAsString("outerSize"));
            int innerSize = _rd.AttributeExists("innerSize") ?
                XmlTypeParser.ParseNumber(_rd.AttributeAsString("innerSize")) :
                outerSize;
            int drill = XmlTypeParser.ParseNumber(_rd.AttributeAsString("drill"));
            ViaElement.ViaShape shape = _rd.AttributeAsEnum<ViaElement.ViaShape>("shape", ViaElement.ViaShape.Circle);
            string signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("via"))
                throw new InvalidDataException("Se esperaba </via>");

            ViaElement via = new ViaElement(layerSet, position, outerSize, innerSize, drill, shape);
            if (signalName != null) {
                Signal signal = _board.GetSignal(signalName);
                _board.Connect(signal, via);
            }

            return via;
        }

        /// <summary>
        /// Procesa un node 'hole'
        /// </summary>
        /// <returns>L'objecte 'HoleElement' obtigut.</returns>
        /// 
        private HoleElement ParseHoleNode() {

            if (!_rd.IsStartTag("hole"))
                throw new InvalidDataException("Se esperaba <hole>");

            Point position = XmlTypeParser.ParsePoint(_rd.AttributeAsString("position"));
            int drill = XmlTypeParser.ParseNumber(_rd.AttributeAsString("drill"));

            _rd.NextTag();
            if (!_rd.IsEndTag("hole"))
                throw new InvalidDataException("Se esperaba </hole>");

            var hole = new HoleElement(position, drill);

            return hole;
        }

        /// <summary>
        /// Procesa un node 'text'
        /// </summary>
        /// <returns>L'objecte 'TextElement' obtingut.</returns>
        /// 
        private TextElement ParseTextNode() {

            if (!_rd.IsStartTag("text"))
                throw new InvalidDataException("Se esperaba <text>");

            LayerSet layerSet = LayerSet.Parse(_rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(_rd.AttributeAsString("position"));
            Angle rotation = XmlTypeParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            int height = XmlTypeParser.ParseNumber(_rd.AttributeAsString("height"));
            HorizontalTextAlign horizontalAlign = _rd.AttributeAsEnum("horizontalAlign", HorizontalTextAlign.Left);
            VerticalTextAlign verticalAlign = _rd.AttributeAsEnum("verticalAlign", VerticalTextAlign.Bottom);
            int thickness = XmlTypeParser.ParseNumber(_rd.AttributeAsString("thickness"));
            string value = _rd.AttributeAsString("value");

            _rd.NextTag();
            if (!_rd.IsEndTag("text"))
                throw new InvalidDataException("Se esperaba </text>");

            TextElement text = new TextElement(layerSet, position, rotation, height, thickness, horizontalAlign, verticalAlign, value);

            return text;
        }
    }
}
