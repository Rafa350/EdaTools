namespace MikroPic.EdaTools.v1.Core.Model.Board {

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

        private readonly LayerId _id;
        private readonly BoardSide _side;
        private readonly LayerFunction _function = LayerFunction.Unknown;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Identificador de la capa.</param>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="function">Functio de la capa.</param>
        /// 
        public Layer(LayerId id, BoardSide side, LayerFunction function) {

            _id = id;
            _side = side;
            _function = function;
        }

        /// <summary>
        /// Obte un clon de l'objecte.
        /// </summary>
        /// <returns>El clon obtingut.</returns>
        /// 
        public Layer Clone() {

            return new Layer(_id, _side, _function);
        }

        /// <inheritdoc/>
        /// 
        public void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el identificador.
        /// </summary>
        /// 
        public LayerId Id =>
            _id;

        /// <summary>
        /// Obte el nom.
        /// </summary>
        /// 
        public string Name => 
            _id.ToString();

        /// <summary>
        /// Obte la cara
        /// </summary>
        /// 
        public BoardSide Side => 
            _side;

        /// <summary>
        /// Obte la funcio.
        /// </summary>
        /// 
        public LayerFunction Function => 
            _function;

        /// <summary>
        /// Indica si la capa esta en la cara superior.
        /// </summary>
        /// 
        public bool IsTop => 
            _side == BoardSide.Top;

        /// <summary>
        /// Indica si es la capa de coure superior.
        /// </summary>
        /// 
        public bool IsTopCopper => 
            (_side == BoardSide.Top) && (_function == LayerFunction.Signal);

        /// <summary>
        /// Indica si la capa esta en la cara inferior.
        /// </summary>
        /// 
        public bool IsBottom => 
            _side == BoardSide.Bottom;

        /// <summary>
        /// Indica si es la capa de coure inferior.
        /// </summary>
        /// 
        public bool IsBottomCopper => 
            (_side == BoardSide.Bottom) && (_function == LayerFunction.Signal);

        /// <summary>
        /// Indica si es una capa interna.
        /// </summary>
        /// 
        public bool IsInner =>
            _side == BoardSide.Inner;
    }
}
