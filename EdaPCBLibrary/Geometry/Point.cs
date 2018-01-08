namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    public struct Point {

        private Measure x;
        private Measure y;

        public Point(Measure x, Measure y) {

            this.x = x;
            this.y = y;
        }

        public Point(Point other) {

            x = other.x;
            y = other.y;
        }

        public override int GetHashCode() {

            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override bool Equals(object obj) {

            if ((obj != null) && (obj is Point))
                return ((Point)obj).x == x && ((Point)obj).y == y;
            else
                return false;
        }

        public override string ToString() {

            return "";
        }

        public Measure X {
            get {
                return x;
            }
        }

        public Measure Y {
            get {
                return y;
            }
        }
    }
}
