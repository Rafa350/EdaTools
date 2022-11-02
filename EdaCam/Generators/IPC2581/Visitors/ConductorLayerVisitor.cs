using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Visitors {

    internal sealed class ConductorLayerVisitor: EdaElementVisitor {

        private const double _scale = 1000000.0;

        private readonly EdaLayerId _layerId;
        private readonly DataCache _cache;
        private readonly XmlWriter _writer;

        public ConductorLayerVisitor(EdaLayerId layerId, DataCache cache, XmlWriter writer) {

            _layerId = layerId;
            _cache = cache;
            _writer = writer;
        }

        public override void Visit(EdaLineElement element) {

            if (element.IsOnLayer(_layerId)) {

                var lineDescEntry = _cache.GetLineDescEntry(element.Thickness, element.LineCap);
                var signal = Board.GetSignal(element, null, false);

                _writer.WriteStartElement("Set");
                if (signal != null)
                    _writer.WriteAttributeString("net", signal.Name);
                _writer.WriteStartElement("Features");
                _writer.WriteStartElement("Line");
                _writer.WriteAttributeDouble("startX", element.StartPosition.X / _scale);
                _writer.WriteAttributeDouble("startY", element.StartPosition.Y / _scale);
                _writer.WriteAttributeDouble("endX", element.EndPosition.X / _scale);
                _writer.WriteAttributeDouble("endY", element.EndPosition.Y / _scale);
                _writer.WriteStartElement("LineDescRef");
                _writer.WriteAttributeInteger("id", lineDescEntry.Id);
                _writer.WriteEndElement(); // LineDescRef                    
                _writer.WriteEndElement(); // Line                    
                _writer.WriteEndElement(); // Features                    
                _writer.WriteEndElement(); // Set
            }
        }

        public override void Visit(EdaArcElement element) {
        }

        public override void Visit(EdaSmtPadElement element) {

            if (element.IsOnLayer(_layerId)) {

                var flat = element.CornerShape == EdaSmtPadElement.CornerShapeType.Flat;
                var rectEntry = _cache.GetRectEntry(element.Size, element.CornerRatio, flat);
                var signal = Board.GetSignal(element, Part, false);
                var tr = Part == null ? new EdaTransformation() : Part.GetLocalTransformation();
                var rotation = Part == null ? EdaAngle.Zero : Part.Rotation;
                var location = tr.Transform(element.Position);

                WritePad(rectEntry.Id, signal, location, rotation);
            }
        }

        public override void Visit(EdaThtPadElement element) {

            if (element.IsOnLayer(_layerId)) {

                var size =
                    _layerId.IsTop ? element.TopSize :
                    _layerId.IsBottom ? element.BottomSize :
                    element.InnerSize;
                var flat = element.CornerShape == EdaThtPadElement.CornerShapeType.Flat;
                var rectEntry = _cache.GetRectEntry(size, element.CornerRatio, flat);
                var signal = Board.GetSignal(element, Part, false);
                var tr = Part == null ? new EdaTransformation() : Part.GetLocalTransformation();
                var rotation = Part == null ? EdaAngle.Zero : Part.Rotation;
                var location = tr.Transform(element.Position);

                WritePad(rectEntry.Id, signal, location, rotation);
            }
        }

        public override void Visit(EdaViaElement element) {

            if (element.IsOnLayer(_layerId)) {

                var circleEntry = _cache.GetCircleEntry(element.OuterSize);
                var signal = Board.GetSignal(element, null, false);
                var location = element.Position;

                _writer.WriteStartElement("Set");
                if (signal != null)
                    _writer.WriteAttributeString("net", signal.Name);
                _writer.WriteAttributeString("padUsage", "VIA");
                _writer.WriteStartElement("Pad");
                _writer.WritePointElement("Location", location, _scale);
                _writer.WriteStartElement("StandardPrimitiveRef");
                _writer.WriteAttributeInteger("id", circleEntry.Id);
                _writer.WriteEndElement(); // StandardPrimitiveRef
                _writer.WriteEndElement(); // Pad
                _writer.WriteEndElement(); // Set
            }
        }

        public override void Visit(EdaRegionElement element) {

            if (element.IsOnLayer(_layerId)) {

                //var lineDescEntry = _dataCache.GetLineDescEntry(element.Thickness);
                var fillDescEntry = _cache.GetFillDescEntry(true);
                var signal = Board.GetSignal(element, null, false);

                _writer.WriteStartElement("Set");
                if (signal != null)
                    _writer.WriteAttributeString("net", signal.Name);
                _writer.WriteStartElement("Features");

                var polygons = Board.GetRegionPolygons(element, _layerId, null);
                foreach (var polygon in polygons) {
                    _writer.WriteStartElement("Contour");
                    _writer.WritePolygonElement(polygon, fillDescEntry.Id, _scale);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement(); // Features
                _writer.WriteEndElement(); // Set
            }
        }

        private void WritePad(int entryId, EdaSignal signal, EdaPoint location, EdaAngle rotation) {

            _writer.WriteStartElement("Set");
            if (signal != null)
                _writer.WriteAttributeString("net", signal.Name);
            _writer.WriteStartElement("Pad");
            if (!rotation.IsZero) {
                _writer.WriteStartElement("Xform");
                _writer.WriteAttributeDouble("rotation", rotation.AsDegrees);
                _writer.WriteEndElement(); // Xform
            }
            _writer.WritePointElement("Location", location, _scale);
            _writer.WriteStartElement("StandardPrimitiveRef");
            _writer.WriteAttributeInteger("id", entryId);
            _writer.WriteEndElement(); // StandardPrimitiveRef                
            _writer.WriteEndElement(); // Pad                
            _writer.WriteEndElement(); // Set
        }
    }

}
