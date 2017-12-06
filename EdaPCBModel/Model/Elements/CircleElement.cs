namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class CircleElement: SingleLayerElement {

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
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Thickness");
                thickness = value;
            }
        }

        public double Radius {
            get {
                return radius;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Radius");
                radius = value;
            }
        }

        public double Diameter {
            get {
                return radius * 2;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Diameter");
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
