using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Visitors {

    internal class SolderMaskLayerVisitor: EdaElementVisitor {

        private const double _scale = 1000000.0;

        private readonly EdaLayerId _layerId;
        private readonly DataCache _cache;
        private readonly XmlWriter _writer;

        public SolderMaskLayerVisitor(EdaLayerId layerId, DataCache cache, XmlWriter writer) {

            _layerId = layerId;
            _cache = cache;
            _writer = writer;
        }

        public override void Visit(EdaSmtPadElement element) {

            if (element.IsOnLayer(_layerId)) {

                var size = element.Size.Inflated(element.MaskClearance);
                var rectRoundEntry = _cache.GetRectRoundEntry(size, element.CornerRatio);
                var tr = Part == null ? new EdaTransformation() : Part.GetLocalTransformation();
                var rotation = Part == null ? EdaAngle.Zero : Part.Rotation;
                var location = tr.Transform(element.Position);

                WritePad(rectRoundEntry.Id, location, rotation);
            }
        }

        public override void Visit(EdaThtPadElement element) {

            if (element.IsOnLayer(_layerId)) {

                var size =
                    _layerId.IsTop ? element.TopSize :
                    _layerId.IsBottom ? element.BottomSize :
                    element.InnerSize;
                var maskSize = size.Inflated(element.MaskClearance);
                var rectRoundEntry = _cache.GetRectRoundEntry(maskSize, element.CornerRatio);
                var tr = Part == null ? new EdaTransformation() : Part.GetLocalTransformation();
                var rotation = Part == null ? EdaAngle.Zero : Part.Rotation;
                var location = tr.Transform(element.Position);

                WritePad(rectRoundEntry.Id, location, rotation);
            }
        }

        private void WritePad(int entryId, EdaPoint location, EdaAngle rotation) {

            _writer.WriteStartElement("Set");
            _writer.WriteStartElement("Pad");
            if (!Part.Rotation.IsZero) {
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
