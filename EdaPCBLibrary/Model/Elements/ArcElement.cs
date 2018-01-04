namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;

    public sealed class ArcElement: LineElement {

        private double angle;

        /// <summary>
        /// Constructor per defecte de l'objecte.
        /// </summary>
        /// 
        public ArcElement():
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="startPosition">Punt inicial.</param>
        /// <param name="endPosition">Punt final.</param>
        /// <param name="layer">Capa.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="angle">Angle del arc.</param>
        /// <param name="lineCap">Extrems de linia.</param>
        /// 
        public ArcElement(Point startPosition, Point endPosition, Layer layer, double thickness, double angle, LineCapStyle lineCap) :
            base(startPosition, endPosition, layer, thickness, lineCap) {

            this.angle = angle;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="transform">Transformacio a aplicar.</param>
        /// <param name="inflate">Increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(Matrix transform, double inflate) {

            return PolygonBuilder.Build(this, null, inflate);
        }

        /// <summary>
        /// Obte el centre de l'arc.
        /// </summary>
        /// 
        public Point Center {
            get {
                double x1 = StartPosition.X;
                double y1 = EndPosition.Y;
                double x2 = EndPosition.X;
                double y2 = EndPosition.Y;

                // Calcula el punt central
                //
                double mx = (x1 + x2) / 2.0;
                double my = (y1 + y2) / 2.0;

                // Calcula la distancia entre els dos punts.
                //
                double d = Math.Sqrt(Math.Pow(x2 - x1, 2.0) + Math.Pow(y2 - y1, 2.0));

                // Calcula el radi
                //
                double r = Math.Abs((d / 2.0) / Math.Sin((angle / 2.0) * Math.PI / 180.0));

                // Calcula el centre
                //
                if (angle > 0)
                    return new Point(
                        mx + Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (y1 - y2) / d,
                        my + Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (x2 - x1) / d);

                else
                    return new Point(
                        mx - Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (y1 - y2) / d,
                        my - Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (x2 - x1) / d);
            }
        }

        /// <summary>
        /// Obte o asigna l'angle del arc.
        /// </summary>
        /// 
        public double Angle {
            get {
                return angle;
            }
            set {
                if (Math.Abs(value) >= 360.0)
                    value = value % 360.0;

                if (angle != value) {
                    angle = value;
                    Invalidate();
                }
            }
        }

        public double StartAngle {
            get {
                Point center = Center;
                return Math.Atan((StartPosition.Y - center.Y) / (StartPosition.X - center.X));
            }
        }

        public double EndAngle {
            get {
                Point center = Center;
                return Math.Atan((EndPosition.Y - center.Y) / (EndPosition.X - center.X));
            }
        }

        /// <summary>
        /// Obte el radi de l'arc.
        /// </summary
        /// >
        public double Radius {
            get {
                // La semi-distancia entre els dos punts es un catet
                //
                double sd = Math.Sqrt(Math.Pow(EndPosition.X - StartPosition.X, 2.0) + Math.Pow(EndPosition.Y - StartPosition.Y, 2.0)) / 2.0;

                // La hipotenusa es el radi. Aplicant trigonometria...
                //
                return Math.Abs(sd / Math.Sin((angle / 2.0) * Math.PI / 180.0));
            }
        }
    }
}
