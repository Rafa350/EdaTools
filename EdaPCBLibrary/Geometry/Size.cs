namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    public struct Size {

        private Measure width;
        private Measure height;

        public Size(Measure width, Measure height) {

            this.width = width;
            this.height = height;
        }

        public Size(Size other) {

            width = other.width;
            height = other.height;
        }

        public Measure Width {
            get {
                return width;
            }
        }

        public Measure Height {
            get {
                return height;
            }
        }
    }
}
