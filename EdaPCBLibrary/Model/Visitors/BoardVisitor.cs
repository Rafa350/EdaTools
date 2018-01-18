namespace MikroPic.EdaTools.v1.Pcb.Model.Visitors {

    public abstract class BoardVisitor : DefaultVisitor {

        private Board currentBoard;
        private Part currentPart;

        public override void Visit(Board board) {

            currentBoard = board;
            try {

                if (board.Elements != null)
                    foreach (Element element in board.Elements)
                        element.AcceptVisitor(this);

                if (board.Parts != null)
                    foreach (Part part in board.Parts)
                        part.AcceptVisitor(this);
            }
            finally {
                currentBoard = null;
            }
        }

        public override void Visit(Part part) {

            currentPart = part;
            try {
                if (part.Block.Elements != null)
                    foreach (Element element in part.Block.Elements)
                        element.AcceptVisitor(this);
            }
            finally {
                currentPart = null;
            }
        }

        protected Board CurrentBoard {
            get {
                return currentBoard;
            }
        }

        protected Part CurrentPart {
            get {
                return currentPart;
            }
        }
    }
}
