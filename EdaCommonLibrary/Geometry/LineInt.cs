namespace MikroPic.EdaTools.v1.Geometry {

    public struct LineInt {

        private PointInt start;
        private PointInt end;

        public LineInt(PointInt start, PointInt end) {

            this.start = start;
            this.end = end;
        }

        public PointInt Start {
            get {
                return start;
            }
        }

        public PointInt End {
            get {
                return end;
            }
        }
    }
}
