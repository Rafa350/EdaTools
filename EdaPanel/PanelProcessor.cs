using System;
using System.Collections.Generic;
using System.IO;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.IO;
using MikroPic.EdaTools.v1.Core.IO;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
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
                if (item is EdaPcbItem pcb) {

                    if (!sourceBoardCache.TryGetValue(pcb.FileName, out EdaBoard sourceBoard)) {
                        var path = locator.GetPath(pcb.FileName);
                        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                            var reader = new EdaBoardStreamReader(stream);
                            sourceBoard = reader.ReadBoard();
                        }
                        sourceBoardCache.Add(pcb.FileName, sourceBoard);
                    }
                    AddBoard(sourceBoard, pcb.Position, pcb.Rotation, index++);
                }

                else if (item is EdaCutItem cut)
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
                    targetBoard.AddLayer(CloneLayer(layer));

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
                    string panelPartName = String.Format("{1}@{0}", index, part.Name);
                    EdaPart panelPart = ClonePart(part, panelPartName, component);
                    transformableParts.Add(panelPart);
                    targetBoard.AddPart(panelPart);

                    foreach (var panelElement in panelPart.Elements) {
                        if (panelElement is EdaPadBaseElement panelPad) {
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
                List<EdaElementBase> transformableElements = new List<EdaElementBase>();
                foreach (var boardElement in board.Elements) {

                    EdaElementBase panelElement = CloneElement(boardElement);
                    if (boardElement.IsOnLayer(EdaLayerId.Profile)) {
                        boardElement.LayerSet.Remove(EdaLayerId.Profile);
                        boardElement.LayerSet.Add(EdaLayerId.Get("LocalProfile"));
                    }
                    transformableElements.Add(panelElement);
                    targetBoard.AddElement(panelElement);

                    if (boardElement is IEdaConectable conectable) {
                        EdaSignal signal = board.GetSignal(conectable, null, false);
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
        private void AddCut(EdaCutItem cut) {

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
            EdaTransformation t = new EdaTransformation();
            t.Translate(cut.StartPosition);
            t.Rotate(cut.StartPosition, EdaAngle.FromRadiants(rad));

            // Afegeix les linies de tall
            //
            for (int i = 0; i < cutPoints.Length; i += 2) {

                // Transforma els punts a la posicio real
                //
                EdaPoint q1 = t.Transform(new EdaPoint(cutPoints[i], 0));
                EdaPoint q2 = t.Transform(new EdaPoint(cutPoints[i + 1], 0));

                // Afegeix la linia a la placa
                //
                targetBoard.AddElement(new EdaLineElement {
                    LayerSet = new EdaLayerSet(EdaLayerId.Milling),
                    StartPosition = q1,
                    EndPosition = q2,
                    Thickness = cut.Thickness,
                    LineCap = EdaLineCap.Round
                });
            }

            // Afegeix els forats
            //
            for (int i = 0; i < holePoints.Length; i++) {

                // Transforma els punts a la posicio real
                //
                int drill = cut.HoleDiameter;
                int offset = (cut.Thickness - drill) / 2;
                EdaPoint q1 = t.Transform(new EdaPoint(holePoints[i], -offset));
                EdaPoint q2 = t.Transform(new EdaPoint(holePoints[i], offset));

                // Afegeix els forats a la placa
                //
                targetBoard.AddElement(new EdaCircularHoleElement {
                    Position = q1,
                    Diameter = drill,
                    Platted = false
                });

                targetBoard.AddElement(new EdaCircularHoleElement {
                    Position = q2,
                    Diameter = drill,
                    Platted = false
                });
            }
        }

        /// <summary>
        /// Afegeix el perfil extern del panell.
        /// </summary>
        /// <param name="size">El tamany del panell.</param>
        /// 
        private void AddProfile(EdaSize size) {

            var rect = new EdaRect(new EdaPoint(0, 0), size);

            targetBoard.AddElement(new EdaLineElement {
                LayerSet = new EdaLayerSet(EdaLayerId.Profile),
                StartPosition = new EdaPoint(rect.Left, rect.Top),
                EndPosition = new EdaPoint(rect.Right, rect.Top),
                Thickness = 100000,
                LineCap = EdaLineCap.Round
            });

            targetBoard.AddElement(new EdaLineElement {
                LayerSet = new EdaLayerSet(EdaLayerId.Profile),
                StartPosition = new EdaPoint(rect.Left, rect.Bottom),
                EndPosition = new EdaPoint(rect.Right, rect.Bottom),
                Thickness = 100000,
                LineCap = EdaLineCap.Round
            });

            targetBoard.AddElement(new EdaLineElement {
                LayerSet = new EdaLayerSet(EdaLayerId.Profile),
                StartPosition = new EdaPoint(rect.Left, rect.Top),
                EndPosition = new EdaPoint(rect.Left, rect.Bottom),
                Thickness = 100000,
                LineCap = EdaLineCap.Round
            });

            targetBoard.AddElement(new EdaLineElement {
                LayerSet = new EdaLayerSet(EdaLayerId.Profile),
                StartPosition = new EdaPoint(rect.Right, rect.Top),
                EndPosition = new EdaPoint(rect.Right, rect.Bottom),
                Thickness = 100000,
                LineCap = EdaLineCap.Round
            });
        }

        private static EdaLayer CloneLayer(EdaLayer layer) {

            return layer.Clone();
        }

        /// <summary>
        /// Clona un senyal
        /// </summary>
        /// <param name="signal">El senyal a clonar.</param>
        /// <param name="name">El nom de la senyal clonada.</param>
        /// <returns>El clon.</returns>
        /// 
        private static EdaSignal CloneSignal(EdaSignal signal, string name) {

            return new EdaSignal {
                Name = name,
                Clearance = signal.Clearance
            };
        }

        /// <summary>
        /// Clona un part
        /// </summary>
        /// <param name="part">El part a clonar.</param>
        /// <param name="name">El nom del part clonat.</param>
        /// <param name="component">El component.</param>
        /// <returns>El clon.</returns>
        /// 
        private static EdaPart ClonePart(EdaPart part, string name, EdaComponent component) {

            var newPart = new EdaPart {
                Name = name,
                Component = component,
                Position = part.Position,
                Rotation = part.Rotation,
                Side = part.Side
            };

            if (part.HasAttributes)
                foreach (var attr in part.Attributes) {
                    var newAttr = new EdaPartAttribute(attr.Name, attr.Value);
                    newPart.AddAttribute(newAttr);
                }

            return newPart;
        }

        /// <summary>
        /// Clona un component.
        /// </summary>
        /// <param name="component">El component a clonar.</param>
        /// <returns>El clon.</returns>
        /// 
        private static EdaComponent CloneComponent(EdaComponent component) {

            var newComponent = new EdaComponent {
                Name = component.Name,
                Description = component.Description,
            };

            if (component.HasElements)
                foreach (var element in component.Elements)
                    newComponent.AddElement(CloneElement(element));

            if (component.HasAttributes)
                foreach (var attr in component.Attributes) {
                    var newAttr = new EdaComponentAttribute(attr.Name, attr.Value);
                    component.AddAttribute(newAttr);
                }

            return newComponent;
        }

        /// <summary>
        /// Clona un element.
        /// </summary>
        /// <param name="element">L'element a clonar.</param>
        /// <returns>El clon.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// 
        private static EdaElementBase CloneElement(EdaElementBase element) {

            if (element is EdaCircleElement circle)
                return new EdaCircleElement {
                    Position = circle.Position,
                    Radius = circle.Radius,
                    Thickness = circle.Thickness,
                    LayerSet = circle.LayerSet
                };

            else if (element is EdaCircularHoleElement circleHole)
                return new EdaCircularHoleElement {
                    Position = circleHole.Position,
                    Diameter = circleHole.Diameter,
                    Platted = circleHole.Platted
                };

            else if (element is EdaLineElement line)
                return new EdaLineElement {
                    StartPosition = line.StartPosition,
                    EndPosition = line.EndPosition,
                    Thickness = line.Thickness,
                    LayerSet = line.LayerSet
                };

            else if (element is EdaSmtPadElement smdPad)
                return new EdaSmtPadElement {
                    Name = smdPad.Name,
                    Position = smdPad.Position,
                    Rotation = smdPad.Rotation,
                    LayerSet = smdPad.LayerSet,
                    Size = smdPad.Size,
                    CornerRatio = smdPad.CornerRatio,
                    CornerShape = smdPad.CornerShape
                };

            else if (element is EdaThtPadElement thPad)
                return new EdaThtPadElement {
                    Name = thPad.Name,
                    Position = thPad.Position,
                    Rotation = thPad.Rotation,
                    LayerSet = thPad.LayerSet,
                    TopSize = thPad.TopSize,
                    InnerSize = thPad.InnerSize,
                    BottomSize = thPad.BottomSize,
                    CornerRatio = thPad.CornerRatio,
                    CornerShape = thPad.CornerShape,
                    DrillDiameter = thPad.DrillDiameter,
                    Slot = thPad.Slot
                };

            else if (element is EdaViaElement via)
                return new EdaViaElement {
                    Position = via.Position,
                    OuterSize = via.OuterSize,
                    InnerSize = via.InnerSize,
                    LayerSet = via.LayerSet,
                    DrillDiameter = via.DrillDiameter
                };

            else if (element is EdaTextElement text)
                return new EdaTextElement {
                    Position = text.Position,
                    Rotation = text.Rotation,
                    Thickness = text.Thickness,
                    LayerSet = text.LayerSet,
                    Height = text.Height,
                    HorizontalAlign = text.HorizontalAlign,
                    VerticalAlign = text.VerticalAlign,
                    Value = text.Value
                };

            else if (element is EdaRegionElement region)
                return new EdaRegionElement {
                    Thickness = region.Thickness,
                    Priority = region.Priority,
                    Clearance = region.Clearance,
                };

            else
                throw new InvalidOperationException($"No es posible cloner el elemento de tipo {element.GetType()}.");
        }

        private sealed class TransformVisitor: EdaDefaultBoardVisitor {

            private readonly EdaTransformation transformation;

            public TransformVisitor(EdaPoint offset, EdaAngle rotation) {

                transformation = new EdaTransformation();
                transformation.Translate(offset);
                transformation.Rotate(rotation);
            }

            public override void Visit(EdaLineElement line) {

                line.StartPosition = transformation.Transform(line.StartPosition);
                line.EndPosition = transformation.Transform(line.EndPosition);
            }

            public override void Visit(EdaArcElement arc) {

                arc.StartPosition = transformation.Transform(arc.StartPosition);
                arc.EndPosition = transformation.Transform(arc.EndPosition);
            }

            public override void Visit(EdaRectangleElement rectangle) {

                rectangle.Position = transformation.Transform(rectangle.Position);
            }

            public override void Visit(EdaCircleElement circle) {

                circle.Position = transformation.Transform(circle.Position);
            }

            public override void Visit(EdaSmtPadElement pad) {

                pad.Position = transformation.Transform(pad.Position);
            }

            public override void Visit(EdaThtPadElement pad) {

                pad.Position = transformation.Transform(pad.Position);
            }

            public override void Visit(EdaViaElement via) {

                via.Position = transformation.Transform(via.Position);
            }

            public override void Visit(EdaRegionElement region) {

                //foreach (var segment in region.Segments)
                //  segment.Position = transformation.ApplyTo(segment.Position);
            }

            public override void Visit(EdaPart part) {

                part.Position = transformation.Transform(part.Position);
            }
        }
    }
}
