using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    internal enum IPCLayerSide {
        Top,
        Bottom,
        Internal,
        All,
        None
    }

    internal enum IPCLayerFunction {
        Conductor,
        Drill,
        Outline,
        SolderMask,
        SolderPaste,
        SilkScreen
    }

    internal class IPCLayer {

        private readonly string _name;
        private readonly IPCLayerSide _side;
        private readonly IPCLayerFunction _function;
        private readonly EdaLayerSet _layerSet;

        public IPCLayer(string name, IPCLayerSide side, IPCLayerFunction function, EdaLayerSet layerSet) {

            _name = name;
            _side = side;
            _function = function;
            _layerSet = layerSet;
        }

        public string Name =>
            _name;

        public IPCLayerSide Side =>
            _side;

        public IPCLayerFunction Function =>
            _function;

        public EdaLayerSet LayerSet =>
            _layerSet;
    }
}
