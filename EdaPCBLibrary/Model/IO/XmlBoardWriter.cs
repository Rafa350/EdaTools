namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// Clase per la escriptura de plaques en un stream.
    /// </summary>
    public sealed class XmlBoardWriter {

        private Stream stream;

        private class Visitor : DefaultVisitor {

            private readonly XmlWriter writer;
            private readonly CultureInfo ci = CultureInfo.InvariantCulture;
            private Board board;
            private Part currentPart = null;

            /// <summary>
            /// Constructor del objecte. Visita els objectes d'una placa,
            /// per generar el stream de sortida.
            /// </summary>
            /// <param name="board">La placa a visitar.</param>
            /// <param name="writer">Objecte per escriure el stream de sortida.</param>
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

                writer.WriteAttribute("layers", GetLayerNames(line));
                writer.WriteAttribute("startPosition", FormatPoint(line.StartPosition));
                writer.WriteAttribute("endPosition", FormatPoint(line.EndPosition));
                if (line.Thickness > 0)
                    writer.WriteAttribute("thickness", FormatNumber(line.Thickness));
                if (line.LineCap != LineElement.LineCapStyle.Round)
                    writer.WriteAttribute("lineCap", line.LineCap);

                Signal signal = board.GetSignal(line, currentPart, false);
                if (signal != null)
                    writer.WriteAttribute("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'ArcElement'.
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                writer.WriteStartElement("arc");

                writer.WriteAttribute("layers", GetLayerNames(arc));
                writer.WriteAttribute("startPosition", FormatPoint(arc.StartPosition));
                writer.WriteAttribute("endPosition", FormatPoint(arc.EndPosition));
                writer.WriteAttribute("angle", FormatAngle(arc.Angle));
                if (arc.Thickness > 0)
                    writer.WriteAttribute("thickness", FormatNumber(arc.Thickness));
                if (arc.LineCap != LineElement.LineCapStyle.Round)
                    writer.WriteAttribute("lineCap", arc.LineCap);

                Signal signal = board.GetSignal(arc, currentPart, false);
                if (signal != null)
                    writer.WriteAttribute("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'RectangleElement'.
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                writer.WriteStartElement("rectangle");

                writer.WriteAttribute("layers", GetLayerNames(rectangle));
                writer.WriteAttribute("position", FormatPoint(rectangle.Position));
                writer.WriteAttribute("size", FormatSize(rectangle.Size));
                if (!rectangle.Rotation.IsZero)
                    writer.WriteAttribute("rotation", FormatAngle(rectangle.Rotation));
                if (rectangle.Thickness > 0)
                    writer.WriteAttribute("thickness", FormatNumber(rectangle.Thickness));

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'CircleElement'
            /// </summary>
            /// <param name="circle">L'element a visitar.</param>
            /// 
            public override void Visit(CircleElement circle) {

                writer.WriteStartElement("circle");

                writer.WriteAttribute("layers", GetLayerNames(circle));
                writer.WriteAttribute("position", FormatPoint(circle.Position));
                writer.WriteAttribute("radius", FormatNumber(circle.Radius));
                if (circle.Thickness > 0)
                    writer.WriteAttribute("thickness", FormatNumber(circle.Thickness));

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'TextElement'
            /// </summary>
            /// <param name="text">L'element a visitar</param>
            /// 
            public override void Visit(TextElement text) {

                writer.WriteStartElement("text");

                writer.WriteAttribute("layers", GetLayerNames(text));
                writer.WriteAttribute("position", FormatPoint(text.Position));
                if (!text.Rotation.IsZero)
                    writer.WriteAttribute("rotation", FormatAngle(text.Rotation));
                writer.WriteAttribute("height", FormatNumber(text.Height));
                writer.WriteAttribute("thickness", FormatNumber(text.Thickness));
                if (text.Align != TextAlign.TopLeft)
                    writer.WriteAttribute("align", text.Align);
                if (!String.IsNullOrEmpty(text.Value))
                    writer.WriteAttribute("value", text.Value);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'HoleElement'.
            /// </summary>
            /// <param name="hole">L'element a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                writer.WriteStartElement("hole");

                writer.WriteAttribute("layers", GetLayerNames(hole));
                writer.WriteAttribute("position", FormatPoint(hole.Position));
                writer.WriteAttribute("drill", FormatNumber(hole.Drill));

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'SmdPadElement'
            /// </summary>
            /// <param name="pad">L'element a visitar-</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                writer.WriteStartElement("spad");

                writer.WriteAttribute("name", pad.Name);
                writer.WriteAttribute("layers", GetLayerNames(pad));
                writer.WriteAttribute("position", FormatPoint(pad.Position));
                if (!pad.Rotation.IsZero)
                    writer.WriteAttribute("rotation", FormatAngle(pad.Rotation));
                writer.WriteAttribute("size", FormatSize(pad.Size));
                if (!pad.Roundness.IsZero)
                    writer.WriteAttribute("roundness", FormRatio(pad.Roundness));

                Signal signal = board.GetSignal(pad, currentPart, false);
                if (signal != null)
                    writer.WriteAttribute("signal", signal.Name);

                writer.WriteEndElement();
            }

            public override void Visit(ThPadElement pad) {

                writer.WriteStartElement("tpad");

                writer.WriteAttribute("name", pad.Name);
                writer.WriteAttribute("layers", GetLayerNames(pad));
                writer.WriteAttribute("position", FormatPoint(pad.Position));
                if (!pad.Rotation.IsZero)
                    writer.WriteAttribute("rotation", FormatAngle(pad.Rotation));
                writer.WriteAttribute("size", FormatNumber(pad.TopSize));
                writer.WriteAttribute("drill", FormatNumber(pad.Drill));
                if (pad.Shape != ThPadElement.ThPadShape.Circular)
                    writer.WriteAttribute("shape", pad.Shape.ToString());

                Signal signal = board.GetSignal(pad, currentPart, false);
                if (signal != null)
                    writer.WriteAttribute("signal", signal.Name);

                writer.WriteEndElement();
            }

            public override void Visit(PartAttribute parameter) {

                writer.WriteStartElement("attribute");

                writer.WriteAttribute("name", parameter.Name);
                writer.WriteAttribute("value", parameter.Value);
                if (!parameter.IsVisible)
                    writer.WriteAttribute("visible", parameter.IsVisible);
                if (parameter.UsePosition)
                    writer.WriteAttribute("position", FormatPoint(parameter.Position));
                if (parameter.UseRotation)
                    writer.WriteAttribute("rotation", FormatAngle(parameter.Rotation));
                if (parameter.UseHeight)
                    writer.WriteAttribute("height", FormatNumber(parameter.Height));
                if (parameter.UseAlign)
                    writer.WriteAttribute("align", parameter.Align);

                writer.WriteEndElement();
            }

            public override void Visit(RegionElement region) {

                writer.WriteStartElement("region");

                writer.WriteAttribute("layers", GetLayerNames(region));
                if (region.Thickness > 0)
                    writer.WriteAttribute("thickness", FormatNumber(region.Thickness));
                Signal signal = board.GetSignal(region, currentPart, false);
                if (signal != null)
                    writer.WriteAttribute("signal", signal.Name);

                foreach (RegionElement.Segment segment in region.Segments) {
                    writer.WriteStartElement("segment");
                    writer.WriteAttribute("position", FormatPoint(segment.Position));
                    if (!segment.Angle.IsZero)
                        writer.WriteAttribute("angle", FormatAngle(segment.Angle));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte Part
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(Part part) {

                bool first;

                currentPart = part;
                try {
                    writer.WriteStartElement("part");

                    // Escriu els parametres
                    //
                    writer.WriteAttribute("name", part.Name);
                    writer.WriteAttribute("block", part.Block.Name);
                    writer.WriteAttribute("position", FormatPoint(part.Position));
                    if (!part.Rotation.IsZero)
                        writer.WriteAttribute("rotation", FormatAngle(part.Rotation));
                    if (part.Side != BoardSide.Top)
                        writer.WriteAttribute("side", part.Side);

                    // Escriu la llista de pads.
                    //
                    first = true;
                    foreach (PadElement pad in part.Pads) {
                        Signal signal = board.GetSignal(pad, part, false);
                        if (signal != null) {
                            if (first) {
                                first = false;
                                writer.WriteStartElement("pads");
                            }
                            writer.WriteStartElement("pad");
                            writer.WriteAttribute("name", pad.Name);
                            writer.WriteAttribute("signal", signal.Name);
                            writer.WriteEndElement();
                        }
                    }
                    if (!first)
                        writer.WriteEndElement();

                    // Escriu la llista d'atributs.
                    //
                    first = true;
                    foreach (PartAttribute attribute in part.Attributes) {
                        if (first) {
                            first = false;
                            writer.WriteStartElement("attributes");
                        }
                        attribute.AcceptVisitor(this);
                    }
                    if (!first)
                        writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                finally {
                    currentPart = null;
                }
            }

            /// <summary>
            /// Visita una via.
            /// </summary>
            /// <param name="via">La via a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                writer.WriteStartElement("via");

                writer.WriteAttribute("position", FormatPoint(via.Position));
                writer.WriteAttribute("layers", GetLayerNames(via));
                writer.WriteAttribute("drill", FormatNumber(via.Drill));
                writer.WriteAttribute("outerSize", FormatNumber(via.OuterSize));
                if (via.InnerSize != via.OuterSize)
                    writer.WriteAttribute("innerSize", FormatNumber(via.InnerSize));
                if (via.Shape != ViaElement.ViaShape.Circular)
                    writer.WriteAttribute("shape", via.Shape);
                if (via.Type != ViaElement.ViaType.Through)
                    writer.WriteAttribute("type", via.Type);

                Signal signal = board.GetSignal(via, null, false);
                if (signal != null)
                    writer.WriteAttribute("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita una capa.
            /// </summary>
            /// <param name="layer">La capa a visitar.</param>
            /// 
            public override void Visit(Layer layer) {

                writer.WriteStartElement("layer");

                writer.WriteAttribute("name", layer.Name);
                writer.WriteAttribute("side", layer.Side);
                writer.WriteAttribute("function", layer.Function);
                writer.WriteAttribute("color", FormatColor(layer.Color));
                if (!layer.IsVisible)
                    writer.WriteAttribute("visible", "false");

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita una senyal.
            /// </summary>
            /// <param name="signal">La senyal a visitar.</param>
            /// 
            public override void Visit(Signal signal) {

                writer.WriteStartElement("signal");

                writer.WriteAttribute("name", signal.Name);
                if (signal.Clearance > 0)
                    writer.WriteAttribute("clearance", FormatNumber(signal.Clearance));

                writer.WriteEndElement();
            }

            public override void Visit(Block block) {

                writer.WriteStartElement("block");

                writer.WriteAttribute("name", block.Name);

                writer.WriteStartElement("elements");
                foreach (Element element in block.Elements)
                    element.AcceptVisitor(this);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            public override void Visit(Board board) {

                writer.WriteStartElement("board");

                writer.WriteAttribute("version", "211");
                writer.WriteAttribute("units", "mm");
                writer.WriteAttribute("position", FormatPoint(board.Position));
                writer.WriteAttribute("rotation", FormatAngle(board.Rotation));

                writer.WriteStartElement("layers");
                foreach (Layer layer in board.Layers)
                    layer.AcceptVisitor(this);
                writer.WriteEndElement();

                writer.WriteStartElement("layerPairs");
                foreach (Layer layer in board.Layers) {
                    Layer pairLayer = board.GetLayerPair(layer);
                    if (pairLayer != null) {
                        writer.WriteStartElement("pair");
                        writer.WriteAttribute("layer1", layer.Name);
                        writer.WriteAttribute("layer2", pairLayer.Name);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                writer.WriteStartElement("signals");
                foreach (Signal signal in board.Signals)
                    signal.AcceptVisitor(this);
                writer.WriteEndElement();

                writer.WriteStartElement("blocks");
                foreach (Block block in board.Blocks)
                    block.AcceptVisitor(this);
                writer.WriteEndElement();

                writer.WriteStartElement("parts");
                foreach (Part part in board.Parts)
                    part.AcceptVisitor(this);
                writer.WriteEndElement();

                if (board.Elements != null) {
                    writer.WriteStartElement("elements");
                    foreach (Element element in board.Elements)
                        element.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            private string[] GetLayerNames(Element element) {

                IEnumerable<Layer> layers = board.GetLayers(element);
                if (layers != null) {
                    List<String> names = new List<String>();
                    foreach (Layer layer in layers)
                        names.Add(layer.Name);
                    return names.ToArray();
                }
                else
                    return null;
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

            /// <summary>
            /// Formateja un punt en mm
            /// </summary>
            /// <param name="point">El valor del punt.</param>
            /// <returns>El valor formatejat.</returns>
            /// 
            private static string FormatPoint(PointInt point) {

                return String.Format(
                    "{0}, {1}",
                    XmlConvert.ToString(point.X / 1000000.0),
                    XmlConvert.ToString(point.Y / 1000000.0));
            }

            /// <summary>
            /// Formateja un tamany en mm
            /// </summary>
            /// <param name="size">El tamany a formatejar.</param>
            /// <returns>El valor formatejat.</returns>
            /// 
            private static string FormatSize(SizeInt size) {

                return String.Format(
                    "{0}, {1}",
                    XmlConvert.ToString(size.Width / 1000000.0),
                    XmlConvert.ToString(size.Height / 1000000.0));
            }

            /// <summary>
            /// Formateja un angle en graus
            /// </summary>
            /// <param name="angle">El valor del angle</param>
            /// <returns>El angle formatejat.</returns>
            /// 
            private static string FormatAngle(Angle angle) {

                return XmlConvert.ToString(angle.Degrees / 100.0);
            }

            /// <summary>
            /// Formateja un color a ARGB
            /// </summary>
            /// <param name="color">El color a formatejar.</param>
            /// <returns>El valor formatejat.</returns>
            /// 
            private static string FormatColor(Color color) {

                return String.Format(
                    "{0}, {1}, {2}, {3}", 
                    XmlConvert.ToString(color.A),
                    XmlConvert.ToString(color.R),
                    XmlConvert.ToString(color.G),
                    XmlConvert.ToString(color.B));
            }

            /// <summary>
            /// Formateja un valoe a poercentatge
            /// </summary>
            /// <param name="ratio">El valor del percentatge.</param>
            /// <returns>El valor formatejat.</returns>
            /// 
            private static string FormRatio(Ratio ratio) {

                return XmlConvert.ToString(ratio.Percent / 1000.0);
            }
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="stream">Stream de sortida.</param>
        /// 
        public XmlBoardWriter(Stream stream) {

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

                IVisitor visitor = new Visitor(board, writer);
                visitor.Run();

                writer.WriteEndDocument();
            }
        }
    }
}
