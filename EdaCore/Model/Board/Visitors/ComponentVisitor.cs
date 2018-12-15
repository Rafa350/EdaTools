namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using System;

    public abstract class ComponentVisitor : DefaultVisitor {

        private Board currentBoard;
        private Component currentComponent;

        public override void Visit(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            Board savedBoard = currentBoard;
            currentBoard = board;
            try {
                if (board.HasComponents)
                    foreach (var component in board.Components)
                        component.AcceptVisitor(this);
            }
            finally {
                currentBoard = savedBoard;
            }
        }

        public override void Visit(Component component) {

            if (component == null)
                throw new ArgumentNullException("component");

            Component savedComponent = currentComponent;
            currentComponent = component;
            try {
                if (component.HasElements)
                    foreach (var element in component.Elements)
                        element.AcceptVisitor(this);

                if (component.HasAttributes)
                    foreach (var attribute in component.Attributes)
                        attribute.AcceptVisitor(this);
            }
            finally {
                currentComponent = savedComponent;
            }
        }

        protected Board Board {
            get {
                return currentBoard;
            }
        }

        protected Component Component {
            get {
                return currentComponent;
            }
        }
    }
}
