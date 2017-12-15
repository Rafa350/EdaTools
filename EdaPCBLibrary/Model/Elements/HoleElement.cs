namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class HoleElement: ElementBase {

        private double drill;

        public HoleElement() {

        }

        public HoleElement(Point position, double drill):
            base(position) {

            if (position == null)
                throw new ArgumentNullException("position");

            this.drill = drill;
        }

        public override bool IsOnLayer(Layer layer) {

            return layer.Id == LayerId.Holes;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
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
