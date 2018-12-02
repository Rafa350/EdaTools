namespace MikroPic.EdaTools.v1.Base.Geometry {

    public readonly struct Segment {

        private readonly Point start;
        private readonly Point end;

        public Segment(Point start, Point end) {

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
