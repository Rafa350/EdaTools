namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons {

    using System;
    using System.Collections.Generic;

    using MikroPic.EdaTools.v1.Base.Geometry;

    /// <summary>
    /// Clase que representa un poligon amb fills. Aquesta clase es inmutable.
    /// </summary>
    /// 
    public sealed class Polygon {

        private readonly EdaPoint[] _points;
        private readonly Polygon[] _childs;
        private Rect? _bbox = null;

        /// <summary>
        /// Constructor del poligon.
        /// </summary>
        /// <param name="points">Llista de punts.</param>
        /// 
        public Polygon(params EdaPoint[] points) {

            if ((points != null) && (points.Length < 3))
                throw new InvalidOperationException("La lista ha de contener un minimo de 3 puntos.");

            _points = points;
        }

        /// <summary>
        /// Constructor del poligon.
        /// </summary>
        /// <param name="points">Lista de punts.</param>
        /// <param name="childs">Llista de fills.</param>
        /// 
        public Polygon(EdaPoint[] points, params Polygon[] childs) {

            if ((points != null) && (points.Length < 3))
                throw new InvalidOperationException("La lista ha de contener un minimo de 3 puntos.");

            _points = points;
            _childs = childs;
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
            if (_childs != null) {
                clonedChilds = new Polygon[_childs.Length];
                for (int i = 0; i < _childs.Length; i++)
                    clonedChilds[i] = _childs[i].Clone();
            }

            return new Polygon(ClonePoints(), clonedChilds);
        }

        /// <summary>
        /// Clona la llista de punts
        /// </summary>
        /// <returns>La nova llista de punts.</returns>
        /// 
        public EdaPoint[] ClonePoints() {

            EdaPoint[] clonedPoints = null;
            if (_points != null) {
                clonedPoints = new EdaPoint[_points.Length];
                _points.CopyTo(clonedPoints, 0);
            }
            return clonedPoints;
        }

        /// <summary>
        /// Aplica una transformacio al poligon
        /// </summary>
        /// <param name="transformation">La transformacio.</param>
        /// 
        private void Transform(Transformation transformation) {

            if (_points != null)
                transformation.ApplyTo(_points);

            if (_childs != null)
                for (int i = 0; i < _childs.Length; i++)
                    _childs[i].Transform(transformation);
        }

        public void Reverse() {

            Array.Reverse(_points);
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
        private static int GetSignedArea(EdaPoint[] points) {

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
        private static Rect GetBoundingBox(EdaPoint[] points) {

            if (points == null)
                return new Rect(0, 0, 0, 0);

            else {
                int minX = Int32.MaxValue;
                int minY = Int32.MaxValue;
                int maxX = Int32.MinValue;
                int maxY = Int32.MinValue;

                for (int i = 0; i < points.Length; i++) {

                    int x = points[i].X;
                    int y = points[i].Y;

                    if (x < minX)
                        minX = x;
                    if (y < minY)
                        minY = y;

                    if (x > maxX)
                        maxX = x;
                    if (y > maxY)
                        maxY = y;
                }

                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
        }

        public bool IsClockwise => GetSignedArea(_points) < 0;

        /// <summary>
        /// Obte l'area del poligon
        /// </summary>
        /// 
        public int Area => Math.Abs(GetSignedArea(_points));

        /// <summary>
        /// Obte el bounding-box del poligon.
        /// </summary>
        /// 
        public Rect BoundingBox {
            get {
                if (!_bbox.HasValue)
                    _bbox = GetBoundingBox(_points);
                return _bbox.Value;
            }
        }

        /// <summary>
        /// Obte el numero de punts.
        /// </summary>
        /// 
        public int NumPoints => _points == null ? 0 : _points.Length;

        /// <summary>
        /// Obte el numero de fills.
        /// </summary>
        /// 
        public int NumChilds => _childs == null ? 0 : _childs.Length;

        /// <summary>
        /// Obte els punts del poligon.
        /// </summary>
        /// 
        public IEnumerable<EdaPoint> Points => _points;

        /// <summary>
        /// Obte els fills del poligon.
        /// </summary>
        /// 
        public IEnumerable<Polygon> Childs => _childs;
    }
}
