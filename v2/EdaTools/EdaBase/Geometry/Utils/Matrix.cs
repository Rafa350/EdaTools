namespace MikroPic.EdaTools.v1.Base.Geometry.Utils {

    public sealed class Matrix {

        public static readonly Matrix Identity = new Matrix();

        public void Translate(int x, int y) {

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
    }
}
