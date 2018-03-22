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

            stream.Position = 0;

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
                Layer layer = CreateLayer(layerNode);
                board.AddLayer(layer);
            }
        }

        /// <summary>
        /// Procesa els senyals.
        /// </summary>
        /// 
        private void ProcessSignals() {

            foreach (XmlNode signalNode in doc.SelectNodes("board/signals/signal")) {
                Signal signal = CreateSignal(signalNode);
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
                            element = CreateLineElement(elementNode);
                            break;

                        case "arc":
                            element = CreateArcElement(elementNode);
                            break;

                        case "rectangle":
                            element = CreateRectangleElement(elementNode);
                            break;

                        case "circle":
                            element = CreateCircleElement(elementNode);
                            break;

                        case "region":
                            element = CreateRegionElement(elementNode);
                            break;

                        case "text":
                            element = CreateTextElement(elementNode);
                            break;

                        case "spad":
                            element = CreateSmdPadElement(elementNode);
                            break;

                        case "tpad":
                            element = CreateThPadElement(elementNode);
                            break;

                        case "hole":
                            element = CreateHoleElement(elementNode);
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

                PointInt position = ParsePoint(partNode.AttributeAsString("position"));
                Angle rotation = ParseAngle(partNode.AttributeAsString("rotation"));
                BoardSide side = partNode.AttributeAsEnum<BoardSide>("side", BoardSide.Top);

                string blockName = partNode.AttributeAsString("block");

                Block block = board.GetBlock(blockName);
                Part part = new Part(block, name, position, rotation, side);
                board.AddPart(part);

                foreach (XmlNode attributeNode in partNode.SelectNodes("attributes/attribute")) {
                    PartAttribute attr = CreatePartAttribute(attributeNode);
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
                        element = CreateLineElement(elementNode);
                        break;

                    case "arc":
                        element = CreateArcElement(elementNode);
                        break;

                    case "rectangle":
                        element = CreateCircleElement(elementNode);
                        break;

                    case "circle":
                        element = CreateRectangleElement(elementNode);
                        break;

                    case "region":
                        element = CreateRegionElement(elementNode);
                        break;

                    case "via":
                        element = CreateViaElement(elementNode);
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
        private Layer CreateLayer(XmlNode node) {

            string name = node.AttributeAsString("name");
            BoardSide side = node.AttributeAsEnum<BoardSide>("side");
            LayerFunction function = node.AttributeAsEnum<LayerFunction>("function", LayerFunction.Unknown);
            Color color = ParseColor(node.AttributeAsString("color"), Colors.White);

            return new Layer(name, side, function, color);
        }

        /// <summary>
        /// Procesa un node 'signal'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'Signal' obtingut.</returns>
        /// 
        private Signal CreateSignal(XmlNode node) {

            string name = node.AttributeAsString("name");

            return new Signal(name);
        }

        /// <summary>
        /// Procesa un node 'line'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'LineElement' obtingut.</returns>
        /// 
        private Element CreateLineElement(XmlNode node) {

            PointInt startPosition = ParsePoint(node.AttributeAsString("startPosition"));
            PointInt endPosition = ParsePoint(node.AttributeAsString("endPosition"));
            int thickness = ParseNumber(node.AttributeAsString("thickness"));
            LineElement.LineCapStyle lineCap = node.AttributeAsEnum<LineElement.LineCapStyle>("lineCap");

            return new LineElement(startPosition, endPosition, thickness, lineCap);
        }

        /// <summary>
        /// Procesa un node 'arc'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ArcElement' obtingut.</returns>
        /// 
        private Element CreateArcElement(XmlNode node) {

            PointInt startPosition = ParsePoint(node.AttributeAsString("startPosition"));
            PointInt endPosition = ParsePoint(node.AttributeAsString("endPosition"));
            int thickness = ParseNumber(node.AttributeAsString("thickness"));
            Angle angle = ParseAngle(node.AttributeAsString("angle"));

            LineElement.LineCapStyle lineCap = node.AttributeAsEnum<LineElement.LineCapStyle>("lineCap");

            return new ArcElement(startPosition, endPosition, thickness, angle, lineCap);
        }

        /// <summary>
        /// Procesa un node 'rectangle'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RectanglerElement' obtingut.</returns>
        /// 
        private Element CreateRectangleElement(XmlNode node) {

            PointInt position = ParsePoint(node.AttributeAsString("position"));
            SizeInt size = ParseSize(node.AttributeAsString("size"));
            Angle rotation = ParseAngle(node.AttributeAsString("rotation"));
            int thickness = ParseNumber(node.AttributeAsString("thickness"));

            return new RectangleElement(position, size, rotation, thickness);
        }

        /// <summary>
        /// Procesa un node 'circle'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'CircleElement' obtingut.</returns>
        /// 
        private Element CreateCircleElement(XmlNode node) {

            PointInt position = ParsePoint(node.AttributeAsString("position"));
            int radius = ParseNumber(node.AttributeAsString("radius"));
            int thickness = ParseNumber(node.AttributeAsString("thickness"));

            return new CircleElement(position, radius, thickness);
        }

        /// <summary>
        /// Procesa un node 'region'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'RegiuonElement' obtingut.</returns>
        /// 
        private Element CreateRegionElement(XmlNode node) {

            int thickness = ParseNumber(node.AttributeAsString("thickness"));

            RegionElement region = new RegionElement(thickness);

            foreach (XmlNode segmentNode in node.SelectNodes("segment")) {

                PointInt position = ParsePoint(segmentNode.AttributeAsString("position"));
                Angle angle = ParseAngle(segmentNode.AttributeAsString("angle"));

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
        private Element CreateTextElement(XmlNode node) {

            PointInt position = ParsePoint(node.AttributeAsString("position"));
            Angle rotation = ParseAngle(node.AttributeAsString("rotation"));
            int height = ParseNumber(node.AttributeAsString("height"));
            TextAlign align = node.AttributeAsEnum<TextAlign>("align", TextAlign.TopLeft);
            int thickness = ParseNumber(node.AttributeAsString("thickness"));
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
        private Element CreateThPadElement(XmlNode node) {

            string name = node.AttributeAsString("name");
            PointInt position = ParsePoint(node.AttributeAsString("position"));
            int size = ParseNumber(node.AttributeAsString("size"));
            Angle rotation = ParseAngle(node.AttributeAsString("rotation"));
            int drill = ParseNumber(node.AttributeAsString("drill"));
            ThPadElement.ThPadShape shape = node.AttributeAsEnum<ThPadElement.ThPadShape>("shape", ThPadElement.ThPadShape.Circular);

            return new ThPadElement(name, position, rotation, size, shape, drill);
        }

        /// <summary>
        /// Procesa un node 'spad'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'SmdPadElement' obtingut.</returns>
        /// 
        private Element CreateSmdPadElement(XmlNode node) {

            string name = node.AttributeAsString("name");
            PointInt position = ParsePoint(node.AttributeAsString("position"));
            SizeInt size = ParseSize(node.AttributeAsString("size"));
            Angle rotation = ParseAngle(node.AttributeAsString("rotation"));
            Ratio roundness = ParseRatio(node.AttributeAsString("roundness"));

            return new SmdPadElement(name, position, size, rotation, roundness);
        }

        /// <summary>
        /// Procesa un node 'via'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'ViaElement' obtingut.</returns>
        /// 
        private Element CreateViaElement(XmlNode node) {

            PointInt position = ParsePoint(node.AttributeAsString("position"));
            int size = ParseNumber(node.AttributeAsString("size"));
            int drill = ParseNumber(node.AttributeAsString("drill"));
            ViaElement.ViaShape shape = node.AttributeAsEnum<ViaElement.ViaShape>("shape", ViaElement.ViaShape.Circular);

            return new ViaElement(position, size, drill, shape);
        }

        /// <summary>
        /// Procesa un node 'hole'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'objecte 'HoleElement' obtingut.</returns>
        /// 
        private Element CreateHoleElement(XmlNode node) {

            PointInt position = ParsePoint(node.AttributeAsString("position"));
            int drill = ParseNumber(node.AttributeAsString("drill"));

            return new HoleElement(position, drill);
        }

        /// <summary>
        /// Procesa un node 'attribute'
        /// </summary>
        /// <param name="node">El node a procesar.</param>
        /// <returns>'L'objecte 'PartAttribute' obtingut.</returns>
        /// 
        private PartAttribute CreatePartAttribute(XmlNode node) {

            string name = node.AttributeAsString("name");
            string value = node.AttributeAsString("value");
            bool isVisible = node.AttributeAsBoolean("visible", false);

            PartAttribute attribute = new PartAttribute(name, value, isVisible);

            if (node.AttributeExists("position"))
                attribute.Position = ParsePoint(node.AttributeAsString("position"));

            if (node.AttributeExists("rotation"))
                attribute.Rotation = ParseAngle(node.AttributeAsString("rotation"));

            if (node.AttributeExists("height"))
                attribute.Height = ParseNumber(node.AttributeAsString("height"));

            if (node.AttributeExists("align"))
                attribute.Align = node.AttributeAsEnum<TextAlign>("align", TextAlign.TopLeft);

            return attribute;
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
