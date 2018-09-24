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
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;
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
            ParseDocumentNode(board);

            return board;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseDocumentNode(Board board) {

            if (!rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            rd.NextTag();
            ParseBoardNode(board);

            // Llegeix el tag final
            //
            rd.NextTag();
        }

        /// <summary>
        /// Procesa el node 'board'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseBoardNode(Board board) {

            if (!rd.IsStartTag("board"))
                throw new InvalidDataException("Se esperaba <board>");

            version = rd.AttributeAsInteger("version");

            rd.NextTag();

            ParseLayersNode(board);
            rd.NextTag();

            if (rd.TagName == "signals") {
                ParseSignalsNode(board);
                rd.NextTag();
            }

            if (rd.TagName == "blocks") {
                ParseBlocksNode(board);
                rd.NextTag();
            }

            if (rd.TagName == "parts") {
                ParsePartsNode(board);
                rd.NextTag();
            }

            ParseBoardElementsNode(board);
            rd.NextTag();
        }

        /// <summary>
        /// Procesa el node 'layers'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseLayersNode(Board board) {

            if (!rd.IsStartTag("layers"))
                throw new InvalidDataException("Se esperaba <layers>");

            while (rd.NextTag() && rd.IsStartTag("layer"))
                ParseLayerNode(board);
        }

        /// <summary>
        /// Procesa el node 'layer'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseLayerNode(Board board) {

            // Comprova que el node sigui correcte
            //
            if (!rd.IsStartTag("layer"))
                throw new InvalidDataException("Se esperaba <layer>");

            // Obte els atributs de la capa
            //
            LayerId layerId = LayerId.Parse(rd.AttributeAsString("id"));
            LayerFunction function = rd.AttributeAsEnum<LayerFunction>("function", LayerFunction.Unknown);

            // Crea la capa i l'afeigeig a la placa.
            //
            Layer layer = new Layer(layerId, function);
            board.AddLayer(layer);

            // Llegeix el tag final
            //
            rd.NextTag();
        }

        /// <summary>
        /// Procesa el node 'signals'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseSignalsNode(Board board) {

            // Comprova que el node sigui correcte
            //
            if (!rd.IsStartTag("signals"))
                throw new InvalidDataException("Se esperaba <signals>");

            while (rd.NextTag() && rd.IsStartTag("signal"))
                ParseSignalNode(board);
        }

        /// <summary>
        /// Procesa el node 'signal'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseSignalNode(Board board) {

            // Comprova que el node sigui correcte
            //
            if (!rd.IsStartTag("signal"))
                throw new InvalidDataException("Se esperaba <signal>");

            // Obte els atributs de la senyal
            //
            string name = rd.AttributeAsString("name");

            // Crea la senyal i l'afegeix a la placa
            //
            Signal signal = new Signal(name);
            board.AddSignal(signal);

            rd.NextTag();
        }

        /// <summary>
        /// Procesa el node 'blocks'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseBlocksNode(Board board) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("blocks"))
                throw new InvalidDataException("Se esperaba <blocks>");

            while (rd.NextTag() && rd.IsStartTag("block"))
               ParseBlockNode(board);
        }

        /// <summary>
        /// Procesa el node 'block'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseBlockNode(Board board) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("block"))
                throw new InvalidDataException("Se esperaba <block>");

            // Obte els atriburs del bloc
            //
            string name = rd.AttributeAsString("name");

            // Crea el bloc i l'afegeix a la placa
            //
            Block block = new Block(name);
            board.AddBlock(block);

            rd.NextTag();
            ParseBlockElementsNode(block);

            rd.NextTag();
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <param name="block">El bloc</param>
        /// 
        private void ParseBlockElementsNode(Block block) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            // Obte els elements
            //
            List<Element> elementList = new List<Element>();
            while (rd.NextTag() && rd.IsStart) {
                switch (rd.TagName) {
                    case "line":
                        ParseLineNode(elementList);
                        break;

                    case "arc":
                        ParseArcNode(elementList);
                        break;

                    case "rectangle":
                        ParseRectangleNode(elementList);
                        break;

                    case "circle":
                        ParseCircleNode(elementList);
                        break;

                    case "region":
                        ParseRegionNode(elementList);
                        break;

                    case "tpad":
                        ParseTPadNode(elementList);
                        break;

                    case "spad":
                        ParseSPadNode(elementList);
                        break;

                    case "hole":
                        ParseHoleNode(elementList);
                        break;

                    case "text":
                        ParseTextNode(elementList);
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <line>, <arc>, <rectangle>, <circle>, <tpad>, <spad>, <via>, <text>, <region> o <hole>");
                }
            }

            // Afegeix els elements al bloc
            //
            block.AddElements(elementList);
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseBoardElementsNode(Board board) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            // Obte els elements
            //
            List<Element> elementList = new List<Element>();
            while (rd.NextTag() && rd.IsStart) {
                switch (rd.TagName) {
                    case "line":
                        ParseLineNode(elementList);
                        break;

                    case "arc":
                        ParseArcNode(elementList);
                        break;

                    case "rectangle":
                        ParseRectangleNode(elementList);
                        break;

                    case "circle":
                        ParseCircleNode(elementList);
                        break;

                    case "region":
                        ParseRegionNode(elementList);
                        break;

                    case "tpad":
                        ParseTPadNode(elementList);
                        break;

                    case "spad":
                        ParseSPadNode(elementList);
                        break;

                    case "via":
                        ParseViaNode(elementList);
                        break;

                    case "hole":
                        ParseHoleNode(elementList);
                        break;

                    case "text":
                        ParseTextNode(elementList);
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <line>, <arc>, <rectangle>, <circle>, <tpad>, <spad>, <via>, <text>, <region> o <hole>");
                }
            }

            // Afegeix els elements al bloc
            //
            board.AddElements(elementList);
        }

        /// <summary>
        /// Procesa un node 'parts'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParsePartsNode(Board board) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("parts"))
                throw new InvalidDataException("Se esperaba <parts>");

            while (rd.NextTag() && rd.IsStartTag("part"))
                ParsePartNode(board);
        }

        /// <summary>
        /// Procesa un node 'part'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParsePartNode(Board board) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("part"))
                throw new InvalidDataException("Se esperaba <part>");

            // Obte els atributs de l'objecte
            //
            string name = rd.AttributeAsString("name");
            Point position =  XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            BoardSide side = rd.AttributeAsEnum("side", BoardSide.Top);
            string blockName = rd.AttributeAsString("block");

            // Crea l'objecte i l'afegeix a la placa
            //
            Block block = board.GetBlock(blockName);
            Part part = new Part(block, name, position, rotation, side);
            board.AddPart(part);

            while (rd.NextTag() && rd.IsStart) {
                switch (rd.TagName) {
                    case "attributes":
                        ParsePartAttributesNode(part);
                        break;

                    case "pads":
                        ParsePartPadsNode(part);
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <pads> o <attributes>");
                }
            }
        }

        /// <summary>
        /// Procesa el node 'attributes'
        /// </summary>
        /// <param name="part">El part</param>
        /// 
        private void ParsePartAttributesNode(Part part) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("attributes"))
                throw new InvalidDataException("Se esperaba <attributes>");

            while (rd.NextTag() && rd.IsStartTag("attribute"))
                ParsePartAttributeNode(part);
        }

        /// <summary>
        /// Procesa un node 'attribute'
        /// </summary>
        /// <param name="part">El part.</param>
        /// 
        private void ParsePartAttributeNode(Part part) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("attribute"))
               throw new InvalidDataException("Se esperaba <attribute>");

            // Obte els atributs de l'objecte
            //
            string name = rd.AttributeAsString("name");
            string value = rd.AttributeAsString("value");
            bool visible = rd.AttributeAsBoolean("visible", false);

            // Crea l'objecte i l'afegeix a la llista
            //
            PartAttribute attribute = new PartAttribute(name, value, visible);
            part.AddAttribute(attribute);

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
        }

        /// <summary>
        /// Procesa el node 'pads'
        /// </summary>
        /// <param name="part">El part.</param>
        /// 
        private void ParsePartPadsNode(Part part) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("pads"))
                throw new InvalidDataException("Se esperaba <attributes>");

            while (rd.NextTag() && rd.IsStartTag("pad"))
                ParsePartPadNode(part);
        }

        /// <summary>
        /// Procesa un node 'pad'.
        /// </summary>
        /// <param name="part">El part.</param>
        /// 
        private void ParsePartPadNode(Part part) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("pad"))
                throw new InvalidDataException("Se esperaba <pad>");

            string padName = rd.AttributeAsString("name");
            string signalName = rd.AttributeAsString("signal");

            PadElement pad = part.GetPad(padName);
            Signal signal = board.GetSignal(signalName);
            board.Connect(signal, pad, part);

            rd.NextTag();
        }

        /// <summary>
        /// Procesa el node 'line'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseLineNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("line"))
                throw new InvalidDataException("Se esperaba <line>");

            // Obte els atributs del element
            //
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point startPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("startPosition"));
            Point endPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("endPosition"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            LineElement.LineCapStyle lineCap = rd.AttributeAsEnum<LineElement.LineCapStyle>("lineCap", LineElement.LineCapStyle.Round);

            // Crea l'element i l'afegeix a la llista
            //
            LineElement line = new LineElement(layerSet, startPosition, endPosition, thickness, lineCap);
            elementList.Add(line);

            // Assigna la senyal 
            //
            if (rd.AttributeExists("signal")) {
                string signalName = rd.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, line);
                }
            }

            // Llegeix el final del node
            //
            rd.NextTag();
        }

        /// <summary>
        /// Procesa un node 'arc'.
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseArcNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("arc"))
                throw new InvalidDataException("Se esperaba <arc>");

            // Obte els atributs de l'element
            //
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point startPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("startPosition"));
            Point endPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("endPosition"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            Angle angle = XmlTypeParser.ParseAngle(rd.AttributeAsString("angle"));
            LineElement.LineCapStyle lineCap = rd.AttributeAsEnum<LineElement.LineCapStyle>("lineCap", LineElement.LineCapStyle.Round);

            // Crea l'element i l'afegeix a la llista
            //
            ArcElement arc = new ArcElement(layerSet, startPosition, endPosition, thickness, angle, lineCap);
            elementList.Add(arc);

            // Assigna la senyal 
            //
            if (rd.AttributeExists("signal")) {
                string signalName = rd.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, arc);
                }
            }

            rd.NextTag();
        }

        /// <summary>
        /// Procesa un node 'rectangle'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseRectangleNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("rectangle"))
                throw new InvalidDataException("Se esperaba <rectangle>");

            // Obte els atributs de l'element.
            //
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Size size = XmlTypeParser.ParseSize(rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness", "0"));
            Ratio roundness = XmlTypeParser.ParseRatio(rd.AttributeAsString("roundness", "0"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);

            // Crea l'element i l'afegeix a la llista
            //
            RectangleElement rectangle = new RectangleElement(layerSet, position, size, roundness, rotation, thickness, filled);
            elementList.Add(rectangle);

            rd.NextTag();
        }

        /// <summary>
        /// Procesa un node 'circle'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseCircleNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("circle"))
                throw new InvalidDataException("Se esperaba <circle>");

            // Obte els atributs de l'element.
            //
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int radius = XmlTypeParser.ParseNumber(rd.AttributeAsString("radius"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness", "0"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);
 
            // Crea l'element i l'afegeix a la llista
            //
            CircleElement circle = new CircleElement(layerSet, position, radius, thickness, filled);
            elementList.Add(circle);

            rd.NextTag();
        }

        /// <summary>
        /// Procesa un node 'region'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseRegionNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("region"))
                throw new InvalidDataException("Se esperaba <region>");

            // Obte els atributs de l'element
            //
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);
            int clearance = XmlTypeParser.ParseNumber(rd.AttributeAsString("clearance", "0"));

            // Crea l'element i l'afegeix a la llista
            //
            RegionElement region = new RegionElement(layerSet, thickness, filled, clearance);
            elementList.Add(region);

            // Assigna la senyal 
            //
            if (rd.AttributeExists("signal")) {
                string signalName = rd.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, region);
                }
            }

            while (rd.NextTag() && rd.IsStart)
                ParseRegionSegmentNode(region);
        }

        /// <summary>
        /// Procesa un node 'segment'
        /// </summary>
        /// <param name="region">La regio.</param>
        /// 
        private void ParseRegionSegmentNode(RegionElement region) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("segment"))
                throw new InvalidDataException("Se esperaba <segment>");

            // Obte els atributs del segment
            //
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Angle angle = XmlTypeParser.ParseAngle(rd.AttributeAsString("angle", "0"));

            // Crea el segment i l'afegeix a la regio.
            //
            RegionElement.Segment segment = new RegionElement.Segment(position, angle);
            region.Add(segment);

            rd.NextTag();
        }

        /// <summary>
        /// Procesa un node tpad
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseTPadNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("tpad"))
                throw new InvalidDataException("Se esperaba <tpad>");

            // Obte els atributs de l'element
            //
            string name = rd.AttributeAsString("name");
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int size = XmlTypeParser.ParseNumber(rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int drill = XmlTypeParser.ParseNumber(rd.AttributeAsString("drill"));
            ThPadElement.ThPadShape shape = rd.AttributeAsEnum<ThPadElement.ThPadShape>("shape", ThPadElement.ThPadShape.Circle);

            // Crea l'element i l'afegeix a la llista
            //
            ThPadElement pad = new ThPadElement(name, layerSet, position, rotation, size, shape, drill);
            elementList.Add(pad);

            // Assigna la senyal 
            //
            if (rd.AttributeExists("signal")) {
                string signalName = rd.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, pad);
                }
            }

            // Llegeix el final del node
            //
            rd.NextTag();
        }

        /// <summary>
        /// Procesa un node tpad
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseSPadNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("spad"))
                throw new InvalidDataException("Se esperaba <spad>");

            // Obte els atributs de l'element.
            //
            string name = rd.AttributeAsString("name");
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Size size = XmlTypeParser.ParseSize(rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            Ratio roundness = XmlTypeParser.ParseRatio(rd.AttributeAsString("roundness", "0"));

            // Crea l'element i l'afegeix a la llista
            //
            SmdPadElement pad = new SmdPadElement(name, layerSet, position, size, rotation, roundness);
            elementList.Add(pad);

            // Assigna la senyal 
            //
            if (rd.AttributeExists("signal")) {
                string signalName = rd.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, pad);
                }
            }

            rd.NextTag();
        }

        /// <summary>
        /// Procesa un node 'via'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseViaNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("via"))
                throw new InvalidDataException("Se esperaba <via>");

            // Obte els atributs de l'element
            //
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int outerSize = XmlTypeParser.ParseNumber(rd.AttributeAsString("outerSize"));
            int innerSize = rd.AttributeExists("innerSize") ?
                XmlTypeParser.ParseNumber(rd.AttributeAsString("innerSize")) :
                outerSize;
            int drill = XmlTypeParser.ParseNumber(rd.AttributeAsString("drill"));
            ViaElement.ViaShape shape = rd.AttributeAsEnum<ViaElement.ViaShape>("shape", ViaElement.ViaShape.Circle);

            // Crtea l'element i l'afegeix a la llista
            //
            ViaElement via = new ViaElement(layerSet, position, outerSize, innerSize, drill, shape);
            elementList.Add(via);

            // Assigna la senyal 
            //
            if (rd.AttributeExists("signal")) {
                string signalName = rd.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, via);
                }
            }

            rd.NextTag();
        }

        /// <summary>
        /// Procesa un node 'hole'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseHoleNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("hole"))
                throw new InvalidDataException("Se esperaba <hole>");

            // Obte els atributs de l'element
            //
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int drill = XmlTypeParser.ParseNumber(rd.AttributeAsString("drill"));

            // Crea l'element i l'afegeix a la llista
            //
            HoleElement hole = new HoleElement(layerSet, position, drill);
            elementList.Add(hole);

            rd.NextTag();
        }

        /// <summary>
        /// Procesa un node 'text'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseTextNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("text"))
                throw new InvalidDataException("Se esperaba <text>");

            // Obte els parametres de l'objecte
            //
            LayerSet layerSet = LayerSet.Parse(rd.AttributeAsString("layers"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int height = XmlTypeParser.ParseNumber(rd.AttributeAsString("height"));
            HorizontalTextAlign horizontalAlign = rd.AttributeAsEnum("horizontalAlign", HorizontalTextAlign.Left);
            VerticalTextAlign verticalAlign = rd.AttributeAsEnum("verticalAlign", VerticalTextAlign.Bottom);
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            string value = rd.AttributeAsString("value");

            // Crea l'objecte i l'afegeix al la llista
            //
            TextElement text = new TextElement(layerSet, position, rotation, height, thickness, horizontalAlign, verticalAlign);
            elementList.Add(text);
            text.Value = value;

            // Llegeix el final del node
            //
            rd.NextTag();
        }
    }
}
