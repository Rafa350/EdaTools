namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

    public abstract class SignalVisitor : DefaultVisitor {

        private readonly Board board;
        private Signal signal;

        public SignalVisitor(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        public override void Run() {

            board.AcceptVisitor(this);
        }

        public override void Visit(Board board) {

            foreach (var signal in board.Signals)
                signal.AcceptVisitor(this);
        }

        public override void Visit(Signal signal) {

            this.signal = signal;
            try {
                IEnumerable<Tuple<IConectable, Part>> items = board.GetConnectedItems(signal);
                if (items != null)
                    foreach (var item in items) {
                        Element element = item.Item1 as Element;
                        if (element != null)
                            element.AcceptVisitor(this);
                    }
            }
            finally {
                this.signal = null;
            }
        }

        protected Board Board {
            get {
                return board;
            }
        }

        protected Signal Signal {
            get {
                return signal;
            }
        }
    }
}
