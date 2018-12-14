namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using System;

    public abstract class ComponentVisitor: DefaultVisitor {

        private readonly Board board;
        private Component component;

        public ComponentVisitor(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        public override void Run() {

            board.AcceptVisitor(this);
        }

        public override void Visit(Board board) {

            if (board.HasComponents)
                foreach (var component in board.Components)
                    component.AcceptVisitor(this);
        }

        public override void Visit(Component component) {

            this.component = component;
            try {
                if (component.HasElements)
                    foreach (var element in component.Elements)
                        element.AcceptVisitor(this);

                if (component.HasAttributes)
                    foreach (var attribute in component.Attributes)
                        attribute.AcceptVisitor(this);
            }
            finally {
                this.component = null;
            }
        }
    }
}
