namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System.Windows.Media;
    
    public enum LayerFunction {
        Unknown,
        Signal,
        Design,
        Mechanical,
    }

    public enum LayerSide {
        Unknown,
        Top,
        Inner,
        Bottom
    }

    public sealed class Layer: IVisitable {

        private readonly LayerId id = LayerId.Unknown;
        private readonly LayerSide side = LayerSide.Unknown;
        private readonly LayerFunction function = LayerFunction.Unknown;
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
        /// <param name="side">Cara on es troba la capa.</param>
        /// <param name="function">Functio de la capa.</param>
        /// <param name="color">Color dels elements.</param>
        /// <param name="isVisible">Indica si la capa es visible.</param>
        /// 
        public Layer(LayerId id, string name, LayerSide side, LayerFunction function, Color color, bool isVisible = true) {

            this.id = id;
            this.name = name;
            this.side = side;
            this.function = function;
            this.color = color;
            this.isVisible = isVisible;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el identificador de la capa.
        /// </summary>
        /// 
        public LayerId Id {
            get {
                return id;
            }
        }

        /// <summary>
        /// Obte la funcio de la capa
        /// </summary>
        /// 
        public LayerFunction Function {
            get {
                return function;
            }
        }

        /// <summary>
        /// Obte el cara on es troba la capa de la capa.
        /// </summary>
        /// 
        public LayerSide Side {
            get {
                return side;
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
