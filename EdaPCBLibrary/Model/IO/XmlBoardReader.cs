namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// Clase per la lectura de plaques des d'un stream
    /// </summary>
    /// 
    public sealed class XmlBoardReader {

        private readonly XmlTagReader reader;
        private Board board;

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

            reader.NextTag();
            return ParseBoardNode();
        }

        /// <summary>
        /// Procesa el node 'board'
        /// </summary>
        /// <returns>La placa obtinguda</returns>
        /// 
        private Board ParseBoardNode() {

            if ((reader.TagName != "board") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <board>");

            board = new Board();

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

            return board;
        }

        /// <summary>
        /// Procesa el node 'layers'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseLayersNode(Board board) {

            if ((reader.TagName != "layers") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <layers>");

            while (reader.NextTag() && (reader.TagName == "layer") && reader.IsStart)
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
            if ((reader.TagName != "layer") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <layer>");

            // Obte els atributs de la capa
            //
            string name = reader.AttributeAsString("name");
            BoardSide side = reader.AttributeAsEnum("side", BoardSide.Unknown);
            LayerFunction function = reader.AttributeAsEnum("function", LayerFunction.Unknown);
            Color color = ParseColor(reader.AttributeAsString("color"), Colors.Wheat);
            bool isVisible = isVisible = reader.AttributeAsBoolean("isVisible", true);

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
            if ((reader.TagName != "layerPairs") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <layerPairs>");

            while (reader.NextTag() && (reader.TagName == "pair") && reader.IsStart)
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
            if ((reader.TagName != "pair") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <pair>");

            // Obte els atributs del parell de capes
            //
            string name1 = reader.Attributes["layer1"];
            string name2 = reader.Attributes["layer2"];

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
            if ((reader.TagName != "signals") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <signals>");

            while (reader.NextTag() && (reader.TagName == "signal") && reader.IsStart)
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
            if ((reader.TagName != "signal") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <signal>");

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
            if ((reader.TagName != "blocks") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <blocks>");

            while (reader.NextTag() && (reader.TagName == "block") && reader.IsStart)
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
            if ((reader.TagName != "block") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <block>");

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
            if ((reader.TagName != "elements") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <elements>");

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
                        throw new InvalidOperationException("Se esperaba <line>, <arc>, <rectangle>, <circle>, <tpad>, <spad>, <via>, <text>, <region> o <hole>");
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
            if ((reader.TagName != "elements") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <elements>");

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
                        throw new InvalidOperationException("Se esperaba <line>, <arc>, <rectangle>, <circle>, <tpad>, <spad>, <via>, <text>, <region> o <hole>");
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
            if ((reader.TagName != "parts") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <parts>");

            while (reader.NextTag() && (reader.TagName == "part") && reader.IsStart)
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
            if ((reader.TagName != "part") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <part>");

            // Obte els atributs de l'objecte
            //
            string name = reader.AttributeAsString("name");
            PointInt position = ParsePoint(reader.AttributeAsString("position"));
            Angle rotation = ParseAngle(reader.AttributeAsString("rotation"));
            BoardSide side = reader.AttributeAsEnum<BoardSide>("side", BoardSide.Top);
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
                        throw new InvalidOperationException("Se esperaba <pads> o <attributes>");
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
            if ((reader.TagName != "attributes") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <attributes>");

            while (reader.NextTag() && (reader.TagName == "attribute") && reader.IsStart)
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
            if ((reader.TagName != "attribute") || !reader.IsStart)
               throw new InvalidOperationException("Se esperaba <attribute>");

            // Obte els atributs de l'objecte
            //
            string name = reader.AttributeAsString("name");
            string value = reader.AttributeAsString("value");
            bool isVisible = reader.AttributeAsBoolean("visible", false);

            // Crea l'objecte i l'afegeix a la llista
            //
            PartAttribute attribute = new PartAttribute(name, value, isVisible);
            part.AddAttribute(attribute);

            if (reader.AttributeExists("position"))
                attribute.Position = ParsePoint(reader.AttributeAsString("position"));

            if (reader.AttributeExists("rotation"))
                attribute.Rotation = ParseAngle(reader.AttributeAsString("rotation"));

            if (reader.AttributeExists("height"))
                attribute.Height = ParseNumber(reader.AttributeAsString("height"));

            if (reader.AttributeExists("align"))
                attribute.Align = reader.AttributeAsEnum("align", TextAlign.TopLeft);

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
            if ((reader.TagName != "pads") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <attributes>");

            while (reader.NextTag() && (reader.TagName == "pad") && reader.IsStart)
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
            if ((reader.TagName != "pad") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <pad>");

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
            if ((reader.TagName != "line") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <line>");

            // Obte els atributs del element
            //
            PointInt startPosition = ParsePoint(reader.AttributeAsString("startPosition"));
            PointInt endPosition = ParsePoint(reader.AttributeAsString("endPosition"));
            int thickness = ParseNumber(reader.AttributeAsString("thickness"));
            LineElement.LineCapStyle lineCap = reader.AttributeAsEnum("lineCap", LineElement.LineCapStyle.Round);
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
            if ((reader.TagName != "arc") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <arc>");

            // Obte els atributs de l'element
            //
            PointInt startPosition = ParsePoint(reader.AttributeAsString("startPosition"));
            PointInt endPosition = ParsePoint(reader.AttributeAsString("endPosition"));
            int thickness = ParseNumber(reader.AttributeAsString("thickness"));
            Angle angle = ParseAngle(reader.AttributeAsString("angle"));
            string[] layerNames = reader.AttributeAsStrings("layers");
            LineElement.LineCapStyle lineCap = reader.AttributeAsEnum("lineCap", LineElement.LineCapStyle.Round);

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
            if ((reader.TagName != "rectangle") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <rectangle>");

            // Obte els atributs de l'element.
            //
            PointInt position = ParsePoint(reader.AttributeAsString("position"));
            SizeInt size = ParseSize(reader.AttributeAsString("size"));
            Angle rotation = Angle.Zero;
            if (reader.AttributeExists("rotation"))
                ParseAngle(reader.AttributeAsString("rotation"));
            int thickness = 0;
            if (reader.AttributeExists("thickness"))
                thickness = ParseNumber(reader.AttributeAsString("thickness"));
            bool filled = thickness == 0;
            if (reader.AttributeExists("filled"))
                filled = reader.AttributeAsBoolean("filled");
            Ratio roundness = Ratio.Zero;
            if (reader.AttributeExists("roundness"))
                roundness = ParseRatio(reader.AttributeAsString("roundness"));
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
            if ((reader.TagName != "circle") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <circle>");

            // Obte els atributs de l'element.
            //
            PointInt position = ParsePoint(reader.AttributeAsString("position"));
            int radius = ParseNumber(reader.AttributeAsString("radius"));
            int thickness = ParseNumber(reader.AttributeAsString("thickness"));
            bool filled = thickness == 0;
            if (reader.AttributeExists("filled"))
                filled = reader.AttributeAsBoolean("filled");
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
            if ((reader.TagName != "region") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <region>");

            // Obte els atributs de l'element
            //
            int thickness = ParseNumber(reader.AttributeAsString("thickness"));
            bool filled = thickness == 0;
            if (reader.AttributeExists("filled"))
                filled = reader.AttributeAsBoolean("filled");
            int clearance = 0;
            if (reader.AttributeExists("clearance"))
                clearance = ParseNumber(reader.AttributeAsString("clearance"));
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
            if ((reader.TagName != "segment") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <segment>");

            // Obte els atributs del segment
            //
            PointInt position = ParsePoint(reader.AttributeAsString("position"));
            Angle angle = ParseAngle(reader.AttributeAsString("angle"));

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
            if ((reader.TagName != "tpad") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <tpad>");

            // Obte els atributs de l'element
            //
            string name = reader.AttributeAsString("name");
            PointInt position = ParsePoint(reader.AttributeAsString("position"));
            int size = ParseNumber(reader.AttributeAsString("size"));
            Angle rotation = ParseAngle(reader.AttributeAsString("rotation"));
            int drill = ParseNumber(reader.AttributeAsString("drill"));
            ThPadElement.ThPadShape shape = reader.AttributeAsEnum("shape", ThPadElement.ThPadShape.Circular);
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
            if ((reader.TagName != "spad") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <spad>");

            // Obte els atributs de l'element.
            //
            string name = reader.AttributeAsString("name");
            PointInt position = ParsePoint(reader.AttributeAsString("position"));
            SizeInt size = ParseSize(reader.AttributeAsString("size"));
            Angle rotation = ParseAngle(reader.AttributeAsString("rotation"));
            Ratio roundness = ParseRatio(reader.AttributeAsString("roundness"));
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
            if ((reader.TagName != "via") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <via>");

            // Obte els atributs de l'element
            //
            PointInt position = ParsePoint(reader.AttributeAsString("position"));
            int size = ParseNumber(reader.AttributeAsString("size"));
            int drill = ParseNumber(reader.AttributeAsString("drill"));
            ViaElement.ViaShape shape = reader.AttributeAsEnum<ViaElement.ViaShape>("shape", ViaElement.ViaShape.Circular);
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
            if ((reader.TagName != "hole") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <hole>");

            // Obte els atributs de l'element
            //
            PointInt position = ParsePoint(reader.AttributeAsString("position"));
            int drill = ParseNumber(reader.AttributeAsString("drill"));
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
            if ((reader.TagName != "text") || !reader.IsStart)
                throw new InvalidOperationException("Se esperaba <text>");

            // Obte els parametres de l'objecte
            //
            PointInt position = ParsePoint(reader.AttributeAsString("position"));
            Angle rotation = ParseAngle(reader.AttributeAsString("rotation"));
            int height = ParseNumber(reader.AttributeAsString("height"));
            TextAlign align = reader.AttributeAsEnum<TextAlign>("align", TextAlign.TopLeft);
            int thickness = ParseNumber(reader.AttributeAsString("thickness"));
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
        /// Convereteix un string a PointInt
        /// </summary>
        /// <param name="s">La string a convertir.</param>
        /// <returns>El resultat de la converssio.</returns>
        /// 
        private static PointInt ParsePoint(string s) {

            string[] ss = s.Split(',');
            double x = XmlConvert.ToDouble(ss[0]);
            double y = XmlConvert.ToDouble(ss[1]);

            return new PointInt((int)(x * 1000000.0), (int)(y * 1000000.0));
        }

        /// <summary>
        /// Converteix una string a SizeInt.
        /// </summary>
        /// <param name="s">La string a convertir.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        private static SizeInt ParseSize(string s) {

            string[] ss = s.Split(',');
            double w = XmlConvert.ToDouble(ss[0]);
            double h = XmlConvert.ToDouble(ss[1]);

            return new SizeInt((int)(w * 1000000.0), (int)(h * 1000000.0));
        }

        /// <summary>
        /// Converteix una string a Ratio
        /// </summary>
        /// <param name="s">La string a convertir.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        private static Ratio ParseRatio(string s) {

            if (String.IsNullOrEmpty(s))
                return Ratio.Zero;
            else {
                double v = XmlConvert.ToDouble(s);
                return Ratio.FromPercent((int)(v * 1000.0));
            }
        }

        /// <summary>
        /// Converteix una string a Angle
        /// </summary>
        /// <param name="s">La string a convertir</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        private static Angle ParseAngle(string s) {

            if (String.IsNullOrEmpty(s))
                return Angle.Zero;
            else {
                double v = XmlConvert.ToDouble(s);
                return Angle.FromDegrees((int)(v * 100.0));
            }
        }

        /// <summary>
        /// Converteix una string a int
        /// </summary>
        /// <param name="s">La string a convertir.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        private static int ParseNumber(string s) {

            if (String.IsNullOrEmpty(s))
                return 0;
            else {
                double v = XmlConvert.ToDouble(s);
                return (int)(v * 1000000.0);
            }
        }

        /// <summary>
        /// Converteix un string a Color
        /// </summary>
        /// <param name="s">La string a conversit.</param>
        /// <param name="defColor">Valor per defecte.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        private static Color ParseColor(string s, Color defColor) {

            if (String.IsNullOrEmpty(s))
                return defColor;
            else {
                string[] ss = s.Split(',');
                byte a = XmlConvert.ToByte(ss[0]);
                byte r = XmlConvert.ToByte(ss[1]);
                byte g = XmlConvert.ToByte(ss[2]);
                byte b = XmlConvert.ToByte(ss[3]);

                return Color.FromArgb(a, r, g, b);
            }
        }
    }
}
