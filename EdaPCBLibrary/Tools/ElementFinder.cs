namespace MikroPic.EdaTools.v1.Pcb.Tools {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class ElementFinder {

        private interface IChecker {

            bool Check(ElementBase element);
        }

        private sealed class LayerChecker: IChecker {

            private readonly Layer layer;

            public LayerChecker(Layer layer) {

                this.layer = layer;
            }

            public bool Check(ElementBase element) {

                return element.IsOnLayer(layer);
            }
        }

        private sealed class CheckerVisitor: DefaultVisitor {

            private readonly List<ElementBase> elements = new List<ElementBase>();
            private readonly IChecker checker;
            private readonly Board board;

            public CheckerVisitor(Board board, IChecker checker) {

                this.board = board;
                this.checker = checker;
            }

            public override void Visit(SmdPadElement pad) {

                if (checker.Check(pad))
                    elements.Add(pad);
            }

            public override void Visit(ThPadElement pad) {

                if (checker.Check(pad))
                    elements.Add(pad);
            }

            public override void Visit(ViaElement via) {

                if (checker.Check(via))
                    elements.Add(via);
            }

            public IEnumerable<ElementBase> Elements {
                get {
                    return elements;
                }
            }
        }

        private readonly Board board;

        public ElementFinder(Board board) {

            this.board = board;
        }

        public IEnumerable<ElementBase> ElementsOfLayer(Layer layer) {

            LayerChecker checker = new LayerChecker(layer);
            CheckerVisitor visitor = new CheckerVisitor(board, checker);
            board.AcceptVisitor(visitor);
            return visitor.Elements;
        }
    }
}
