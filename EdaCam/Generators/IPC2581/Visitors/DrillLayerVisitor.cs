using System.Xml;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Visitors {

    internal sealed class DrillLayerVisitor: EdaElementVisitor {

        private const double _scale = 1000000.0;

        private readonly EdaLayerId _layerId;
        private readonly DataCache _cache;
        private readonly XmlWriter _writer;
        private int _holeNumber = 0;

        public DrillLayerVisitor(EdaLayerId layerId, DataCache cache, XmlWriter writer) {

            _layerId = layerId;
            _cache = cache;
            _writer = writer;
        }

        public override void Visit(EdaViaElement element) {

            if (element.IsOnLayer(_layerId)) {
                _writer.WriteStartElement("Set");
                _writer.WriteStartElement("Hole");
                _writer.WriteAttributeString("name", string.Format("H{0}", _holeNumber++));
                _writer.WriteAttributeString("platingStatus", "VIA");
                _writer.WriteAttributeDouble("x", element.Position.X / _scale);
                _writer.WriteAttributeDouble("y", element.Position.Y / _scale);
                _writer.WriteAttributeDouble("diameter", element.DrillDiameter / _scale);
                _writer.WriteAttributeDouble("plusTol", 0);
                _writer.WriteAttributeDouble("minusTol", 0);
                _writer.WriteEndElement();
                _writer.WriteEndElement();
            }
        }

        public override void Visit(EdaThtPadElement element) {

            if (element.IsOnLayer(_layerId)) {

                var tr = Part.GetLocalTransformation();
                var position = tr.Transform(element.Position);

                _writer.WriteStartElement("Set");
                _writer.WriteStartElement("Hole");
                _writer.WriteAttributeString("name", string.Format("H{0}", _holeNumber++));
                _writer.WriteAttributeString("platingStatus", "VIA");
                _writer.WriteAttributeDouble("x", position.X / _scale);
                _writer.WriteAttributeDouble("y", position.Y / _scale);
                _writer.WriteAttributeDouble("diameter", element.DrillDiameter / _scale);
                _writer.WriteAttributeDouble("plusTol", 0);
                _writer.WriteAttributeDouble("minusTol", 0);
                _writer.WriteEndElement();
                _writer.WriteEndElement();
            }
        }
    }
}
