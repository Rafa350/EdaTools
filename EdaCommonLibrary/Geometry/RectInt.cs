namespace MikroPic.EdaTools.v1.Geometry {

    public struct RectInt {

        private readonly PointInt position;
        private readonly SizeInt size;

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

        public int MinX {
            get {
                return position.X;
            }
        }

        public int MinY {
            get {
                return position.Y;
            }
        }

        public int MaxX {
            get {
                return position.X + size.Width;
            }
        }

        public int MaxY {
            get {
                return position.Y + size.Height;
            }
        }
    }
}
