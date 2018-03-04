namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Clase que representa un pad throug hole
    /// </summary>
    public sealed class ThPadElement: PadElement, IRotation {

        public enum ThPadShape {
            Square,
            Octogonal,
            Circular,
            Oval
        }

        private const double drcTopSizeMin = 0.125;
        private const double drcTopSizeMax = 2.5;
        private const double drcTopSizePercent = 0.25;

        private ThPadShape shape = ThPadShape.Circular;
        private Angle rotation;
        private double topSize;
        private double innerSize;
        private double bottomSize;
        private double drill;

        /// <summary>
        /// Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public ThPadElement():
            base() {
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Orientacio.</param>
        /// <param name="size">Tamany/diametre del pad.</param>
        /// <param name="shape">Diametre del forat.</param>
        /// <param name="drill">Forma de la corona.</param>
        /// 
        public ThPadElement(string name, Point position, Angle rotation, double size, ThPadShape shape, double drill):
            base(name, position) {

            if (size < 0)
                throw new ArgumentOutOfRangeException("size");

            if (drill <= 0)
                throw new ArgumentOutOfRangeException("drill");

            this.rotation = rotation;
            this.topSize = size;
            this.innerSize = size;
            this.bottomSize = size;
            this.drill = drill;
            this.shape = shape;
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        private Point[] BuildPoints(BoardSide side, double spacing) {

            switch (shape) {
                case ThPadShape.Square:
                    return PolygonBuilder.BuildRectangle(
                        Position,
                        new Size(topSize + (spacing * 2), topSize + (spacing * 2)),
                        spacing,
                        rotation);

                case ThPadShape.Octogonal:
                    return PolygonBuilder.BuildPolygon(
                        8,
                        Position,
                        (topSize / 2) + spacing,
                        rotation + Angle.FromDegrees(22.5));

                case ThPadShape.Oval:
                    return PolygonBuilder.BuildRectangle(
                        Position,
                        new Size((topSize * 2) + (spacing * 2), topSize + (spacing * 2)),
                        (topSize / 2) + spacing,
                        rotation);

                default:
                    return PolygonBuilder.BuildCircle(
                        Position,
                        (topSize / 2) + spacing);
            }
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            Point[] points = BuildPoints(side, 0);
            Point[] holePoints = PolygonBuilder.BuildCircle(Position, drill / 2);
            return new Polygon(points, new Polygon[] { new Polygon(holePoints) });
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, double spacing) {

            Point[] points = BuildPoints(side, spacing);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea el poligon del thermal.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <param name="width">Amplada dels conductors.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetThermalPolygon(BoardSide side, double spacing, double width) {

            double w = ((shape == ThPadShape.Oval ? 2 : 1) * topSize) + (spacing * 2.25);
            double h = topSize + (spacing * 2.25);

            Polygon pour = GetOutlinePolygon(side, spacing);
            Polygon thermal = new Polygon(PolygonBuilder.BuildCross(Position, new Size(w, h), width, rotation));

            List<Polygon> childs = new List<Polygon>();
            childs.AddRange(PolygonProcessor.Clip(pour, thermal, PolygonProcessor.ClipOperation.Diference));
            if (childs.Count != 4)
                throw new InvalidProgramException("Thermal generada incorrectamente.");
            return new Polygon(null, childs.ToArray());
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            double w = ((shape == ThPadShape.Oval ? 2 : 1) * topSize);
            double h = topSize;

            return new Rect(Position.X - w / 2, Position.Y - h / 2, w, h);
        }

        /// <summary>
        /// Obte o asigna l'orientacio del pad.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                    rotation = value;
            }
        }

        /// <summary>
        /// Obte o asigna la forma del pad.
        /// </summary>
        /// 
        public ThPadShape Shape {
            get {
                return shape;
            }
            set {
                    shape = value;
            }
        }

        /// <summary>
        /// Obte o asigna el diametre del forat.
        /// </summary>
        /// 
        public double Drill {
            get {
                return drill;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Drill");

                    drill = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany del pad.
        /// </summary>
        /// 
        public double Size {
            get {
                double dimension = topSize == 0 ? 2 * (drill * drcTopSizePercent) + drill : topSize;
                return Math.Max(drcTopSizeMin, Math.Min(drcTopSizeMax, dimension));
            }
            set {
                    topSize = value;
            }
        }
    }
}
