namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using System;

    public abstract class ComponentVisitor : DefaultBoardVisitor {

        private EdaBoard currentBoard;
        private EdaComponent currentComponent;

        public override void Visit(EdaBoard board) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            EdaBoard savedBoard = currentBoard;
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

        public override void Visit(EdaComponent component) {

            if (component == null)
                throw new ArgumentNullException(nameof(component));

            EdaComponent savedComponent = currentComponent;
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

        protected EdaBoard Board {
            get {
                return currentBoard;
            }
        }

        protected EdaComponent Component {
            get {
                return currentComponent;
            }
        }
    }
}
