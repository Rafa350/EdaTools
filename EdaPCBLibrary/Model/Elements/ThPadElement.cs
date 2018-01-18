namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
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
        private double rotation;
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
        public ThPadElement(string name, Point position, double rotation, double size, ThPadShape shape, double drill):
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

        /// <summary>
        /// Calcula el numero de serie del element.
        /// </summary>
        /// <returns>El numero de serie.</returns>
        /// 
        protected override int GetSerial() {

            string s = String.Format("{0}${1}${2}${3}${4}${5}${6}${7}${8}${9}",
                GetType().FullName,
                Name,
                Position.X,
                Position.Y,
                rotation,
                topSize,
                innerSize,
                bottomSize,
                drill,
                shape);
            return s.GetHashCode();
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="inflate">Increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(double inflate = 0) {

            Polygon polygon;
            switch (shape) {
                case ThPadShape.Square:
                    polygon = PolygonBuilder.BuildRectangle(
                        Position, 
                        new Size(topSize + (inflate * 2), topSize + (inflate * 2)), 
                        inflate,
                        rotation);
                    break;

                case ThPadShape.Octogonal:
                    polygon = PolygonBuilder.BuildRegularPolygon(
                        8, 
                        Position, 
                        (topSize / 2) + inflate, 
                        rotation);
                    break;

                case ThPadShape.Oval:
                    polygon = PolygonBuilder.BuildRectangle(
                        Position, 
                        new Size((topSize * 2) + (inflate * 2), topSize + (inflate * 2)), 
                        (topSize / 2) + inflate,
                        rotation);
                    break;

                default:
                    polygon = PolygonBuilder.BuildCircle(
                        Position, 
                        (topSize / 2) + inflate);
                    break;
            }

            // Si esta inflat, no genera el forat
            //
            if (inflate == 0)
                polygon.AddChild(PolygonBuilder.BuildCircle(Position, drill / 2));

            return polygon;
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox() {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Obte o asigna l'orientacio del pad.
        /// </summary>
        /// 
        public double Rotation {
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
