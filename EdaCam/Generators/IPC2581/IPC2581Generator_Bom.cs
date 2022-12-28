using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed partial class IPC2581Generator {

        /// <summary>
        /// Escriu la seccio 'Bom'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Bom(EdaBoard board) {

            // Obte els components unics de la placa en un diccionari
            //
            var dictionary = new Dictionary<string, List<EdaPart>>();
            foreach (var part in board.Parts) {
                var partMPN = part.GetAttribute("MPN")?.Value;
                if (partMPN != null) {
                    if (!dictionary.TryGetValue(partMPN, out var value)) {
                        value = new List<EdaPart>();
                        dictionary.Add(partMPN, value);
                    }
                    value.Add(part);
                }
            }

            _writer.WriteStartElement("Bom");
            _writer.WriteAttributeString("name", "board_0_bom");

            _writer.WriteStartElement("BomHeader");
            _writer.WriteAttributeString("assembly", "assembly_0");
            _writer.WriteAttributeString("revision", "1.0");
            _writer.WriteEndElement();

            foreach (var entry in dictionary) {

                var name = entry.Key;
                var parts = entry.Value;

                _writer.WriteStartElement("BomItem");
                _writer.WriteAttributeString("OEMDesignNumberRef", name);
                _writer.WriteAttributeInteger("quantity", parts.Count);

                foreach (var part in parts) {

                    var package = part.Component.Name;

                    _writer.WriteStartElement("RefDes");
                    _writer.WriteAttributeString("name", part.Name);
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
