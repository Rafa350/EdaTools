namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    using System;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

    /// <summary>
    /// Clase que representa una via
    /// </summary>
    /// 
    public sealed class ViaElement : Element, IPosition, IConectable {

        public enum ViaShape {
            Square,
            Octagon,
            Circle
        }

        public enum ViaType {
            Through,
            Blind,
            Buried
        }

        private const double cos2250 = 0.92387953251128675612818318939679;

        private int drcOuterSizeMin = 125000;
        private int drcOuterSizeMax = 2500000;
        private Ratio drcOuterSizePercent = Ratio.P25;
        private int drcInnerSizeMin = 125000;
        private int drcInnerSizeMax = 2500000;
        private Ratio drcInnerSizePercent = Ratio.P25;

        private Point position;
        private int drill;
        private int outerSize = 0;
        private int innerSize = 0;
        private ViaShape shape = ViaShape.Circle;
        private ViaType type = ViaType.Through;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany/diametre de la corona.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// <param name="shape">Forma de la corona.</param>
        /// 
        public ViaElement(LayerSet layerSet, Point position, int size, int drill, ViaShape shape) :
            this(layerSet, position, size, size, drill, shape) {
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="outerSize">Tamany/diametre de la corona per capes externes.</param>
        /// <param name="innerSize">Tamany/diametre de la corona per capes internes.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// <param name="shape">Forma de la corona.</param>
        /// 
        public ViaElement(LayerSet layerSet, Point position, int outerSize, int innerSize, int drill, ViaShape shape) :
            base(layerSet) {

            if (innerSize < 0)
                throw new ArgumentOutOfRangeException(nameof(innerSize));

            if (outerSize < 0)
                throw new ArgumentOutOfRangeException(nameof(outerSize));

            if (drill <= 0)
                throw new ArgumentOutOfRangeException(nameof(drill));

            this.position = position;
            this.outerSize = outerSize;
            this.innerSize = innerSize;
            this.drill = drill;
            this.shape = shape;
        }

        /// <summary>
        /// Obte un clon de l'element.
        /// </summary>
        /// <returns>El clon de l'element.</returns>
        /// 
        public override Element Clone() {

            return new ViaElement(LayerSet, position, outerSize, innerSize, drill, shape);
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el hash del objecte.
        /// </summary>
        /// <returns>El hash.</returns>
        /// 
        public override int GetHashCode() =>
            position.GetHashCode() + 
            (outerSize * 31) + 
            (innerSize * 111) + 
            (drill * 13) +
            (shape.GetHashCode() * 23) + 
            (type.GetHashCode() * 73);

        /// <summary>
        /// Calcula la llista de puns pels poligons
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private Point[] MakePoints(BoardSide side, int spacing) {

            int size = side == BoardSide.Inner ? InnerSize : OuterSize;
            int sizeM2 = size * 2;
            int sizeD2 = size / 2;

            int spacingM2 = spacing * 2;
            int spacingD2 = spacing / 2;

            ViaShape shape = side == BoardSide.Inner ? ViaShape.Circle : this.shape;

            switch (shape) {
                case ViaShape.Square:
                    return PolygonBuilder.MakeRectangle(
                        position,
                        new Size(size + spacingM2, size + spacingM2),
                        0,
                        Angle.FromValue(0));

                case ViaShape.Octagon:
                    return PolygonBuilder.MakeRegularPolygon(
                        8,
                        position,
                        (int)((double)sizeD2 / cos2250) + spacing,
                        Angle.FromValue(2250));

                default:
                    return PolygonBuilder.MakeCircle(
                        position,
                        sizeD2 + spacing);
            }
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            int hash = GetHashCode() * side.GetHashCode() * 273;
            Polygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                Point[] points = MakePoints(side, 0);
                Point[] holePoints = PolygonBuilder.MakeCircle(position, drill / 2);
                polygon = new Polygon(points, new Polygon(holePoints));

                PolygonCache.Save(hash, polygon); 
            }

            return polygon;
        }

        /// <summary>
        /// Crea el poligon exterior del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            int hash = GetHashCode() + (side.GetHashCode() * 11327) + (spacing * 131);
            Polygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                Point[] points = MakePoints(side, spacing);
                polygon = new Polygon(points);

                PolygonCache.Save(hash, polygon);
            }

            return polygon;
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            int size = side == BoardSide.Inner ? InnerSize : OuterSize;
            return new Rect(position.X - (size / 2), position.Y - (size / 2), size, size);
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
                int ring = Math.Max(drcOuterSizeMin, Math.Min(drcOuterSizeMax, drill * drcOuterSizePercent));
                return drill + ring * 2;
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
                int ring = Math.Max(drcInnerSizeMin, Math.Min(drcInnerSizeMax, drill * drcInnerSizePercent));
                return drill + ring * 2;
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

        /// <summary>
        /// Obte el tipus d'element.
        /// </summary>
        /// 
        public override ElementType ElementType =>
            ElementType.Via;
    }
}
