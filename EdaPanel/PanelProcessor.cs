namespace MikroPic.EdaTools.v1.Panel {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.IO;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;
    using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.Items;

    public sealed class PanelProcessor {

        private readonly Dictionary<string, Board> sourceBoardCache = new Dictionary<string, Board>();
        private readonly Board targetBoard;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="targetBoard">La placa de desti.</param>
        /// 
        public PanelProcessor(Board targetBoard) {

            if (targetBoard == null)
                throw new ArgumentNullException("board");

            this.targetBoard = targetBoard;
        }

        /// <summary>
        /// Genera un panell sobre la placa de desti.
        /// </summary>
        /// <param name="project">El projecte del panell.</param>
        /// <param name="locator">Localitzador de recursos.</param>
        /// 
        public void Panelize(Panel project, IStreamLocator locator) {

            // Afegeix les capes minimes necesaries pel panell.
            //
            AddLayers();

            // Afegeix els elements del panell.
            //
            int index = 0;
            foreach (var item in project.Items) {
                if (item is PcbItem pcb) {

                    if (!sourceBoardCache.TryGetValue(pcb.FileName, out Board sourceBoard)) {
                        string path = locator.GetPath(pcb.FileName);
                        using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                            BoardStreamReader reader = new BoardStreamReader(stream);
                            sourceBoard = reader.Read();
                        }
                        sourceBoardCache.Add(pcb.FileName, sourceBoard);
                    }
                    AddBoard(sourceBoard, pcb.Position, pcb.Rotation, index++);
                }

                else if (item is CutItem cut)
                    AddCut(cut);
            }

            // Afegeix el perfil exterior del panell.
            //
            AddProfile(project.Size);
        }

        /// <summary>
        /// Afegeix les capes necesaries pel panell.
        /// </summary>
        /// 
        private void AddLayers() {

            if (targetBoard.GetLayer(LayerId.Milling, false) == null)
                targetBoard.AddLayer(new Layer(LayerId.Milling, BoardSide.None, LayerFunction.Mechanical));
            if (targetBoard.GetLayer(LayerId.Get("LocalProfile"), false) == null)
                targetBoard.AddLayer(new Layer(LayerId.Get("LocalProfile"), BoardSide.None, LayerFunction.Mechanical));
        }

        /// <summary>
        /// Afegeix una placa
        /// </summary>
        /// <param name="board">La placa a afeigir.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Orientacio.</param>
        /// <param name="index">Index de la placa.</param>
        /// 
        private void AddBoard(Board board, Point position, Angle rotation, int index) {

            // Afegeix les capes que no existeixin en la placa de destinacio. Les 
            // capes son comuns a totes les plaques que formen el panel.
            //
            foreach (var layer in board.Layers)
                if (targetBoard.GetLayer(layer.Id, false) == null)
                    targetBoard.AddLayer(layer.Clone());

            // Afegeix els senyals. Cada placa te les seves propies.
            //
            foreach (var signal in board.Signals) {
                string signalName = String.Format("{1}@{0}", index, signal.Name);
                if (targetBoard.GetSignal(signalName, false) == null)
                    targetBoard.AddSignal(signal.Clone(signalName));
            }

            // Afegeix els components que no existeixin en la placa de destinacio. Els
            // components son comuns a totes les plaques.
            //
            if (board.HasComponents) {
                foreach (var component in board.Components)
                    if (targetBoard.GetComponent(component.Name, false) == null)
                        targetBoard.AddComponent(component.Clone());
            }

            // Afegeix els parts a la placa
            //
            if (board.HasParts) {
                List<Part> transformableParts = new List<Part>();
                foreach (var part in board.Parts) {
                    Component component = targetBoard.GetComponent(part.Component.Name);
                    Part panelPart = part.Clone(String.Format("{1}@{0}", index, part.Name), component);
                    transformableParts.Add(panelPart);
                    targetBoard.AddPart(panelPart);

                    foreach (var panelElement in panelPart.Elements) {
                        if (panelElement is PadElement panelPad) {
                            Signal signal = board.GetSignal(part.GetPad(panelPad.Name), part, false);
                            if (signal != null) {
                                string panelSignalName = String.Format("{1}@{0}", index, signal.Name);
                                targetBoard.Connect(targetBoard.GetSignal(panelSignalName), panelPad, panelPart);
                            }
                        }
                    }
                }

                TransformVisitor visitor = new TransformVisitor(position, rotation);
                foreach (var part in transformableParts)
                    part.AcceptVisitor(visitor);
            }

            // Afegeix els elements de la placa
            //
            if (board.HasElements) {
                List<Element> transformableElements = new List<Element>();
                foreach (var boardElement in board.Elements) {

                    Element panelElement = boardElement.Clone();
                    if (boardElement.IsOnLayer(LayerId.Profile)) {
                        boardElement.LayerSet.Remove(LayerId.Profile);
                        boardElement.LayerSet.Add(LayerId.Get("LocalProfile"));
                    }
                    transformableElements.Add(panelElement);
                    targetBoard.AddElement(panelElement);

                    if (boardElement is IConectable) {
                        Signal signal = board.GetSignal(boardElement, null, false);
                        if (signal != null) {
                            string panelSignalName = String.Format("{1}@{0}", index, signal.Name);
                            targetBoard.Connect(targetBoard.GetSignal(panelSignalName), panelElement as IConectable);
                        }
                    }
                }

                TransformVisitor visitor = new TransformVisitor(position, rotation);
                foreach (var element in transformableElements)
                    element.AcceptVisitor(visitor);
            }
        }

        /// <summary>
        /// Afegeix les rutes de fresat
        /// </summary>
        /// <param name="cut">El item.</param>
        /// 
        private void AddCut(CutItem cut) {

            // Obte els punts de tall d'una linia 
            //
            int[] refPoints = cut.GetReferencePoints();
            int[] cutPoints = cut.GetCutReferencePoints(refPoints);
            int[] holePoints = cut.GetHoleReferencePoints(refPoints);

            // Obte la pendent 
            //
            double dx = cut.EndPosition.X - cut.StartPosition.X;
            double dy = cut.EndPosition.Y - cut.StartPosition.Y;
            double rad = Math.Atan2(dy, dx);

            // Calcula la transformacio
            //
            Transformation t = new Transformation();
            t.Translate(cut.StartPosition);
            t.Rotate(cut.StartPosition, Angle.FromRadiants(rad));

            // Afegeix les linies de tall
            //
            for (int i = 0; i < cutPoints.Length; i += 2) {

                // Transforma els punts a la posicio real
                //
                Point q1 = t.ApplyTo(new Point(cutPoints[i], 0));
                Point q2 = t.ApplyTo(new Point(cutPoints[i + 1], 0));

                // Afegeix la linia a la placa
                //
                targetBoard.AddElement(new LineElement(new LayerSet(LayerId.Milling), q1, q2,
                    cut.Thickness, LineElement.CapStyle.Round));
            }

            // Afegeix els forats
            //
            for (int i = 0; i < holePoints.Length; i++) {

                // Transforma els punts a la posicio real
                //
                int drill = cut.HoleDiameter;
                int offset = (cut.Thickness - drill) / 2;
                Point q1 = t.ApplyTo(new Point(holePoints[i], -offset));
                Point q2 = t.ApplyTo(new Point(holePoints[i], offset));

                // Afegeix els forats a la placa
                //
                targetBoard.AddElement(new HoleElement(new LayerSet(LayerId.Holes), q1, drill));
                targetBoard.AddElement(new HoleElement(new LayerSet(LayerId.Holes), q2, drill));
            }
        }

        /// <summary>
        /// Afegeix el perfil extern del panell.
        /// </summary>
        /// <param name="size">El tamany del panell.</param>
        /// 
        private void AddProfile(Size size) {

            Rect rect = new Rect(new Point(0, 0), size);
            targetBoard.AddElement(new LineElement(new LayerSet(LayerId.Profile), new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top), 100000, LineElement.CapStyle.Round));
            targetBoard.AddElement(new LineElement(new LayerSet(LayerId.Profile), new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Bottom), 100000, LineElement.CapStyle.Round));
            targetBoard.AddElement(new LineElement(new LayerSet(LayerId.Profile), new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Bottom), 100000, LineElement.CapStyle.Round));
            targetBoard.AddElement(new LineElement(new LayerSet(LayerId.Profile), new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom), 100000, LineElement.CapStyle.Round));
        }

        private sealed class TransformVisitor : DefaultBoardVisitor {

            private readonly Transformation transformation;

            public TransformVisitor(Point offset, Angle rotation) {

                transformation = new Transformation();
                transformation.Translate(offset);
                transformation.Rotate(rotation);
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
