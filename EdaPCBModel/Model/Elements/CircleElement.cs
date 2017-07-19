namespace MikroPic.EdaTools.v1.Model.Elements {

    using System;
    using System.Windows;

    public sealed class CircleElement: ElementBase {

        private Point position;
        private double thickness;
        private double radius;

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public double Thickness {
            get {
                return thickness;
            }
            set {
                thickness = value;
            }
        }

        public double Radius {
            get {
                return radius;
            }
            set {
                radius = value;
            }
        }

        public double Diameter {
            get {
                return radius * 2;
            }
            set {
                radius = value / 2;
            }
        }

        public bool Filled {
            get {
                return thickness == 0;
            }
            set {
                if (value)
                    thickness = 0;
            }
        }
    }
}
