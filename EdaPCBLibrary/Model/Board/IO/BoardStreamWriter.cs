namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

    using MikroPic.EdaTools.v1.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;
    using MikroPic.EdaTools.v1.Xml;
    using System;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Clase per la escriptura de plaques en un stream.
    /// </summary>
    /// 
    public sealed class BoardStreamWriter {

        private readonly Stream stream;

        private class Visitor : DefaultVisitor {

            private readonly XmlWriter writer;
            private readonly Board board;
            private Part currentPart = null;

            /// <summary>
            /// Constructor del objecte. Visita els objectes d'una placa,
            /// per generar el stream de sortida.
            /// </summary>
            /// <param name="board">La placa a visitar.</param>
            /// <param name="wr">Objecte per escriure el stream de sortida.</param>
            /// 
            public Visitor(Board board, XmlWriter writer) {

                this.board = board;
                this.writer = writer;
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

                writer.WriteStartElement("line");

                writer.WriteAttributeString("layers", line.LayerSet.ToString());
                writer.WriteAttributeString("startPosition", XmlTypeFormater.FormatPoint(line.StartPosition));
                writer.WriteAttributeString("endPosition", XmlTypeFormater.FormatPoint(line.EndPosition));
                if (line.Thickness > 0)
                    writer.WriteAttributeString("thickness", XmlTypeFormater.FormatNumber(line.Thickness));
                if (line.LineCap != LineElement.LineCapStyle.Round)
                    writer.WriteAttributeEnum("lineCap", line.LineCap);

                Signal signal = board.GetSignal(line, currentPart, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'ArcElement'.
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                writer.WriteStartElement("arc");

                writer.WriteAttributeString("layers", arc.LayerSet.ToString());
                writer.WriteAttributeString("startPosition", XmlTypeFormater.FormatPoint(arc.StartPosition));
                writer.WriteAttributeString("endPosition", XmlTypeFormater.FormatPoint(arc.EndPosition));
                writer.WriteAttributeString("angle", XmlTypeFormater.FormatAngle(arc.Angle));
                if (arc.Thickness > 0)
                    writer.WriteAttributeString("thickness", XmlTypeFormater.FormatNumber(arc.Thickness));
                if (arc.LineCap != LineElement.LineCapStyle.Round)
                    writer.WriteAttributeEnum("lineCap", arc.LineCap);

                Signal signal = board.GetSignal(arc, currentPart, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'RectangleElement'.
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                writer.WriteStartElement("rectangle");

                writer.WriteAttributeString("layers", rectangle.LayerSet.ToString());
                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(rectangle.Position));
                writer.WriteAttributeString("size", XmlTypeFormater.FormatSize(rectangle.Size));
                if (!rectangle.Rotation.IsZero)
                    writer.WriteAttributeString("rotation", XmlTypeFormater.FormatAngle(rectangle.Rotation));
                if (rectangle.Thickness > 0) {
                    writer.WriteAttributeString("thickness", XmlTypeFormater.FormatNumber(rectangle.Thickness));
                    if (rectangle.Filled)
                        writer.WriteAttributeBool("filled", true);
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'CircleElement'
            /// </summary>
            /// <param name="circle">L'element a visitar.</param>
            /// 
            public override void Visit(CircleElement circle) {

                writer.WriteStartElement("circle");

                writer.WriteAttributeString("layers", circle.LayerSet.ToString());
                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(circle.Position));
                writer.WriteAttributeString("radius", XmlTypeFormater.FormatNumber(circle.Radius));
                if (circle.Thickness > 0) {
                    writer.WriteAttributeString("thickness", XmlTypeFormater.FormatNumber(circle.Thickness));
                    if (circle.Filled)
                        writer.WriteAttributeBool("filled", true);
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'TextElement'
            /// </summary>
            /// <param name="text">L'element a visitar</param>
            /// 
            public override void Visit(TextElement text) {

                writer.WriteStartElement("text");

                writer.WriteAttributeString("layers", text.LayerSet.ToString());
                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(text.Position));
                if (!text.Rotation.IsZero)
                    writer.WriteAttributeString("rotation", XmlTypeFormater.FormatAngle(text.Rotation));
                writer.WriteAttributeString("height", XmlTypeFormater.FormatNumber(text.Height));
                writer.WriteAttributeString("thickness", XmlTypeFormater.FormatNumber(text.Thickness));
                if (text.HorizontalAlign != HorizontalTextAlign.Left)
                    writer.WriteAttributeEnum("horizontalAlign", text.HorizontalAlign);
                if (text.VerticalAlign != VerticalTextAlign.Bottom)
                    writer.WriteAttributeEnum("verticalAlign", text.VerticalAlign);
                if (!String.IsNullOrEmpty(text.Value))
                    writer.WriteAttributeString("value", text.Value);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'HoleElement'.
            /// </summary>
            /// <param name="hole">L'element a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                writer.WriteStartElement("hole");

                writer.WriteAttributeString("layers", hole.LayerSet.ToString());
                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(hole.Position));
                writer.WriteAttributeString("drill", XmlTypeFormater.FormatNumber(hole.Drill));

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'SmdPadElement'
            /// </summary>
            /// <param name="pad">L'element a visitar-</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                writer.WriteStartElement("spad");

                writer.WriteAttributeString("name", pad.Name);
                writer.WriteAttributeString("layers", pad.LayerSet.ToString());
                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(pad.Position));
                if (!pad.Rotation.IsZero)
                    writer.WriteAttributeString("rotation", XmlTypeFormater.FormatAngle(pad.Rotation));
                writer.WriteAttributeString("size", XmlTypeFormater.FormatSize(pad.Size));
                if (!pad.Roundness.IsZero)
                    writer.WriteAttributeString("roundness", XmlTypeFormater.FormatRatio(pad.Roundness));

                Signal signal = board.GetSignal(pad, currentPart, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'ThPadElement'
            /// </summary>
            /// <param name="pad">L'element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                writer.WriteStartElement("tpad");

                writer.WriteAttributeString("name", pad.Name);
                writer.WriteAttributeString("layers", pad.LayerSet.ToString());
                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(pad.Position));
                if (!pad.Rotation.IsZero)
                    writer.WriteAttributeString("rotation", XmlTypeFormater.FormatAngle(pad.Rotation));
                writer.WriteAttributeString("size", XmlTypeFormater.FormatNumber(pad.TopSize));
                writer.WriteAttributeString("drill", XmlTypeFormater.FormatNumber(pad.Drill));
                if (pad.Shape != ThPadElement.ThPadShape.Circle)
                    writer.WriteAttributeEnum("shape", pad.Shape);

                Signal signal = board.GetSignal(pad, currentPart, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'PartAttribute'
            /// </summary>
            /// <param name="attr">L'objecte a visitar.</param>
            /// 
            public override void Visit(PartAttribute attr) {

                writer.WriteStartElement("attribute");

                writer.WriteAttributeString("name", attr.Name);
                writer.WriteAttributeString("value", attr.Value);
                if (!attr.IsVisible)
                    writer.WriteAttributeBool("visible", attr.IsVisible);
                if (attr.UsePosition)
                    writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(attr.Position));
                if (attr.UseRotation)
                    writer.WriteAttributeString("rotation", XmlTypeFormater.FormatAngle(attr.Rotation));
                if (attr.UseHeight)
                    writer.WriteAttributeString("height", XmlTypeFormater.FormatNumber(attr.Height));
                if (attr.UseAlign) {
                    if (attr.HorizontalAlign != HorizontalTextAlign.Left)
                        writer.WriteAttributeEnum("horizontalAlign", attr.HorizontalAlign);
                    if (attr.VerticalAlign != VerticalTextAlign.Bottom)
                        writer.WriteAttributeEnum("verticalAlign", attr.VerticalAlign);
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'BlockAttribute'
            /// </summary>
            /// <param name="attr">L'objecte a visitar.</param>
            /// 
            public override void Visit(ComponentAttribute attr) {

                writer.WriteStartElement("attribute");

                writer.WriteAttributeString("name", attr.Name);
                writer.WriteAttributeString("value", attr.Value);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'RegionElement'
            /// </summary>
            /// <param name="region">L'objecte a visitar.</param>
            /// 
            public override void Visit(RegionElement region) {

                writer.WriteStartElement("region");

                writer.WriteAttributeString("layers", region.LayerSet.ToString());
                if (region.Thickness > 0) {
                    writer.WriteAttributeString("thickness", XmlTypeFormater.FormatNumber(region.Thickness));
                    if (region.Filled)
                        writer.WriteAttributeString("filled", "true");
                }
                if (region.Clearance > 0)
                    writer.WriteAttributeString("clearance", XmlTypeFormater.FormatNumber(region.Clearance));
                Signal signal = board.GetSignal(region, currentPart, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                foreach (var segment in region.Segments) {
                    writer.WriteStartElement("segment");
                    writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(segment.Position));
                    if (!segment.Angle.IsZero)
                        writer.WriteAttributeString("angle", XmlTypeFormater.FormatAngle(segment.Angle));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'Part'
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(Part part) {

                currentPart = part;
                try {
                    writer.WriteStartElement("part");

                    // Escriu els parametres
                    //
                    writer.WriteAttributeString("name", part.Name);
                    writer.WriteAttributeString("component", part.Component.Name);
                    writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(part.Position));
                    if (!part.Rotation.IsZero)
                        writer.WriteAttributeString("rotation", XmlTypeFormater.FormatAngle(part.Rotation));
                    if (part.Side != BoardSide.Top)
                        writer.WriteAttributeEnum("side", part.Side);

                    // Escriu la llista de pads que tenen conexio.
                    //
                    if (part.HasPads) {
                        bool empty = true;
                        foreach (PadElement pad in part.Pads) {
                            Signal signal = board.GetSignal(pad, part, false);
                            if (signal != null) {
                                if (empty) {
                                    empty = false;
                                    writer.WriteStartElement("pads");
                                }
                                writer.WriteStartElement("pad");
                                writer.WriteAttributeString("name", pad.Name);
                                writer.WriteAttributeString("signal", signal.Name);
                                writer.WriteEndElement();
                            }
                        }
                        if (!empty)
                            writer.WriteEndElement();
                    }

                    // Escriu la llista d'atributs.
                    //
                    if (part.HasAttributes) {
                        writer.WriteStartElement("attributes");
                        foreach (var attribute in part.Attributes) 
                            attribute.AcceptVisitor(this);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
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

                writer.WriteStartElement("via");

                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(via.Position));
                writer.WriteAttributeString("layers", via.LayerSet.ToString());
                writer.WriteAttributeString("drill", XmlTypeFormater.FormatNumber(via.Drill));
                writer.WriteAttributeString("outerSize", XmlTypeFormater.FormatNumber(via.OuterSize));
                if (via.InnerSize != via.OuterSize)
                    writer.WriteAttributeString("innerSize", XmlTypeFormater.FormatNumber(via.InnerSize));
                if (via.Shape != ViaElement.ViaShape.Circle)
                    writer.WriteAttributeEnum("shape", via.Shape);
                if (via.Type != ViaElement.ViaType.Through)
                    writer.WriteAttributeEnum("type", via.Type);

                Signal signal = board.GetSignal(via, null, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'Layer'.
            /// </summary>
            /// <param name="layer">L'objecte a visitar.</param>
            /// 
            public override void Visit(Layer layer) {

                writer.WriteStartElement("layer");

                writer.WriteAttributeString("id", layer.Id.FullName);
                writer.WriteAttributeEnum("function", layer.Function);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipous 'Signal'.
            /// </summary>
            /// <param name="signal">L'objecte a visitar.</param>
            /// 
            public override void Visit(Signal signal) {

                writer.WriteStartElement("signal");

                writer.WriteAttributeString("name", signal.Name);
                if (signal.Clearance > 0)
                    writer.WriteAttributeString("clearance", XmlTypeFormater.FormatNumber(signal.Clearance));

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'Block'
            /// </summary>
            /// <param name="block">E'objecte a visitar.</param>
            /// 
            public override void Visit(Component block) {

                writer.WriteStartElement("component");

                writer.WriteAttributeString("name", block.Name);

                if (block.HasElements) {
                    writer.WriteStartElement("elements");
                    foreach (Element element in block.Elements)
                        element.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                if (block.HasAttributes) {
                    writer.WriteStartElement("attributes");
                    foreach (ComponentAttribute attribute in block.Attributes)
                        attribute.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita una placa.
            /// </summary>
            /// <param name="board">La placa a visitar.</param>
            /// 
            public override void Visit(Board board) {

                writer.WriteStartElement("board");
                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(board.Position));
                writer.WriteAttributeString("rotation", XmlTypeFormater.FormatAngle(board.Rotation));

                writer.WriteStartElement("layers");
                foreach (var layer in board.Layers)
                    layer.AcceptVisitor(this);
                writer.WriteEndElement();

                if (board.HasSignals) {
                    writer.WriteStartElement("signals");
                    foreach (var signal in board.Signals)
                        signal.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                if (board.HasComponents) {
                    writer.WriteStartElement("components");
                    foreach (var block in board.Components)
                        block.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                if (board.HasParts) {
                    writer.WriteStartElement("parts");
                    foreach (var part in board.Parts)
                        part.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                if (board.HasElements) {
                    writer.WriteStartElement("elements");
                    foreach (var element in board.Elements)
                        element.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
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

                writer.WriteStartElement("document", "http://MikroPic.com/schemas/edatools/v1/XBRD.xsd");
                writer.WriteAttributeString("version", "213");
                writer.WriteAttributeString("documentType", "board");
                writer.WriteAttributeString("distanceUnits", "mm");
                writer.WriteAttributeString("angleUnits", "deg");

                IVisitor visitor = new Visitor(board, writer);
                visitor.Run();

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }
    }
}
