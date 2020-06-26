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
    public sealed class Layer : IBoardVisitable {

        private readonly BoardSide _side;
        private readonly string _tag;
        private readonly LayerFunction _function = LayerFunction.Unknown;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="tag">Etiqueta de la capa.</param>
        /// <param name="function">Functio de la capa.</param>
        /// 
        public Layer(BoardSide side, string tag, LayerFunction function) {

            _side = side;
            _tag = tag;
            _function = function;
        }

        /// <summary>
        /// Obte un clon de l'objecte.
        /// </summary>
        /// <returns>El clon obtingut.</returns>
        /// 
        public Layer Clone() {

            return new Layer(_side, _tag, _function);
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public void AcceptVisitor(IBoardVisitor visitor) {

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
        public BoardSide Side => _side;

        /// <summary>
        /// Obte l'etiqueta.
        /// </summary>
        /// 
        public string Tag => _tag;

        /// <summary>
        /// Obte el nom.
        /// </summary>
        /// 
        public string Name => GetName(_side, _tag);

        /// <summary>
        /// Obte la funcio.
        /// </summary>
        /// 
        public LayerFunction Function => _function;

        /// <summary>
        /// Indica si la capa esta en la cara superior.
        /// </summary>
        /// 
        public bool IsTop => _side == BoardSide.Top;

        /// <summary>
        /// Indica si es la capa de coure superior.
        /// </summary>
        /// 
        public bool IsTopCopper => (_side == BoardSide.Top) && (_function == LayerFunction.Signal);

        /// <summary>
        /// Indica si la capa esta en la cara inferior.
        /// </summary>
        /// 
        public bool IsBottom => _side == BoardSide.Bottom;

        /// <summary>
        /// Indica si es la capa de coure inferior.
        /// </summary>
        /// 
        public bool IsBottomCopper => (_side == BoardSide.Bottom) && (_function == LayerFunction.Signal);

        /// <summary>
        /// Indica si es una capa interna.
        /// </summary>
        /// 
        public bool IsInner =>_side == BoardSide.Inner;
    }
}
