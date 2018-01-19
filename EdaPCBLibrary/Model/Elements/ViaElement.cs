namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Clase que representa una via
    /// </summary>
    public sealed class ViaElement: Element, IPosition, IConectable {

        public enum ViaShape {
            Square,
            Octogonal,
            Circular
        }

        public enum ViaType {
            Through,
            Blind,
            Buried
        }

        private const double drcOuterMin = 0.125;
        private const double drcOuterMax = 2.5;
        private const double drcOuterPercent = 0.25;
        private const double drcInnerMin = 0.125;
        private const double drcInnerMax = 2.5;
        private const double drcInnerPercent = 0.25;

        private Point position;
        private double drill;
        private double outerSize = 0;
        private double innerSize = 0;
        private ViaShape shape = ViaShape.Circular;
        private ViaType type = ViaType.Through;

        /// <summary>
        /// Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public ViaElement():
            base() {

        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany/diametre de la corona.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// <param name="shape">Forma de la corona.</param>
        /// 
        public ViaElement(Point position, double size, double drill, ViaShape shape) :
            base() {

            if (size < 0)
                throw new ArgumentOutOfRangeException("size");

            if (drill <= 0)
                throw new ArgumentOutOfRangeException("drill");

            this.position = position;
            this.outerSize = size;
            this.innerSize = size;
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
        /// Crea el poligon del element.
        /// </summary>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(double inflate = 0) {

            Polygon polygon;
            switch (shape) {
                case ViaShape.Square:
                    polygon = PolygonBuilder.BuildRectangle(position, new Size((OuterSize / 2) + inflate, (OuterSize / 2) + inflate), 0, 0);
                    break;

                case ViaShape.Octogonal:
                    polygon = PolygonBuilder.BuildRegularPolygon(8, position, (OuterSize / 2) + inflate, 0);
                    break;

                default:
                    polygon = PolygonBuilder.BuildCircle(position, (OuterSize / 2) + inflate);
                    break;
            }

            // Si esta inflat, no genera el forat.
            //
            if (inflate == 0)
                polygon.AddChild(PolygonBuilder.BuildCircle(position, drill / 2));

            return polygon;
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox() {

            double size = OuterSize;
            double hSize = size / 2;
            return new Rect(position.X - hSize, position.Y - hSize, size, size);
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre del cercle.
        /// </summary>
        /// 
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        /// <summary>
        /// Obte o asigna el diametre del forat
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
        /// Obte o asigna el tamany de la corona de les capes externes.
        /// </summary>
        /// 
        public double OuterSize {
            get {
                double dimension = outerSize == 0 ? 2 * (drill * drcOuterPercent) + drill : outerSize;
                return Math.Max(drcOuterMin, Math.Min(drcOuterMax, dimension));
            }
            set {
                outerSize = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de la corona de les capes internes.
        /// </summary>
        /// 
        public double InnerSize {
            get {
                double dimension = innerSize == 0 ? 2 * (drill * drcInnerPercent) + drill : innerSize;
                return Math.Max(drcInnerMin, Math.Min(drcInnerMax, dimension));
            }
            set {
                innerSize = value;
            }
        }

        /// <summary>
        /// Obte o asigna la forma exterior. Les interiors sempre son circulars.
        /// </summary>
        /// 
        public ViaShape Shape {
            get {
                return shape;
            }
            set {
                shape = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tipus de via.
        /// </summary>
        /// 
        public ViaType Type {
            get {
                return type;
            }
            set {
                type = value;
            }
        }
    }
}
