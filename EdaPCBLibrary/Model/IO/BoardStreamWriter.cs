namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Pcb.Model.BoardElements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using MikroPic.EdaTools.v1.Xml;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Clase per la escriptura de plaques en un stream.
    /// </summary>
    /// 
    public sealed class BoardStreamWriter {

        private readonly Stream stream;

        private class Visitor : DefaultVisitor {

            private readonly XmlWriterAdapter wr;
            private readonly CultureInfo ci = CultureInfo.InvariantCulture;
            private readonly Board board;
            private Part currentPart = null;

            /// <summary>
            /// Constructor del objecte. Visita els objectes d'una placa,
            /// per generar el stream de sortida.
            /// </summary>
            /// <param name="board">La placa a visitar.</param>
            /// <param name="wr">Objecte per escriure el stream de sortida.</param>
            /// 
            public Visitor(Board board, XmlWriterAdapter wr) {

                this.board = board;
                this.wr = wr;
            }

            /// <summary>
            /// Executa el visitador.
            /// </summary>
            /// 
            public override void Run() {

                board.AcceptVisitor(this);
            }

            /// <summary>
            /// Visita un element de tipus 'LineElement'
            /// </summary>
            /// <param name="line">L'objecte a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                wr.WriteStartElement("line");

                wr.WriteAttribute("layers", line.LayerSet);
                wr.WriteAttribute("startPosition", line.StartPosition);
                wr.WriteAttribute("endPosition", line.EndPosition);
                if (line.Thickness > 0)
                    wr.WriteAttribute("thickness", FormatNumber(line.Thickness));
                if (line.LineCap != LineElement.LineCapStyle.Round)
                    wr.WriteAttribute("lineCap", line.LineCap);

                Signal signal = board.GetSignal(line, currentPart, false);
                if (signal != null)
                    wr.WriteAttribute("signal", signal.Name);

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'ArcElement'.
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                wr.WriteStartElement("arc");

                wr.WriteAttribute("layers", arc.LayerSet);
                wr.WriteAttribute("startPosition", arc.StartPosition);
                wr.WriteAttribute("endPosition", arc.EndPosition);
                wr.WriteAttribute("angle", arc.Angle);
                if (arc.Thickness > 0)
                    wr.WriteAttribute("thickness", FormatNumber(arc.Thickness));
                if (arc.LineCap != LineElement.LineCapStyle.Round)
                    wr.WriteAttribute("lineCap", arc.LineCap);

                Signal signal = board.GetSignal(arc, currentPart, false);
                if (signal != null)
                    wr.WriteAttribute("signal", signal.Name);

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'RectangleElement'.
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                wr.WriteStartElement("rectangle");

                wr.WriteAttribute("layers", rectangle.LayerSet);
                wr.WriteAttribute("position", rectangle.Position);
                wr.WriteAttribute("size", rectangle.Size);
                if (!rectangle.Rotation.IsZero)
                    wr.WriteAttribute("rotation", rectangle.Rotation);
                if (rectangle.Thickness > 0) {
                    wr.WriteAttribute("thickness", FormatNumber(rectangle.Thickness));
                    if (rectangle.Filled)
                        wr.WriteAttribute("filled", "true");
                }

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'CircleElement'
            /// </summary>
            /// <param name="circle">L'element a visitar.</param>
            /// 
            public override void Visit(CircleElement circle) {

                wr.WriteStartElement("circle");

                wr.WriteAttribute("layers", circle.LayerSet);
                wr.WriteAttribute("position", circle.Position);
                wr.WriteAttribute("radius", FormatNumber(circle.Radius));
                if (circle.Thickness > 0) {
                    wr.WriteAttribute("thickness", FormatNumber(circle.Thickness));
                    if (circle.Filled)
                        wr.WriteAttribute("filled", "true");
                }

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'TextElement'
            /// </summary>
            /// <param name="text">L'element a visitar</param>
            /// 
            public override void Visit(TextElement text) {

                wr.WriteStartElement("text");

                wr.WriteAttribute("layers", text.LayerSet);
                wr.WriteAttribute("position", text.Position);
                if (!text.Rotation.IsZero)
                    wr.WriteAttribute("rotation", text.Rotation);
                wr.WriteAttribute("height", FormatNumber(text.Height));
                wr.WriteAttribute("thickness", FormatNumber(text.Thickness));
                if (text.HorizontalAlign != HorizontalTextAlign.Left)
                    wr.WriteAttribute("horizontalAlign", text.HorizontalAlign);
                if (text.VerticalAlign != VerticalTextAlign.Bottom)
                    wr.WriteAttribute("verticalAlign", text.VerticalAlign);
                if (!String.IsNullOrEmpty(text.Value))
                    wr.WriteAttribute("value", text.Value);

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'HoleElement'.
            /// </summary>
            /// <param name="hole">L'element a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                wr.WriteStartElement("hole");

                wr.WriteAttribute("layers", hole.LayerSet);
                wr.WriteAttribute("position", hole.Position);
                wr.WriteAttribute("drill", FormatNumber(hole.Drill));

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'SmdPadElement'
            /// </summary>
            /// <param name="pad">L'element a visitar-</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                wr.WriteStartElement("spad");

                wr.WriteAttribute("name", pad.Name);
                wr.WriteAttribute("layers", pad.LayerSet);
                wr.WriteAttribute("position", pad.Position);
                if (!pad.Rotation.IsZero)
                    wr.WriteAttribute("rotation", pad.Rotation);
                wr.WriteAttribute("size", pad.Size);
                if (!pad.Roundness.IsZero)
                    wr.WriteAttribute("roundness", pad.Roundness);

                Signal signal = board.GetSignal(pad, currentPart, false);
                if (signal != null)
                    wr.WriteAttribute("signal", signal.Name);

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'ThPadElement'
            /// </summary>
            /// <param name="pad">L'element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                wr.WriteStartElement("tpad");

                wr.WriteAttribute("name", pad.Name);
                wr.WriteAttribute("layers", pad.LayerSet);
                wr.WriteAttribute("position", pad.Position);
                if (!pad.Rotation.IsZero)
                    wr.WriteAttribute("rotation", pad.Rotation);
                wr.WriteAttribute("size", FormatNumber(pad.TopSize));
                wr.WriteAttribute("drill", FormatNumber(pad.Drill));
                if (pad.Shape != ThPadElement.ThPadShape.Circle)
                    wr.WriteAttribute("shape", pad.Shape);

                Signal signal = board.GetSignal(pad, currentPart, false);
                if (signal != null)
                    wr.WriteAttribute("signal", signal.Name);

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'PartAttribute'
            /// </summary>
            /// <param name="attr">L'objecte a visitar.</param>
            /// 
            public override void Visit(PartAttribute attr) {

                wr.WriteStartElement("attribute");

                wr.WriteAttribute("name", attr.Name);
                wr.WriteAttribute("value", attr.Value);
                if (!attr.IsVisible)
                    wr.WriteAttribute("visible", attr.IsVisible);
                if (attr.UsePosition)
                    wr.WriteAttribute("position", attr.Position);
                if (attr.UseRotation)
                    wr.WriteAttribute("rotation", attr.Rotation);
                if (attr.UseHeight)
                    wr.WriteAttribute("height", FormatNumber(attr.Height));
                if (attr.UseAlign) {
                    if (attr.HorizontalAlign != HorizontalTextAlign.Left)
                        wr.WriteAttribute("horizontalAlign", attr.HorizontalAlign);
                    if (attr.VerticalAlign != VerticalTextAlign.Bottom)
                        wr.WriteAttribute("verticalAlign", attr.VerticalAlign);
                }

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'BlockAttribute'
            /// </summary>
            /// <param name="attr">L'objecte a visitar.</param>
            /// 
            public override void Visit(BlockAttribute attr) {

                wr.WriteStartElement("attribute");

                wr.WriteAttribute("name", attr.Name);
                wr.WriteAttribute("value", attr.Value);

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'RegionElement'
            /// </summary>
            /// <param name="region">L'objecte a visitar.</param>
            /// 
            public override void Visit(RegionElement region) {

                wr.WriteStartElement("region");

                wr.WriteAttribute("layers", region.LayerSet);
                if (region.Thickness > 0) {
                    wr.WriteAttribute("thickness", FormatNumber(region.Thickness));
                    if (region.Filled)
                        wr.WriteAttribute("filled", "true");
                }
                if (region.Clearance > 0)
                    wr.WriteAttribute("clearance", FormatNumber(region.Clearance));
                Signal signal = board.GetSignal(region, currentPart, false);
                if (signal != null)
                    wr.WriteAttribute("signal", signal.Name);

                foreach (RegionElement.Segment segment in region.Segments) {
                    wr.WriteStartElement("segment");
                    wr.WriteAttribute("position", segment.Position);
                    if (!segment.Angle.IsZero)
                        wr.WriteAttribute("angle", segment.Angle);
                    wr.WriteEndElement();
                }

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'Part'
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(Part part) {

                currentPart = part;
                try {
                    wr.WriteStartElement("part");

                    // Escriu els parametres
                    //
                    wr.WriteAttribute("name", part.Name);
                    wr.WriteAttribute("block", part.Block.Name);
                    wr.WriteAttribute("position", part.Position);
                    if (!part.Rotation.IsZero)
                        wr.WriteAttribute("rotation", part.Rotation);
                    if (part.Side != BoardSide.Top)
                        wr.WriteAttribute("side", part.Side);

                    // Escriu la llista de pads que tenen conexio.
                    //
                    if (part.HasPads) {
                        bool empty = true;
                        foreach (PadElement pad in part.Pads) {
                            Signal signal = board.GetSignal(pad, part, false);
                            if (signal != null) {
                                if (empty) {
                                    empty = false;
                                    wr.WriteStartElement("pads");
                                }
                                wr.WriteStartElement("pad");
                                wr.WriteAttribute("name", pad.Name);
                                wr.WriteAttribute("signal", signal.Name);
                                wr.WriteEndElement();
                            }
                        }
                        if (!empty)
                            wr.WriteEndElement();
                    }

                    // Escriu la llista d'atributs.
                    //
                    if (part.HasAttributes) {
                        wr.WriteStartElement("attributes");
                        foreach (PartAttribute attribute in part.Attributes) 
                            attribute.AcceptVisitor(this);
                        wr.WriteEndElement();
                    }

                    wr.WriteEndElement();
                }
                finally {
                    currentPart = null;
                }
            }

            /// <summary>
            /// Visita un objecte de tipus 'ViaElement'.
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                wr.WriteStartElement("via");

                wr.WriteAttribute("position", via.Position);
                wr.WriteAttribute("layers", via.LayerSet);
                wr.WriteAttribute("drill", FormatNumber(via.Drill));
                wr.WriteAttribute("outerSize", FormatNumber(via.OuterSize));
                if (via.InnerSize != via.OuterSize)
                    wr.WriteAttribute("innerSize", FormatNumber(via.InnerSize));
                if (via.Shape != ViaElement.ViaShape.Circle)
                    wr.WriteAttribute("shape", via.Shape);
                if (via.Type != ViaElement.ViaType.Through)
                    wr.WriteAttribute("type", via.Type);

                Signal signal = board.GetSignal(via, null, false);
                if (signal != null)
                    wr.WriteAttribute("signal", signal.Name);

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'Layer'.
            /// </summary>
            /// <param name="layer">L'objecte a visitar.</param>
            /// 
            public override void Visit(Layer layer) {

                wr.WriteStartElement("layer");

                wr.WriteAttribute("name", layer.Name);
                wr.WriteAttribute("side", layer.Side);
                wr.WriteAttribute("function", layer.Function);
                wr.WriteAttribute("color", layer.Color);
                if (!layer.IsVisible)
                    wr.WriteAttribute("visible", "false");

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipous 'Signal'.
            /// </summary>
            /// <param name="signal">L'objecte a visitar.</param>
            /// 
            public override void Visit(Signal signal) {

                wr.WriteStartElement("signal");

                wr.WriteAttribute("name", signal.Name);
                if (signal.Clearance > 0)
                    wr.WriteAttribute("clearance", FormatNumber(signal.Clearance));

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'Block'
            /// </summary>
            /// <param name="block">E'objecte a visitar.</param>
            /// 
            public override void Visit(Block block) {

                wr.WriteStartElement("block");

                wr.WriteAttribute("name", block.Name);

                if (block.HasElements) {
                    wr.WriteStartElement("elements");
                    foreach (BoardElement element in block.Elements)
                        element.AcceptVisitor(this);
                    wr.WriteEndElement();
                }

                if (block.HasAttributes) {
                    wr.WriteStartElement("attributes");
                    foreach (BlockAttribute attribute in block.Attributes)
                        attribute.AcceptVisitor(this);
                    wr.WriteEndElement();
                }

                wr.WriteEndElement();
            }

            /// <summary>
            /// Visita una placa.
            /// </summary>
            /// <param name="board">La placa a visitar.</param>
            /// 
            public override void Visit(Board board) {

                wr.WriteStartElement("board");
                wr.WriteAttribute("position", board.Position);
                wr.WriteAttribute("rotation", board.Rotation);

                wr.WriteStartElement("layers");
                foreach (Layer layer in board.Layers)
                    layer.AcceptVisitor(this);
                wr.WriteEndElement();

                wr.WriteStartElement("layerPairs");

                foreach (Layer layer1 in board.Layers) {
                    Layer layer2 = board.GetLayerPair(layer1);
                    if (layer2 != null) {
                        wr.WriteStartElement("pair");
                        wr.WriteAttribute("layer1", layer1.Name);
                        wr.WriteAttribute("layer2", layer2.Name);
                        wr.WriteEndElement();
                    }
                }
                wr.WriteEndElement();

                wr.WriteStartElement("signals");
                foreach (Signal signal in board.Signals)
                    signal.AcceptVisitor(this);
                wr.WriteEndElement();

                wr.WriteStartElement("blocks");
                foreach (Block block in board.Blocks)
                    block.AcceptVisitor(this);
                wr.WriteEndElement();

                wr.WriteStartElement("parts");
                foreach (Part part in board.Parts)
                    part.AcceptVisitor(this);
                wr.WriteEndElement();

                if (board.Elements != null) {
                    wr.WriteStartElement("elements");
                    foreach (BoardElement element in board.Elements)
                        element.AcceptVisitor(this);
                    wr.WriteEndElement();
                }

                wr.WriteEndElement();
            }

            /// <summary>
            /// Formateja una dimensio en mm
            /// </summary>
            /// <param name="value">Valor de la dimensio.</param>
            /// <returns>El valor formatejat.</returns>
            /// 
            private static string FormatNumber(int value) {

                return XmlConvert.ToString(value / 1000000.0);
            }
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="stream">Stream de sortida.</param>
        /// 
        public BoardStreamWriter(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanWrite)
                throw new InvalidOperationException("El stream no es de escritura.");

            this.stream = stream;
        }

        /// <summary>
        /// Escriu la placa en el stream de sortida.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        public void Write(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.CloseOutput = true;

            using (XmlWriter writer = XmlWriter.Create(stream, settings)) {
                writer.WriteStartDocument();

                XmlWriterAdapter wr = new XmlWriterAdapter(writer);

                wr.WriteStartElement("document", "http://MikroPic.com/schemas/edatools/v1/XBRD.xsd");
                wr.WriteAttribute("version", "212");
                wr.WriteAttribute("documentType", "board");
                wr.WriteAttribute("distanceUnits", "mm");
                wr.WriteAttribute("angleUnits", "deg");

                IVisitor visitor = new Visitor(board, wr);
                visitor.Run();

                wr.WriteEndElement();

                writer.WriteEndDocument();
            }
        }
    }
}
