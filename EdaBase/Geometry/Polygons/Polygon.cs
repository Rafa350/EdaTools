namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using System;

    /// <summary>
    /// Clase que representa un poligon amb fills. Aquesta clase es inmutable.
    /// </summary>
    /// 
    public sealed class Polygon {

        private readonly Point[] points;
        private readonly Polygon[] childs;

        /// <summary>
        /// Constructor del poligon.
        /// </summary>
        /// <param name="points">Llista de punts.</param>
        /// 
        public Polygon(params Point[] points) {

            if ((points != null) && (points.Length < 3))
                throw new InvalidOperationException("La lista ha de contener un minimo de 3 puntos.");

            this.points = points;
        }

        /// <summary>
        /// Constructor del poligon.
        /// </summary>
        /// <param name="points">Plista de punts.</param>
        /// <param name="childs">Llista de fills.</param>
        /// 
        public Polygon(Point[] points, params Polygon[] childs) {

            if ((points != null) && (points.Length < 3))
                throw new InvalidOperationException("La lista ha de contener un minimo de 3 puntos.");

            this.points = points;
            this.childs = childs;
        }

        /// <summary>
        /// Clona el poligon. (Copia en profunditat.)
        /// </summary>
        /// <returns>El nou poligon.</returns>
        /// 
        public Polygon Clone() {

            // Clona els fills
            //
            Polygon[] clonedChilds = null;
            if (childs != null) {
                clonedChilds = new Polygon[childs.Length];
                for (int i = 0; i < childs.Length; i++)
                    clonedChilds[i] = childs[i].Clone();
            }

            return new Polygon(ClonePoints(), clonedChilds);
        }

        /// <summary>
        /// Clona la llista de punts
        /// </summary>
        /// <returns>La nova llista de punts.</returns>
        /// 
        public Point[] ClonePoints() {

            Point[] clonedPoints = null;
            if (points != null) {
                clonedPoints = new Point[points.Length];
                points.CopyTo(clonedPoints, 0);
            }
            return clonedPoints;
        }

        /// <summary>
        /// Aplica una transformacio al poligon
        /// </summary>
        /// <param name="transformation">La transformacio.</param>
        /// 
        private void Transform(Transformation transformation) {

            if (points != null)
                transformation.ApplyTo(points);

            if (childs != null)
                for (int i = 0; i < childs.Length; i++)
                    childs[i].Transform(transformation);
        }

        public void Reverse() {

            Array.Reverse(points);
        }

        /// <summary>
        /// Retorna el poligon resultant d'aplicar una transformacio. El
        /// poligon actual no es modifica.
        /// </summary>
        /// <param name="transformation">La transfoprmacio</param>
        /// <returns>El poligon resultant</returns>
        /// 
        public Polygon Transformed(Transformation transformation) {

            Polygon polygon = Clone();
            polygon.Transform(transformation);
            return polygon;
        }

        /// <summary>
        /// Obte l'area del poligon amb signe.
        /// </summary>
        /// <param name="points">Llista de punts.</param>
        /// <returns>L'area del poligon.</returns>
        /// 
        private static int GetSignedArea(Point[] points) {

            if (points == null)
                return 0;

            else {
                int area = 0;
                int i = 0;
                while (i < points.Length - 1) {
                    area +=
                        (points[i + 1].X - points[i].X) *
                        (points[i + 1].Y + points[i].Y);
                    i++;
                }
                area +=
                    (points[0].X - points[i].X) *
                    (points[0].Y + points[i].Y);

                return area / 2;
            }
        }

        /// <summary>
        /// Obte el rectangle envolvent de la llista de punts.
        /// </summary>
        /// <param name="points">La llista de punts.</param>
        /// <returns>El rectangle envolvent.</returns>
        /// 
        private static Rect GetBoundingBox(Point[] points) {

            if (points == null)
                return new Rect(0, 0, 0, 0);

            else {
                int minX = Int32.MaxValue;
                int minY = Int32.MaxValue;
                int maxX = Int32.MinValue;
                int maxY = Int32.MinValue;

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

        public bool IsClockwise {
            get {
                return GetSignedArea(points) < 0;
            }
        }

        /// <summary>
        /// Obte l'area del poligon
        /// </summary>
        /// 
        public int Area {
            get {
                return Math.Abs(GetSignedArea(points));
            }
        }

        /// <summary>
        /// Obte el bounding-box del poligon.
        /// </summary>
        /// 
        public Rect BoundingBox {
            get {
                return GetBoundingBox(points);
            }
        }

        /// <summary>
        /// Obte els punts del poligon.
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
