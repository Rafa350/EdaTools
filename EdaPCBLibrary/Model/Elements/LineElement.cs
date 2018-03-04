namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa una linia.
    /// </summary>
    public class LineElement : Element, IConectable {

        public enum LineCapStyle {
            Round,
            Flat
        }

        private Point startPosition;
        private Point endPosition;
        private double thickness;
        private LineCapStyle lineCap = LineCapStyle.Round;

        /// <summary>
        /// Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public LineElement() :
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="startPosition">La posicio inicial.</param>
        /// <param name="endPosition">La posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="lineCap">Forma dels extrems de linia.</param>
        /// 
        public LineElement(Point startPosition, Point endPosition, double thickness, LineCapStyle lineCap) :
            base() {

            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.thickness = thickness;
            this.lineCap = lineCap;
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

            Point[] points = PolygonBuilder.BuildTrace(startPosition, endPosition, thickness);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, double spacing) {

            Point[] points = PolygonBuilder.BuildTrace(startPosition, endPosition, thickness + (spacing * 2));
            return new Polygon(points);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Obte o asigna la posicio inicial.
        /// </summary>
        /// 
        public Point StartPosition {
            get {
                return startPosition;
            }
            set {
                startPosition = value;
            }
        }

        /// <summary>
        /// Obte o asigna la posicio final.
        /// </summary>
        /// 
        public Point EndPosition {
            get {
                return endPosition;
            }
            set {
                    endPosition = value;
            }
        }

        /// <summary>
        /// Obte la longitut de la linia al quadrat.
        /// </summary>
        /// 
        public double SqrLength {
            get {
                double dx = endPosition.X - startPosition.X;
                double dy = endPosition.Y - startPosition.Y;
                return (dx * dx) + (dy * dy);
            }
        }

        /// <summary>
        /// Obte la longitut de la linia.
        /// </summary>
        /// 
        public double Length {
            get {
                return Math.Sqrt(SqrLength);
            }
        }

        /// <summary>
        ///  Obte o asigna l'amplada de linia.
        /// </summary>
        /// 
        public double Thickness {
            get {
                return thickness;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Thickness");

                    thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tipus d'extrem de linia.
        /// </summary>
        /// 
        public LineCapStyle LineCap {
            get {
                return lineCap;
            }
            set {
                lineCap = value;
            }
        }
    }
}

