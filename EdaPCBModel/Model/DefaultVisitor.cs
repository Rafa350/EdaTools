namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public abstract class DefaultVisitor: IVisitor {

        private Board visitingBoard;
        private Part visitingPart;

        public virtual void Visit(Board board) {

            visitingBoard = board;
            try {

                if (board.Elements != null)
                    foreach (ElementBase element in board.Elements)
                        element.AcceptVisitor(this);

                if (board.Parts != null)
                    foreach (Part part in board.Parts)
                        part.AcceptVisitor(this);

                if (board.Signals != null)
                    foreach (Signal signal in board.Signals)
                        signal.AcceptVisitor(this);
            }
            finally {
                visitingBoard = null;
            }
        }

        public virtual void Visit(Layer layer) {
        }

        public virtual void Visit(Signal signal) {

            if (signal.Elements != null)
                foreach (ElementBase element in signal.Elements)
                    element.AcceptVisitor(this);
        }

        public virtual void Visit(LineElement line) {
        }

        public virtual void Visit(ArcElement arc) {
        }

        public virtual void Visit(RectangleElement rectangle) {
        }

        public virtual void Visit(CircleElement circle) {
        }

        public virtual void Visit(SmdPadElement pad) {
        }

        public virtual void Visit(ThPadElement pad) {
        }

        public virtual void Visit(PolygonElement polygon) {
        }

        public virtual void Visit(TextElement text) {
        }

        public virtual void Visit(HoleElement hole) {
        }

        public virtual void Visit(ViaElement via) {
        }

        public virtual void Visit(Part part) {

            visitingPart = part;
            try {

                if (part.Component != null)
                    part.Component.AcceptVisitor(this);
            }

            finally {
                visitingPart = null;
            }
        }

        public virtual void Visit(Component component) {

            if (component.Elements != null)
                foreach (ElementBase element in component.Elements)
                    element.AcceptVisitor(this);
        }

        public virtual void Visit(Parameter parameter) {
        }

        protected Board VisitingBoard {  get { return visitingBoard; } }
        protected Part VisitingPart { get { return visitingPart; } }
    }
}
