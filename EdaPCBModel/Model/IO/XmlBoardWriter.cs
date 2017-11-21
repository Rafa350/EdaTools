namespace MikroPic.EdaTools.v1.Model.IO {

    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using MikroPic.EdaTools.v1.Model.Elements;
    
    public sealed class XmlBoardWriter {

        private Stream stream;

        private class Visitor: DefaultVisitor {

            private XmlWriter writer;
            private readonly CultureInfo ci = CultureInfo.InvariantCulture;

            public Visitor(XmlWriter writer) {

                this.writer = writer;
            }

            public override void Visit(LineElement line) {

                writer.WriteStartElement("line");
                writer.WriteAttributeString("layer", line.Layer.Name);
                writer.WriteAttribute("startPosition", line.StartPosition);
                writer.WriteAttribute("endPosition", line.EndPosition);
                if (line.Thickness > 0)
                    writer.WriteAttribute("thickness", line.Thickness);
                writer.WriteAttributeString("lineCap", line.LineCap.ToString());
                writer.WriteEndElement();
            }

            public override void Visit(ArcElement arc) {

                writer.WriteStartElement("arc");
                writer.WriteAttributeString("layer", arc.Layer.Name);
                writer.WriteAttribute("startPosition", arc.StartPosition);
                writer.WriteAttribute("endPosition", arc.EndPosition);
                writer.WriteAttribute("angle", arc.Angle);
                if (arc.Thickness > 0)
                    writer.WriteAttribute("thickness", arc.Thickness);
                writer.WriteAttributeString("lineCap", arc.LineCap.ToString());
                writer.WriteEndElement();
            }

            public override void Visit(RectangleElement rectangle) {

                writer.WriteStartElement("rectangle");
                writer.WriteAttributeString("layer", rectangle.Layer.Name);
                writer.WriteAttribute("position", rectangle.Position);
                writer.WriteAttribute("size", rectangle.Size);
                if (rectangle.Rotate > 0)
                    writer.WriteAttribute("rotate", rectangle.Rotate);
                if (rectangle.Thickness > 0)
                    writer.WriteAttribute("thickness", rectangle.Thickness);
                writer.WriteEndElement();
            }

            public override void Visit(CircleElement circle) {

                writer.WriteStartElement("circle");
                writer.WriteAttributeString("layer", circle.Layer.Name);
                writer.WriteAttribute("position", circle.Position);
                writer.WriteAttribute("radius", circle.Radius);
                if (circle.Thickness > 0)
                    writer.WriteAttribute("thickness", circle.Thickness);
                writer.WriteEndElement();
            }

            public override void Visit(TextElement text) {

                writer.WriteStartElement("text");
                if (!String.IsNullOrEmpty(text.Name))
                    writer.WriteAttributeString("name", text.Name);
                writer.WriteAttributeString("layer", text.Layer.Name);
                writer.WriteAttribute("position", text.Position);
                if (text.Rotate != 0)
                    writer.WriteAttribute("rotate", text.Rotate);
                if (!String.IsNullOrEmpty(text.Value))
                    writer.WriteAttributeString("value", text.Value);
                writer.WriteEndElement();
            }

            public override void Visit(HoleElement hole) {

                writer.WriteStartElement("hole");
                writer.WriteAttribute("position", hole.Position);
                writer.WriteAttribute("drill", hole.Drill);
                writer.WriteEndElement();
            }

            public override void Visit(SmdPadElement pad) {

                writer.WriteStartElement("spad");
                writer.WriteAttributeString("name", pad.Name);
                writer.WriteAttributeString("layer", pad.Layer.Name);
                writer.WriteAttribute("position", pad.Position);
                if (pad.Rotate != 0)
                    writer.WriteAttribute("rotate", pad.Rotate);
                writer.WriteAttribute("size", pad.Size);
                if (pad.Roundnes > 0)
                    writer.WriteAttribute("roundness", pad.Roundnes);
                writer.WriteEndElement();
            }

            public override void Visit(ThPadElement pad) {

                writer.WriteStartElement("tpad");
                writer.WriteAttributeString("name", pad.Name);
                writer.WriteAttribute("position", pad.Position);
                if (pad.Rotate != 0)
                    writer.WriteAttribute("rotate", pad.Rotate);
                writer.WriteAttribute("size", pad.Size);
                writer.WriteAttribute("drill",  pad.Drill);
                writer.WriteAttributeString("shape", pad.Shape.ToString());
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

            public override void Visit(PolygonElement polygon) {

                writer.WriteStartElement("polygon");
                writer.WriteAttributeString("layer", polygon.Layer.Name);
                writer.WriteAttribute("position", polygon.Position);
                if (polygon.Thickness > 0)
                    writer.WriteAttribute("thickness", polygon.Thickness);
                foreach (PolygonElement.Segment node in polygon.Nodes) {
                    writer.WriteStartElement("segment");
                    writer.WriteAttribute("position", node.Delta);
                    if (node.Angle != 0)
                        writer.WriteAttribute("angle", node.Angle);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            public override void Visit(Part part) {

                writer.WriteStartElement("part");
                writer.WriteAttributeString("name", part.Name);
                writer.WriteAttributeString("component", part.Component.Name);
                writer.WriteAttribute("position", part.Position);
                if (part.Rotate != 0)
                    writer.WriteAttribute("rotate", part.Rotate);
                if (part.IsMirror)
                    writer.WriteAttributeString("mirror", part.IsMirror.ToString());

                if (part.Parameters != null) {
                    writer.WriteStartElement("attributes");
                    foreach (Parameter parameter in part.Parameters)
                        parameter.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            public override void Visit(ViaElement via) {

                writer.WriteStartElement("via");
                writer.WriteAttribute("position", via.Position);
                writer.WriteAttribute("drill", via.Drill);
                writer.WriteAttribute("size", via.Size);
                writer.WriteAttributeString("shape", via.Shape.ToString());
                writer.WriteEndElement();
            }

            public override void Visit(Signal signal) {
                
                writer.WriteStartElement("signal");
                writer.WriteAttributeString("name", signal.Name);

                if (signal.Elements != null) {
                    writer.WriteStartElement("elements");
                    foreach (ElementBase element in signal.Elements) 
                        element.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            public override void Visit(Component component) {

                writer.WriteStartElement("component");
                writer.WriteAttributeString("name", component.Name);

                if (component.Elements != null) {
                    writer.WriteStartElement("elements");
                    foreach (ElementBase element in component.Elements)
                        element.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            public override void Visit(Layer layer) {

                writer.WriteStartElement("layer");
                writer.WriteAttributeString("name", layer.Name);
                writer.WriteAttributeString("visible", layer.IsVisible.ToString());
                writer.WriteAttributeString("color", layer.Color.ToString());
                writer.WriteEndElement();
            }

            public override void Visit(Board board) {

                writer.WriteStartElement("board");

                if (board.Layers != null) {
                    writer.WriteStartElement("layers");
                    foreach (Layer layer in board.Layers)
                        layer.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                if (board.Components != null) {
                    writer.WriteStartElement("components");
                    foreach (Component component in board.Components)
                        component.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                if (board.Parts != null) {
                    writer.WriteStartElement("parts");
                    foreach (Part part in board.Parts)
                        part.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                if (board.Signals != null) {
                    writer.WriteStartElement("signals");
                    foreach (Signal signal in board.Signals)
                        signal.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        public XmlBoardWriter(Stream stream) {

            this.stream = stream;
        }

        public void Write(Board board) {

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.CloseOutput = true;

            using (XmlWriter writer = XmlWriter.Create(stream, settings)) {

                writer.WriteStartDocument();

                IVisitor visitor = new Visitor(writer);
                board.AcceptVisitor(visitor);

                writer.WriteEndDocument();
            }
        }
    }
}
