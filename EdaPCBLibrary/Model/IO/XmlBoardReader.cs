namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.IO.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Clase per la lectura de plaques des d'un stream
    /// </summary>
    /// 
    public sealed class XmlBoardReader {

        private readonly XmlTagReader reader;
        private Board board;
        private int version;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="stream">Stream de lectura.</param>
        /// 
        public XmlBoardReader(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead)
                throw new InvalidOperationException("El stream no es de lectura.");

            reader = new XmlTagReader(stream);
        }

        /// <summary>
        /// Llegeix una placa.
        /// </summary>
        /// <returns>La placa.</returns>
        /// 
        public Board Read() {

            board = new Board();

            reader.NextTag();
            ParseBoardNode(board);

            return board;
        }

        /// <summary>
        /// Procesa el node 'board'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseBoardNode(Board board) {

            if (!reader.IsStartTag("board"))
                throw new InvalidDataException("Se esperaba <board>");

            version = reader.AttributeAsInteger("version");

            reader.NextTag();
            ParseLayersNode(board);

            reader.NextTag();
            ParseLayerPairsNode(board);

            reader.NextTag();
            ParseSignalsNode(board);

            reader.NextTag();
            ParseBlocksNode(board);

            reader.NextTag();
            ParsePartsNode(board);

            reader.NextTag();
            ParseBoardElementsNode(board);
        }

        /// <summary>
        /// Procesa el node 'layers'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseLayersNode(Board board) {

            if (!reader.IsStartTag("layers"))
                throw new InvalidDataException("Se esperaba <layers>");

            while (reader.NextTag() && reader.IsStartTag("layer"))
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
            if (!reader.IsStartTag("layer"))
                throw new InvalidDataException("Se esperaba <layer>");

            // Obte els atributs de la capa
            //
            string name = reader.AttributeAsString("name");
            BoardSide side = ParseEnumAttribute("side", BoardSide.Unknown);
            LayerFunction function = ParseEnumAttribute("function", LayerFunction.Unknown);
            Color color = ParseColorAttribute("color", new Color(128, 128, 128));
            bool isVisible = ParseBoolAttribute("isVisible", true);

            // Crea la capa i l'afeigeig a la placa.
            //
            Layer layer = new Layer(name, side, function, color, isVisible);
            board.AddLayer(layer);

            // Llegeix el tag final
            //
            reader.NextTag();
        }

        /// <summary>
        /// Procesa el node 'layerPairs'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseLayerPairsNode(Board board) {

            // Comprova que el node sigui correcte
            //
            if (!reader.IsStartTag("layerPairs"))
                throw new InvalidDataException("Se esperaba <layerPairs>");

            while (reader.NextTag() && reader.IsStartTag("pair"))
                ParsePairNode(board);
        }

        /// <summary>
        /// Procesa el node 'pair'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParsePairNode(Board board) {

            // Comprova que el node sigui correcte
            //
            if (!reader.IsStartTag("pair"))
                throw new InvalidDataException("Se esperaba <pair>");

            // Obte els atributs del parell de capes
            //
            string name1 = reader.AttributeAsString("layer1");
            string name2 = reader.AttributeAsString("layer2");

            // Defineix el parell de capes
            //
            Layer layer1 = board.GetLayer(name1);
            Layer layer2 = board.GetLayer(name2);
            try {
                board.DefinePair(layer1, layer2);
            }
            catch {
                // Ignora el error
            }

            reader.NextTag();
        }

        /// <summary>
        /// Procesa el node 'signals'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseSignalsNode(Board board) {

            // Comprova que el node sigui correcte
            //
            if (!reader.IsStartTag("signals"))
                throw new InvalidDataException("Se esperaba <signals>");

            while (reader.NextTag() && reader.IsStartTag("signal"))
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
            if (!reader.IsStartTag("signal"))
                throw new InvalidDataException("Se esperaba <signal>");

            // Obte els atributs de la senyal
            //
            string name = reader.AttributeAsString("name");

            // Crea la senyal i l'afegeix a la placa
            //
            Signal signal = new Signal(name);
            board.AddSignal(signal);

            reader.NextTag();
        }

        /// <summary>
        /// Procesa el node 'blocks'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseBlocksNode(Board board) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("blocks"))
                throw new InvalidDataException("Se esperaba <blocks>");

            while (reader.NextTag() && reader.IsStartTag("block"))
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
            if (!reader.IsStartTag("block"))
                throw new InvalidDataException("Se esperaba <block>");

            // Obte els atriburs del bloc
            //
            string name = reader.AttributeAsString("name");

            // Crea el bloc i l'afegeix a la placa
            //
            Block block = new Block(name);
            board.AddBlock(block);

            reader.NextTag();
            ParseBlockElementsNode(block);

            reader.NextTag();
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <param name="block">El bloc</param>
        /// 
        private void ParseBlockElementsNode(Block block) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            // Obte els elements
            //
            List<Element> elementList = new List<Element>();
            while (reader.NextTag() && reader.IsStart) {
                switch (reader.TagName) {
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
            if (!reader.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            // Obte els elements
            //
            List<Element> elementList = new List<Element>();
            while (reader.NextTag() && reader.IsStart) {
                switch (reader.TagName) {
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
            if (!reader.IsStartTag("parts"))
                throw new InvalidDataException("Se esperaba <parts>");

            while (reader.NextTag() && reader.IsStartTag("part"))
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
            if (!reader.IsStartTag("part"))
                throw new InvalidDataException("Se esperaba <part>");

            // Obte els atributs de l'objecte
            //
            string name = reader.AttributeAsString("name");
            Point position = ParsePointAttribute("position");
            Angle rotation = ParseAngleAttribute("rotation");
            BoardSide side = ParseEnumAttribute("side", BoardSide.Top);
            string blockName = reader.AttributeAsString("block");

            // Crea l'objecte i l'afegeix a la placa
            //
            Block block = board.GetBlock(blockName);
            Part part = new Part(block, name, position, rotation, side);
            board.AddPart(part);

            while (reader.NextTag() && reader.IsStart) {
                switch (reader.TagName) {
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
            if (!reader.IsStartTag("attributes"))
                throw new InvalidDataException("Se esperaba <attributes>");

            while (reader.NextTag() && reader.IsStartTag("attribute"))
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
            if (!reader.IsStartTag("attribute"))
               throw new InvalidDataException("Se esperaba <attribute>");

            // Obte els atributs de l'objecte
            //
            string name = reader.AttributeAsString("name");
            string value = reader.AttributeAsString("value");
            bool isVisible = ParseBoolAttribute("visible", false);

            // Crea l'objecte i l'afegeix a la llista
            //
            PartAttribute attribute = new PartAttribute(name, value, isVisible);
            part.AddAttribute(attribute);

            if (reader.AttributeExists("position"))
                attribute.Position = ParsePointAttribute("position");

            if (reader.AttributeExists("rotation"))
                attribute.Rotation = ParseAngleAttribute("rotation");

            if (reader.AttributeExists("height"))
                attribute.Height = ParseNumberAttribute("height");

            if (reader.AttributeExists("align"))
                attribute.Align = ParseEnumAttribute("align", TextAlign.TopLeft);

            reader.NextTag();
        }

        /// <summary>
        /// Procesa el node 'pads'
        /// </summary>
        /// <param name="part">El part.</param>
        /// 
        private void ParsePartPadsNode(Part part) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("pads"))
                throw new InvalidDataException("Se esperaba <attributes>");

            while (reader.NextTag() && reader.IsStartTag("pad"))
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
            if (!reader.IsStartTag("pad"))
                throw new InvalidDataException("Se esperaba <pad>");

            string padName = reader.AttributeAsString("name");
            string signalName = reader.AttributeAsString("signal");

            PadElement pad = part.GetPad(padName);
            Signal signal = board.GetSignal(signalName);
            board.Connect(signal, pad, part);

            reader.NextTag();
        }

        /// <summary>
        /// Procesa el node 'line'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseLineNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("line"))
                throw new InvalidDataException("Se esperaba <line>");

            // Obte els atributs del element
            //
            Point startPosition = ParsePointAttribute("startPosition");
            Point endPosition = ParsePointAttribute("endPosition");
            int thickness = ParseNumberAttribute("thickness");
            LineElement.LineCapStyle lineCap = ParseEnumAttribute("lineCap", LineElement.LineCapStyle.Round);
            string[] layerNames = reader.AttributeAsStrings("layers");

            // Crea l'element i l'afegeix a la llista
            //
            LineElement line = new LineElement(startPosition, endPosition, thickness, lineCap);
            elementList.Add(line);

            // Asigna les capes al element
            //
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), line);

            // Assigna la senyal 
            //
            if (reader.AttributeExists("signal")) {
                string signalName = reader.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, line);
                }
            }

            // Llegeix el final del node
            //
            reader.NextTag();
        }

        /// <summary>
        /// Procesa un node 'arc'.
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseArcNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("arc"))
                throw new InvalidDataException("Se esperaba <arc>");

            // Obte els atributs de l'element
            //
            Point startPosition = ParsePointAttribute("startPosition");
            Point endPosition = ParsePointAttribute("endPosition");
            int thickness = ParseNumberAttribute("thickness");
            Angle angle = ParseAngleAttribute("angle");
            string[] layerNames = reader.AttributeAsStrings("layers");
            LineElement.LineCapStyle lineCap = ParseEnumAttribute("lineCap", LineElement.LineCapStyle.Round);

            // Crea l'element i l'afegeix a la llista
            //
            ArcElement arc = new ArcElement(startPosition, endPosition, thickness, angle, lineCap);
            elementList.Add(arc);

            // Asigna les capes a l'element
            //
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), arc);

            // Assigna la senyal 
            //
            if (reader.AttributeExists("signal")) {
                string signalName = reader.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, arc);
                }
            }

            reader.NextTag();
        }

        /// <summary>
        /// Procesa un node 'rectangle'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseRectangleNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("rectangle"))
                throw new InvalidDataException("Se esperaba <rectangle>");

            // Obte els atributs de l'element.
            //
            Point position = ParsePointAttribute("position");
            Size size = ParseSizeAttribute("size");
            Angle rotation = ParseAngleAttribute("rotation");
            int thickness = ParseNumberAttribute("thickness");
            bool filled = ParseBoolAttribute("filled", thickness == 0);
            Ratio roundness = ParseRatioAttribute("roundness");
            string[] layerNames = reader.AttributeAsStrings("layers");

            // Crea l'element i l'afegeix a la llista
            //
            RectangleElement rectangle = new RectangleElement(position, size, roundness, rotation, thickness, filled);
            elementList.Add(rectangle);

            // Asigna les capes a l'element
            //
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), rectangle);

            reader.NextTag();
        }

        /// <summary>
        /// Procesa un node 'circle'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseCircleNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("circle"))
                throw new InvalidDataException("Se esperaba <circle>");

            // Obte els atributs de l'element.
            //
            Point position = ParsePointAttribute("position");
            int radius = ParseNumberAttribute("radius");
            int thickness = ParseNumberAttribute("thickness");
            bool filled = ParseBoolAttribute("filled", thickness == 0);
            string[] layerNames = reader.AttributeAsStrings("layers");

            // Crea l'element i l'afegeix a la llista
            //
            CircleElement circle = new CircleElement(position, radius, thickness, filled);
            elementList.Add(circle);

            // Asigna les capes a l'element
            //
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), circle);

            reader.NextTag();
        }

        /// <summary>
        /// Procesa un node 'region'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseRegionNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("region"))
                throw new InvalidDataException("Se esperaba <region>");

            // Obte els atributs de l'element
            //
            int thickness = ParseNumberAttribute("thickness");
            bool filled = reader.AttributeAsBoolean("filled", thickness == 0);
            int clearance = ParseNumberAttribute("clearance");
            string[] layerNames = reader.AttributeAsStrings("layers");

            // Crea l'element i l'afegeix a la llista
            //
            RegionElement region = new RegionElement(thickness, filled, clearance);
            elementList.Add(region);

            // Asigna les capes a l'element
            //
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), region);

            // Assigna la senyal 
            //
            if (reader.AttributeExists("signal")) {
                string signalName = reader.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, region);
                }
            }

            while (reader.NextTag() && reader.IsStart)
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
            if (!reader.IsStartTag("segment"))
                throw new InvalidDataException("Se esperaba <segment>");

            // Obte els atributs del segment
            //
            Point position = ParsePointAttribute("position");
            Angle angle = ParseAngleAttribute("angle");

            // Crea el segment i l'afegeix a la regio.
            //
            RegionElement.Segment segment = new RegionElement.Segment(position, angle);
            region.Add(segment);

            reader.NextTag();
        }

        /// <summary>
        /// Procesa un node tpad
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseTPadNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("tpad"))
                throw new InvalidDataException("Se esperaba <tpad>");

            // Obte els atributs de l'element
            //
            string name = reader.AttributeAsString("name");
            Point position = ParsePointAttribute("position");
            int size = ParseNumberAttribute("size");
            Angle rotation = ParseAngleAttribute("rotation");
            int drill = ParseNumberAttribute("drill");
            ThPadElement.ThPadShape shape = ParseEnumAttribute("shape", ThPadElement.ThPadShape.Circular);
            string[] layerNames = reader.AttributeAsStrings("layers");

            // Crea l'element i l'afegeix a la llista
            //
            ThPadElement pad = new ThPadElement(name, position, rotation, size, shape, drill);
            elementList.Add(pad);

            // Asigna les capes a l'element.
            //
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), pad);

            // Assigna la senyal 
            //
            if (reader.AttributeExists("signal")) {
                string signalName = reader.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, pad);
                }
            }

            // Llegeix el final del node
            //
            reader.NextTag();
        }

        /// <summary>
        /// Procesa un node tpad
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseSPadNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("spad"))
                throw new InvalidDataException("Se esperaba <spad>");

            // Obte els atributs de l'element.
            //
            string name = reader.AttributeAsString("name");
            Point position = ParsePointAttribute("position");
            Size size = ParseSizeAttribute("size");
            Angle rotation = ParseAngleAttribute("rotation");
            Ratio roundness = ParseRatioAttribute("roundness");
            string[] layerNames = reader.AttributeAsStrings("layers");

            // Crea l'element i l'afegeix a la llista
            //
            SmdPadElement pad = new SmdPadElement(name, position, size, rotation, roundness);
            elementList.Add(pad);

            // Asigna les capes a l'element
            //
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), pad);

            // Assigna la senyal 
            //
            if (reader.AttributeExists("signal")) {
                string signalName = reader.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, pad);
                }
            }

            reader.NextTag();
        }

        /// <summary>
        /// Procesa un node 'via'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseViaNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("via"))
                throw new InvalidDataException("Se esperaba <via>");

            // Obte els atributs de l'element
            //
            Point position = ParsePointAttribute("position");
            int size = ParseNumberAttribute("size");
            int drill = ParseNumberAttribute("drill");
            ViaElement.ViaShape shape = ParseEnumAttribute("shape", ViaElement.ViaShape.Circular);
            string[] layerNames = reader.AttributeAsStrings("layers");

            // Crtea l'element i l'afegeix a la llista
            //
            ViaElement via = new ViaElement(position, size, drill, shape);
            elementList.Add(via);

            // Asigna les capes a l'element
            //
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), via);

            // Assigna la senyal 
            //
            if (reader.AttributeExists("signal")) {
                string signalName = reader.AttributeAsString("signal");
                if (signalName != null) {
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, via);
                }
            }

            reader.NextTag();
        }

        /// <summary>
        /// Procesa un node 'hole'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseHoleNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("hole"))
                throw new InvalidDataException("Se esperaba <hole>");

            // Obte els atributs de l'element
            //
            Point position = ParsePointAttribute("position");
            int drill = ParseNumberAttribute("drill");
            string[] layerNames = reader.AttributeAsStrings("layers");

            // Crea l'element i l'afegeix a la llista
            //
            HoleElement hole = new HoleElement(position, drill);
            elementList.Add(hole);

            // Asigna les capes a l'element
            //
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), hole);

            reader.NextTag();
        }

        /// <summary>
        /// Procesa un node 'text'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseTextNode(IList<Element> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!reader.IsStartTag("text"))
                throw new InvalidDataException("Se esperaba <text>");

            // Obte els parametres de l'objecte
            //
            Point position = ParsePointAttribute("position");
            Angle rotation = ParseAngleAttribute("rotation");
            int height = ParseNumberAttribute("height");
            TextAlign align = ParseEnumAttribute("align", TextAlign.TopLeft);
            int thickness = ParseNumberAttribute("thickness");
            string value = reader.AttributeAsString("value");
            string[] layerNames = reader.AttributeAsStrings("layers");

            // Crea l'objecte i l'afegeix al la llista
            //
            TextElement text = new TextElement(position, rotation, height, thickness);
            elementList.Add(text);
            text.Value = value;

            // Asigna les capes a l'element
            //
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), text);

            // Llegeix el final del node
            //
            reader.NextTag();
        }

        /// <summary>
        /// Procesa un atribut de tipus 'PointInt'
        /// </summary>
        /// <param name="name">Nom del atribut.</param>
        /// <returns>El valor de l'atribut.</returns>
        /// 
        private Point ParsePointAttribute(string name) {

            string s = reader.AttributeAsString(name);

            string[] ss = s.Split(',');
            double x = XmlConvert.ToDouble(ss[0]);
            double y = XmlConvert.ToDouble(ss[1]);

            return new Point((int)(x * 1000000.0), (int)(y * 1000000.0));
        }

        /// <summary>
        /// Procesa un atribut de tipus 'SizeInt'.
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <returns>El valor de l'atribut.</returns>
        /// 
        private Size ParseSizeAttribute(string name) {

            string s = reader.AttributeAsString(name);

            string[] ss = s.Split(',');
            double w = XmlConvert.ToDouble(ss[0]);
            double h = XmlConvert.ToDouble(ss[1]);

            return new Size((int)(w * 1000000.0), (int)(h * 1000000.0));
        }

        /// <summary>
        /// Procesa un atribut de tipus 'Ratio'
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <returns>El valor de l'atribut, o Ratio.Zero si no existeix.</returns>
        /// 
        private Ratio ParseRatioAttribute(string name) {

            if (reader.AttributeExists(name)) {
                double v = reader.AttributeAsDouble(name);
                return Ratio.FromPercent((int)(v * 1000.0));
            }
            else
                return Ratio.Zero;
        }

        /// <summary>
        /// Procesa un atribut de tipus 'Angle'
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <returns>El valor de l'atribut. Si no existeix retorna Angle.Zero.</returns>
        /// 
        private Angle ParseAngleAttribute(string name) {

            if (reader.AttributeExists(name)) {
                double v = reader.AttributeAsDouble(name);
                return Angle.FromDegrees((int)(v * 100.0));
            }
            else
                return Angle.Zero;
        }

        /// <summary>
        /// Procesa un atribut de tipus 'Number'
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defValue">El valor per defecte.</param>
        /// <returns>El valor de l'atribut, o el valor per defecte si no existeix.</returns>
        /// 
        private int ParseNumberAttribute(string name, int defValue = 0) {

            if (reader.AttributeExists(name)) {
                double v = reader.AttributeAsDouble(name, defValue);
                return (int)(v * 1000000.0);
            }
            else
                return defValue;
        }

        /// <summary>
        /// Procesa un atribut de tipus 'Color'
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defColor">Valor per defecte.</param>
        /// <returns>El valor de l'atribut, o el valor per defecte si no existeix.</returns>
        /// 
        private Color ParseColorAttribute(string name, Color defColor) {

            if (reader.AttributeExists(name)) {
                string s = reader.AttributeAsString(name);
                return Color.Parse(s);
            }
            else
                return defColor;
        }

        /// <summary>
        /// Procesa un atribut de tipus 'bool'
        /// </summary>
        /// <param name="name">En nom de l'atribut.</param>
        /// <param name="defValue">El valor per defecte.</param>
        /// <returns>El valor de l'atribut, o el valor per defecte si no existeix.</returns>
        /// 
        private bool ParseBoolAttribute(string name, bool defValue) {

            return reader.AttributeAsBoolean(name, defValue);
        }

        /// <summary>
        /// Procesa un atribut de tipus 'enum'
        /// </summary>
        /// <typeparam name="T">El tipus enum</typeparam>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defValue">El valor per defecte.</param>
        /// <returns>El valor de l'atribut, o el valor per defecte si no existeix.</returns>
        /// 
        private T ParseEnumAttribute<T>(string name, T defValue) {

            return reader.AttributeAsEnum(name, defValue);
        }
    }
}
