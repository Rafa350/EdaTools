namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using System;

    /// <summary>
    /// Clase per visitar els elements.
    /// </summary>
    public abstract class ElementVisitor : DefaultBoardVisitor {

        private EdaBoard currentBoard;
        private EdaPart currentPart;

        /// <summary>
        /// Visita un objecte 'Board'
        /// </summary>
        /// <param name="board">L'objecte a visitar.</param>
        /// 
        public override void Visit(EdaBoard board) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            EdaBoard savedBoard = currentBoard;
            currentBoard = board;
            try {
                if (board.HasParts)
                    foreach (var part in board.Parts)
                        part.AcceptVisitor(this);

                if (board.HasElements)
                    foreach (var element in board.Elements)
                        element.AcceptVisitor(this);
            }
            finally {
                currentBoard = savedBoard;
            }
        }

        /// <summary>
        /// Visita un objecte 'Part'
        /// </summary>
        /// <param name="part">L'objecte a visitar.</param>
        /// 
        public override void Visit(EdaPart part) {

            if (part == null)
                throw new ArgumentNullException(nameof(part));

            EdaPart savedPart = currentPart;
            currentPart = part;
            try {
                if (part.HasElements)
                    foreach (var element in part.Elements)
                        element.AcceptVisitor(this);
            }
            finally {
                currentPart = savedPart;
            }
        }

        /// <summary>
        /// Obte la capa que s'esta visitant.
        /// </summary>
        /// 
        protected EdaBoard Board {
            get {
                return currentBoard;
            }
        }

        /// <summary>
        /// Obte el component que s'esta visitant.
        /// </summary>
        /// 
        protected EdaPart Part {
            get {
                return currentPart;
            }
        }
    }
}
