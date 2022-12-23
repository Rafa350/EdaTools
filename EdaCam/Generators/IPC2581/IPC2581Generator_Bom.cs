using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.CoreExtensions.Bom;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed partial class IPC2581Generator {

        /// <summary>
        /// Escriu la seccio 'Bom'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Bom(EdaBoard board) {

            _writer.WriteStartElement("Bom");
            _writer.WriteAttributeString("name", "board_0_bom");

            _writer.WriteStartElement("BomHeader");
            _writer.WriteAttributeString("assembly", "assembly_0");
            _writer.WriteAttributeString("revision", "1.0");
            _writer.WriteEndElement();

            var bomGenerator = new EdaBomGenerator(board);
            var bom = bomGenerator.Generate();
            foreach (var bomEntry in bom) {

                _writer.WriteStartElement("BomItem");
                _writer.WriteAttributeString("OEMDesignNumberRef", bomEntry.Name);
                _writer.WriteAttributeInteger("quantity", bomEntry.ReferenceCount);

                foreach (var reference in bomEntry.References) {

                    var part = board.GetPart(reference);
                    var package = part.Component.Name;

                    _writer.WriteStartElement("RefDes");
                    _writer.WriteAttributeString("name", reference);
                    _writer.WriteAttributeString("packageRef", package);
                    _writer.WriteAttributeBool("populate", true);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }
    }
}
