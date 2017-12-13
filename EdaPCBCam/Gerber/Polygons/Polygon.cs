namespace MikroPic.EdaTools.v1.Cam.Gerber.Polygons {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public sealed class Polygon {

        private List<Point> points = new List<Point>();

        /// <summary>
        /// Crea un poligon a partir d'un cercle.
        /// </summary>
        /// <param name="x">Coordinada X del centre.</param>
        /// <param name="y">Coordinada Y del centre.</param>
        /// <param name="diameter">Diametre del cercle.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon FromCircle(double x, double y, double diameter) {

            Polygon p = new Polygon();

            double radius = diameter / 2;
            double delta = 6 * Math.PI / 180;
            for (double a = 0; a < 2 * Math.PI; a += delta)
                p.AddPoint(x + radius * Math.Sin(a), y + radius * Math.Cos(a));

            return p;
        }

        /// <summary>
        /// Creas un poligon a partir d'un rectangle.
        /// </summary>
        /// <param name="x">Coordinada X del centre.</param>
        /// <param name="y">Coordinada Y del centre.</param>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Angle de rotacio.</param>
        /// <returns>Wl poligon.</returns>
        /// 
        public static Polygon FromRectangle(double x, double y, double width, double height, double rotate) {

            Matrix m = new Matrix();
            m.RotateAt(rotate, x, y);

            Polygon p = new Polygon();

            p.AddPoint(m.Transform(new Point(x, y)));
            p.AddPoint(m.Transform(new Point(x + width, y)));
            p.AddPoint(m.Transform(new Point(x + width, y + height)));
            p.AddPoint(m.Transform(new Point(x, y + height)));

            return p;
        }

        public void AddPoint(double x, double y) {

            points.Add(new Point(x, y));
        }

        public void AddPoint(Point point) {

            points.Add(point);
        }

        public int NumPoints {
            get {
                return points.Count;
            }
        }

        public IEnumerable<Point> Points {
            get {
                return points;
            }
        }
    }
}
