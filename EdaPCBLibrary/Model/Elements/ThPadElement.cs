namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure;
    using System;
    using System.Collections.Generic;

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

        private int drcTopSizeMin = 125000;
        private int drcTopSizeMax = 2500000;
        private Ratio drcTopSizePercent = Ratio.P25;

        private ThPadShape shape = ThPadShape.Circular;
        private Angle rotation;
        private int topSize;
        private int innerSize;
        private int bottomSize;
        private int drill;

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
        public ThPadElement(string name, PointInt position, Angle rotation, int size, ThPadShape shape, int drill):
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
        /// Crea la llista de punts d'un poligon
        /// </summary>
        /// <param name="side">Cara de la placa</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private PointInt[] BuildPoints(BoardSide side, int spacing) {

            int size = TopSize;

            int sizeMul2 = size << 1;
            int sizeDiv2 = size >> 1;

            int spacingMul2 = spacing << 1;
            int spacingDiv2 = spacing >> 1;

            switch (shape) {
                case ThPadShape.Square:
                    return PolygonBuilder.BuildRectangle(
                        Position,
                        new SizeInt(size + spacingMul2, size + spacingMul2),
                        spacing,
                        rotation);

                case ThPadShape.Octogonal:
                    return PolygonBuilder.BuildPolygon(
                        8,
                        Position,
                        sizeDiv2 + spacing,
                        rotation + Angle.FromDegrees(2250));

                case ThPadShape.Oval:
                    return PolygonBuilder.BuildRectangle(
                        Position,
                        new SizeInt(sizeMul2 + spacingMul2, size + spacingMul2),
                        sizeDiv2 + spacing,
                        rotation);

                default:
                    return PolygonBuilder.BuildCircle(
                        Position,
                        sizeDiv2 + spacing);
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
            PointInt[] holePoints = PolygonBuilder.BuildCircle(Position, drill / 2);

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
        /// Crea el poligon del thermal.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <param name="width">Amplada dels conductors.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetThermalPolygon(BoardSide side, int spacing, int width) {

            int w = ((shape == ThPadShape.Oval ? 2 : 1) * topSize) + spacing + spacing;
            int h = topSize + spacing + spacing;

            Polygon pour = GetOutlinePolygon(side, spacing);
            Polygon thermal = new Polygon(PolygonBuilder.BuildCross(Position, new SizeInt(w, h), width, rotation));

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
        public override RectInt GetBoundingBox(BoardSide side) {

            int w = ((shape == ThPadShape.Oval ? 2 : 1) * topSize);
            int h = topSize;

            return new RectInt(Position.X - (w / 2), Position.Y - (h / 2), w, h);
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
        /// Obte o asigna el tamany del pad de la cara superior.
        /// </summary>
        /// 
        public int TopSize {
            get {
                int dimension = topSize == 0 ? 2 * (drill * drcTopSizePercent) + drill : topSize;
                return Math.Max(drcTopSizeMin, Math.Min(drcTopSizeMax, dimension));
            }
            set {
                topSize = value;
            }
        }
    }
}
