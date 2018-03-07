namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa una via
    /// </summary>
    public sealed class ViaElement : Element, IPosition, IConectable {

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

        private const int drcOuterSizeMin = 125000;
        private const int drcOuterSizeMax = 2500000;
        private const int drcOuterSizePercent = 250000;
        private const int drcInnerSizeMin = 125000;
        private const int drcInnerSizeMax = 2500000;
        private const int drcInnerSizePercent = 250000;

        private PointInt position;
        private int drill;
        private int outerSize = 0;
        private int innerSize = 0;
        private ViaShape shape = ViaShape.Circular;
        private ViaType type = ViaType.Through;

        /// <summary>
        /// Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public ViaElement() :
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
        public ViaElement(PointInt position, int size, int drill, ViaShape shape) :
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
        /// Calcula la llista de puns pels poligons
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private PointInt[] BuildPoints(BoardSide side, int spacing) {

            int size = side == BoardSide.Inner ? InnerSize : OuterSize;
            ViaShape shape = side == BoardSide.Inner ? ViaShape.Circular : this.shape;

            switch (shape) {
                case ViaShape.Square:
                    return PolygonBuilder.BuildRectangle(position, new SizeInt(size + spacing * 2, size + spacing * 2), 0, Angle.FromDegrees(0));

                case ViaShape.Octogonal:
                    return PolygonBuilder.BuildPolygon(8, position, (size / 2) + spacing, Angle.FromDegrees(2250));

                default:
                    return PolygonBuilder.BuildCircle(position, (size / 2) + spacing);
            }
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            PointInt[] points = BuildPoints(side, 0);
            PointInt[] holePoints = PolygonBuilder.BuildCircle(position, drill / 2);
            return new Polygon(points, new Polygon[] { new Polygon(holePoints) });
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            PointInt[] points = BuildPoints(side, spacing);
            return new Polygon(points);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override RectInt GetBoundingBox(BoardSide side) {

            int size = side == BoardSide.Inner ? InnerSize : OuterSize;
            return new RectInt(position.X - (size / 2), position.Y - (size / 2), size, size);
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre del cercle.
        /// </summary>
        /// 
        public PointInt Position {
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
        public int Drill {
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
        public int OuterSize {
            get {
                int dimension = outerSize == 0 ? 2 * (drill * drcOuterSizePercent) + drill : outerSize;
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
        public int InnerSize {
            get {
                int dimension = innerSize == 0 ? 2 * (drill * drcInnerSizePercent) + drill : innerSize;
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
