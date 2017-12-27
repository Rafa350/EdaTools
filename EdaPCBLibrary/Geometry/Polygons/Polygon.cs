namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;

    public sealed class Polygon: IEnumerable<Point> {

        private readonly List<Point> points = new List<Point>();
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

            InternalAdd(points);
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        public void Add(Point point) {

            if (point == null)
                throw new ArgumentNullException("point");

            InternalAdd(point);
        }

        /// <summary>
        /// Afegeix ua serie de punts al poligon.
        /// </summary>
        /// <param name="points">Els punta a afeigir.</param>
        /// 
        public void Add(IEnumerable<Point> points) {

            if (points == null)
                throw new ArgumentNullException("points");

            InternalAdd(points);
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        private void InternalAdd(Point point) {

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
        private void InternalAdd(IEnumerable<Point> points) {

            foreach (Point point in points)
                InternalAdd(point);
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

        IEnumerator IEnumerable.GetEnumerator() {

            return points.GetEnumerator();
        }

        public IEnumerator<Point> GetEnumerator() {

            return points.GetEnumerator();
        }
    }
}
