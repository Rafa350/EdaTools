namespace MikroPic.EdaTools.v1.Pcb.Geometry.Fonts {

    public struct GlyphPoint {

        private int x;
        private int y;

        public GlyphPoint(int x, int y) {

            this.x = x;
            this.y = y;
        }

        public int X {
            get {
                return x;
            }
        }

        public int Y {
            get {
                return y;
            }
        }
    }
}
