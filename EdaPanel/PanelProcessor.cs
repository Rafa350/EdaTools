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

namespace MikroPic.EdaTools.v1.Panel {

    public sealed class PanelProcessor {

        private readonly Dictionary<string, EdaBoard> sourceBoardCache = new Dictionary<string, EdaBoard>();
        private readonly EdaBoard targetBoard;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="targetBoard">La placa de desti.</param>
        /// 
        public PanelProcessor(EdaBoard targetBoard) {

            if (targetBoard == null)
                throw new ArgumentNullException(nameof(targetBoard));

            this.targetBoard = targetBoard;
        }

        /// <summary>
        /// Genera un panell sobre la placa de desti.
        /// </summary>
        /// <param name="project">El projecte del panell.</param>
        /// <param name="locator">Localitzador de recursos.</param>
        /// 
        public void Panelize(EdaPanel project, IStreamLocator locator) {

            // Afegeix les capes minimes necesaries pel panell.
            //
            AddLayers();

            // Afegeix els elements del panell.
            //
            int index = 0;
            foreach (var item in project.Items) {
                if (item is PcbItem pcb) {

                    if (!sourceBoardCache.TryGetValue(pcb.FileName, out EdaBoard sourceBoard)) {
                        var path = locator.GetPath(pcb.FileName);
                        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                            var reader = new BoardStreamReader(stream);
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

            if (targetBoard.GetLayer(EdaLayerId.Milling, false) == null)
                targetBoard.AddLayer(new EdaLayer(EdaLayerId.Milling, BoardSide.None, LayerFunction.Mechanical));
            if (targetBoard.GetLayer(EdaLayerId.Get("LocalProfile"), false) == null)
                targetBoard.AddLayer(new EdaLayer(EdaLayerId.Get("LocalProfile"), BoardSide.None, LayerFunction.Mechanical));
        }

        /// <summary>
        /// Afegeix una placa
        /// </summary>
        /// <param name="board">La placa a afeigir.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Orientacio.</param>
        /// <param name="index">Index de la placa.</param>
        /// 
        private void AddBoard(EdaBoard board, EdaPoint position, EdaAngle rotation, int index) {

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
                    targetBoard.AddSignal(CloneSignal(signal, signalName));
            }

            // Afegeix els components que no existeixin en la placa de destinacio. Els
            // components son comuns a totes les plaques.
            //
            if (board.HasComponents) {
                foreach (var component in board.Components)
                    if (targetBoard.GetComponent(component.Name, false) == null)
                        targetBoard.AddComponent(CloneComponent(component));
            }

            // Afegeix els parts a la placa
            //
            if (board.HasParts) {
                List<EdaPart> transformableParts = new List<EdaPart>();
                foreach (var part in board.Parts) {
                    EdaComponent component = targetBoard.GetComponent(part.Component.Name);
                    EdaPart panelPart = ClonePart(part, String.Format("{1}@{0}", index, part.Name), component);
                    transformableParts.Add(panelPart);
                    targetBoard.AddPart(panelPart);

                    foreach (var panelElement in panelPart.Elements) {
                        if (panelElement is PadElement panelPad) {
                            EdaSignal signal = board.GetSignal(part.GetPad(panelPad.Name), part, false);
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
                List<EdaElement> transformableElements = new List<EdaElement>();
                foreach (var boardElement in board.Elements) {

                    EdaElement panelElement = CloneElement(boardElement);
                    if (boardElement.IsOnLayer(EdaLayerId.Profile)) {
                        boardElement.LayerSet.Remove(EdaLayerId.Profile);
                        boardElement.LayerSet.Add(EdaLayerId.Get("LocalProfile"));
                    }
                    transformableElements.Add(panelElement);
                    targetBoard.AddElement(panelElement);

                    if (boardElement is IEdaConectable) {
                        EdaSignal signal = board.GetSignal(boardElement, null, false);
                        if (signal != null) {
                            string panelSignalName = String.Format("{1}@{0}", index, signal.Name);
                            targetBoard.Connect(targetBoard.GetSignal(panelSignalName), panelElement as IEdaConectable);
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
            t.Rotate(cut.StartPosition, EdaAngle.FromRadiants(rad));

            // Afegeix les linies de tall
            //
            for (int i = 0; i < cutPoints.Length; i += 2) {

                // Transforma els punts a la posicio real
                //
                EdaPoint q1 = t.ApplyTo(new EdaPoint(cutPoints[i], 0));
                EdaPoint q2 = t.ApplyTo(new EdaPoint(cutPoints[i + 1], 0));

                // Afegeix la linia a la placa
                //
                targetBoard.AddElement(new LineElement {
                    LayerSet = new EdaLayerSet(EdaLayerId.Milling),
                    StartPosition = q1,
                    EndPosition = q2,
                    Thickness = cut.Thickness,
                    LineCap = LineElement.CapStyle.Round
                });
            }

            // Afegeix els forats
            //
            for (int i = 0; i < holePoints.Length; i++) {

                // Transforma els punts a la posicio real
                //
                int drill = cut.HoleDiameter;
                int offset = (cut.Thickness - drill) / 2;
                EdaPoint q1 = t.ApplyTo(new EdaPoint(holePoints[i], -offset));
                EdaPoint q2 = t.ApplyTo(new EdaPoint(holePoints[i], offset));

                // Afegeix els forats a la placa
                //
                targetBoard.AddElement(new HoleElement {
                    LayerSet = new EdaLayerSet(EdaLayerId.Holes),
                    Position = q1,
                    Drill = drill
                });

                targetBoard.AddElement(new HoleElement {
                    LayerSet = new EdaLayerSet(EdaLayerId.Holes),
                    Position = q2,
                    Drill = drill
                });
            }
        }

        /// <summary>
        /// Afegeix el perfil extern del panell.
        /// </summary>
        /// <param name="size">El tamany del panell.</param>
        /// 
        private void AddProfile(EdaSize size) {

            var rect = new Rect(new EdaPoint(0, 0), size);

            targetBoard.AddElement(new LineElement {
                LayerSet = new EdaLayerSet(EdaLayerId.Profile),
                StartPosition = new EdaPoint(rect.Left, rect.Top),
                EndPosition = new EdaPoint(rect.Right, rect.Top),
                Thickness = 100000,
                LineCap = LineElement.CapStyle.Round
            });

            targetBoard.AddElement(new LineElement {
                LayerSet = new EdaLayerSet(EdaLayerId.Profile),
                StartPosition = new EdaPoint(rect.Left, rect.Bottom),
                EndPosition = new EdaPoint(rect.Right, rect.Bottom),
                Thickness = 100000,
                LineCap = LineElement.CapStyle.Round
            });

            targetBoard.AddElement(new LineElement {
                LayerSet = new EdaLayerSet(EdaLayerId.Profile),
                StartPosition = new EdaPoint(rect.Left, rect.Top),
                EndPosition = new EdaPoint(rect.Left, rect.Bottom),
                Thickness = 100000,
                LineCap = LineElement.CapStyle.Round
            });

            targetBoard.AddElement(new LineElement {
                LayerSet = new EdaLayerSet(EdaLayerId.Profile),
                StartPosition = new EdaPoint(rect.Right, rect.Top),
                EndPosition = new EdaPoint(rect.Right, rect.Bottom),
                Thickness = 100000,
                LineCap = LineElement.CapStyle.Round
            });
        }

        /// <summary>
        /// Clona un senyal
        /// </summary>
        /// <param name="signal">El senyal.</param>
        /// <param name="name">El nom de la senyal clonada.</param>
        /// <returns>El clon.</returns>
        /// 
        private static EdaSignal CloneSignal(EdaSignal signal, string name) {

            var clon = new EdaSignal {
                Name = name,
                Clearance = signal.Clearance
            };

            return clon;
        }

        private static EdaPart ClonePart(EdaPart part, string name, EdaComponent component) {

            var clon = new EdaPart {
                Name = name,
                Component = component,
                Position = part.Position,
                Rotation = part.Rotation,
                Flip = part.Flip
            };

            return clon;
        }

        /// <summary>
        /// Clona un component.
        /// </summary>
        /// <param name="component">El component</param>
        /// <returns>EWl clon</returns>
        /// 
        private static EdaComponent CloneComponent(EdaComponent component) {

            var clon = new EdaComponent {
                Name = component.Name,
                Description = component.Description,
            };

            foreach (var element in component.Elements)
                clon.AddElement(CloneElement(element));

            return clon;
        }

        private static EdaElement CloneElement(EdaElement element) {

            return null;
        }

        private sealed class TransformVisitor : EdaDefaultBoardVisitor {

            private readonly Transformation transformation;

            public TransformVisitor(EdaPoint offset, EdaAngle rotation) {

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

                //foreach (var segment in region.Segments)
                  //  segment.Position = transformation.ApplyTo(segment.Position);
            }

            public override void Visit(HoleElement hole) {

                hole.Position = transformation.ApplyTo(hole.Position);
            }

            public override void Visit(EdaPart part) {

                part.Position = transformation.ApplyTo(part.Position);
            }
        }
    }
}
