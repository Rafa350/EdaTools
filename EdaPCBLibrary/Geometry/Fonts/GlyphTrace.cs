namespace MikroPic.EdaTools.v1.Pcb.Geometry.Fonts {

    /// <summary>
    /// Representa els traços que formen la figura del caracter
    /// </summary>
    public struct GlyphTrace {

        private readonly double x;
        private readonly double y;
        private readonly bool stroke;

        public GlyphTrace(double x, double y, bool stroke) {

            this.x = x;
            this.y = y;
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

        public bool Stroke {
            get {
                return stroke;
            }
        }
    }
}
