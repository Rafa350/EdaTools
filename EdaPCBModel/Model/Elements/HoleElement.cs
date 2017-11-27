namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
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
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Drill");
                drill = value;
            }
        }
    }
}
