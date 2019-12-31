namespace MikroPic.EdaTools.v1.Base.Geometry.Utils {

    public sealed class Matrix {

        private readonly double[,] m = new double[3,3];

        public static readonly Matrix Identity = new Matrix();

        public Matrix() {

            Initialize();
        }


        public void Translate(double x, double y) {

        }

        public void Scale(int sx, int sy) {

        }

        public void ScaleAt(int sx, int sy, int cx, int cy) {

        }

        public void Rotate(double a) {

        }

        public void RotateAt(double a, int cx, int cy) {

        }

        public Point Transform(Point point) {

            return point;
        }

        public Point[] Transform(Point[] points) {

            return points;
        }

        private void Initialize() {

            m[0, 0] = 1;
            m[0, 1] = 0;
            m[0, 2] = 0;

            m[1, 0] = 0;
            m[1, 1] = 1;
            m[1, 2] = 0;
            
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = 1;
        }
    }
}
