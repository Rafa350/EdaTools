namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class HoleElement: Element, IPosition {

        private Point position;
        private double drill;

        public HoleElement() {

        }

        public HoleElement(Point position, double drill) { 

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

        /// <summary>
        ///  Obte o asigna la posicio del centre del cercle.
        /// </summary>
        /// 
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
