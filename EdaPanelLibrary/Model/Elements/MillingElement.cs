namespace MikroPic.EdaTools.v1.Panel.Model.Elements {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Panel.Model;

    public sealed class MillingElement: PanelElement {

        private Point startPosition;
        private Point endPosition;
        private int thickness;
        private int margin;
        private int cuts;

        public MillingElement(Point startPosition, Point endPosition, int thickness, int margin, int cuts) {

            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.thickness = thickness;
            this.margin = margin;
            this.cuts = cuts;
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

        public int Margin {
            get {
                return margin;
            }
            set {
                margin = value;
            }
        }

        public int Cuts {
            get {
                return cuts;
            }
            set {
                cuts = value;
            }
        }
    }
}
