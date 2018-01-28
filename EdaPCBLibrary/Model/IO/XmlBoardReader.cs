namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class XmlBoardReader {

        private readonly Stream stream;
        private XmlDocument doc;
        private Board board;

        public XmlBoardReader(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            this.stream = stream;
        }

        public Board Read() {

            board = new Board();

            ReadDocument();
            ProcessLayers();
            ProcessSignals();
            ProcessBlocks();

            return board;
        }

        private void ReadDocument() {

            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(stream, settings);

            doc = new XmlDocument();
            doc.Load(reader);
        }

        private void ProcessLayers() {

            foreach (XmlNode layerNode in doc.SelectNodes("board/layers/layer")) {
                Layer layer = ParseLayer(layerNode);
                board.AddLayer(layer);
            }
        }

        private void ProcessSignals() {

            foreach (XmlNode signalNode in doc.SelectNodes("board/signals/signal")) {
                Signal signal = ParseSignal(signalNode);
                board.AddSignal(signal);
            }
        }

        private void ProcessBlocks() {

            foreach (XmlNode blockNode in doc.SelectNodes("board/blocks/block")) {

                string name = blockNode.AttributeAsString("name");

                List<Element> elements = new List<Element>();
                foreach (XmlNode elementNode in blockNode.SelectNodes("elements/*")) {

                    switch (elementNode.Name) {
                        case "line":
                            elements.Add(ParseLineElement(elementNode));
                            break;

                        case "arc":
                            elements.Add(ParseArcElement(elementNode));
                            break;

                        case "rectangle":
                            elements.Add(ParseRectangleElement(elementNode));
                            break;

                        case "circle":
                            elements.Add(ParseCircleElement(elementNode));
                            break;

                        case "region":
                            elements.Add(ParseRegionElement(elementNode));
                            break;

                        case "text":
                            elements.Add(ParseTextElement(elementNode));
                            break;

                        case "spad":
                            elements.Add(ParseSmdPadElement(elementNode));
                            break;

                        case "tpad":
                            elements.Add(ParseThPadElement(elementNode));
                            break;

                        case "hole":
                            elements.Add(ParseHoleElement(elementNode));
                            break;
                    }
                }

                Block block = new Block(name, elements);
                board.AddBlock(block);
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

            Layer layer = new Layer(name, side, function, color);
            return layer;
        }

        /// <summary>
        /// Procesa un node 'signal'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Signal' obtingut.</returns>
        /// 
        private Signal ParseSignal(XmlNode node) {

            string name = node.AttributeAsString("name");

            Signal signal = new Signal(name);
            return signal;
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
            string[] layerNames = node.AttributeAsStrings("layers");

            LineElement element = new LineElement(startPosition, endPosition, thickness, lineCap);
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), element);

            return element;
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
            string[] layerNames = node.AttributeAsStrings("layers");

            ArcElement element = new ArcElement(startPosition, endPosition, thickness, angle, lineCap);
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), element);

            return element;
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
            string[] layerNames = node.AttributeAsStrings("layers");

            RectangleElement element = new RectangleElement(position, size, rotation, thickness);
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), element);

            return element;
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
            string[] layerNames = node.AttributeAsStrings("layers");

            CircleElement element = new CircleElement(position, radius, thickness);
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), element);

            return element;
        }

        private Element ParseRegionElement(XmlNode node) {

            RegionElement element = new RegionElement();

            return element;
        }

        private Element ParseTextElement(XmlNode node) {

            Point position = node.AttributeAsPoint("position");
            double rotation = node.AttributeAsDouble("rotation");
            double height = node.AttributeAsDouble("height");
            string value = node.AttributeAsString("value");
            string[] layerNames = node.AttributeAsStrings("layers");

            TextElement element = new TextElement(position, rotation, height);
            element.Value = value;
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), element);

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
            string[] layerNames = node.AttributeAsStrings("layers");

            ThPadElement element = new ThPadElement(name, position, rotation, size, shape, drill);
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), element);

            return element;
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
            string[] layerNames = node.AttributeAsStrings("layers");

            SmdPadElement element = new SmdPadElement(name, position, size, rotation, roundness);
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), element);

            return element;
        }

        private Element ParseHoleElement(XmlNode node) {

            Point position = node.AttributeAsPoint("position");
            double drill = node.AttributeAsDouble("drill");
            string[] layerNames = node.AttributeAsStrings("layers");

            HoleElement element = new HoleElement(position, drill);
            if (layerNames != null)
                foreach (string layerName in layerNames)
                    board.Place(board.GetLayer(layerName), element);

            return element;
        }
    }
}
