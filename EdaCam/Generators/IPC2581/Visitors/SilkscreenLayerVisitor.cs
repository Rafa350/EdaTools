using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Xml;
using MikroPic.EdaTools.v1.Core.Infrastructure;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Visitors {

    internal sealed class SilkscreenLayerVisitor: EdaElementVisitor {

        private const double _scale = 1000000.0;

        private readonly EdaLayerId _layerId;
        private readonly DataCache _cache;
        private readonly XmlWriter _writer;
        private readonly Font _font;

        private sealed class Drawer: TextDrawer {

            private readonly XmlWriter _writer;
            private readonly int _id;
            private readonly EdaAngle _rotation;

            public Drawer(Font font, XmlWriter writer, int id, EdaAngle rotation) :
                base(font) {

                _writer = writer;
                _id = id;
                _rotation = rotation;
            }

            protected override void Trace(EdaPoint position, bool stroke, bool first) {

                if (first)
                    _writer.WriteStartElement("PolyBegin");
                else if (!stroke) {
                    TraceEndGlyph();
                    TraceStartGlyph();
                    _writer.WriteStartElement("PolyBegin");
                }
                else
                    _writer.WriteStartElement("PolyStepSegment");
                _writer.WriteAttributeDouble("x", position.X / _scale);
                _writer.WriteAttributeDouble("y", position.Y / _scale);
                _writer.WriteEndElement();
            }

            protected override void TraceStartGlyph() {

                _writer.WriteStartElement("Features");
                _writer.WriteStartElement("Polyline");
            }

            protected override void TraceEndGlyph() {

                _writer.WriteStartElement("LineDescRef");
                _writer.WriteAttributeInteger("id", _id);
                _writer.WriteEndElement();

                _writer.WriteEndElement(); // Polyline
                _writer.WriteEndElement(); // Features                    
            }
        }

        public SilkscreenLayerVisitor(EdaLayerId layerId, DataCache cache, XmlWriter writer) {

            _layerId = layerId;
            _cache = cache;
            _writer = writer;
            _font = FontFactory.Instance.GetFont("Standard");
        }

        public override void Visit(EdaLineElement element) {

            if (element.IsOnLayer(_layerId)) {

                var lineDescEntry = _cache.GetLineDescEntry(element.Thickness, element.LineCap);
                var tr = Part == null ? new EdaTransformation() : Part.GetLocalTransformation();
                var startPosition = tr.Transform(element.StartPosition);
                var endPosition = tr.Transform(element.EndPosition);

                _writer.WriteStartElement("Set");
                _writer.WriteStartElement("Features");
                _writer.WriteStartElement("Line");
                _writer.WriteAttributeDouble("startX", startPosition.X / _scale);
                _writer.WriteAttributeDouble("startY", startPosition.Y / _scale);
                _writer.WriteAttributeDouble("endX", endPosition.X / _scale);
                _writer.WriteAttributeDouble("endY", endPosition.Y / _scale);
                _writer.WriteStartElement("LineDescRef");
                _writer.WriteAttributeInteger("id", lineDescEntry.Id);
                _writer.WriteEndElement(); // LineDescRef                    
                _writer.WriteEndElement(); // Line                    
                _writer.WriteEndElement(); // Features                    
                _writer.WriteEndElement(); // Set
            }
        }

        public override void Visit(EdaArcElement element) {

            if (element.IsOnLayer(_layerId)) {

                var lineDescEntry = _cache.GetLineDescEntry(element.Thickness, element.LineCap);
                var tr = Part == null ? new EdaTransformation() : Part.GetLocalTransformation();
                var startPosition = tr.Transform(element.StartPosition);
                var endPosition = tr.Transform(element.EndPosition);
                var centerPosition = tr.Transform(element.Center);

                _writer.WriteStartElement("Set");
                _writer.WriteStartElement("Features");
                _writer.WriteStartElement("Arc");
                _writer.WriteAttributeDouble("startX", startPosition.X / _scale);
                _writer.WriteAttributeDouble("startY", startPosition.Y / _scale);
                _writer.WriteAttributeDouble("endX", endPosition.X / _scale);
                _writer.WriteAttributeDouble("endY", endPosition.Y / _scale);
                _writer.WriteAttributeDouble("centerX", centerPosition.X / _scale);
                _writer.WriteAttributeDouble("centerY", centerPosition.Y / _scale);
                _writer.WriteAttributeBool("clockwise", false);
                _writer.WriteStartElement("LineDescRef");
                _writer.WriteAttributeInteger("id", lineDescEntry.Id);
                _writer.WriteEndElement(); // LineDescRef                    
                _writer.WriteEndElement(); // Arc
                _writer.WriteEndElement(); // Features                    
                _writer.WriteEndElement(); // Set
            }
        }

        public override void Visit(EdaCircleElement element) {

            if (element.IsOnLayer(_layerId)) {

                var tr = Part == null ? new EdaTransformation() : Part.GetLocalTransformation();
                var position = tr.Transform(element.Position);

                var polygon = element.GetPolygon(_layerId);

                _writer.WriteStartElement("Set");
                _writer.WriteStartElement("Features");
                _writer.WritePointElement("Location", position, _scale);
                _writer.WriteStartElement("Circle");
                _writer.WriteAttributeDouble("diameter", element.Diameter / _scale);
                if (element.Filled) {
                    var fillDescEntry = _cache.GetFillDescEntry(true);
                    _writer.WriteStartElement("FillDescRef");
                    _writer.WriteAttributeInteger("id", fillDescEntry.Id);
                    _writer.WriteEndElement();
                }
                else {
                    var lineDescEntry = _cache.GetLineDescEntry(element.Thickness, EdaLineCap.Flat);
                    _writer.WriteStartElement("LineDescRef");
                    _writer.WriteAttributeInteger("id", lineDescEntry.Id);
                    _writer.WriteEndElement();
                }
                _writer.WriteEndElement(); // Circle
                _writer.WriteEndElement(); // Features                    
                _writer.WriteEndElement(); // Set
            }
        }

        public override void Visit(EdaRectangleElement element) {

            if (element.IsOnLayer(_layerId)) {

            }
        }

        public override void Visit(EdaTextElement element) {

            if (element.IsOnLayer(_layerId)) {

                var paa = new PartAttributeAdapter(Part, element);
                if (paa.IsVisible) {

                    var tr = Part == null ? new EdaTransformation() : Part.GetLocalTransformation();
                    var position = tr.Transform(paa.Position);

                    var rotation = (Part == null ? EdaAngle.Zero : Part.Rotation) + paa.Rotation;

                    _writer.WriteStartElement("Set");
                    _writer.WriteAttributeString("geometricUsage", "TEXT");
                    _writer.WriteStartElement("NonStandardAttribute");
                    _writer.WriteAttributeString("name", "TEXT");
                    _writer.WriteAttributeString("type", "STRING");
                    _writer.WriteAttributeString("value", paa.Value);
                    _writer.WriteEndElement();

                    var lineDescEntry = _cache.GetLineDescEntry(element.Thickness, EdaLineCap.Round);
                    var drawer = new Drawer(_font, _writer, lineDescEntry.Id, rotation);
                    drawer.Draw(paa.Value, position, element.HorizontalAlign, element.VerticalAlign, element.Height);

                    _writer.WriteEndElement(); // Set
                }
            }
        }
    }
}
