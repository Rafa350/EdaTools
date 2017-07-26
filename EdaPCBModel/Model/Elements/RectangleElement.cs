﻿namespace MikroPic.EdaTools.v1.Model.Elements {

    using System.Windows;

    public sealed class RectangleElement: ElementBase {

        private Point position;
        private Size size;
        private double rotate;
        private double thickness;

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

        public Size Size {
            get {
                return size;
            }
            set {
                size = value;
            }
        }

        public double Rotate {
            get {
                return rotate;
            }
            set {
                rotate = value;
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
