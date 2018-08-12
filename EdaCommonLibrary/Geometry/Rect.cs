namespace MikroPic.EdaTools.v1.Geometry {

    public struct Rect {

        private readonly Point position;
        private readonly Size size;

        public Rect(int x, int y, int width, int height) {

            position = new Point(x, y);
            size = new Size(width, height);
        }

        public Rect(Point position, Size size) {

            this.position = position;
            this.size = size;
        }

        public Point Position {
            get {
                return position;
            }
        }

        public Size Size {
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
