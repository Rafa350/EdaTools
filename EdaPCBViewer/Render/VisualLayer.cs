namespace MikroPic.EdaTools.v1.Designer.Render {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Geometry;

    public sealed class VisualLayer {

        private readonly LayerSet layerSet;
        private readonly string name;
        private bool visible;
        private Color color;

        public VisualLayer(string name, LayerSet layerSet):
            this(name, layerSet, true, new Color(255, 128, 128, 128)) {

        }

        public VisualLayer(string name, LayerSet layerSet, bool visible, Color color) {

            this.name = name;
            this.layerSet = layerSet;
            this.visible = visible;
            this.color = color;
        }

        public string Name {
            get {
                return name;
            }
        }

        public LayerSet Layers {
            get {
                return layerSet;
            }
        }

        public bool Visible {
            get {
                return visible;
            }
            set {
                visible = value;
            }
        }

        public Color Color {
            get {
                return color;
            }
            set {
                color = value;
            }
        }
    }
}
