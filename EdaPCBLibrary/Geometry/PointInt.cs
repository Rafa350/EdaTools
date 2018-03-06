namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    public struct PointInt {

        private int x;
        private int y;

        public PointInt(int x, int y) {

            this.x = x;
            this.y = y;
        }

        public int X {
            get {
                return x;
            }
//            set {
                //x = value;
           // }
        }

        public int Y {
            get {
                return y;
            }
            //set {
            //    y = value;
           // }
        }
    }
}
