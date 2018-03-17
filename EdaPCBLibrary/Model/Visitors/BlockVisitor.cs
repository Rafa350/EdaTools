namespace MikroPic.EdaTools.v1.Pcb.Model.Visitors {

    using System;

    public abstract class BlockVisitor: DefaultVisitor {

        private readonly Board board;
        private Block block;

        public BlockVisitor(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        public override void Run() {

            board.AcceptVisitor(this);
        }

        public override void Visit(Board board) {

            foreach (Block block in board.Blocks)
                block.AcceptVisitor(this);
        }

        public override void Visit(Block block) {

            this.block = block;

            foreach (Element element in block.Elements)
                element.AcceptVisitor(this);

            foreach (BlockAttribute attribute in block.Attributes)
                attribute.AcceptVisitor(this);

            this.block = null;
        }
    }
}
