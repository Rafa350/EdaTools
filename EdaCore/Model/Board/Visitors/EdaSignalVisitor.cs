namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using System;
    using System.Collections.Generic;

    using MikroPic.EdaTools.v1.Core.Model.Board;

    public abstract class EdaSignalVisitor : EdaDefaultBoardVisitor {

        private EdaBoard currentBoard;
        private EdaSignal currentSignal;

        public override void Visit(EdaBoard board) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            EdaBoard savedBoard = currentBoard;
            currentBoard = board;
            try {
                foreach (var signal in board.Signals)
                    signal.AcceptVisitor(this);
            }
            finally {
                currentBoard = savedBoard;
            }
        }

        public override void Visit(EdaSignal signal) {

            EdaSignal savedSignal = currentSignal;
            currentSignal = signal;
            try {
                IEnumerable<Tuple<IEdaConectable, EdaPart>> items = currentBoard.GetConnectedItems(signal);
                if (items != null)
                    foreach (var item in items) {
                        if (item.Item1 is EdaElement element)
                            element.AcceptVisitor(this);
                    }
            }
            finally {
                currentSignal = savedSignal;
            }
        }

        protected EdaBoard Board {
            get {
                return currentBoard;
            }
        }

        protected EdaSignal Signal {
            get {
                return currentSignal;
            }
        }
    }
}
