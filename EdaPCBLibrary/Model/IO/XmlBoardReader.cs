namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

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
        /// <param name="stream">Stream d'entrada.</param>
        /// 
        public XmlBoardReader(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

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
                Point position = partNode.AttributeAsPoint("position");
                double rotation = partNode.AttributeAsDouble("rotation");
                string blockName = partNode.AttributeAsString("block");

                Block block = board.GetBlock(blockName);
                Part part = new Part(block, name, position, rotation, false);
                board.AddPart(part);

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
            LayerFunction function = node.AttributeAsEnum<LayerFunction>("function");
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

            Point startPosition = node.AttributeAsPoint("startPosition");
            Point endPosition = node.AttributeAsPoint("endPosition");
            double thickness = node.AttributeAsDouble("thickness");
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

            Point startPosition = node.AttributeAsPoint("startPosition");
            Point endPosition = node.AttributeAsPoint("endPosition");
            double thickness = node.AttributeAsDouble("thickness");
            double angle = node.AttributeAsDouble("angle");
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

            Point position = node.AttributeAsPoint("position");
            Size size = node.AttributeAsSize("size");
            double rotation = node.AttributeAsDouble("rotation");
            double roundness = node.AttributeAsDouble("roundness");
            double thickness = node.AttributeAsDouble("thickness");

            return new RectangleElement(position, size, rotation, thickness);
        }

        /// <summary>
        /// Procesa un node 'circle'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'CircleElement' obtingut.</returns>
        /// 
        private Element ParseCircleElement(XmlNode node) {

            Point position = node.AttributeAsPoint("position");
            double radius = node.AttributeAsDouble("radius");
            double thickness = node.AttributeAsDouble("thickness");

            return new CircleElement(position, radius, thickness);
        }

        /// <summary>
        /// Procesa un node 'region'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RegiuonElement' obtingut.</returns>
        /// 
        private Element ParseRegionElement(XmlNode node) {

            double thickness = node.AttributeAsDouble("thickness");

            RegionElement region = new RegionElement(thickness);

            foreach (XmlNode segmentNode in node.SelectNodes("segment")) {

                Point position = segmentNode.AttributeAsPoint("position");
                double angle = segmentNode.AttributeAsDouble("angle");

                region.Add(new RegionElement.Segment(position, angle));
            }

            return region;
        }

        private Element ParseTextElement(XmlNode node) {

            Point position = node.AttributeAsPoint("position");
            double rotation = node.AttributeAsDouble("rotation");
            double height = node.AttributeAsDouble("height");
            string value = node.AttributeAsString("value");

            TextElement element = new TextElement(position, rotation, height);
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
            Point position = node.AttributeAsPoint("position");
            double size = node.AttributeAsDouble("size");
            double rotation = node.AttributeAsDouble("rotation");
            double drill = node.AttributeAsDouble("drill");
            ThPadElement.ThPadShape shape = node.AttributeAsEnum<ThPadElement.ThPadShape>("shape");

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
            Point position = node.AttributeAsPoint("position");
            Size size = node.AttributeAsSize("size");
            double rotation = node.AttributeAsDouble("rotation");
            double roundness = node.AttributeAsDouble("roundness");

            return new SmdPadElement(name, position, size, rotation, roundness);
        }

        /// <summary>
        /// Procesa un node 'via'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ViaElement' obtingut.</returns>
        /// 
        private Element ParseViaElement(XmlNode node) {

            Point position = node.AttributeAsPoint("position");
            double size = node.AttributeAsDouble("size");
            double drill = node.AttributeAsDouble("drill");
            ViaElement.ViaShape shape = node.AttributeAsEnum<ViaElement.ViaShape>("shape");

            return new ViaElement(position, size, drill, shape);
        }

        private Element ParseHoleElement(XmlNode node) {

            Point position = node.AttributeAsPoint("position");
            double drill = node.AttributeAsDouble("drill");

            return new HoleElement(position, drill);
        }
    }
}
