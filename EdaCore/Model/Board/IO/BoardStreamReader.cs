using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.IO;

namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

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
        private EdaBoard _board;
        private int _version;

        /// <summary>
        /// Constructor estatic de la clase
        /// </summary>
        /// 
        static BoardStreamReader() {

            schemas = new XmlSchemaSet();

            string schemaResourceName = "MikroPic.EdaTools.v1.Core.Model.Board.IO.Schemas.BoardDocument.xsd";
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

            var settings = new XmlReaderSettings();
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
        public EdaBoard Read() {

            _board = new EdaBoard();

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
        private IEnumerable<EdaLayer> ParseLayersNode() {

            if (!_rd.IsStartTag("layers"))
                throw new InvalidDataException("Se esperaba <layers>");

            List<EdaLayer> layers = new List<EdaLayer>();

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
        /// 
        private IEnumerable<EdaSignal> ParseSignalsNode() {

            if (!_rd.IsStartTag("signals"))
                throw new InvalidDataException("Se esperaba <signals>");

            List<EdaSignal> signals = new List<EdaSignal>();

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

            var blocks = new List<EdaComponent>();

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
        private EdaComponent ParseComponentNode() {

            if (!_rd.IsStartTag("component"))
                throw new InvalidDataException("Se esperaba <component>");

            string name = _rd.AttributeAsString("name");

            var block = new EdaComponent(name);

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
        private IEnumerable<EdaElement> ParseComponentElementsNode() {

            if (!_rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            List<EdaElement> elements = new List<EdaElement>();

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
        private IEnumerable<EdaElement> ParseBoardElementsNode() {

            if (!_rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            List<EdaElement> elements = new List<EdaElement>();

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
            bool flip = _rd.AttributeAsBoolean("flip", false);
            string blockName = _rd.AttributeAsString("component");

            var block = _board.GetComponent(blockName);
            EdaPart part = new EdaPart(block, name, position, rotation, flip);

            _rd.NextTag();
            while (_rd.IsStart) {
                switch (_rd.TagName) {
                    case "attributes":
                        part.AddAttributes(ParsePartAttributesNode());
                        break;

                    case "pads":
                        foreach (var padInfo in ParsePartPadsNode()) {
                            PadElement pad = part.GetPad(padInfo.Name);
                            EdaSignal signal = _board.GetSignal(padInfo.SignalName);
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
        private IEnumerable<EdaPartAttribute> ParsePartAttributesNode() {

            if (!_rd.IsStartTag("attributes"))
                throw new InvalidDataException("Se esperaba <attributes>");

            List<EdaPartAttribute> attributes = new List<EdaPartAttribute>();

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

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var startPosition = EdaParser.ParsePoint(_rd.AttributeAsString("startPosition"));
            var endPosition = EdaParser.ParsePoint(_rd.AttributeAsString("endPosition"));
            var thickness = EdaParser.ParseScalar(_rd.AttributeAsString("thickness", "0"));
            var lineCap = _rd.AttributeAsEnum<LineElement.CapStyle>("lineCap", LineElement.CapStyle.Round);
            var signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("line"))
                throw new InvalidDataException("Se esperaba </line>");

            var line = new LineElement {
                LayerSet = layerSet,
                StartPosition = startPosition,
                EndPosition = endPosition,
                Thickness = thickness,
                LineCap = lineCap
            };

            if (signalName != null) {
                var signal = _board.GetSignal(signalName);
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

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var startPosition = EdaParser.ParsePoint(_rd.AttributeAsString("startPosition"));
            var endPosition = EdaParser.ParsePoint(_rd.AttributeAsString("endPosition"));
            var thickness = EdaParser.ParseScalar(_rd.AttributeAsString("thickness"));
            var angle = EdaParser.ParseAngle(_rd.AttributeAsString("angle"));
            var lineCap = _rd.AttributeAsEnum<LineElement.CapStyle>("lineCap", LineElement.CapStyle.Round);
            var signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("arc"))
                throw new InvalidDataException("Se esperaba </arc>");

            var arc = new ArcElement {
                LayerSet = layerSet,
                StartPosition = startPosition,
                EndPosition = endPosition,
                Thickness = thickness,
                Angle = angle,
                LineCap = lineCap
            };

            if (signalName != null) {
                var signal = _board.GetSignal(signalName);
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

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var size = EdaParser.ParseSize(_rd.AttributeAsString("size"));
            var rotation = EdaParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            var thickness = EdaParser.ParseScalar(_rd.AttributeAsString("thickness", "0"));
            var roundness = EdaRatio.Parse(_rd.AttributeAsString("roundness", "0"));
            var filled = _rd.AttributeAsBoolean("filled", thickness == 0);

            _rd.NextTag();
            if (!_rd.IsEndTag("rectangle"))
                throw new InvalidDataException("Se esperaba </rectangle>");

            return new RectangleElement {
                LayerSet = layerSet,
                Position = position,
                Size = size,
                Roundness = roundness,
                Rotation = rotation,
                Thickness = thickness,
                Filled = filled
            };
        }

        /// <summary>
        /// Procesa un node 'circle'
        /// </summary>
        /// <returns>L'objecte 'CircleElement' obtingut.</returns>
        /// 
        private CircleElement ParseCircleNode() {

            if (!_rd.IsStartTag("circle"))
                throw new InvalidDataException("Se esperaba <circle>");

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var radius = EdaParser.ParseScalar(_rd.AttributeAsString("radius"));
            var thickness = EdaParser.ParseScalar(_rd.AttributeAsString("thickness", "0"));
            var filled = _rd.AttributeAsBoolean("filled", thickness == 0);

            _rd.NextTag();
            if (!_rd.IsEndTag("circle"))
                throw new InvalidDataException("Se esperaba </circle>");

            return new CircleElement {
                LayerSet = layerSet,
                Position = position,
                Radius = radius,
                Thickness = thickness,
                Filled = filled
            };
        }

        /// <summary>
        /// Procesa un node 'region'
        /// </summary>
        /// <returns>L'objecte 'RegionElement' obtingut.</returns>
        /// 
        private RegionElement ParseRegionNode() {

            if (!_rd.IsStartTag("region"))
                throw new InvalidDataException("Se esperaba <region>");

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var thickness = _rd.AttributeExists("thickness") ?
                EdaParser.ParseScalar(_rd.AttributeAsString("thickness")) :
                0;
            var filled = _rd.AttributeAsBoolean("filled", thickness == 0);
            var clearance = EdaParser.ParseScalar(_rd.AttributeAsString("clearance", "0"));
            var signalName = _rd.AttributeAsString("signal");

            var region = new RegionElement {
                LayerSet = layerSet,
                Thickness = thickness,
                Filled = filled,
                Clearance = clearance
            };

            if (signalName != null) {
                var signal = _board.GetSignal(signalName);
                _board.Connect(signal, region);
            }

            _rd.NextTag();
            var segments = new List<EdaArcPoint>();
            while (_rd.IsStartTag("segment")) {
                segments.Add(ParseRegionSegmentNode());
                _rd.NextTag();
            }
            region.AddSegments(segments);

            if (!_rd.IsEndTag("region"))
                throw new InvalidDataException("Se esperaba </region>");

            return region;
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
        private ThPadElement ParseTPadNode() {

            if (!_rd.IsStartTag("tpad"))
                throw new InvalidDataException("Se esperaba <tpad>");

            var name = _rd.AttributeAsString("name");
            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var size = EdaParser.ParseScalar(_rd.AttributeAsString("size"));
            var rotation = EdaParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            var drill = EdaParser.ParseScalar(_rd.AttributeAsString("drill"));
            var shape = _rd.AttributeAsEnum<ThPadElement.ThPadShape>("shape", ThPadElement.ThPadShape.Circle);
            var signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("tpad"))
                throw new InvalidDataException("Se esperaba </tpad>");

            var pad = new ThPadElement {
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

            if (signalName != null) {
                var signal = _board.GetSignal(signalName);
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

            var name = _rd.AttributeAsString("name");
            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var size = EdaParser.ParseSize(_rd.AttributeAsString("size"));
            var rotation = EdaParser.ParseAngle(_rd.AttributeAsString("rotation", "0"));
            var roundness = EdaParser.ParseRatio(_rd.AttributeAsString("roundness", "0"));
            var signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("spad"))
                throw new InvalidDataException("Se esperaba </spad>");

            var pad = new SmdPadElement {
                Name = name,
                LayerSet = layerSet,
                Position = position,
                Size = size,
                Rotation = rotation,
                Roundness = roundness
            };

            if (signalName != null) {
                var signal = _board.GetSignal(signalName);
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

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var outerSize = EdaParser.ParseScalar(_rd.AttributeAsString("outerSize"));
            var innerSize = _rd.AttributeExists("innerSize") ?
                EdaParser.ParseScalar(_rd.AttributeAsString("innerSize")) :
                outerSize;
            var drill = EdaParser.ParseScalar(_rd.AttributeAsString("drill"));
            var shape = _rd.AttributeAsEnum<ViaElement.ViaShape>("shape", ViaElement.ViaShape.Circle);
            var signalName = _rd.AttributeAsString("signal");

            _rd.NextTag();
            if (!_rd.IsEndTag("via"))
                throw new InvalidDataException("Se esperaba </via>");

            var via = new ViaElement { 
                LayerSet = layerSet,
                Position = position, 
                OuterSize = outerSize, 
                InnerSize = innerSize, 
                Drill = drill, 
                Shape = shape 
            };

            if (signalName != null) {
                var signal = _board.GetSignal(signalName);
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

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
            var position = EdaParser.ParsePoint(_rd.AttributeAsString("position"));
            var drill = EdaParser.ParseScalar(_rd.AttributeAsString("drill"));

            _rd.NextTag();
            if (!_rd.IsEndTag("hole"))
                throw new InvalidDataException("Se esperaba </hole>");

            return new HoleElement {
                LayerSet = layerSet,
                Position = position,
                Drill = drill
            };
        }

        /// <summary>
        /// Procesa un node 'text'
        /// </summary>
        /// <returns>L'objecte 'TextElement' obtingut.</returns>
        /// 
        private TextElement ParseTextNode() {

            if (!_rd.IsStartTag("text"))
                throw new InvalidDataException("Se esperaba <text>");

            var layerSet = EdaParser.ParseLayerSet(_rd.AttributeAsString("layers"));
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

            return new TextElement {
                LayerSet = layerSet,
                Position = position,
                Rotation = rotation,
                Height = height,
                Thickness = thickness,
                HorizontalAlign = horizontalAlign,
                VerticalAlign = verticalAlign,
                Value = value
            };
        }
    }
}
