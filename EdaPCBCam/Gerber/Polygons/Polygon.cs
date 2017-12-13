namespace MikroPic.EdaTools.v1.Cam.Gerber.Polygons {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using ClipperLib;

    public sealed class Polygon {

        private List<IntPoint> points = new List<IntPoint>();

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

        public Polygon() {

        }

        public Polygon(IEnumerable<Point> points) {

            foreach (var point in points)
                AddPoint(point);
        }

        /// <summary>
        /// Constructor intern.
        /// </summary>
        /// <param name="points">Enumeracio amb els punt a afeigir.</param>
        /// 
        private Polygon(IEnumerable<IntPoint> points) {

            this.points.AddRange(points);
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="x">Coordinada X del punt.</param>
        /// <param name="y">Coordinada Y del punt.</param>
        /// 
        public void AddPoint(double x, double y) {

            points.Add(new IntPoint(x * 10000, y * 10000));
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        public void AddPoint(Point point) {

            points.Add(new IntPoint(point.X * 10000, point.Y * 10000));
        }

        public IEnumerable<Polygon> Clip(Polygon clipPolygon) {

            Clipper cp = new Clipper();
            cp.AddPath(points, PolyType.ptSubject, true);
            cp.AddPath(clipPolygon.points, PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(ClipType.ctDifference, solution);

            List<Polygon> polygons = new List<Polygon>();
            foreach (var poly in solution)
                polygons.Add(new Polygon(poly));
            return polygons;
        }

        public int NumPoints {
            get {
                return points.Count;
            }
        }

        public IEnumerable<Point> Points {
            get {
                List<Point> p = new List<Point>(points.Count);
                foreach (var point in points)
                    p.Add(new Point(point.X / 10000, point.Y / 10000));
                return p;
            }
        }
    }
}
