namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Clase que representa un poligon amb fills. Aquesta clase es inmutable.
    /// </summary>
    public sealed class Polygon {

        private readonly Point[] points;
        private readonly Polygon[] childs;

        public Polygon(Point[] points, Polygon[] childs = null) {

            if ((points != null) && (points.Length < 3))
                throw new InvalidOperationException("Lista de puntos invalida.");

            this.points = points;
            this.childs = childs;
        }

        /// <summary>
        /// Clona el poligon. (Copia en profunditat.)
        /// </summary>
        /// <returns>El nou poligon.</returns>
        /// 
        public Polygon Clone() {

            Polygon[] clonedChilds;
            if (childs != null) {
                clonedChilds = new Polygon[childs.Length];
                for (int i = 0; i < childs.Length; i++)
                    clonedChilds[i] = childs[i].Clone();
            }
            else
                clonedChilds = null;

            return new Polygon(points, clonedChilds);
        }

        /// <summary>
        /// Clona la llista de punts
        /// </summary>
        /// <returns>La nova llista de punts.</returns>
        /// 
        public Point[] ClonePoints() {

            if (points == null)
                return null;
            else {
                Point[] a = new Point[points.Length];
                points.CopyTo(a, 0);
                return a;
            }
        }

        /// <summary>
        /// Aplica una matriu de transformacio al poligon.
        /// </summary>
        /// <param name="m">La matriu.</param>
        /// 
        public void Transform(Matrix m) {

            if (points != null)
                for (int i = 0; i < points.Length; i++) {
                    double x = points[i].X;
                    double y = points[i].Y;
                    double xadd = y * m.M21 + m.OffsetX;
                    double yadd = x * m.M12 + m.OffsetY;
                    x *= m.M11;
                    x += xadd;
                    y *= m.M22;
                    y += yadd;
                    points[i] = new Point(x, y);
                }

            if (childs != null)
                for (int i = 0; i < childs.Length; i++)
                    childs[i].Transform(m);
        }

        public Polygon Transformed(Matrix m) {

            Polygon polygon = Clone();
            polygon.Transform(m);
            return polygon;
        }

        /// <summary>
        /// Obte el bounding-box del poligon.
        /// </summary>
        /// 
        public Rect BoundingBox {
            get {
                if (points == null)
                    return Rect.Empty;

                else {
                    double minX = Double.MaxValue;
                    double minY = Double.MaxValue;
                    double maxX = Double.MinValue;
                    double maxY = Double.MinValue;

                    for (int i = 0; i < points.Length; i++) {

                        if (points[i].X < minX)
                            minX = points[i].X;
                        if (points[i].Y < minY)
                            minY = points[i].Y;

                        if (points[i].X > maxX)
                            maxX = points[i].X;
                        if (points[i].Y > maxY)
                            maxY = points[i].Y;
                    }

                    return new Rect(minX, minY, maxX - minX, maxY - minY);
                }
            }
        }

        /// <summary>
        /// Obte els punta del poligon.
        /// </summary>
        /// 
        public Point[] Points {
            get {
                return points;
            }
        }

        /// <summary>
        /// Obte els fills del poligon.
        /// </summary>
        /// 
        public Polygon[] Childs {
            get {
                return childs;
            }
        }
    }
}
