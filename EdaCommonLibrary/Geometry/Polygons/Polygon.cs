namespace MikroPic.EdaTools.v1.Geometry.Polygons {

    using System;

    /// <summary>
    /// Clase que representa un poligon amb fills. Aquesta clase es inmutable.
    /// </summary>
    public sealed class Polygon {

        private readonly PointInt[] points;
        private readonly Polygon[] childs;

        public Polygon(PointInt[] points, Polygon[] childs = null) {

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

            return new Polygon(ClonePoints(), clonedChilds);
        }

        /// <summary>
        /// Clona la llista de punts
        /// </summary>
        /// <returns>La nova llista de punts.</returns>
        /// 
        public PointInt[] ClonePoints() {

            if (points == null)
                return null;
            else {
                PointInt[] clonedPoints = new PointInt[points.Length];
                points.CopyTo(clonedPoints, 0);
                return clonedPoints;
            }
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
        /// Obte el bounding-box del poligon.
        /// </summary>
        /// 
        public RectInt BoundingBox {
            get {
                if (points == null)
                    return new RectInt(0, 0, 0, 0);

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

                    return new RectInt(minX, minY, maxX - minX, maxY - minY);
                }
            }
        }

        /// <summary>
        /// Obte els punts del poligon.
        /// </summary>
        /// 
        public PointInt[] Points {
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
