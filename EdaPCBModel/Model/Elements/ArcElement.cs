namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;

    public sealed class ArcElement: LineElement {

        private double angle;

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
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
                double co = Math.Sqrt(Math.Pow(EndPosition.X - StartPosition.X, 2) + Math.Pow(EndPosition.Y - StartPosition.Y, 2)) / 2;
                return Math.Abs(co / Math.Sin((angle / 2) * Math.PI / 180));
            }
        }
    }
}
