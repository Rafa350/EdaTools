namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class ArcElement: LineElement {

        private double angle;

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public ArcElement():
            base() {

        }

        public ArcElement(Point position, Layer layer, Point endPosition, double thickness, double angle, LineCapStyle lineCap) :
            base(position, layer, endPosition, thickness, lineCap) {

            this.angle = angle;
        }

        public Point Center {
            get {
                double x1 = Position.X;
                double y1 = Position.Y;
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

        public double Angle {
            get {
                return angle;
            }
            set {
                if (Math.Abs(value) >= 360.0)
                    value = value % 360.0;
                //if (value < 0)
                //    value = 360 - value;
                angle = value;
            }
        }

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
