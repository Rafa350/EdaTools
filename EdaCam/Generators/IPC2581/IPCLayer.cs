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
        SolderPaste
    }

    internal class IPCLayer {

        private readonly string _name;
        private readonly IPCLayerSide _side;
        private readonly IPCLayerFunction _function;

        public IPCLayer(string name, IPCLayerSide side, IPCLayerFunction function) {

            _name = name;
            _side = side;
            _function = function;
        }

        public string Name =>
            _name;

        public IPCLayerSide Side =>
            _side;

        public IPCLayerFunction Function =>
            _function;
    }
}
