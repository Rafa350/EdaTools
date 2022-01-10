using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons {

    /// <summary>
    /// Objecte que representa un poligon amb fills. Aquesta clase es inmutable.
    /// </summary>
    /// 
    public sealed class EdaPolygon {

        private readonly List<EdaPoint> _points;
        private readonly List<EdaPolygon> _childs;
        private EdaRect? _bbox = null;

        /// <summary>
        /// Construei un poligon simple.
        /// </summary>
        /// <param name="points">Els punts.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> points) {

            if (points != null)
                _points = new List<EdaPoint>(points);
        }

        /// <summary>
        /// Constreix un poligon amb un poligon (forat) interior.
        /// </summary>
        /// <param name="points">Els punts.</param>
        /// <param name="child">El poligons interior.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> points, EdaPolygon child) {

            if (points != null)
                _points = new List<EdaPoint>(points);

            if (child != null) {
                _childs = new List<EdaPolygon>(1);
                _childs.Add(child);
            }
        }

        /// <summary>
        /// Construeix un poligon, amb mes d'un poligon (forat) interior.
        /// </summary>
        /// <param name="points">Els punts.</param>
        /// <param name="childs">Els poligons interiors.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> points, IEnumerable<EdaPolygon> childs) {

            if (points != null)
                _points = new List<EdaPoint>(points);

            if (childs != null)
                _childs = new List<EdaPolygon>(childs);
        }


        /// <summary>
        /// Obte l'area del poligon amb signe.
        /// </summary>
        /// <returns>L'area del poligon.</returns>
        /// 
        /*private int GetSignedArea() {

            if (_points == null)
                return 0;

            else {
                int area = 0;
                int i = 0;
                while (i < _points.Count - 1) {
                    area +=
                        (_points[i + 1].X - _points[i].X) *
                        (_points[i + 1].Y + _points[i].Y);
                    i++;
                }
                area +=
                    (_points[0].X - _points[i].X) *
                    (_points[0].Y + _points[i].Y);

                return area / 2;
            }
        }*/

        /// <summary>
        /// Obte el rectangle envolvent de la llista de punts.
        /// </summary>
        /// <returns>El rectangle envolvent.</returns>
        /// 
        private EdaRect GetBoundingBox() {

            if (_points == null)
                return new EdaRect(0, 0, 0, 0);

            else {
                int minX = Int32.MaxValue;
                int minY = Int32.MaxValue;
                int maxX = Int32.MinValue;
                int maxY = Int32.MinValue;

                foreach (var point in _points) {

                    int x = point.X;
                    int y = point.Y;

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

        /// <summary>
        /// Obte el bounding-box del poligon.
        /// </summary>
        /// 
        public EdaRect BoundingBox {
            get {
                if (!_bbox.HasValue)
                    _bbox = GetBoundingBox();
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
            _childs == null ? 0 : _childs.Count;

        /// <summary>
        /// Indica si el poligon te punts.
        /// </summary>
        /// 
        public bool HasPoints =>
            NumPoints > 0;

        /// <summary>
        /// Indica si el poligon te fills
        /// </summary>
        /// 
        public bool HasChilds =>
            NumChilds > 0;

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
        public IEnumerable<EdaPolygon> Childs =>
            _childs;
    }
}
