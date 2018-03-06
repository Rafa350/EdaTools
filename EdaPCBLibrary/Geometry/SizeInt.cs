namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    public struct SizeInt {

        private int width;
        private int height;

        public SizeInt(int width, int height) {

            this.width = width;
            this.height = height;
        }

        public int Width {
            get {
                return width;
            }
            set {
                width = value;
            }
        }

        public int Height {
            get {
                return height;
            }
            set {
                height = value;
            }
        }
    }
}
