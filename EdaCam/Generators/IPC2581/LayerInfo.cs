namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    internal enum LayerFunction {
        Conductor,
        Drill,
        Outline,
        SolderMask,
        SolderPaste
    }

    internal enum LayerSide {
        None,
        Top,
        Bottom,
        All
    }

    internal sealed class LayerInfo {
        public string Name { get; set; }
        public LayerSide Side { get; set; }
        public LayerFunction Function { get; set; }
    }
}
