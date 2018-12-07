﻿namespace MikroPic.EdaTools.v1.Panelizer {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.Elements;
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

            panelBoard = board;
        }

        /// <summary>
        /// Genera un panell.
        /// </summary>
        /// <param name="panel">El panell.</param>
        /// 
        public void Panelize(Panel panel) {

            // Afegeix les capes minimes necesaries pel panell.
            //
            AddLayers();

            // Afegeix els elements del panell.
            //
            int index = 0;
            foreach (var element in panel.Elements) {
                if (element is PlaceElement place)
                    AddBoard(place.Board, place.Position, place.Rotation, index++);
                
                else if (element is MillingElement milling)
                    AddMilling(milling.StartPosition, milling.EndPosition, milling.Tickness, milling.CutSpacing, milling.Margin, milling.Cuts);
            }

            // Afegeix el perfil exterior del panell.
            //
            AddProfile(panel.Size);
        }

        /// <summary>
        /// Afegeix les capes necesaries pel panell.
        /// </summary>
        /// 
        private void AddLayers() {

            if (panelBoard.GetLayer(Layer.MillingId, false) == null)
                panelBoard.AddLayer(new Layer(Layer.MillingId, LayerFunction.Mechanical));
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
                if (panelBoard.GetLayer(layer.Id, false) == null)
                    panelBoard.AddLayer(layer.Clone());

            // Afegeix els senyals. Cada placa te les seves propies
            //
            foreach (var signal in board.Signals) {
                string panelSignalName = String.Format("B{0}.{1}", index, signal.Name);
                if (panelBoard.GetSignal(panelSignalName, false) == null) {
                    Signal panelSignal = signal.Clone(panelSignalName);
                    panelBoard.AddSignal(panelSignal);
                }
            }

            // Afegeix els components que no existeixin en la placa de destinacio. Els
            // components son comuns a totes les plaques.
            //
            if (board.HasComponents) {
                foreach (var component in board.Components)
                    if (panelBoard.GetComponent(component.Name, false) == null)
                        panelBoard.AddComponent(component.Clone());
            }

            // Afegeix els parts a la placa
            //
            if (board.HasParts) {
                List<Part> transformableParts = new List<Part>();
                foreach (var part in board.Parts) {
                    Component component = panelBoard.GetComponent(part.Component.Name);
                    Part panelPart = part.Clone(String.Format("B{0}.{1}", index, part.Name), component);
                    transformableParts.Add(panelPart);
                    panelBoard.AddPart(panelPart);

                    foreach (var panelElement in panelPart.Elements) {
                        if (panelElement is PadElement panelPad) {
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

                    //if (element.LayerSet.Contains(Layer.ProfileId))
                    //    continue;

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

        /// <summary>
        /// Afegeix les rutes de fresat
        /// </summary>
        /// <param name="startPosition">Posicio inicial.</param>
        /// <param name="endPosition">Posicio final.</param>
        /// <param name="thickness">Amplada del fresat.</param>
        /// <param name="spacing">Espaiat entre talls</param>
        /// <param name="margin">Marge dels extrems</param>
        /// <param name="cuts">Numero de talls.</param>
        /// 
        private void AddMilling(Point startPosition, Point endPosition, int thickness, int spacing, int margin, int cuts) {

            // Obte el conjunt de capes
            //
            LayerSet millingLayerSet = new LayerSet(Layer.MillingId);
            LayerSet holesLayerSet = new LayerSet(Layer.HolesId);

            // Obte els punts de tall d'una linia 
            //
            double dx = endPosition.X - startPosition.X;
            double dy = endPosition.Y - startPosition.Y;
            int length = (int)Math.Sqrt((dx * dx) + (dy * dy));
            int[] refPoints = GetReferencePoints(cuts, length, margin);
            int[] cutPoints = GetCutReferencePoints(refPoints, spacing);
            int[] holePoints = GetHoleReferencePoints(refPoints, 1000000);

            // Obte la pendent 
            //
            double rad = Math.Atan2(dy, dx);

            // Calcula la transformacio
            //
            Transformation t = new Transformation(startPosition, Angle.FromRadiants(rad));

            // Afegeix les linies de tall
            //
            for (int i = 0; i < cutPoints.Length; i += 2) {

                // Transforma els punts a la posicio real
                //
                Point q1 = t.ApplyTo(new Point(cutPoints[i], 0));
                Point q2 = t.ApplyTo(new Point(cutPoints[i + 1], 0));

                // Afegeix la linia a la placa
                //
                panelBoard.AddElement(new LineElement(millingLayerSet, q1, q2,
                    thickness, LineElement.LineCapStyle.Round));
            }

            // Afegeix els forats
            //
            for (int i = 0; i < holePoints.Length; i++) {

                // Transforma els punts a la posicio real
                //
                int drill = 500000;
                int offset = (thickness - drill) / 2;
                Point q1 = t.ApplyTo(new Point(holePoints[i], -offset));
                Point q2 = t.ApplyTo(new Point(holePoints[i], offset));

                // Afegeix els forats a la placa
                //
                panelBoard.AddElement(new HoleElement(holesLayerSet, q1, drill));
                panelBoard.AddElement(new HoleElement(holesLayerSet, q2, drill));
            }
        }

        /// <summary>
        /// Afegeix el perfil extern del panell.
        /// </summary>
        /// <param name="size">El tamany del panell.</param>
        /// 
        private void AddProfile(Size size) {

            Rect rect = new Rect(new Point(0, 0), size);
            LayerSet profileLayer = new LayerSet(Layer.ProfileId);
            panelBoard.AddElement(new LineElement(profileLayer, new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top), 100000, LineElement.LineCapStyle.Round));
            panelBoard.AddElement(new LineElement(profileLayer, new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Bottom), 100000, LineElement.LineCapStyle.Round));
            panelBoard.AddElement(new LineElement(profileLayer, new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Bottom), 100000, LineElement.LineCapStyle.Round));
            panelBoard.AddElement(new LineElement(profileLayer, new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom), 100000, LineElement.LineCapStyle.Round));
        }

        /// <summary>
        /// Calcula els punts de referencia.
        /// </summary>
        /// <param name="cuts">Nombre de talls.</param>
        /// <param name="length">Longitut total.</param>
        /// <param name="margin">Longitut dels marges laterals.</param>
        /// <returns>La llista de punts de referencia.</returns>
        /// 
        private static int[] GetReferencePoints(int cuts, int length, int margin) {

            if (cuts <= 1)
                return new int[] {
                    0,
                    length };

            else if (cuts == 2)
                return new int[] {
                    0,
                    length / 2,
                    length };

            else if (cuts == 3) {
                return new int[] {
                    0,
                    margin,
                    length - margin,
                    length
                };
            }
            else {

                int numPoints = cuts + 1;
                int cutLength = (length - (margin * 2)) / (cuts - 2);

                int[] points = new int[numPoints];

                int i = 0;
                points[i++] = 0;
                points[i++] = margin;
                while (i < numPoints - 2) {
                    points[i] = margin + (cutLength * (i - 1));
                    i++;
                }
                points[i++] = length - margin;
                points[i] = length;

                return points;
            }
        }

        /// <summary>
        /// Obte els punts de referencia dels tall, a partir dels punts de referencia basics.
        /// </summary>
        /// <param name="refPoints">Punts de referencia.</param>
        /// <param name="spacing">Espaiat dels talls.</param>
        /// <returns>Punts del talls.</returns>
        /// 
        private static int[] GetCutReferencePoints(int[] refPoints, int spacing) {

            int s = spacing / 2;
            int[] points = new int[(refPoints.Length - 1) * 2];

            int r = 0;
            int p = 0;
            points[p++] = refPoints[r++];
            points[p++] = refPoints[r] - s;
            while (r < refPoints.Length - 2) {
                points[p++] = refPoints[r] + s;
                r++;
                points[p++] = refPoints[r] - s;
            }
            points[p++] = refPoints[r++] + s;
            points[p] = refPoints[r];

            return points;
        }

        /// <summary>
        /// Obte els punts de referencia dels forats, a partir dels punts de referencia basics.
        /// </summary>
        /// <param name="refPoints">Punts de referencia.</param>
        /// <param name="spacing">Espaiat entre forats.</param>
        /// <returns>Els dels forats.</returns>
        /// 
        private static int[] GetHoleReferencePoints(int[] refPoints, int spacing) {

            int[] points = new int[(refPoints.Length - 2) * 5];

            int r = 1;
            int p = 0;
            while (r < refPoints.Length - 1) {
                points[p++] = refPoints[r] - spacing - spacing;
                points[p++] = refPoints[r] - spacing;
                points[p++] = refPoints[r];
                points[p++] = refPoints[r] + spacing;
                points[p++] = refPoints[r] + spacing + spacing;
                r++;
            }

            return points;
        }

        private sealed class TransformVisitor: DefaultVisitor { 

            private readonly IEnumerable<MikroPic.EdaTools.v1.Core.Model.Board.IVisitable> visitables;
            private readonly Transformation transformation;

            public TransformVisitor(IEnumerable<MikroPic.EdaTools.v1.Core.Model.Board.IVisitable> visitables, Point offset, Angle rotation) {

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
