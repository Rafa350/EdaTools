namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    /// <summary>
    /// Clase per visitar els elements de una serie de capes en particular.
    /// </summary>
    public abstract class ElementVisitor : DefaultVisitor {

        private readonly Board board;
        private readonly Layer layer;
        private Part part;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="layer">La capa a procesar.</param>
        /// 
        protected ElementVisitor(Board board, Layer layer) {

            this.board = board;
            this.layer = layer;
        }

        /// <summary>
        /// Executa el visitador.
        /// </summary>
        /// 
        public override void Run() {

            board.AcceptVisitor(this);
        }

        /// <summary>
        /// Visita un objecte 'Board'
        /// </summary>
        /// <param name="board">L'objecte a visitar.</param>
        /// 
        public override void Visit(Board board) {

            if (board.HasParts)
                foreach (var part in board.Parts)
                    part.AcceptVisitor(this);

            if (board.HasElements)
                foreach (var element in board.Elements)
                    if ((layer == null) || element.LayerSet.Contains(layer.Id))
                        element.AcceptVisitor(this);
        }

        /// <summary>
        /// Visita un objecte 'Part'
        /// </summary>
        /// <param name="part">L'objecte a visitar.</param>
        /// 
        public override void Visit(Part part) {

            this.part = part;
            try {
                if (part.HasElements)
                    foreach (var element in part.Elements)
                        if ((layer == null) || element.LayerSet.Contains(layer.Id))
                            element.AcceptVisitor(this);
            }
            finally {
                this.part = null;
            }
        }

        /// <summary>
        /// Obte la capa que s'esta visitant.
        /// </summary>
        /// 
        protected Board Board {
            get {
                return board;
            }
        }

        /// <summary>
        /// Obte el component que s'esta visitant.
        /// </summary>
        /// 
        protected Part Part {
            get {
                return part;
            }
        }

        /// <summary>
        /// Obte la capa que s'estat visitant.
        /// </summary>
        /// 
        protected Layer Layer {
            get {
                return layer;
            }
        }
    }
}
