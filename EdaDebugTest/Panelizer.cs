namespace EdaDebugTest {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.BoardElements;
    using MikroPic.EdaTools.v1.Pcb.Model.PanelElements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;

    public sealed class Panelizer {

        private readonly Board dstBoard;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="board">La placa de desti.</param>
        /// 
        public Panelizer(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.dstBoard = board;
        }

        public void Panelize(Panel panel) {

            foreach (var element in panel.Elements) {
                PlaceElement place = element as PlaceElement;
                if (place != null)
                    Panelize(place.Board, place.Position, place.Rotation);
            }
        }

        public void Panelize(Board board, Point position, Angle rotation) {

            // Afegeix les capes que no existeixin en la placa de destinacio
            //
            foreach (var layer in board.Layers)
                if (dstBoard.GetLayer(layer.Name, false) == null)
                    dstBoard.AddLayer(layer.Clone());

            // Afegeix els blocs que no existeixin en la placa de destinacio.
            //
            if (board.HasBlocks) {
                foreach (var block in board.Blocks)
                    if (dstBoard.GetBlock(block.Name, false) == null)
                        dstBoard.AddBlock(block.Clone());
            }

            // Afegeix els parts a la placa
            //
            if (board.HasParts) {
                List<Part> parts = new List<Part>();
                foreach (var part in board.Parts) {
                    Block block = dstBoard.GetBlock(part.Block.Name);
                    parts.Add(part.Clone(block));
                }

                TransformVisitor visitor = new TransformVisitor(parts, position, rotation);
                visitor.Run();

                dstBoard.AddParts(parts);
            }

            // Afegeix els elements de la placa
            //
            if (board.HasElements) {
                List<BoardElement> elements = new List<BoardElement>();
                foreach (var element in board.Elements)
                    elements.Add(element.Clone());

                TransformVisitor visitor = new TransformVisitor(elements, position, rotation);
                visitor.Run();

                dstBoard.AddElements(elements);
            }
        }

        private sealed class TransformVisitor: DefaultVisitor {

            private readonly IEnumerable<IVisitable> visitables;
            private readonly Transformation transformation;

            public TransformVisitor(IEnumerable<IVisitable> visitables, Point offset, Angle rotation) {

                this.visitables = visitables;
                transformation = new Transformation(offset, rotation);
            }

            public override void Run() {

                foreach (var visitable in visitables)
                    visitable.AcceptVisitor(this);
            }

            public override void Visit(LineElement line) {

                line.StartPosition = transformation.ApplyTo(line.StartPosition);
                line.EndPosition = transformation.ApplyTo(line.EndPosition);
            }

            public override void Visit(ArcElement arc) {

                arc.StartPosition = transformation.ApplyTo(arc.StartPosition);
                arc.EndPosition = transformation.ApplyTo(arc.EndPosition);
            }

            public override void Visit(RectangleElement rectangle) {

                rectangle.Position = transformation.ApplyTo(rectangle.Position);
            }

            public override void Visit(CircleElement circle) {

                circle.Position = transformation.ApplyTo(circle.Position);
            }

            public override void Visit(SmdPadElement pad) {

                pad.Position = transformation.ApplyTo(pad.Position);
            }

            public override void Visit(ThPadElement pad) {

                pad.Position = transformation.ApplyTo(pad.Position);
            }

            public override void Visit(ViaElement via) {

                via.Position = transformation.ApplyTo(via.Position);
            }

            public override void Visit(HoleElement hole) {

                hole.Position = transformation.ApplyTo(hole.Position);
            }

            public override void Visit(Part part) {

                part.Position = transformation.ApplyTo(part.Position);
            }
        }
    }
}
