namespace MikroPic.EdaTools.v1.Panel.Model.Elements {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Panel.Model;

    public sealed class MillingElement: PanelElement {

        private Point startPosition;
        private Point endPosition;
        private int thickness;

        public MillingElement(Point startPosition, Point endPosition, int thickness) {

            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.thickness = thickness;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public Point StartPosition {
            get {
                return startPosition;
            }
            set {
                startPosition = value;
            }
        }

        public Point EndPosition {
            get {
                return endPosition;
            }
            set {
                endPosition = value;
            }
        }

        public int Tickness {
            get {
                return thickness;
            }
            set {
                thickness = value;
            }
        }
    }
}
