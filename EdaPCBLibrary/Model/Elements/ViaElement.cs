namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
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

        private const double drcOuterSizeMin = 0.125;
        private const double drcOuterSizeMax = 2.5;
        private const double drcOuterSizePercent = 0.25;
        private const double drcInnerSizeMin = 0.125;
        private const double drcInnerSizeMax = 2.5;
        private const double drcInnerSizePercent = 0.25;

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
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            Polygon polygon = GetOutlinePolygon(side, 0);
            polygon.AddChild(PolygonBuilder.BuildCircle(position, drill / 2));
            return polygon;
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, double spacing) {

            double size = side == BoardSide.Inner ? InnerSize : OuterSize;
            ViaShape shape = side == BoardSide.Inner ? ViaShape.Circular : this.shape;

            switch (shape) {
                case ViaShape.Square:
                    return PolygonBuilder.BuildRectangle(position, new Size(size + spacing * 2, size + spacing * 2), 0, Angle.FromDegrees(0));

                case ViaShape.Octogonal:
                    return PolygonBuilder.BuildRegularPolygon(8, position, (size / 2) + spacing, Angle.FromDegrees(22.5));

                default:
                    return PolygonBuilder.BuildCircle(position, (size / 2) + spacing);
            }
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            double size = side == BoardSide.Inner ? InnerSize : OuterSize;
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
                double dimension = outerSize == 0 ? 2 * (drill * drcOuterSizePercent) + drill : outerSize;
                return Math.Max(drcOuterSizeMin, Math.Min(drcOuterSizeMax, dimension));
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
                double dimension = innerSize == 0 ? 2 * (drill * drcInnerSizePercent) + drill : innerSize;
                return Math.Max(drcInnerSizeMin, Math.Min(drcInnerSizeMax, dimension));
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
