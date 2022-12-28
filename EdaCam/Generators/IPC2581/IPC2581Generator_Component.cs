using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed partial class IPC2581Generator {

        /// <summary>
        /// Escriu la la seccio 'Component'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Component(EdaBoard board) {

            foreach (var part in board.Parts) {

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
}
