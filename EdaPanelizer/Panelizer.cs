﻿namespace MikroPic.EdaTools.v1.Panelizer {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
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
                if (element is PlaceElement place) {
                    AddBoard(place.Board, place.Position, place.Rotation, index++);
                    //AddBoardRoute(place.Board, place.Position, place.Rotation, 2000000);
                }
                else if (element is MillingElement milling)
                    AddMilling(milling.StartPosition, milling.EndPosition, milling.Tickness, milling.Spacing, milling.Margin, milling.Cuts);
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

                    if (element.LayerSet.Contains(Layer.ProfileId))
                        continue;

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

        private void AddMilling(Point startPosition, Point endPosition, int thickness, int spacing, int margin, int cuts) {

            // Obte el conjunt de capes
            //
            LayerSet layerSet = new LayerSet(Layer.MillingId);

            // Obte els punts de tall d'una linia 
            //
            double dx = endPosition.X - startPosition.X;
            double dy = endPosition.Y - startPosition.Y;
            int length = (int)Math.Sqrt((dx * dx) + (dy * dy));
            int[] points = CutLine(length, cuts, spacing, margin);

            // Obte la pendent 
            //
            double rad = Math.Atan2(dy, dx);

            // Repeteix per tots els punbt de la linia
            //
            for (int j = 0; j < points.Length; j += 2) {

                // Transforma els punts a la posicio real
                //
                Transformation t = new Transformation(startPosition, Angle.FromRadiants(rad));
                Point q1 = t.ApplyTo(new Point(points[j], 0));
                Point q2 = t.ApplyTo(new Point(points[j + 1], 0));

                // Afegeix la linia a la placa
                //
                panelBoard.AddElement(
                    new LineElement(
                        layerSet,
                        q1,
                        q2,
                        thickness,
                        LineElement.LineCapStyle.Round));
            }
        }

        /// <summary>
        /// Afegeix el fresat de separacio de la placa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Orientacio.</param>
        /// <param name="thickness">Amplada del fresat.</param>
        /// 
        private void AddBoardRoute(Board board, Point position, Angle rotation, int thickness) {

            Polygon outline = board.GetOutlinePolygon();
            Polygon route = PolygonProcessor.Offset(outline, thickness / 2);

            Transformation transformation = new Transformation(position, rotation);
            LayerSet layerSet = new LayerSet(Layer.MillingId);
            for (int i = 0; i < route.Points.Length - 1; i++) {

                // Calcula els punts de tall d'una recta de longitut equivalent
                //
                Point p1 = route.Points[i];
                Point p2 = route.Points[i + 1];
                double dx = p2.X - p1.X;
                double dy = p2.Y - p1.Y;
                int length = (int)Math.Sqrt((dx * dx) + (dy * dy));
                int[] line = CutLine(length, 2, 4000000, 0);

                // Calcula la pendent del segment
                //
                double rad = Math.Atan2(dy, dx);

                for (int j = 0; j < line.Length; j += 2) {

                    Transformation t = new Transformation(p1, Angle.FromRadiants(rad));
                    Point q1 = t.ApplyTo(new Point(line[j], 0));
                    Point q2 = t.ApplyTo(new Point(line[j + 1], 0));

                    panelBoard.AddElement(new LineElement(
                        layerSet, 
                        transformation.ApplyTo(q1),
                        transformation.ApplyTo(q2), 
                        thickness, 
                        LineElement.LineCapStyle.Round));
                }
            }
            panelBoard.AddElement(new LineElement(layerSet, transformation.ApplyTo(route.Points[route.Points.Length - 1]),
                transformation.ApplyTo(route.Points[0]), thickness, LineElement.LineCapStyle.Round));
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

        private int[] CutLine(int length, int numCuts, int spacing, int margin) {

            int s = spacing / 2;

            if (numCuts == 1)
                return new int[] {
                    0,
                    length };

            else if (numCuts == 2)
                return new int[] {
                    0,
                    (length / 2) - s,
                    (length / 2) + s,
                    length };

            else if (numCuts == 3) {
                return new int[] {
                    0,
                    margin - s,
                    margin + s,
                    length - margin - s,
                    length - margin + s,
                    length
                };
            }
            else {
                int cutLength = length / (numCuts + 1);
                if (cutLength > (spacing * 4)) {

                    int numPoints = (2 * numCuts) + 2;

                    int[] points = new int[numPoints];

                    int ptIdx = 0;
                    points[ptIdx++] = 0;
                    for (int cutIdx = 1; cutIdx <= numCuts; cutIdx++) {
                        points[ptIdx++] = (cutLength * cutIdx) - s;
                        points[ptIdx++] = (cutLength * cutIdx) + s;
                    }
                    points[ptIdx++] = length;

                    return points;
                }
                else
                    return new int[] { 0, length };
            }
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
