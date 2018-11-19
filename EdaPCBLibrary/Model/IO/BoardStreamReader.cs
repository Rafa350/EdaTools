namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Xml;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;

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

        private readonly XmlReaderAdapter rd;
        private Board board;
        private int version;

        /// <summary>
        /// Constructor estatic de la clase
        /// </summary>
        /// 
        static BoardStreamReader() {

            schemas = new XmlSchemaSet();

            string schemaResourceName = "MikroPic.EdaTools.v1.Pcb.Model.IO.Schemas.XBRD.xsd";
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
                throw new ArgumentNullException("stream");

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
            rd = new XmlReaderAdapter(reader);
        }

        /// <summary>
        /// Llegeix una placa.
        /// </summary>
        /// <returns>La placa.</returns>
        /// 
        public Board Read() {

            board = new Board();

            rd.NextTag();
            ParseDocumentNode();

            return board;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// 
        private void ParseDocumentNode() {

            if (!rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            rd.NextTag();
            ParseBoardNode();

            rd.NextTag();
            if (!rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");
}

        /// <summary>
        /// Procesa el node 'board'
        /// </summary>
        /// 
        private void ParseBoardNode() {

            if (!rd.IsStartTag("board"))
                throw new InvalidDataException("Se esperaba <board>");

            version = rd.AttributeAsInteger("version");

            rd.NextTag();
            if (rd.TagName == "layers") {
                board.AddLayers(ParseLayersNode());
                rd.NextTag();
            }
            if (rd.TagName == "signals") {
                board.AddSignals(ParseSignalsNode());
                rd.NextTag();
            }
            if (rd.TagName == "blocks") {
                board.AddBlocks(ParseBlocksNode());
                rd.NextTag();
            }
            if (rd.TagName == "parts") {
                board.AddParts(ParsePartsNode());
                rd.NextTag();
            }
            if (rd.TagName == "elements") {
                board.AddElements(ParseBoardElementsNode());
                rd.NextTag();
            }

            if (!rd.IsEndTag("board"))
                throw new InvalidDataException("Se esperaba </board>");
        }

        /// <summary>
        /// Procesa el node 'layers'.
        /// </summary>
        /// <returns>La llista d'objectes 'Layer' obtinguda.</returns>
        /// 
        private IEnumerable<Layer> ParseLayersNode() {

            if (!rd.IsStartTag("layers"))
                throw new InvalidDataException("Se esperaba <layers>");

            List<Layer> layers = new List<Layer>();

            rd.NextTag();
            while (rd.IsStartTag("layer")) {
                layers.Add(ParseLayerNode());
                rd.NextTag();
            }

            if (!rd.IsEndTag("layers"))
                throw new InvalidDataException("Se esperaba </layers>");

            return layers;
        }

        /// <summary>
        /// Procesa el node 'layer'.
        /// </summary>
        /// <returns>L'objecte 'Layer' obtingut.</returns>
        /// 
        private Layer ParseLayerNode() {

            if (!rd.IsStartTag("layer"))
                throw new InvalidDataException("Se esperaba <layer>");

            LayerId layerId = LayerId.Parse(rd.AttributeAsString("id"));
            LayerFunction function = rd.AttributeAsEnum<LayerFunction>("function", LayerFunction.Unknown);

            rd.NextTag();
            if (!rd.IsEndTag("layer"))
                throw new InvalidDataException("Se esperaba </layer>");

            Layer layer = new Layer(layerId, function);
            return layer;
        }

        /// <summary>
        /// Procesa el node 'signals'.
        /// </summary>
        /// 
        private IEnumerable<Signal> ParseSignalsNode() {

            if (!rd.IsStartTag("signals"))
                throw new InvalidDataException("Se esperaba <signals>");

            List<Signal> signals = new List<Signal>();

            rd.NextTag();
            while (rd.IsStartTag("signal")) {
                signals.Add(ParseSignalNode());
                rd.NextTag();
            }

            if (!rd.IsEndTag("signals"))
                throw new InvalidDataException("Se esperaba </signals>");

            return signals;
        }

        /// <summary>
        /// Procesa el node 'signal'
        /// </summary>
        /// <returns>L'objecte 'Signal' obtingut.</returns>
        /// 
        private Signal ParseSignalNode() {

            if (!rd.IsStartTag("signal"))
                throw new InvalidDataException("Se esperaba <signal>");

            string name = rd.AttributeAsString("name");

            rd.NextTag();
            if (!rd.IsEndTag("signal"))
                throw new InvalidDataException("Se esperaba </signal>");

            Signal signal = new Signal(name);
            return signal;
        }

        /// <summary>
        /// Procesa el node 'blocks'
        /// </summary>
        /// <returns>La llista d'objectes 'Block' obtinguda.</returns>
        /// 
        private IEnumerable<Block> ParseBlocksNode() {

            if (!rd.IsStartTag("blocks"))
                throw new InvalidDataException("Se esperaba <blocks>");

            List<Block> blocks = new List<Block>();

            rd.NextTag();
            while (rd.IsStartTag("block")) {
                blocks.Add(ParseBlockNode());
                rd.NextTag();
            }

            if (!rd.IsEndTag("blocks"))
                throw new InvalidDataException("Se esperaba </blocks>");

            return blocks;
        }

        /// <summary>
        /// Procesa el node 'block'.
        /// </summary>
        /// <returns>L'objecte 'Block' obtingut.</returns>
        /// 
        private Block ParseBlockNode() {

            if (!rd.IsStartTag("block"))
                throw new InvalidDataException("Se esperaba <block>");

            string name = rd.AttributeAsString("name");

            Block block = new Block(name);

            rd.NextTag();
            block.AddElements(ParseBlockElementsNode());
            rd.NextTag();

            if (!rd.IsEndTag("block"))
                throw new InvalidDataException("Se esperaba </block>");

            return block;
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <returns>La llista d'objectres 'Element' obtinguda.</returns>
        /// 
        private IEnumerable<Element> ParseBlockElementsNode() {

            if (!rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            List<Element> elements = new List<Element>();

            rd.NextTag();
            while (rd.IsStart) {
                switch (rd.TagName) {
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
                rd.NextTag();
            }

            if (!rd.IsEndTag("elements"))
                throw new InvalidDataException("Se esperaba </elements>");

            return elements;
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <returns>La llista d'objectes 'Element' obtinguda.</returns>
        /// 
        private IEnumerable<Element> ParseBoardElementsNode() {

            if (!rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            List<Element> elements = new List<Element>();

            rd.NextTag();
            while (rd.IsStart) {
                switch (rd.TagName) {
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
                rd.NextTag();
            }

            if (!rd.IsEndTag("elements"))
                throw new InvalidDataException("Se esperaba </elements>");

            return elements;
        }

        /// <summary>
        /// Procesa un node 'parts'.
        /// </summary>
        /// <returns>La llista d'objectes 'Part' obtinguts.</returns>
        /// 
        private IEnumerable<Part> ParsePartsNode() {

            if (!rd.IsStartTag("parts"))
                throw new InvalidDataException("Se esperaba <parts>");

            List<Part> parts = new List<Part>();

            rd.NextTag();
            while (rd.IsStartTag("part")) {
                parts.Add(ParsePartNode());
                rd.NextTag();
            }

            if (!rd.IsEndTag("parts"))
                throw new InvalidDataException("Se esperaba </parts>");

            return parts;
        }

        /// <summary>
        /// Procesa un node 'part'.
        /// </summary>
        /// <returns>L'objecte 'Part' obtingut.</returns>
        /// 
        private Part ParsePartNode() {

            if (!rd.IsStartTag("part"))
                throw new InvalidDataException("Se esperaba <part>");

            string name = rd.AttributeAsString("name");
            Point position =  XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            BoardSide side = rd.AttributeAsEnum("side", BoardSide.Top);
            string blockName = rd.AttributeAsString("block");

            Block block = board.GetBlock(blockName);
            Part part = new Part(block, name, position, rotation, side);

            rd.NextTag();
            while (rd.IsStart) {
                switch (rd.TagName) {
                    case "attributes":
                        part.AddAttributes(ParsePartAttributesNode());
                        break;

                    case "pads":
                        foreach (var padInfo in ParsePartPadsNode()) {
                            PadElement pad = part.GetPad(padInfo.Name);
                            Signal signal = board.GetSignal(padInfo.SignalName);
                            board.Connect(signal, pad, part);
                        }
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <pads> o <attributes>");
                }
                rd.NextTag();
            }

            if (!rd.IsEndTag("part"))
                throw new InvalidDataException("Se esperaba </part>");

            return part;
        }

        /// <summary>
        /// Procesa el node 'attributes'
        /// </summary>
        /// 
        private IEnumerable<PartAttribute> ParsePartAttributesNode() {

            if (!rd.IsStartTag("attributes"))
                throw new InvalidDataException("Se esperaba <attributes>");

            List<PartAttribute> attributes = new List<PartAttribute>();

            rd.NextTag();
            while (rd.IsStartTag("attribute")) {
                attributes.Add(ParsePartAttributeNode());
                rd.NextTag();
            }

            if (!rd.IsEndTag("attributes"))
                throw new InvalidDataException("Se esperaba </attributes>");

            return attributes;
        }

        /// <summary>
        /// Procesa un node 'attribute'
        /// </summary>
        /// <returns>L'objecte 'partAttribute' obtingut.</returns>
        /// 
        private PartAttribute ParsePartAttributeNode() {

            if (!rd.IsStartTag("attribute"))
               throw new InvalidDataException("Se esperaba <attribute>");

            string name = rd.AttributeAsString("name");
            string value = rd.AttributeAsString("value");
            bool visible = rd.AttributeAsBoolean("visible", false);

            PartAttribute attribute = new PartAttribute(name, value, visible);

            if (rd.AttributeExists("position"))
                attribute.Position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));

            if (rd.AttributeExists("rotation"))
                attribute.Rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation"));

            if (rd.AttributeExists("height"))
                attribute.Height = XmlTypeParser.ParseNumber(rd.AttributeAsString("height"));

            if (rd.AttributeExists("horizontalAlign"))
                attribute.HorizontalAlign = rd.AttributeAsEnum("horizontalAlign", HorizontalTextAlign.Left);

            if (rd.AttributeExists("verticalAlign"))
                attribute.VerticalAlign = rd.AttributeAsEnum("verticalAlign", VerticalTextAlign.Bottom);

            rd.NextTag();
            if (!rd.IsEndTag("attribute"))
                throw new InvalidDataException("Se esperaba </attribute>");

            return attribute;
        }

        /// <summary>
        /// Procesa el node 'pads'
        /// </summary>
        /// <returns>La llista d'objectes 'PadInfo' obtinguda.</returns>
        /// 
        private IEnumerable<PadInfo> ParsePartPadsNode() {

            if (!rd.IsStartTag("pads"))
                throw new InvalidDataException("Se esperaba <pads>");

            List<PadInfo> pads = new List<PadInfo>();

            rd.NextTag();
            while (rd.IsStartTag("pad")) {
                pads.Add(ParsePartPadNode());
                rd.NextTag();
            }

            if (!rd.IsEndTag("pads"))
                throw new InvalidDataException("Se esperaba </pads>");

            return pads;
        }

        /// <summary>
        /// Procesa un node 'pad'.
        /// </summary>
        /// <returns>L'objecte 'PadInfo' obtingut.</returns>
        /// 
        private PadInfo ParsePartPadNode() {

            if (!rd.IsStartTag("pad"))
                throw new InvalidDataException("Se esperaba <pad>");

            string padName = rd.AttributeAsString("name");
            string signalName = rd.AttributeAsString("signal");

            rd.NextTag();
            if (!rd.IsEndTag("pad"))
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

            if (!rd.IsStartTag("line"))
                throw new InvalidDataException("Se esperaba <line>");

            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point startPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("startPosition"));
            Point endPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("endPosition"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            LineElement.LineCapStyle lineCap = rd.AttributeAsEnum<LineElement.LineCapStyle>("lineCap", LineElement.LineCapStyle.Round);
            string signalName = rd.AttributeAsString("signal");

            rd.NextTag();
            if (!rd.IsEndTag("line"))
                throw new InvalidDataException("Se esperaba </line>");

            LineElement line = new LineElement(layerSet, startPosition, endPosition, thickness, lineCap);
            if (signalName != null) {
                Signal signal = board.GetSignal(signalName);
                board.Connect(signal, line);
            }

            return line;
        }

        /// <summary>
        /// Procesa un node 'arc'.
        /// </summary>
        /// <returns>L'objecte 'ArcElement' obtingut.</returns>
        /// 
        private ArcElement ParseArcNode() {

            if (!rd.IsStartTag("arc"))
                throw new InvalidDataException("Se esperaba <arc>");

            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point startPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("startPosition"));
            Point endPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("endPosition"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            Angle angle = XmlTypeParser.ParseAngle(rd.AttributeAsString("angle"));
            LineElement.LineCapStyle lineCap = rd.AttributeAsEnum<LineElement.LineCapStyle>("lineCap", LineElement.LineCapStyle.Round);
            string signalName = rd.AttributeAsString("signal");

            rd.NextTag();
            if (!rd.IsEndTag("arc"))
                throw new InvalidDataException("Se esperaba </arc>");

            ArcElement arc = new ArcElement(layerSet, startPosition, endPosition, thickness, angle, lineCap);
            if (signalName != null) {
                Signal signal = board.GetSignal(signalName);
                board.Connect(signal, arc);
            }

            return arc;
        }

        /// <summary>
        /// Procesa un node 'rectangle'
        /// </summary>
        /// <returns>L'objecte 'RectangleElement' obtingut.</returns>
        /// 
        private RectangleElement ParseRectangleNode() {

            if (!rd.IsStartTag("rectangle"))
                throw new InvalidDataException("Se esperaba <rectangle>");

            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Size size = XmlTypeParser.ParseSize(rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness", "0"));
            Ratio roundness = XmlTypeParser.ParseRatio(rd.AttributeAsString("roundness", "0"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);

            rd.NextTag();
            if (!rd.IsEndTag("rectangle"))
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

            if (!rd.IsStartTag("circle"))
                throw new InvalidDataException("Se esperaba <circle>");

            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int radius = XmlTypeParser.ParseNumber(rd.AttributeAsString("radius"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness", "0"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);
 
            rd.NextTag();
            if (!rd.IsEndTag("circle"))
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

            if (!rd.IsStartTag("region"))
                throw new InvalidDataException("Se esperaba <region>");

            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);
            int clearance = XmlTypeParser.ParseNumber(rd.AttributeAsString("clearance", "0"));
            string signalName = rd.AttributeAsString("signal");

            RegionElement region = new RegionElement(layerSet, thickness, filled, clearance);
            if (signalName != null) {
                Signal signal = board.GetSignal(signalName);
                board.Connect(signal, region);
            }

            rd.NextTag();
            while (rd.IsStartTag("segment")) {
                region.Add(ParseRegionSegmentNode());
                rd.NextTag();
            }

            if (!rd.IsEndTag("region"))
                throw new InvalidDataException("Se esperaba </region>");

            return region;
        }

        /// <summary>
        /// Procesa un node 'segment'
        /// </summary>
        /// <returns>L'objecte 'RegionElement.Segment' obtingut.</returns>
        /// 
        private RegionElement.Segment ParseRegionSegmentNode() {

            if (!rd.IsStartTag("segment"))
                throw new InvalidDataException("Se esperaba <segment>");

            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Angle angle = XmlTypeParser.ParseAngle(rd.AttributeAsString("angle", "0"));

            rd.NextTag();
            if (!rd.IsEndTag("segment"))
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

            if (!rd.IsStartTag("tpad"))
                throw new InvalidDataException("Se esperaba <tpad>");

            string name = rd.AttributeAsString("name");
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int size = XmlTypeParser.ParseNumber(rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int drill = XmlTypeParser.ParseNumber(rd.AttributeAsString("drill"));
            ThPadElement.ThPadShape shape = rd.AttributeAsEnum<ThPadElement.ThPadShape>("shape", ThPadElement.ThPadShape.Circle);
            string signalName = rd.AttributeAsString("signal");

            rd.NextTag();
            if (!rd.IsEndTag("tpad"))
                throw new InvalidDataException("Se esperaba </tpad>");

            ThPadElement pad = new ThPadElement(name, layerSet, position, rotation, size, shape, drill);
            if (signalName != null) {
                Signal signal = board.GetSignal(signalName);
                board.Connect(signal, pad);
            }

            return pad;
        }

        /// <summary>
        /// Procesa un node tpad
        /// </summary>
        /// <returns>L'objecte 'SPadElement' obtingut.</returns>
        /// 
        private SmdPadElement ParseSPadNode() {

            if (!rd.IsStartTag("spad"))
                throw new InvalidDataException("Se esperaba <spad>");

            string name = rd.AttributeAsString("name");
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Size size = XmlTypeParser.ParseSize(rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            Ratio roundness = XmlTypeParser.ParseRatio(rd.AttributeAsString("roundness", "0"));
            string signalName = rd.AttributeAsString("signal");

            rd.NextTag();
            if (!rd.IsEndTag("spad"))
                throw new InvalidDataException("Se esperaba </spad>");

            SmdPadElement pad = new SmdPadElement(name, layerSet, position, size, rotation, roundness);
            if (signalName != null) {
                Signal signal = board.GetSignal(signalName);
                board.Connect(signal, pad);
            }

            return pad;
        }

        /// <summary>
        /// Procesa un node 'via'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private ViaElement ParseViaNode() {

            if (!rd.IsStartTag("via"))
                throw new InvalidDataException("Se esperaba <via>");

            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int outerSize = XmlTypeParser.ParseNumber(rd.AttributeAsString("outerSize"));
            int innerSize = rd.AttributeExists("innerSize") ?
                XmlTypeParser.ParseNumber(rd.AttributeAsString("innerSize")) :
                outerSize;
            int drill = XmlTypeParser.ParseNumber(rd.AttributeAsString("drill"));
            ViaElement.ViaShape shape = rd.AttributeAsEnum<ViaElement.ViaShape>("shape", ViaElement.ViaShape.Circle);
            string signalName = rd.AttributeAsString("signal");

            rd.NextTag();
            if (!rd.IsEndTag("via"))
                throw new InvalidDataException("Se esperaba </via>");

            ViaElement via = new ViaElement(layerSet, position, outerSize, innerSize, drill, shape);
            if (signalName != null) {
                Signal signal = board.GetSignal(signalName);
                board.Connect(signal, via);
            }

            return via;
        }

        /// <summary>
        /// Procesa un node 'hole'
        /// </summary>
        /// <returns>L'objecte 'HoleElement' obtigut.</returns>
        /// 
        private HoleElement ParseHoleNode() {

            if (!rd.IsStartTag("hole"))
                throw new InvalidDataException("Se esperaba <hole>");

            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int drill = XmlTypeParser.ParseNumber(rd.AttributeAsString("drill"));

            rd.NextTag();
            if (!rd.IsEndTag("hole"))
                throw new InvalidDataException("Se esperaba </hole>");

            HoleElement hole = new HoleElement(layerSet, position, drill);

            return hole;
        }

        /// <summary>
        /// Procesa un node 'text'
        /// </summary>
        /// <returns>L'objecte 'TextElement' obtingut.</returns>
        /// 
        private TextElement ParseTextNode() {

            if (!rd.IsStartTag("text"))
                throw new InvalidDataException("Se esperaba <text>");

            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int height = XmlTypeParser.ParseNumber(rd.AttributeAsString("height"));
            HorizontalTextAlign horizontalAlign = rd.AttributeAsEnum("horizontalAlign", HorizontalTextAlign.Left);
            VerticalTextAlign verticalAlign = rd.AttributeAsEnum("verticalAlign", VerticalTextAlign.Bottom);
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            string value = rd.AttributeAsString("value");

            rd.NextTag();
            if (!rd.IsEndTag("text"))
                throw new InvalidDataException("Se esperaba </text>");

            TextElement text = new TextElement(layerSet, position, rotation, height, thickness, horizontalAlign, verticalAlign, value);

            return text;
        }
    }
}
