using System.Xml;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Visitors {

    internal sealed class ComponentVisitor: EdaDefaultBoardVisitor {

        private const double _scale = 1000000.0;

        private readonly XmlWriter _writer;

        public ComponentVisitor(XmlWriter writer) {

            _writer = writer;
        }

        public override void Visit(EdaBoard board) {

            foreach (var part in board.Parts)
                part.AcceptVisitor(this);
        }

        public override void Visit(EdaPart part) {

            _writer.WriteStartElement("Component");
            _writer.WriteAttributeString("refDes", part.Name);
            _writer.WriteAttributeString("layerRef", part.Side == PartSide.Top ? "top" : "bottom");
            _writer.WriteAttributeString("packageRef", part.Component.Name);
            var mpn = part.GetAttribute("MPN");
            if (mpn != null)
                _writer.WriteAttributeString("part", mpn.Value);
            if (!part.Rotation.IsZero) {
                _writer.WriteStartElement("Xform");
                _writer.WriteAttributeDouble("rotation", part.Rotation.AsDegrees);
                _writer.WriteEndElement(); // Xform
            }
            _writer.WritePointElement("Location", part.Position, _scale);
            _writer.WriteEndElement();
        }
    }
}

