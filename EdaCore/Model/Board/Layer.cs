namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System.Text;

    /// <summary>
    /// Funcio de la capa.
    /// </summary>
    /// 
    public enum LayerFunction {
        Unknown,     // Desconeguda
        Signal,      // Pistes, vias, pads, th, etc
        Design,      // Silk, names, etc
        Mechanical,  // Forats, fressat, etc
        Outline      // Perfil extern
    }

    /// <summary>
    /// Clare quie representa una capa de la placa.
    /// </summary>
    /// 
    public sealed class Layer: IVisitable {

        private readonly BoardSide side;
        private readonly string tag;
        private readonly LayerFunction function = LayerFunction.Unknown;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="tag">Etiqueta de la capa.</param>
        /// <param name="function">Functio de la capa.</param>
        /// 
        public Layer(BoardSide side, string tag, LayerFunction function) {

            this.side = side;
            this.tag = tag;
            this.function = function;
        }

        /// <summary>
        /// Obte un clon de l'objecte.
        /// </summary>
        /// <returns>El clon obtingut.</returns>
        /// 
        public Layer Clone() {

            return new Layer(side, tag, function);
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Genera un nom de capa.
        /// </summary>
        /// <param name="side">Cara.</param>
        /// <param name="name">Etiqueta.</param>
        /// <returns>El nom.</returns>
        /// 
        public static string GetName(BoardSide side, string name) {

            StringBuilder sb = new StringBuilder();
            switch (side) {
                case BoardSide.Top:
                    sb.Append("Top.");
                    break;
                case BoardSide.Bottom:
                    sb.Append("Bottom.");
                    break;
            }
            sb.Append(name);

            return sb.ToString();
        }

        /// <summary>
        /// Obte la cara
        /// </summary>
        /// 
        public BoardSide Side {
            get {
                return side;
            }
        }

        /// <summary>
        /// Obte l'etiqueta.
        /// </summary>
        /// 
        public string Tag {
            get {
                return tag;
            }
        }

        /// <summary>
        /// Obte el nom.
        /// </summary>
        /// 
        public string Name {
            get {
                return GetName(side, tag);
            }
        }

        /// <summary>
        /// Obte la funcio.
        /// </summary>
        /// 
        public LayerFunction Function {
            get {
                return function;
            }
        }

        /// <summary>
        /// Indica si la capa esta en la cara superior.
        /// </summary>
        /// 
        public bool IsTop {
            get {
                return side == BoardSide.Top;
            }
        }

        /// <summary>
        /// Indica si es la capa de coure superior.
        /// </summary>
        /// 
        public bool IsTopCopper {
            get {
                return (side == BoardSide.Top) && (function == LayerFunction.Signal);
            }
        }

        /// <summary>
        /// Indica si la capa esta en la cara inferior.
        /// </summary>
        /// 
        public bool IsBottom {
            get {
                return side == BoardSide.Bottom;
            }
        }

        /// <summary>
        /// Indica si es la capa de coure inferior.
        /// </summary>
        /// 
        public bool IsBottomCopper {
            get {
                return (side == BoardSide.Bottom) && (function == LayerFunction.Signal);
            }
        }

        /// <summary>
        /// Indica si es una capa interna.
        /// </summary>
        /// 
        public bool IsInner {
            get {
                return side == BoardSide.Inner;
            }
        }
    }
}
