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

                if (part.Block != null)
                    part.Block.AcceptVisitor(this);

                if (part.Pads != null)
                    foreach (Pad pad in part.Pads)
                        pad.AcceptVisitor(this);
            }
            finally {
                currentPart = null;
            }
        }

        public override void Visit(Block block) {

            foreach (Element element in block.Elements)
                element.AcceptVisitor(this);
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
