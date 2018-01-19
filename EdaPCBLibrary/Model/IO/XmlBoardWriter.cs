﻿namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    public sealed class XmlBoardWriter {

        private Stream stream;

        private class Visitor: DefaultVisitor {

            private readonly XmlWriter writer;
            private readonly CultureInfo ci = CultureInfo.InvariantCulture;
            private Board board;
            private Part currentPart = null;

            /// <summary>
            /// Constructor del objecte. Visita els obectes d'una placa,
            /// per generar el stream de sortida.
            /// </summary>
            /// <param name="writer">Objecte per escriure el stream de sortida.</param>
            /// 
            public Visitor(XmlWriter writer) {

                this.writer = writer;
            }

            /// <summary>
            /// Visita un element de tipus linea
            /// </summary>
            /// <param name="line">L'objecte a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                writer.WriteStartElement("line");

                writer.WriteAttribute("layers", board.GetLayers(line));
                writer.WriteAttribute("startPosition", line.StartPosition);
                writer.WriteAttribute("endPosition", line.EndPosition);
                if (line.Thickness > 0)
                    writer.WriteAttribute("thickness", line.Thickness);
                if (line.LineCap != LineElement.LineCapStyle.Round)
                    writer.WriteAttributeString("lineCap", line.LineCap.ToString());

                Signal signal = board.GetSignal(line, currentPart, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus arc.
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                writer.WriteStartElement("arc");

                writer.WriteAttribute("layers", board.GetLayers(arc));
                writer.WriteAttribute("startPosition", arc.StartPosition);
                writer.WriteAttribute("endPosition", arc.EndPosition);
                writer.WriteAttribute("angle", arc.Angle);
                if (arc.Thickness > 0)
                    writer.WriteAttribute("thickness", arc.Thickness);
                writer.WriteAttributeString("lineCap", arc.LineCap.ToString());

                Signal signal = board.GetSignal(arc, currentPart, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus rectangle.
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                writer.WriteStartElement("rectangle");

                writer.WriteAttribute("layers", board.GetLayers(rectangle));
                writer.WriteAttribute("position", rectangle.Position);
                writer.WriteAttribute("size", rectangle.Size);
                if (rectangle.Rotation != 0)
                    writer.WriteAttribute("rotation", rectangle.Rotation);
                if (rectangle.Thickness > 0)
                    writer.WriteAttribute("thickness", rectangle.Thickness);

                writer.WriteEndElement();
            }

            public override void Visit(CircleElement circle) {

                writer.WriteStartElement("circle");

                writer.WriteAttribute("layers", board.GetLayers(circle));
                writer.WriteAttribute("position", circle.Position);
                writer.WriteAttribute("radius", circle.Radius);
                if (circle.Thickness > 0)
                    writer.WriteAttribute("thickness", circle.Thickness);

                writer.WriteEndElement();
            }

            public override void Visit(TextElement text) {

                writer.WriteStartElement("text");

                writer.WriteAttribute("layers", board.GetLayers(text));
                writer.WriteAttribute("position", text.Position);
                if (text.Rotation != 0)
                    writer.WriteAttribute("rotation", text.Rotation);
                if (!String.IsNullOrEmpty(text.Value))
                    writer.WriteAttributeString("value", text.Value);

                writer.WriteEndElement();
            }

            public override void Visit(HoleElement hole) {

                writer.WriteStartElement("hole");

                writer.WriteAttribute("layers", board.GetLayers(hole));
                writer.WriteAttribute("position", hole.Position);
                writer.WriteAttribute("drill", hole.Drill);

                writer.WriteEndElement();
            }

            public override void Visit(SmdPadElement pad) {

                writer.WriteStartElement("spad");

                writer.WriteAttributeString("name", pad.Name);
                writer.WriteAttribute("layers", board.GetLayers(pad));
                writer.WriteAttribute("position", pad.Position);
                if (pad.Rotation != 0)
                    writer.WriteAttribute("rotation", pad.Rotation);
                writer.WriteAttribute("size", pad.Size);
                if (pad.Roundnes > 0)
                    writer.WriteAttribute("roundness", pad.Roundnes);

                Signal signal = board.GetSignal(pad, currentPart, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                writer.WriteEndElement();
            }

            public override void Visit(ThPadElement pad) {

                writer.WriteStartElement("tpad");

                writer.WriteAttributeString("name", pad.Name);
                writer.WriteAttribute("layers", board.GetLayers(pad));
                writer.WriteAttribute("position", pad.Position);
                if (pad.Rotation != 0)
                    writer.WriteAttribute("rotation", pad.Rotation);
                writer.WriteAttribute("size", pad.Size);
                writer.WriteAttribute("drill",  pad.Drill);
                if (pad.Shape != ThPadElement.ThPadShape.Circular)
                    writer.WriteAttributeString("shape", pad.Shape.ToString());

                Signal signal = board.GetSignal(pad, currentPart, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                writer.WriteEndElement();
            }

            public override void Visit(Parameter parameter) {

                writer.WriteStartElement("attribute");

                writer.WriteAttributeString("name", parameter.Name);
                if ((parameter.Position.X != 0) || (parameter.Position.Y != 0))
                    writer.WriteAttribute("position", parameter.Position);
                if (parameter.Rotate != 0)
                    writer.WriteAttribute("rotate", parameter.Rotate);
                writer.WriteAttributeString("value", parameter.Value);

                writer.WriteEndElement();
            }

            public override void Visit(RegionElement region) {

                writer.WriteStartElement("region");

                writer.WriteAttribute("layers", board.GetLayers(region));
                if (region.Thickness > 0)
                    writer.WriteAttribute("thickness", region.Thickness);
                if (region.Isolation > 0)
                    writer.WriteAttribute("isolation", region.Isolation);
                Signal signal = board.GetSignal(region, currentPart, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                foreach (RegionElement.Segment segment in region.Segments) {
                    writer.WriteStartElement("segment");
                    writer.WriteAttribute("position", segment.Position);
                    if (segment.Angle != 0)
                        writer.WriteAttribute("angle", segment.Angle);
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
                    writer.WriteAttributeString("name", part.Name);
                    writer.WriteAttributeString("block", part.Block.Name);
                    writer.WriteAttribute("position", part.Position);
                    if (part.Rotation != 0)
                        writer.WriteAttribute("rotation", part.Rotation);
                    if (part.IsFlipped)
                        writer.WriteAttributeString("flipped", part.IsFlipped.ToString());

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
                            writer.WriteAttributeString("name", pad.Name);
                            writer.WriteAttributeString("signal", signal.Name);
                            writer.WriteEndElement();
                        }
                    }
                    if (!first)
                        writer.WriteEndElement();

                    // Escriu la llista d'atributs.
                    //
                    first = true;
                    foreach (Parameter parameter in part.Parameters) {
                        if (first) {
                            first = false;
                            writer.WriteStartElement("attributes");
                        }
                        parameter.AcceptVisitor(this);
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

                writer.WriteAttribute("position", via.Position);
                writer.WriteAttribute("layers", board.GetLayers(via));
                writer.WriteAttribute("drill", via.Drill);
                writer.WriteAttribute("outerSize", via.OuterSize);
                if (via.InnerSize != via.OuterSize)
                    writer.WriteAttribute("innerSize", via.InnerSize);
                if (via.Shape != ViaElement.ViaShape.Circular)
                    writer.WriteAttributeString("shape", via.Shape.ToString());
                if (via.Type != ViaElement.ViaType.Through)
                    writer.WriteAttributeString("type", via.Type.ToString());

                Signal signal = board.GetSignal(via, null, false);
                if (signal != null)
                    writer.WriteAttributeString("signal", signal.Name);

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita una capa.
            /// </summary>
            /// <param name="layer">La capa a visitar.</param>
            /// 
            public override void Visit(Layer layer) {

                writer.WriteStartElement("layer");

                writer.WriteAttributeString("name", layer.Name);
                writer.WriteAttributeString("id", layer.Id.ToString());
                writer.WriteAttributeString("side", layer.Side.ToString());
                writer.WriteAttributeString("function", layer.Function.ToString());
                writer.WriteAttribute("color", layer.Color);
                if (!layer.IsVisible)
                    writer.WriteAttributeString("visible", "false");

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita una senyal.
            /// </summary>
            /// <param name="signal">La senyal a visitar.</param>
            /// 
            public override void Visit(Signal signal) {
                
                writer.WriteStartElement("signal");

                writer.WriteAttributeString("name", signal.Name);

                writer.WriteEndElement();
            }

            public override void Visit(Block block) {

                writer.WriteStartElement("block");

                writer.WriteAttributeString("name", block.Name);

                writer.WriteStartElement("elements");
                foreach (Element element in block.Elements)
                    element.AcceptVisitor(this);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            public override void Visit(Board board) {

                this.board = board;

                writer.WriteStartElement("board");

                writer.WriteAttributeString("version", "210");
                writer.WriteAttributeString("units", "mm");

                writer.WriteStartElement("layers");
                foreach (Layer layer in board.Layers)
                    layer.AcceptVisitor(this);
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
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="stream">Stream de sortida.</param>
        /// 
        public XmlBoardWriter(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

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

                IVisitor visitor = new Visitor(writer);
                visitor.Visit(board);

                writer.WriteEndDocument();
            }
        }
    }
}
