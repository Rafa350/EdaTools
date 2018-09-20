namespace EdaDebugTest {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.PanelElements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;

    public sealed class Panelizer {

        private readonly Board panelBoard;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="board">La placa de desti.</param>
        /// 
        public Panelizer(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.panelBoard = board;
        }

        public void Panelize(Panel panel) {

            int index = 0;
            foreach (var element in panel.Elements) {
                PlaceElement place = element as PlaceElement;
                if (place != null)
                    Panelize(place.Board, index++, place.Position, place.Rotation);
            }
        }

        public void Panelize(Board board, int index, Point position, Angle rotation) {

            // Afegeix les capes que no existeixin en la placa de destinacio
            //
            foreach (var layer in board.Layers)
                if (panelBoard.GetLayer(layer.Id, false) == null)
                    panelBoard.AddLayer(layer.Clone());

            // Afegeix els senyals
            //
            foreach (var signal in board.Signals) {
                string panelSignalName = String.Format("B{0}.{1}", index, signal.Name);
                if (panelBoard.GetSignal(panelSignalName, false) == null) {
                    Signal panelSignal = signal.Clone();
                    panelSignal.Name = panelSignalName;
                    panelBoard.AddSignal(panelSignal);
                }
            }

            // Afegeix els blocs que no existeixin en la placa de destinacio.
            //
            if (board.HasBlocks) {
                foreach (var block in board.Blocks)
                    if (panelBoard.GetBlock(block.Name, false) == null)
                        panelBoard.AddBlock(block.Clone());
            }

            // Afegeix els parts a la placa
            //
            if (board.HasParts) {
                List<Part> transformableParts = new List<Part>();
                foreach (var part in board.Parts) {
                    Block block = panelBoard.GetBlock(part.Block.Name);
                    Part panelPart = part.Clone(block);
                    panelPart.Name = String.Format("B{0}.{1}", index, panelPart.Name);
                    transformableParts.Add(panelPart);
                    panelBoard.AddPart(panelPart);

                    foreach (var panelElement in panelPart.Elements) {
                        PadElement panelPad = panelElement as PadElement;
                        if (panelPad != null) {
                            Signal signal = board.GetSignal(part.GetPad(panelPad.Name), part, false);
                            if (signal != null) {
                                string panelSignalName = String.Format("B{0}.{1}", index, signal.Name);
                                panelBoard.Connect(panelBoard.GetSignal(panelSignalName), panelPad, panelPart);
                            }
                        }
                    }
                }

                TransformVisitor visitor = new TransformVisitor(transformableParts, position, rotation);
                visitor.Run();
            }

            // Afegeix els elements de la placa
            //
            if (board.HasElements) {
                List<Element> transformableElements = new List<Element>();
                foreach (var element in board.Elements) {

                    Element panelElement = element.Clone();
                    transformableElements.Add(panelElement);
                    panelBoard.AddElement(panelElement);

                    if (element is IConectable) {
                        Signal signal = board.GetSignal(element, null, false);
                        if (signal != null) {
                            string panelSignalName = String.Format("B{0}.{1}", index, signal.Name);
                            panelBoard.Connect(panelBoard.GetSignal(panelSignalName), panelElement as IConectable);
                        }
                    }
                }

                TransformVisitor visitor = new TransformVisitor(transformableElements, position, rotation);
                visitor.Run();
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

            public override void Visit(RegionElement region) {

                foreach (var segment in region.Segments)
                    segment.Position = transformation.ApplyTo(segment.Position);
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
