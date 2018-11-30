namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using System;

    public abstract class BlockVisitor: DefaultVisitor {

        private readonly Board board;
        private Component block;

        public BlockVisitor(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        public override void Run() {

            board.AcceptVisitor(this);
        }

        public override void Visit(Board board) {

            foreach (Component block in board.Components)
                block.AcceptVisitor(this);
        }

        public override void Visit(Component block) {

            this.block = block;

            foreach (Element element in block.Elements)
                element.AcceptVisitor(this);

            if (block.HasAttributes)
                foreach (ComponentAttribute attribute in block.Attributes)
                    attribute.AcceptVisitor(this);

            this.block = null;
        }
    }
}
