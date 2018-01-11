namespace MikroPic.EdaTools.v1.Pcb.Model.Visitors {

    public abstract class BoardVisitor: DefaultVisitor {

        private Board visitingBoard;
        private Part visitingPart;

        public override void Visit(Board board) {

            visitingBoard = board;
            try {

                if (board.Elements != null)
                    foreach (Element element in board.Elements)
                        element.AcceptVisitor(this);

                if (board.Parts != null)
                    foreach (Part part in board.Parts)
                        part.AcceptVisitor(this);
            }
            finally {
                visitingBoard = null;
            }
        }

        public override void Visit(Part part) {

            visitingPart = part;
            try {

                if (part.Component != null)
                    part.Component.AcceptVisitor(this);
            }

            finally {
                visitingPart = null;
            }
        }

        public override void Visit(Component component) {

            foreach (Element element in component.Elements)
                element.AcceptVisitor(this);
        }

        protected Board VisitingBoard {  get { return visitingBoard; } }
        protected Part VisitingPart { get { return visitingPart; } }
    }
}
