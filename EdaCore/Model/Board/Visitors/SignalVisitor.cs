namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Core.Model.Board;

    public abstract class SignalVisitor : DefaultBoardVisitor {

        private Board currentBoard;
        private Signal currentSignal;

        public override void Visit(Board board) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            Board savedBoard = currentBoard;
            currentBoard = board;
            try {
                foreach (var signal in board.Signals)
                    signal.AcceptVisitor(this);
            }
            finally {
                currentBoard = savedBoard;
            }
        }

        public override void Visit(Signal signal) {

            Signal savedSignal = currentSignal;
            currentSignal = signal;
            try {
                IEnumerable<Tuple<IConectable, Part>> items = currentBoard.GetConnectedItems(signal);
                if (items != null)
                    foreach (var item in items) {
                        if (item.Item1 is Element element)
                            element.AcceptVisitor(this);
                    }
            }
            finally {
                currentSignal = savedSignal;
            }
        }

        protected Board Board {
            get {
                return currentBoard;
            }
        }

        protected Signal Signal {
            get {
                return currentSignal;
            }
        }
    }
}
