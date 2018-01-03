namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;

    public sealed class Polygon: IEnumerable<Point> {

        private readonly List<Point> points = new List<Point>();
        private List<Polygon> holes;
        private double minX = Double.MaxValue;
        private double minY = Double.MaxValue;
        private double maxX = Double.MinValue;
        private double maxY = Double.MinValue;

        /// <summary>
        /// Constructor. Crea un poligon buit.
        /// </summary>
        /// 
        public Polygon() {
        }

        /// <summary>
        /// Constructor. Crea un poligon a partir d'una coleccio de punts.
        /// </summary>
        /// <param name="points">Coleccio de punts.</param>
        /// 
        public Polygon(IEnumerable<Point> points) {

            if (points == null)
                throw new ArgumentNullException("points");

            InternalAddPoint(points);
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        public void Add(Point point) {

            if (point == null)
                throw new ArgumentNullException("point");

            InternalAddPoint(point);
        }

        /// <summary>
        /// Afegeix ua serie de punts al poligon.
        /// </summary>
        /// <param name="points">Els punta a afeigir.</param>
        /// 
        public void Add(IEnumerable<Point> points) {

            if (points == null)
                throw new ArgumentNullException("points");

            InternalAddPoint(points);
        }

        public void AddHole(Polygon polygon) {

            if (polygon == null)
                throw new ArgumentNullException("hole");

            InternalAddHole(polygon);
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        private void InternalAddPoint(Point point) {

            if (point.X < minX)
                minX = point.X;
            if (point.Y < minY)
                minY = point.Y;

            if (point.X > maxX)
                maxX = point.X;
            if (point.Y > maxY)
                maxY = point.Y;

            points.Add(point);
        }

        /// <summary>
        /// Afegeix una serie de punts al poligon.
        /// </summary>
        /// <param name="points">Els punts a afeigir.</param>
        /// 
        private void InternalAddPoint(IEnumerable<Point> points) {

            foreach (Point point in points)
                InternalAddPoint(point);
        }

        /// <summary>
        /// Afegeig un forat al poligon.
        /// </summary>
        /// <param name="hole">El forat a afeigir.</param>
        /// 
        private void InternalAddHole(Polygon hole) {

            if (holes == null)
                holes = new List<Polygon>();

            holes.Add(hole);
        }

        /// <summary>
        /// Afegeix una serie de forats al poligon.
        /// </summary>
        /// <param name="holes">Els forats a afeigir.</param>
        /// 
        private void InternalAddHole(IEnumerable<Polygon> holes) {

            foreach (Polygon hole in holes)
                InternalAddHole(hole);
        }

        /// <summary>
        /// Obte el bounding-box del poligon.
        /// </summary>
        /// 
        public Rect BoundingBox {
            get {
                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
        }

        /// <summary>
        /// Obte el numero de puns en el poligon
        /// </summary>
        /// 
        public int Count {
            get {
                return points.Count;
            }
        }

        public bool HasHoles {
            get {
                return holes != null;
            }
        }

        public IEnumerable<Polygon> Holes {
            get {
                return holes;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {

            return points.GetEnumerator();
        }

        public IEnumerator<Point> GetEnumerator() {

            return points.GetEnumerator();
        }
    }
}
