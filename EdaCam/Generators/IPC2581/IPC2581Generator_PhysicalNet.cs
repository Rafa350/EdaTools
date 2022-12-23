using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public partial class IPC2581Generator {

        private void WriteSection_PhysicalNet(EdaBoard board) {

            _writer.WriteStartElement("PhyNetGroup");
            _writer.WriteAttributeString("name", "board_0_allnets");

            foreach (var signal in board.Signals) {

                var items = board.GetConectionItems(signal);
                if (items != null) {

                    _writer.WriteStartElement("PhyNet");
                    _writer.WriteAttributeString("name", signal.Name);

                    foreach (var item in items) {

                        // Procesa els pads
                        //
                        if (item.Conectable is EdaPadBaseElement element) {

                            // Nomes explora les capes exterior de coure
                            //
                            var layerIds = new EdaLayerId[] { EdaLayerId.TopCopper, EdaLayerId.BottomCopper };

                            foreach (var layerId in layerIds) {

                                if (element.IsOnLayer(layerId)) {

                                    EdaPoint position = element.Position;
                                    if (item.Part != null) {
                                        var transformation = item.Part.GetLocalTransformation();
                                        position = transformation.Transform(position);
                                    }

                                    _writer.WriteStartElement("PhyNetPoint");
                                    _writer.WriteAttributeDouble("x", position.X / _scale);
                                    _writer.WriteAttributeDouble("y", position.Y / _scale);
                                    _writer.WriteAttributeString("layerRef", layerId.IsTop ? _topLayer.Name : _bottomLayer.Name);
                                    _writer.WriteAttributeString("netNode", "END");
                                    _writer.WriteAttributeString("exposure", "EXPOSED");

                                    int id = -1;
                                    if (element is EdaSmtPadElement smtPad) {
                                        var entry = _dataCache.GetRectEntry(smtPad.Size, smtPad.CornerRatio, false);
                                        id = entry.Id;
                                    }
                                    else if (element is EdaThtPadElement thtPad) {
                                        var size = layerId.IsTop ? thtPad.TopSize : thtPad.BottomSize;
                                        var flat = thtPad.CornerShape == EdaThtPadElement.CornerShapeType.Flat;
                                        var entry = _dataCache.GetRectEntry(size, thtPad.CornerRatio, flat);
                                        id = entry.Id;
                                    }
                                    if (id != -1) {
                                        _writer.WriteStartElement("StandardPrimitiveRef");
                                        _writer.WriteAttributeInteger("id", id);
                                        _writer.WriteEndElement(); // StandardPrimitiveRef
                                    }

                                    _writer.WriteEndElement(); // PhyNetPoint
                                }
                            }
                        }

                        // Procesa les vias
                        //
                        else if (item.Conectable is EdaViaElement via) {

                        }
                    }

                    _writer.WriteEndElement(); // PhyNet
                }
            }

            _writer.WriteEndElement(); // PhyNetGroup
        }
    }
}
