namespace MikroPic.EdaTools.v1.Geometry {

    public struct Line {

        private Point start;
        private Point end;

        public Line(Point start, Point end) {

            this.start = start;
            this.end = end;
        }

        public Point Start {
            get {
                return start;
            }
        }

        public Point End {
            get {
                return end;
            }
        }
    }
}
