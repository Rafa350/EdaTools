using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Visitors {

    internal sealed class PackageVisitor: EdaDefaultBoardVisitor {

        private const double _scale = 1000000.0;

        private readonly XmlWriter _writer;
        private readonly DataCache _cache;

        public PackageVisitor(DataCache cache, XmlWriter writer) {

            _writer = writer;
            _cache = cache;
        }

        public override void Visit(EdaBoard board) {

            foreach (var component in board.Components)
                component.AcceptVisitor(this);
        }

        public override void Visit(EdaComponent component) {

            _writer.WriteStartElement("Package");
            _writer.WriteAttributeString("name", component.Name);
            _writer.WriteAttributeString("type", "OTHER");

            foreach (var pad in component.Pads())
                pad.AcceptVisitor(this);

            _writer.WriteEndElement();
        }

        public override void Visit(EdaSmtPadElement element) {

            var flat = element.CornerShape == EdaSmtPadElement.CornerShapeType.Flat;
            var rectEntry = _cache.GetRectEntry(element.Size, element.CornerRatio, flat);

            _writer.WriteStartElement("Pin");
            _writer.WriteAttributeString("number", element.Name);
            _writer.WriteAttributeString("type", "SURFACE");
            _writer.WriteAttributeString("electricalType", "ELECTRICAL");
            if (!element.Rotation.IsZero) {
                _writer.WriteStartElement("Xform");
                _writer.WriteAttributeAngle("rotation", element.Rotation);
                _writer.WriteEndElement(); // Xform
            }
            _writer.WritePointElement("Location", element.Position, _scale);
            _writer.WriteStartElement("StandardPrimitiveRef");
            _writer.WriteAttributeInteger("id", rectEntry.Id);
            _writer.WriteEndElement();
            _writer.WriteEndElement();
        }

        public override void Visit(EdaThtPadElement element) {

            return;

            var flat = element.CornerShape == EdaThtPadElement.CornerShapeType.Flat;
            var rectEntry = _cache.GetRectEntry(element.TopSize, element.CornerRatio, flat);

            _writer.WriteStartElement("Pin");
            _writer.WriteAttributeString("number", element.Name);
            _writer.WriteAttributeString("type", "THRU");
            _writer.WriteAttributeString("electricalType", "ELECTRICAL");
            _writer.WritePointElement("Location", element.Position, _scale);
            _writer.WriteStartElement("StandardPrimitiveRef");
            _writer.WriteAttributeInteger("id", rectEntry.Id);
            _writer.WriteEndElement();
            _writer.WriteEndElement();
        }
    }
}
