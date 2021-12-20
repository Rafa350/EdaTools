using System;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    public abstract class EdaComponentVisitor : EdaDefaultBoardVisitor {

        private EdaBoard _currentBoard;
        private EdaComponent _currentComponent;

        public override void Visit(EdaBoard board) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            EdaBoard savedBoard = _currentBoard;
            _currentBoard = board;
            try {
                if (board.HasComponents)
                    foreach (var component in board.Components)
                        component.AcceptVisitor(this);
            }
            finally {
                _currentBoard = savedBoard;
            }
        }

        public override void Visit(EdaComponent component) {

            if (component == null)
                throw new ArgumentNullException(nameof(component));

            EdaComponent savedComponent = _currentComponent;
            _currentComponent = component;
            try {
                if (component.HasElements)
                    foreach (var element in component.Elements)
                        element.AcceptVisitor(this);

                if (component.HasAttributes)
                    foreach (var attribute in component.Attributes)
                        attribute.AcceptVisitor(this);
            }
            finally {
                _currentComponent = savedComponent;
            }
        }

        protected EdaBoard Board =>
            _currentBoard;

        protected EdaComponent Component =>
            _currentComponent;
    }
}
