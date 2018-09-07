namespace MikroPic.EdaTools.v1.Pcb.Model.BoardElements {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure.Polygons;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa un pad throug hole
    /// </summary>
    /// 
    public sealed class ThPadElement : PadElement {

        public enum ThPadShape {
            Square,
            Octagon,
            Circle,
            Oval
        }

        private int drcTopSizeMin = 175000;
        private int drcTopSizeMax = 2500000;
        private Ratio drcTopSizePercent = Ratio.P25;
        private int drcBottomSizeMin = 175000;
        private int drcBottomSizeMax = 2500000;
        private Ratio drcBottomSizePercent = Ratio.P25;

        private ThPadShape shape = ThPadShape.Circle;
        private int topSize;
        private int innerSize;
        private int bottomSize;
        private int drill;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Orientacio.</param>
        /// <param name="size">Tamany/diametre del pad.</param>
        /// <param name="shape">Diametre del forat.</param>
        /// <param name="drill">Forma de la corona.</param>
        /// 
        public ThPadElement(string name, LayerSet layerSet, Point position, Angle rotation, int size, ThPadShape shape, int drill) :
            this(name, layerSet, position, rotation, size, size, size, shape, drill) { 
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Orientacio.</param>
        /// <param name="topSize">Tamany/diametre del pad.</param>
        /// <param name="innerSize">Tamany/diametre del pad.</param>
        /// <param name="innerSize">Tamany/diametre del pad.</param>
        /// <param name="bottom">Diametre del forat.</param>
        /// <param name="drill">Forma de la corona.</param>
        /// 
        public ThPadElement(string name, LayerSet layerSet, Point position, Angle rotation, int topSize, int innerSize,
            int bottomSize, ThPadShape shape, int drill):
            base(name, layerSet, position, rotation) {

            if (topSize < 0)
                throw new ArgumentOutOfRangeException("topSize");

            if (innerSize < 0)
                throw new ArgumentOutOfRangeException("innerSize");

            if (bottomSize < 0)
                throw new ArgumentOutOfRangeException("bottomSize");

            if (drill <= 0)
                throw new ArgumentOutOfRangeException("drill");

            this.topSize = topSize;
            this.innerSize = innerSize;
            this.bottomSize = bottomSize;
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
        private Point[] MakePoints(BoardSide side, int spacing) {

            int size =
                side == BoardSide.Top ? TopSize :
                side == BoardSide.Bottom ? BottomSize :
                InnerSize;
            int sizeM2 = size * 2;
            int sizeD2 = size / 2;

            int spacingM2 = spacing * 2;
            int spacingD2 = spacing / 2;

            switch (shape) {
                case ThPadShape.Square:
                    return PolygonBuilder.BuildRectangle(
                        Position,
                        new Size(size + spacingM2, size + spacingM2),
                        spacing,
                        Rotation);

                case ThPadShape.Octagon: {
                    int s = (int)((double)sizeD2 / Math.Cos(22.5 * Math.PI / 180.0));
                    return PolygonBuilder.BuildPolygon(
                        8,
                        Position,
                        s + spacing,
                        Rotation + Angle.FromDegrees(2250));
                }

                case ThPadShape.Oval:
                    return PolygonBuilder.BuildRectangle(
                        Position,
                        new Size(sizeM2 + spacingM2, size + spacingM2),
                        sizeD2 + spacing,
                        Rotation);

                default:
                    return PolygonBuilder.BuildCircle(
                        Position,
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

            Point[] points = MakePoints(side, 0);
            Point[] holePoints = PolygonBuilder.BuildCircle(Position, drill / 2);
            return new Polygon(points, new Polygon(holePoints));
        }

        /// <summary>
        /// Crea el poligon exterior del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            Point[] points = MakePoints(side, spacing);
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
            Polygon thermal = new Polygon(PolygonBuilder.BuildCross(Position, new Size(w, h), width, Rotation));

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

            int w = ((shape == ThPadShape.Oval ? 2 : 1) * topSize);
            int h = topSize;

            return new Rect(Position.X - (w / 2), Position.Y - (h / 2), w, h);
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
                if (topSize == 0) {
                    int ring = Math.Max(drcTopSizeMin, Math.Min(drcTopSizeMax, drill * drcTopSizePercent));
                    return drill + ring * 2;
                }
                else
                    return topSize;
            }
            set {
                topSize = value;
            }
        }
        /// <summary>
        /// Obte o asigna el tamany del pad de la cara inferior
        /// </summary>
        /// 
        public int BottomSize {
            get {
                if (bottomSize == 0) {
                    int ring = Math.Max(drcBottomSizeMin, Math.Min(drcBottomSizeMax, drill * drcBottomSizePercent));
                    return drill + ring * 2;
                }
                else
                    return bottomSize;
            }
            set {
                bottomSize = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany del pad de la capa interna.
        /// </summary>
        /// 
        public int InnerSize {
            get {
                if (innerSize == 0) {
                    int ring = Math.Max(drcTopSizeMin, Math.Min(drcTopSizeMax, drill * drcTopSizePercent));
                    return drill + ring * 2;
                }
                else
                    return innerSize;
            }
            set {
                innerSize = value;
            }
        }
    }
}
