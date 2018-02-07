namespace MikroPic.EdaTools.v1.Pcb.Geometry.Fonts {

    public struct GlyphTrace {

        private readonly double x;
        private readonly double y;
        private readonly double width;
        private readonly bool stroke;

        public GlyphTrace(double x, double y, double width, bool stroke) {

            this.x = x;
            this.y = y;
            this.width = width;
            this.stroke = stroke;
        }

        public double X {
            get {
                return x;
            }
        }

        public double Y {
            get {
                return y;
            }
        }

        public double Width {
            get {
                return width;
            }
        }

        public bool Stroke {
            get {
                return stroke;
            }
        }
    }
}
