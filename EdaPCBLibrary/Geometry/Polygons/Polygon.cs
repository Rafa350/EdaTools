namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public sealed class Polygon {

        private readonly List<Point> points;

        /// <summary>
        /// Constructor. Crea un poligon buit.
        /// </summary>
        /// 
        public Polygon() {

            points = new List<Point>();
        }

        /// <summary>
        /// Constructor. Crea un poligon a partir d'una coleccio de punts.
        /// </summary>
        /// <param name="points">Coleccio de punts.</param>
        /// 
        public Polygon(List<Point> points) {

            if (points == null)
                throw new ArgumentNullException("points");

            this.points = points;
        }

        /// <summary>
        /// Constructor. Crea un poligon a partir d'una coleccio de punts.
        /// </summary>
        /// <param name="points">Coleccio de punts.</param>
        /// 
        public Polygon(IEnumerable<Point> points) {

            if (points == null)
                throw new ArgumentNullException("points");

            this.points = new List<Point>(points);
        }

        public void Clear() {

            points.Clear();
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        public void Add(Point point) {

            if (point == null)
                throw new ArgumentNullException("point");

            points.Add(point);
        }

        /// <summary>
        /// Afegeix ua serie de punts al poligon.
        /// </summary>
        /// <param name="points">Els punta a afeigir.</param>
        /// 
        public void Add(IEnumerable<Point> points) {

            if (points == null)
                throw new ArgumentNullException("points");

            this.points.AddRange(points);
        }

        /*
        public Polygon Offset(double delta) {

            ClipperOffset co = new ClipperOffset();
            co.AddPath(points, JoinType.jtRound, EndType.etClosedPolygon);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            co.Execute(ref solution, delta * 10000);

            return new Polygon(solution[0]);
        }

        public IList<Polygon> Clip(IEnumerable<Polygon> polygons, ClipType clipType) {

            Clipper cp = new Clipper();

            cp.AddPath(points, PolyType.ptSubject, true);
            foreach (Polygon polygon in polygons)
                cp.AddPath(polygon.points, PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(clipType, solution);

            List<Polygon> result = new List<Polygon>();
            foreach (var poly in solution)
                result.Add(new Polygon(poly));
            return result;
        }

        public IList<Polygon> Clip(Polygon polygon, ClipType clipType) {

            Clipper cp = new Clipper();

            cp.AddPath(points, PolyType.ptSubject, true);
            cp.AddPath(polygon.points, PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(clipType, solution);

            List<Polygon> result = new List<Polygon>();
            foreach (var poly in solution)
                result.Add(new Polygon(poly));
            return result;
        }
        */

        /// <summary>
        /// Tanca el poligon.
        /// </summary>
        /// 
        public void Close() {

            if (!IsClosed)
                points.Add(points[0]);
        }

        /// <summary>
        /// Obte un indicador del poligon tancat.
        /// </summary>
        /// 
        public bool IsClosed {
            get {
                return 
                    (points.Count >= 3) &&
                    (points[0] == points[points.Count - 1]);
            }
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
