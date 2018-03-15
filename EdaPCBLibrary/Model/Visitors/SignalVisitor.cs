namespace MikroPic.EdaTools.v1.Pcb.Model.Visitors {

    using System;
    using MikroPic.EdaTools.v1.Pcb.Model;

    public abstract class SignalVisitor: DefaultVisitor {

        private readonly Board board;

        public SignalVisitor(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }
    }
}
