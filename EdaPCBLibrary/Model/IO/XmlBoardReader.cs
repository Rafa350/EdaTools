namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Geometry;

    /// <summary>
    /// Clase per la lectura de plaques des d'un stream
    /// </summary>
    public sealed class XmlBoardReader {

        private readonly Stream stream;
        private XmlDocument doc;
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

            this.stream = stream;
        }

        /// <summary>
        /// Llegeix una placa.
        /// </summary>
        /// <returns>La placa.</returns>
        /// 
        public Board Read() {

            board = new Board();

            ReadDocument();
            ProcessLayers();
            ProcessSignals();
            ProcessBlocks();
            ProcessParts();
            ProcessElements();

            return board;
        }

        /// <summary>
        /// Llegeix el document XML
        /// </summary>
        /// 
        private void ReadDocument() {

            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(stream, settings);

            doc = new XmlDocument();
            doc.Load(reader);
        }

        /// <summary>
        /// Procesa les capes.
        /// </summary>
        /// 
        private void ProcessLayers() {

            foreach (XmlNode layerNode in doc.SelectNodes("board/layers/layer")) {
                Layer layer = ParseLayer(layerNode);
                board.AddLayer(layer);
            }
        }

        /// <summary>
        /// Procesa els senyals.
        /// </summary>
        /// 
        private void ProcessSignals() {

            foreach (XmlNode signalNode in doc.SelectNodes("board/signals/signal")) {
                Signal signal = ParseSignal(signalNode);
                board.AddSignal(signal);
            }
        }

        /// <summary>
        /// Procesa els blocs.
        /// </summary>
        /// 
        private void ProcessBlocks() {

            foreach (XmlNode blockNode in doc.SelectNodes("board/blocks/block")) {

                string name = blockNode.AttributeAsString("name");

                Block block = new Block(name);
                board.AddBlock(block);

                foreach (XmlNode elementNode in blockNode.SelectNodes("elements/*")) {

                    Element element = null;

                    switch (elementNode.Name) {
                        case "line":
                            element = ParseLineElement(elementNode);
                            break;

                        case "arc":
                            element = ParseArcElement(elementNode);
                            break;

                        case "rectangle":
                            element = ParseRectangleElement(elementNode);
                            break;

                        case "circle":
                            element = ParseCircleElement(elementNode);
                            break;

                        case "region":
                            element = ParseRegionElement(elementNode);
                            break;

                        case "text":
                            element = ParseTextElement(elementNode);
                            break;

                        case "spad":
                            element = ParseSmdPadElement(elementNode);
                            break;

                        case "tpad":
                            element = ParseThPadElement(elementNode);
                            break;

                        case "hole":
                            element = ParseHoleElement(elementNode);
                            break;
                    }

                    if (element != null) {
                        block.AddElement(element);

                        string[] layerNames = elementNode.AttributeAsStrings("layers");
                        if (layerNames != null)
                            foreach (string layerName in layerNames)
                                board.Place(board.GetLayer(layerName), element);
                    }
                }
            }
        }

        /// <summary>
        /// Procesa els components.
        /// </summary>
        /// 
        private void ProcessParts() {

            foreach (XmlNode partNode in doc.SelectNodes("board/parts/part")) {

                string name = partNode.AttributeAsString("name");
                PointInt position = partNode.AttributeAsPoint("position");
                Angle rotation = partNode.AttributeAsAngle("rotation");
                string blockName = partNode.AttributeAsString("block");

                Block block = board.GetBlock(blockName);
                Part part = new Part(block, name, position, rotation, false);
                board.AddPart(part);

                foreach (XmlNode attributeNode in partNode.SelectNodes("attributes/attribute")) {
                    PartAttribute attr = ParseAttribute(attributeNode);
                    part.AddAttribute(attr);
                }

                Dictionary<string, PadElement> padDictionary = new Dictionary<string, PadElement>();
                foreach (PadElement pad in part.Pads)
                    padDictionary.Add(pad.Name, pad);

                foreach (XmlNode padNode in partNode.SelectNodes("pads/pad")) {

                    string padName = padNode.AttributeAsString("name");
                    string signalName = padNode.AttributeAsString("signal");

                    PadElement pad = padDictionary[padName];
                    Signal signal = board.GetSignal(signalName);
                    board.Connect(signal, pad, part);
                }
            }
        }

        /// <summary>
        /// Procesa els elements.
        /// </summary>
        /// 
        private void ProcessElements() {

            foreach (XmlNode elementNode in doc.SelectNodes("board/elements/*")) {

                Element element = null;

                switch (elementNode.Name) {
                    case "line":
                        element = ParseLineElement(elementNode);
                        break;

                    case "arc":
                        element = ParseArcElement(elementNode);
                        break;

                    case "rectangle":
                        element = ParseCircleElement(elementNode);
                        break;

                    case "circle":
                        element = ParseRectangleElement(elementNode);
                        break;

                    case "region":
                        element = ParseRegionElement(elementNode);
                        break;

                    case "via":
                        element = ParseViaElement(elementNode);
                        break;
                }

                if (element != null) {

                    board.AddElement(element);

                    string[] layerNames = elementNode.AttributeAsStrings("layers");
                    if (layerNames != null)
                        foreach (string layerName in layerNames)
                            board.Place(board.GetLayer(layerName), element);

                    if (element is IConectable) {
                        string signalName = elementNode.AttributeAsString("signal");
                        if (signalName != null) {
                            Signal signal = board.GetSignal(signalName);
                            board.Connect(signal, (IConectable) element);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Procesa un node 'layer'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>El objecte 'Layer' obtingut.</returns>
        /// 
        private Layer ParseLayer(XmlNode node) {

            string name = node.AttributeAsString("name");
            BoardSide side = node.AttributeAsEnum<BoardSide>("side");
            LayerFunction function = node.AttributeAsEnum<LayerFunction>("function", LayerFunction.Unknown);
            Color color = node.AttributeAsColor("color");

            return new Layer(name, side, function, color);
        }

        /// <summary>
        /// Procesa un node 'signal'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Signal' obtingut.</returns>
        /// 
        private Signal ParseSignal(XmlNode node) {

            string name = node.AttributeAsString("name");

            return new Signal(name);
        }

        /// <summary>
        /// Procesa un node 'line'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'LineElement' obtingut.</returns>
        /// 
        private Element ParseLineElement(XmlNode node) {

            PointInt startPosition = node.AttributeAsPoint("startPosition");
            PointInt endPosition = node.AttributeAsPoint("endPosition");
            int thickness = node.AttributeAsDouble("thickness");
            LineElement.LineCapStyle lineCap = node.AttributeAsEnum<LineElement.LineCapStyle>("lineCap");

            return new LineElement(startPosition, endPosition, thickness, lineCap);
        }

        /// <summary>
        /// Procesa un node 'arc'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ArcElement' obtingut.</returns>
        /// 
        private Element ParseArcElement(XmlNode node) {

            PointInt startPosition = node.AttributeAsPoint("startPosition");
            PointInt endPosition = node.AttributeAsPoint("endPosition");
            int thickness = node.AttributeAsDouble("thickness");
            Angle angle = node.AttributeAsAngle("angle");
            LineElement.LineCapStyle lineCap = node.AttributeAsEnum<LineElement.LineCapStyle>("lineCap");

            return new ArcElement(startPosition, endPosition, thickness, angle, lineCap);
        }

        /// <summary>
        /// Procesa un node 'rectangle'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RectanglerElement' obtingut.</returns>
        /// 
        private Element ParseRectangleElement(XmlNode node) {

            PointInt position = node.AttributeAsPoint("position");
            SizeInt size = node.AttributeAsSize("size");
            Angle rotation = node.AttributeAsAngle("rotation");
            int roundness = node.AttributeAsDouble("roundness");
            int thickness = node.AttributeAsDouble("thickness");

            return new RectangleElement(position, size, rotation, thickness);
        }

        /// <summary>
        /// Procesa un node 'circle'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'CircleElement' obtingut.</returns>
        /// 
        private Element ParseCircleElement(XmlNode node) {

            PointInt position = node.AttributeAsPoint("position");
            int radius = node.AttributeAsDouble("radius");
            int thickness = node.AttributeAsDouble("thickness");

            return new CircleElement(position, radius, thickness);
        }

        /// <summary>
        /// Procesa un node 'region'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RegiuonElement' obtingut.</returns>
        /// 
        private Element ParseRegionElement(XmlNode node) {

            int thickness = node.AttributeAsDouble("thickness");

            RegionElement region = new RegionElement(thickness);

            foreach (XmlNode segmentNode in node.SelectNodes("segment")) {

                PointInt position = segmentNode.AttributeAsPoint("position");
                Angle angle = segmentNode.AttributeAsAngle("angle");

                region.Add(new RegionElement.Segment(position, angle));
            }

            return region;
        }

        /// <summary>
        /// Procesa un node 'text'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'TextElement' obtingut.</returns>
        /// 
        private Element ParseTextElement(XmlNode node) {

            PointInt position = node.AttributeAsPoint("position");
            Angle rotation = node.AttributeAsAngle("rotation");
            int height = node.AttributeAsDouble("height");
            TextAlign align = node.AttributeAsEnum<TextAlign>("align", TextAlign.TopLeft);
            int thickness = node.AttributeAsDouble("thickness");
            string value = node.AttributeAsString("value");

            TextElement element = new TextElement(position, rotation, height, thickness);
            element.Value = value;
            return element;
        }

        /// <summary>
        /// Procesa un node 'tpad'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ThPadElement' obtingut.</returns>
        /// 
        private Element ParseThPadElement(XmlNode node) {

            string name = node.AttributeAsString("name");
            PointInt position = node.AttributeAsPoint("position");
            int size = node.AttributeAsDouble("size");
            Angle rotation = node.AttributeAsAngle("rotation");
            int drill = node.AttributeAsDouble("drill");
            ThPadElement.ThPadShape shape = node.AttributeAsEnum<ThPadElement.ThPadShape>("shape", ThPadElement.ThPadShape.Circular);

            return new ThPadElement(name, position, rotation, size, shape, drill);
        }

        /// <summary>
        /// Procesa un node 'spad'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'SmdPadElement' obtingut.</returns>
        /// 
        private Element ParseSmdPadElement(XmlNode node) {

            string name = node.AttributeAsString("name");
            PointInt position = node.AttributeAsPoint("position");
            SizeInt size = node.AttributeAsSize("size");

            Angle rotation = Angle.Zero;
            if (node.AttributeExists("rotation"))
                rotation = ParseAngle(node.AttributeAsString("rotation"));

            int roundness = 0;
            if (node.AttributeExists("roundness"))
                roundness = ParsePercent(node.AttributeAsString("roundness"));

            return new SmdPadElement(name, position, size, rotation, roundness);
        }

        /// <summary>
        /// Procesa un node 'via'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ViaElement' obtingut.</returns>
        /// 
        private Element ParseViaElement(XmlNode node) {

            PointInt position = node.AttributeAsPoint("position");
            int size = node.AttributeAsDouble("size");
            int drill = node.AttributeAsDouble("drill");
            ViaElement.ViaShape shape = node.AttributeAsEnum<ViaElement.ViaShape>("shape", ViaElement.ViaShape.Circular);

            return new ViaElement(position, size, drill, shape);
        }

        /// <summary>
        /// Procesa un node 'hole'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'HoleElement' obtingut.</returns>
        /// 
        private Element ParseHoleElement(XmlNode node) {

            PointInt position = node.AttributeAsPoint("position");
            int drill = node.AttributeAsDouble("drill");

            return new HoleElement(position, drill);
        }

        /// <summary>
        /// Procesa un node 'attribute'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>'L'objecte 'PartAttribute' obtingut.</returns>
        /// 
        private PartAttribute ParseAttribute(XmlNode node) {

            string name = node.AttributeAsString("name");
            string value = node.AttributeAsString("value");
            bool isVisible = node.AttributeAsBoolean("visible", false);

            PartAttribute attribute = new PartAttribute(name, value, isVisible);

            if (node.AttributeExists("position"))
                attribute.Position = node.AttributeAsPoint("position");

            if (node.AttributeExists("rotation"))
                attribute.Rotation = node.AttributeAsAngle("rotation");

            if (node.AttributeExists("align"))
                attribute.Align = node.AttributeAsEnum<TextAlign>("align", TextAlign.TopLeft);

            return attribute;
        }

        private static int ParsePercent(string txt) {

            return (int)(XmlConvert.ToDouble(txt) * 1000.0);
        }

        private static Angle ParseAngle(string txt) {

            return Angle.FromDegrees((int)(XmlConvert.ToDouble(txt) * 100.0));
        }
    }
}
