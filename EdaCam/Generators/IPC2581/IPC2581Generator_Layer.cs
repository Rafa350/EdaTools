namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed partial class IPC2581Generator {

        /// <summary>
        /// Escriu la seccio 'Layer'
        /// </summary>
        /// 
        private void WriteSection_Layer() {

            foreach (var layer in _layers) {

                string functionName = "CONDUCTOR";
                switch (layer.Function) {
                    case IPCLayerFunction.Drill:
                        functionName = "DRILL";
                        break;

                    case IPCLayerFunction.Outline:
                        functionName = "BOARD_OUTLINE";
                        break;

                    case IPCLayerFunction.SolderMask:
                        functionName = "SOLDERMASK";
                        break;

                    case IPCLayerFunction.SolderPaste:
                        functionName = "SOLDERPASTE";
                        break;

                    case IPCLayerFunction.SilkScreen:
                        functionName = "SILKSCREEN";
                        break;
                }

                string sideName = "NONE";
                switch (layer.Side) {
                    case IPCLayerSide.Top:
                        sideName = "TOP";
                        break;

                    case IPCLayerSide.Bottom:
                        sideName = "BOTTOM";
                        break;

                    case IPCLayerSide.All:
                        sideName = "ALL";
                        break;
                }

                _writer.WriteStartElement("Layer");
                _writer.WriteAttributeString("name", layer.Name);
                _writer.WriteAttributeString("layerFunction", functionName);
                _writer.WriteAttributeString("side", sideName);
                if (layer.Function == IPCLayerFunction.Drill) {
                    _writer.WriteStartElement("Span");
                    _writer.WriteAttributeString("fromLayer", _topLayer.Name);
                    _writer.WriteAttributeString("toLayer", _bottomLayer.Name);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }
        }
    }
}
