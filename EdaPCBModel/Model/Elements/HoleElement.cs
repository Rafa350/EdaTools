namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class HoleElement: ElementBase {

        private Point position;
        private double drill;

        public HoleElement() {

        }

        public HoleElement(Point position, double drill) {

            if (position == null)
                throw new ArgumentNullException("position");

            this.position = position;
            this.drill = drill;
        }

        public override bool InLayer(Layer layer) {

            return layer.Id == LayerId.Holes;
        }

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
