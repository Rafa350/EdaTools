using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed partial class IPC2581Generator {

        /// <summary>
        /// Escriu la seccio 'Package'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Package(EdaBoard board) {

            foreach (var component in board.Components) {

                _writer.WriteStartElement("Package");
                _writer.WriteAttributeString("name", component.Name);
                _writer.WriteAttributeString("type", "OTHER");

                foreach (var pad in component.Pads()) {

                    _writer.WriteStartElement("Pin");
                    _writer.WriteAttributeString("number", pad.Name);
                    _writer.WriteAttributeString("type", pad is EdaThtPadElement ? "THRU" : "SURFACE");
                    _writer.WriteAttributeString("electricalType", "ELECTRICAL");

                    if (!pad.Rotation.IsZero) {
                        _writer.WriteStartElement("Xform");
                        _writer.WriteAttributeDouble("rotation", pad.Rotation.AsDegrees);
                        _writer.WriteEndElement(); // Xform
                    }

                    _writer.WritePointElement("Location", pad.Position, _scale);

                    int entryId = -1;
                    if (pad is EdaThtPadElement thtPad) {
                        var flat = thtPad.CornerShape == EdaThtPadElement.CornerShapeType.Flat;
                        entryId = _dataCache.GetRectEntry(thtPad.TopSize, thtPad.CornerRatio, flat).Id;
                    }
                    else if (pad is EdaSmtPadElement smdPad) {
                        var flat = smdPad.CornerShape == EdaSmtPadElement.CornerShapeType.Flat;
                        entryId = _dataCache.GetRectEntry(smdPad.Size, smdPad.CornerRatio, flat).Id;
                    }
                    if (entryId != -1) {
                        _writer.WriteStartElement("StandardPrimitiveRef");
                        _writer.WriteAttributeInteger("id", entryId);
                        _writer.WriteEndElement();
                    }

                    _writer.WriteEndElement(); // Pin
                }

                _writer.WriteEndElement(); // Package
            }
        }
    }
}

