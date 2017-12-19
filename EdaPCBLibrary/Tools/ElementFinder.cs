namespace MikroPic.EdaTools.v1.Pcb.Tools {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;

    public sealed class ElementFinder {

        private interface IChecker {

            bool Check(Element element);
        }

        private sealed class LayerChecker: IChecker {

            private readonly Layer layer;

            public LayerChecker(Layer layer) {

                this.layer = layer;
            }

            public bool Check(Element element) {

                return element.IsOnLayer(layer);
            }
        }

        private sealed class CheckerVisitor: BoardVisitor {

            private readonly List<Element> elements = new List<Element>();
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

            public IEnumerable<Element> Elements {
                get {
                    return elements;
                }
            }
        }

        private readonly Board board;

        public ElementFinder(Board board) {

            this.board = board;
        }

        public IEnumerable<Element> ElementsOfLayer(Layer layer) {

            LayerChecker checker = new LayerChecker(layer);
            CheckerVisitor visitor = new CheckerVisitor(board, checker);
            board.AcceptVisitor(visitor);
            return visitor.Elements;
        }
    }
}
