namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    public class LineElement: Element, IConectable {

        public enum LineCapStyle {
            Round,
            Flat
        }

        private Point startPosition;
        private Point endPosition;
        private LayerId layerId = LayerId.Unknown;
        private double thickness;
        private LineCapStyle lineCap = LineCapStyle.Round;

        /// <summary>
        /// Constructor per defecte de l'objecte.
        /// </summary>
        /// 
        public LineElement():
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="startPosition">La posicio inicial.</param>
        /// <param name="endPosition">La posicio final.</param>
        /// <param name="layerId">Identificador de la capa.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="lineCap">Forma dels extrems de linia.</param>
        /// 
        public LineElement(Point startPosition, Point endPosition, LayerId layerId, double thickness, LineCapStyle lineCap) :
            base() {

            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.layerId = layerId;
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
        /// Comprova si pertany a la capa especificada.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public override bool IsOnLayer(LayerId layerId) {

            return this.layerId == layerId;
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="inflate">Increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(double inflate = 0) {

            return PolygonBuilder.BuildLine(startPosition, endPosition, thickness + (inflate * 2));
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        protected override Rect GetBoundingBox() {

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
                if (startPosition != value) {
                    startPosition = value;
                    Invalidate();
                }
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
                if (endPosition != value) {
                    endPosition = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Obte o asigna el identificador de la capa.
        /// </summary>
        /// 
        public LayerId LayerId {
            get {
                return layerId;
            }
            set {
                layerId = value;
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

                if (thickness != value) {
                    thickness = value;
                    Invalidate();
                }
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
                if (lineCap != value) {
                    lineCap = value;
                    Invalidate();
                }
            }
        }
    }
}
