namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using System;

    /// <summary>
    /// Clase per visitar els elements.
    /// </summary>
    public abstract class ElementVisitor : DefaultBoardVisitor {

        private Board currentBoard;
        private Part currentPart;

        /// <summary>
        /// Visita un objecte 'Board'
        /// </summary>
        /// <param name="board">L'objecte a visitar.</param>
        /// 
        public override void Visit(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            Board savedBoard = currentBoard;
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
        public override void Visit(Part part) {

            if (part == null)
                throw new ArgumentNullException("part");

            Part savedPart = currentPart;
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
        protected Board Board {
            get {
                return currentBoard;
            }
        }

        /// <summary>
        /// Obte el component que s'esta visitant.
        /// </summary>
        /// 
        protected Part Part {
            get {
                return currentPart;
            }
        }
    }
}
