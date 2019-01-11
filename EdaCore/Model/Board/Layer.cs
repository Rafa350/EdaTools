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
        private readonly string name;
        private readonly LayerFunction function = LayerFunction.Unknown;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="name">Nom de la capa.</param>
        /// <param name="function">Functio de la capa.</param>
        /// 
        public Layer(BoardSide side, string name, LayerFunction function) {

            this.side = side;
            this.name = name;
            this.function = function;
        }

        /// <summary>
        /// Obte un clon de l'objecte.
        /// </summary>
        /// <returns>El clon obtingut.</returns>
        /// 
        public Layer Clone() {

            return new Layer(side, name, function);
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
        /// Genera un identificador de capa.
        /// </summary>
        /// <param name="side">Capa</param>
        /// <param name="name">Nom</param>
        /// <returns>El identificador</returns>
        /// 
        public static string GetId(BoardSide side, string name) {

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
        /// Obte el nom.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Obte el identificador de la capa
        /// </summary>
        /// 
        public string Id {
            get {
                return GetId(side, name);
            }
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
        /// Obte la funcio de la capa
        /// </summary>
        /// 
        public LayerFunction Function {
            get {
                return function;
            }
        }

        public bool IsTop {
            get {
                return side == BoardSide.Top;
            }
        }

        public bool IsBottom {
            get {
                return side == BoardSide.Bottom;
            }
        }

        public bool IsInner {
            get {
                return side == BoardSide.Inner;
            }
        }
    }
}
