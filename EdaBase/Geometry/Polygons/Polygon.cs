using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons {

    /// <summary>
    /// Clase que representa un poligon amb fills. Aquesta clase es inmutable.
    /// </summary>
    /// 
    public sealed class Polygon {

        private readonly EdaPoints _points;
        private readonly Polygon[] _childs;
        private EdaRect? _bbox = null;

        /// <summary>
        /// Constructor del poligon.
        /// </summary>
        /// <param name="points">Llista de punts.</param>
        /// 
        public Polygon(EdaPoints points) {

            if ((points != null) && (points.Count < 3))
                throw new InvalidOperationException("La lista ha de contener un minimo de 3 puntos.");

            _points = points;
        }

        /// <summary>
        /// Constructor del poligon.
        /// </summary>
        /// <param name="points">Lista de punts.</param>
        /// <param name="childs">Llista de fills.</param>
        /// 
        public Polygon(EdaPoints points, params Polygon[] childs) {

            if ((points != null) && (points.Count < 3))
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
        public EdaPoints ClonePoints() {

            return _points == null ? null : EdaPoints.Create(_points);
        }

        public void Reverse() {

        }

        /// <summary>
        /// Aplica una transformacio al poligon
        /// </summary>
        /// <param name="transformation">La transformacio.</param>
        /// 
        private void Transform(Transformation transformation) {

            if (_points != null)
                for (int i = 0; i < _points.Count; i++)
                    _points[i] = transformation.ApplyTo(_points[i]);

            if (_childs != null)
                for (int i = 0; i < _childs.Length; i++)
                    _childs[i].Transform(transformation);
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
        private static int GetSignedArea(EdaPoints points) {

            if (points == null)
                return 0;

            else {
                int area = 0;
                int i = 0;
                while (i < points.Count - 1) {
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
        private static EdaRect GetBoundingBox(EdaPoints points) {

            if (points == null)
                return new EdaRect(0, 0, 0, 0);

            else {
                int minX = Int32.MaxValue;
                int minY = Int32.MaxValue;
                int maxX = Int32.MinValue;
                int maxY = Int32.MinValue;

                for (int i = 0; i < points.Count; i++) {

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

                return new EdaRect(minX, minY, maxX - minX, maxY - minY);
            }
        }

        public bool IsClockwise =>
            GetSignedArea(_points) < 0;

        /// <summary>
        /// Obte l'area del poligon
        /// </summary>
        /// 
        public int Area =>
            Math.Abs(GetSignedArea(_points));

        /// <summary>
        /// Obte el bounding-box del poligon.
        /// </summary>
        /// 
        public EdaRect BoundingBox {
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
        public int NumPoints =>
            _points == null ? 0 : _points.Count;

        /// <summary>
        /// Obte el numero de fills.
        /// </summary>
        /// 
        public int NumChilds =>
            _childs == null ? 0 : _childs.Length;

        /// <summary>
        /// Obte els punts del poligon.
        /// </summary>
        /// 
        public IEnumerable<EdaPoint> Points =>
            _points;

        /// <summary>
        /// Obte els fills del poligon.
        /// </summary>
        /// 
        public IEnumerable<Polygon> Childs =>
            _childs;
    }
}
