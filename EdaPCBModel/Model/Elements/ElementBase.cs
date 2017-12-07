namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System.Windows;

    public abstract class ElementBase: IVisitable {

        private Point position;

        public ElementBase () {

        }

        public ElementBase(Point position) {

            this.position = position;
        }

        public abstract void AcceptVisitor(IVisitor visitor);

        public abstract bool InLayer(Layer layer);

        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }
    }
}
