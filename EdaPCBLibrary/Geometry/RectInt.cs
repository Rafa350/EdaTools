namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    public struct RectInt {

        private PointInt position;
        private SizeInt size;

        public RectInt(int x, int y, int width, int height) {

            position = new PointInt(x, y);
            size = new SizeInt(width, height);
        }

        public RectInt(PointInt position, SizeInt size) {

            this.position = position;
            this.size = size;
        }

        public PointInt Position {
            get {
                return position;
            }
        }

        public SizeInt Size {
            get {
                return size;
            }
        }
    }
}
