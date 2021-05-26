namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

    using System;
    using System.IO;
    using System.Xml;
    using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Base.Xml;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

    /// <summary>
    /// Clase per la escriptura de plaques en un stream.
    /// </summary>
    /// 
    public sealed class BoardStreamWriter {

        private const int version = 213;
        private const string distanceUnits = "mm";
        private const string angleUnits = "deg";

        private readonly Stream stream;

        private class Visitor : DefaultBoardVisitor {

            private readonly XmlWriter writer;
            private Board currentBoard;
            private Part currentPart;

            /// <summary>
            /// Constructor del objecte. Visita els objectes d'una placa,
            /// per generar el stream de sortida.
            /// </summary>
            /// <param name="wr">Objecte per escriure el stream de sortida.</param>
            /// 
            public Visitor(XmlWriter writer) {

                this.writer = writer;
            }

            /// <summary>
            /// Visita un element de tipus 'LineElement'
            /// </summary>
            /// <param name="line">L'objecte a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                writer.WriteStartElement("line");

                writer.WriteAttributeString("layer", line.LayerId.ToString());
                writer.WriteAttributeString("startPosition", XmlTypeFormater.FormatPoint(line.StartPosition));
                writer.WriteAttributeString("endPosition", XmlTypeFormater.FormatPoint(line.EndPosition));
                if (line.Thickness > 0)
                    writer.WriteAttributeString("thickness", XmlTypeFormater.FormatNumber(line.Thickness));
                if (line.LineCap != LineElement.CapStyle.Round)
                    writer.WriteAttributeEnum("lineCap", line.LineCap);

                if (currentBoard != null) {
                    Signal signal = currentBoard.GetSignal(line, currentPart, false);
                    if (signal != null)
                        writer.WriteAttributeString("signal", signal.Name);
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'ArcElement'.
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                writer.WriteStartElement("arc");

                writer.WriteAttributeString("layer", arc.LayerId.ToString());
                writer.WriteAttributeString("startPosition", XmlTypeFormater.FormatPoint(arc.StartPosition));
                writer.WriteAttributeString("endPosition", XmlTypeFormater.FormatPoint(arc.EndPosition));
                writer.WriteAttributeString("angle", XmlTypeFormater.FormatAngle(arc.Angle));
                if (arc.Thickness > 0)
                    writer.WriteAttributeString("thickness", XmlTypeFormater.FormatNumber(arc.Thickness));
                if (arc.LineCap != LineElement.CapStyle.Round)
                    writer.WriteAttributeEnum("lineCap", arc.LineCap);

                if (currentBoard != null) {
                    Signal signal = currentBoard.GetSignal(arc, currentPart, false);
                    if (signal != null)
                        writer.WriteAttributeString("signal", signal.Name);
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'RectangleElement'.
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                writer.WriteStartElement("rectangle");

                writer.WriteAttributeString("layer", rectangle.LayerId.ToString());
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

                writer.WriteAttributeString("layer", circle.LayerId.ToString());
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

                writer.WriteAttributeString("layer", text.LayerId.ToString());
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
                writer.WriteAttributeString("layer", pad.LayerId.ToString());
                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(pad.Position));
                if (!pad.Rotation.IsZero)
                    writer.WriteAttributeString("rotation", XmlTypeFormater.FormatAngle(pad.Rotation));
                writer.WriteAttributeString("size", XmlTypeFormater.FormatSize(pad.Size));
                if (!pad.Roundness.IsZero)
                    writer.WriteAttributeString("roundness", XmlTypeFormater.FormatRatio(pad.Roundness));
                if (pad.Stop) {
                    writer.WriteAttributeString("stop", "true");
                    if (pad.StopMargin > 0)
                        writer.WriteAttributeString("stopMargin", XmlTypeFormater.FormatNumber(pad.StopMargin));
                }
                if (pad.Cream) {
                    writer.WriteAttributeString("cream", "true");
                    if (pad.CreamMargin > 0)
                        writer.WriteAttributeString("creamMargin", XmlTypeFormater.FormatNumber(pad.CreamMargin));
                }

                if (currentBoard != null) {
                    Signal signal = currentBoard.GetSignal(pad, currentPart, false);
                    if (signal != null)
                        writer.WriteAttributeString("signal", signal.Name);
                }

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
                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(pad.Position));
                if (!pad.Rotation.IsZero)
                    writer.WriteAttributeString("rotation", XmlTypeFormater.FormatAngle(pad.Rotation));
                writer.WriteAttributeString("size", XmlTypeFormater.FormatNumber(pad.TopSize));
                writer.WriteAttributeString("drill", XmlTypeFormater.FormatNumber(pad.Drill));
                if (pad.Shape != ThPadElement.ThPadShape.Circle)
                    writer.WriteAttributeEnum("shape", pad.Shape);
                if (pad.Stop) {
                    writer.WriteAttributeString("stop", "true");
                    writer.WriteAttributeString("stopMargin", XmlTypeFormater.FormatNumber(pad.StopMargin));
                }

                if (currentBoard != null) {
                    Signal signal = currentBoard.GetSignal(pad, currentPart, false);
                    if (signal != null)
                        writer.WriteAttributeString("signal", signal.Name);
                }

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

                writer.WriteAttributeString("layer", region.LayerId.ToString());
                if (region.Thickness > 0) {
                    writer.WriteAttributeString("thickness", XmlTypeFormater.FormatNumber(region.Thickness));
                    if (region.Filled)
                        writer.WriteAttributeString("filled", "true");
                }
                if (region.Clearance > 0)
                    writer.WriteAttributeString("clearance", XmlTypeFormater.FormatNumber(region.Clearance));

                if (currentBoard != null) {
                    Signal signal = currentBoard.GetSignal(region, currentPart, false);
                    if (signal != null)
                        writer.WriteAttributeString("signal", signal.Name);
                }

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
                    if (part.Flip)
                        writer.WriteAttributeString("flip", "true");

                    // Escriu la llista de pads que tenen conexio.
                    //
                    if (part.HasPads) {
                        bool empty = true;
                        foreach (PadElement pad in part.Pads) {
                            Signal signal = currentBoard.GetSignal(pad, part, false);
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

                writer.WriteAttributeString("topLayer", via.TopLayerId.ToString());
                writer.WriteAttributeString("bottomLayer", via.BottomLayerId.ToString());
                writer.WriteAttributeString("position", XmlTypeFormater.FormatPoint(via.Position));
                writer.WriteAttributeString("drill", XmlTypeFormater.FormatNumber(via.Drill));
                writer.WriteAttributeString("outerSize", XmlTypeFormater.FormatNumber(via.OuterSize));
                if (via.InnerSize != via.OuterSize)
                    writer.WriteAttributeString("innerSize", XmlTypeFormater.FormatNumber(via.InnerSize));
                if (via.Shape != ViaElement.ViaShape.Circle)
                    writer.WriteAttributeEnum("shape", via.Shape);

                if (currentBoard != null) {
                    Signal signal = currentBoard.GetSignal(via, null, false);
                    if (signal != null)
                        writer.WriteAttributeString("signal", signal.Name);
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'Layer'.
            /// </summary>
            /// <param name="layer">L'objecte a visitar.</param>
            /// 
            public override void Visit(Layer layer) {

                writer.WriteStartElement("layer");

                writer.WriteAttributeString("id", layer.Id.ToString());
                writer.WriteAttributeEnum("side", layer.Side);
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
            /// Visita un objecte de tipus 'Component'
            /// </summary>
            /// <param name="component">E'objecte a visitar.</param>
            /// 
            public override void Visit(Component component) {

                writer.WriteStartElement("component");

                writer.WriteAttributeString("name", component.Name);
                if (!String.IsNullOrEmpty(component.Description))
                    writer.WriteAttributeString("description", component.Description);

                if (component.HasElements) {
                    writer.WriteStartElement("elements");
                    foreach (var element in component.Elements)
                        element.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                if (component.HasAttributes) {
                    writer.WriteStartElement("attributes");
                    foreach (var attribute in component.Attributes)
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

                currentBoard = board;

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
                    foreach (var component in board.Components)
                        component.AcceptVisitor(this);
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

            /// <summary>
            /// Visita una llibreria
            /// </summary>
            /// <param name="library">La llibreria.</param>
            /// 
            public override void Visit(Library library) {

                writer.WriteStartElement("library");

                writer.WriteAttributeString("name", library.Name);
                if (!String.IsNullOrEmpty(library.Description))
                    writer.WriteAttributeString("description", library.Description);

                if (library.HasComponents) {
                    writer.WriteStartElement("components");
                    foreach (var component in library.Components)
                        component.AcceptVisitor(this);
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
                throw new ArgumentNullException(nameof(stream));

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
                throw new ArgumentNullException(nameof(board));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.CloseOutput = true;

            using (XmlWriter writer = XmlWriter.Create(stream, settings)) {

                writer.WriteStartDocument();

                writer.WriteStartElement("document", "http://MikroPic.com/schemas/edatools/v1/XBRD.xsd");
                writer.WriteAttributeInteger("version", version);
                writer.WriteAttributeString("documentType", "board");
                writer.WriteAttributeString("distanceUnits", distanceUnits);
                writer.WriteAttributeString("angleUnits", angleUnits);

                IBoardVisitor visitor = new Visitor(writer);
                board.AcceptVisitor(visitor);

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Escriu la placa en el stream de sortida.
        /// </summary>
        /// <param name="library">La placa.</param>
        /// 
        public void Write(Library library) {

            if (library == null)
                throw new ArgumentNullException(nameof(library));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.CloseOutput = true;

            using (XmlWriter writer = XmlWriter.Create(stream, settings)) {

                writer.WriteStartDocument();

                writer.WriteStartElement("document", "http://MikroPic.com/schemas/edatools/v1/XLIB.xsd");
                writer.WriteAttributeInteger("version", version);
                writer.WriteAttributeString("documentType", "componentLibrary");
                writer.WriteAttributeString("distanceUnits", distanceUnits);
                writer.WriteAttributeString("angleUnits", angleUnits);

                IBoardVisitor visitor = new Visitor(writer);
                library.AcceptVisitor(visitor);

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }
    }
}
