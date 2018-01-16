namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Clase que representa un poligon amb fills.
    /// </summary>
    public sealed class Polygon {

        private List<Point> points;
        private List<Polygon> childs;

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

            InternalAddPoints(points);
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        public void AddPoint(Point point) {

            if (point == null)
                throw new ArgumentNullException("point");

            InternalAddPoint(point);
        }

        /// <summary>
        /// Afegeix una serie de punts al poligon.
        /// </summary>
        /// <param name="points">Els punta a afeigir.</param>
        /// 
        public void AddPoints(IEnumerable<Point> points) {

            if (points == null)
                throw new ArgumentNullException("points");

            InternalAddPoints(points);
        }

        /// <summary>
        /// Afegeig un poligon fill.
        /// </summary>
        /// <param name="child">El fill a afeigir.</param>
        /// 
        public void AddChild(Polygon child) {

            if (child == null)
                throw new ArgumentNullException("hole");

            InternalAddChild(child);
        }

        /// <summary>
        /// Clona el poligon. (Copia en profunditat.)
        /// </summary>
        /// <returns>El nou poligon.</returns>
        /// 
        public Polygon Clone() {

            Polygon polygon = new Polygon(points);
            if (childs != null)
                foreach (Polygon child in childs)
                    polygon.AddChild(child.Clone());

            return polygon;
        }

        /// <summary>
        /// Aplica una matriu de transformacio al poligon.
        /// </summary>
        /// <param name="m">La matriu.</param>
        /// 
        public void Transform(Matrix m) {

            if (points != null)
                for (int i = 0; i < points.Count; i++)
                    points[i] = m.Transform(points[i]);

            if (childs != null)
                foreach (Polygon child in childs)
                    child.Transform(m);
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        private void InternalAddPoint(Point point) {

            if (points == null)
                points = new List<Point>();

            points.Add(point);
        }

        /// <summary>
        /// Afegeix una serie de punts al poligon.
        /// </summary>
        /// <param name="points">Els punts a afeigir.</param>
        /// 
        private void InternalAddPoints(IEnumerable<Point> points) {

            if (this.points == null)
                this.points = new List<Point>(points);
            else
                this.points.AddRange(points);
        }

        /// <summary>
        /// Afegeig un poligon fill.
        /// </summary>
        /// <param name="child">El fill a afeigir.</param>
        /// 
        private void InternalAddChild(Polygon child) {

            if (childs == null)
                childs = new List<Polygon>();

            childs.Add(child);
        }

        /// <summary>
        /// Afegeix una serie de poligons fills.
        /// </summary>
        /// <param name="childs">Els fills a afeigir.</param>
        /// 
        private void InternalAddChilds(IEnumerable<Polygon> childs) {

            if (this.childs == null)
                this.childs = new List<Polygon>(childs);
            else
                this.childs.AddRange(childs);
        }

        /// <summary>
        /// Obte el bounding-box del poligon.
        /// </summary>
        /// 
        public Rect BoundingBox {
            get {
                double minX = Double.MaxValue;
                double minY = Double.MaxValue;
                double maxX = Double.MinValue;
                double maxY = Double.MinValue;

                foreach (Point point in points) {

                    if (point.X < minX)
                        minX = point.X;
                    if (point.Y < minY)
                        minY = point.Y;

                    if (point.X > maxX)
                        maxX = point.X;
                    if (point.Y > maxY)
                        maxY = point.Y;
                }

                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
        }

        public bool IsValid {
            get {
                return PointCount == 0 || PointCount >= 3;
            }
        }

        public bool HasPoints {
            get {
                return points != null;
            }
        }

        /// <summary>
        /// Obte el numero de puns en el poligon
        /// </summary>
        /// 
        public int PointCount {
            get {
                return points == null ? 0 :  points.Count;
            }
        }

        public IEnumerable<Point> Points {
            get {
                return points;
            }
        }

        /// <summary>
        /// Indica si el poligon te fills.
        /// </summary>
        /// 
        public bool HasChilds {
            get {
                return childs != null;
            }
        }

        public int ChildCount {
            get {
                return (childs == null) ? 0 : childs.Count;
            }
        }

        /// <summary>
        /// Enumera la llista de fills.
        /// </summary>
        /// 
        public IEnumerable<Polygon> Childs {
            get {
                return childs;
            }
        }
    }
}
