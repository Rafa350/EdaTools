namespace MikroPic.EdaTools.v1.Base.Geometry {

    // ********************
    // TODO: Opcio per que no sigui inmutable
    // ********************

    public readonly struct Segment {

        private readonly Point start;
        private readonly Point end;

        public Segment(Point start, Point end) {

            this.start = start;
            this.end = end;
        }

        public Segment(Point start, int length, Angle angle) {

            this.start = start;
            this.end = start;
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
