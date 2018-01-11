namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System.Windows.Media;
    
    public enum LayerClass {
        Unknown,
        TopSignal,
        InnerSignal,
        BottomSignal,
        Design,
        Mechanical,
    }

    public sealed class Layer: IVisitable {

        private readonly LayerIdentifier id = LayerIdentifier.Unknown;
        private readonly LayerClass cls = LayerClass.Unknown;
        private string name;
        private Color color;
        private bool isVisible = true;

        /// <summary>
        /// Constructor per defecte.
        /// </summary>
        /// 
        public Layer() {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Identificador de la capa.</param>
        /// <param name="name">Nom de la capa.</param>
        /// <param name="color">Color dels elements.</param>
        /// <param name="isVisible">Indica si la capa es visible.</param>
        /// <param name="isMirror">Indica si la capa es dibuixa en mirall.</param>
        /// 
        public Layer(LayerIdentifier id, string name, Color color, bool isVisible = true) {

            this.id = id;
            this.name = name;
            this.color = color;
            this.isVisible = isVisible;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public LayerIdentifier Id {
            get {
                return id;
            }
        }

        public LayerClass Class {
            get {
                return cls;
            }
        }

        /// <summary>
        /// Obte o asigna el nom de la capa.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        /// <summary>
        /// Obte o asigna el color de la capa.
        /// </summary>
        /// 
        public Color Color {
            get {
                return color;
            }
            set {
                color = value;
            }
        }

        public bool IsVisible {
            get {
                return isVisible;
            }
            set {
                isVisible = value;
            }
        }
    }
}
