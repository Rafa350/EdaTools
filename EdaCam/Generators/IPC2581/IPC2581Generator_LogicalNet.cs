using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed partial class IPC2581Generator {

        /// <summary>
        /// Escriu la seccio 'LogicalNet'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_LogicalNet(EdaBoard board) {

            foreach (var signal in board.Signals) {
                var items = board.GetConectionItems(signal);
                if (items != null) {

                    _writer.WriteStartElement("LogicalNet");
                    _writer.WriteAttributeString("name", signal.Name);

                    foreach (var item in items) {
                        if (item.Part != null) {

                            string partName = item.Part.Name;
                            string pinName = (item.Conectable as EdaPadBaseElement).Name;

                            _writer.WriteStartElement("PinRef");
                            _writer.WriteAttributeString("componentRef", partName);
                            _writer.WriteAttributeString("pin", pinName);
                            _writer.WriteEndElement();
                        }
                    }

                    _writer.WriteEndElement();
                }
            }
        }
    }
}
