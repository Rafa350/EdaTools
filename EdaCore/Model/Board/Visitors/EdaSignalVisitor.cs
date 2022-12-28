using System;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    public abstract class EdaSignalVisitor: EdaDefaultBoardVisitor {

        private EdaBoard _currentBoard;
        private EdaSignal _currentSignal;

        public override void Visit(EdaBoard board) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            EdaBoard savedBoard = _currentBoard;
            _currentBoard = board;
            try {
                foreach (var signal in board.Signals)
                    signal.AcceptVisitor(this);
            }
            finally {
                _currentBoard = savedBoard;
            }
        }

        public override void Visit(EdaSignal signal) {

            EdaSignal savedSignal = _currentSignal;
            _currentSignal = signal;
            try {
                var items = _currentBoard.GetConectionItems(signal);
                if (items != null)
                    foreach (var item in items) {
                        if (item.Conectable is EdaElementBase element)
                            element.AcceptVisitor(this);
                    }
            }
            finally {
                _currentSignal = savedSignal;
            }
        }

        protected EdaBoard Board =>
            _currentBoard;

        protected EdaSignal Signal =>
            _currentSignal;
    }
}
