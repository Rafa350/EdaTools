﻿namespace MikroPic.EdaTools.v1.Model.Elements {

    using System.Windows;

    public sealed class HoleElement: ElementBase {

        private Point position;
        private double drill;

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

        public double Drill {
            get {
                return drill;
            }
            set {
                drill = value;
            }
        }
    }
}
