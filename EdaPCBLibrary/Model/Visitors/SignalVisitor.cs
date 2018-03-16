namespace MikroPic.EdaTools.v1.Pcb.Model.Visitors {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

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

            foreach (Signal signal in board.Signals)
                signal.AcceptVisitor(this);
        }

        public override void Visit(Signal signal) {

            this.signal = signal;

            IEnumerable<Tuple<IConectable, Part>> items = board.GetConnectedItems(signal);
            if (items != null)
                foreach (Tuple<IConectable, Part> item in items) {
                    Element element = item.Item1 as Element;
                    if (element != null)
                        element.AcceptVisitor(this);
                }

            this.signal = null;
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
