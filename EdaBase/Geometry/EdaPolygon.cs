using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Objecte que representa un poligon amb forats. Aquesta clase es inmutable.
    /// Els punt van ordenats en direccio contrari al rellotge.
    /// </summary>
    /// 
    public sealed class EdaPolygon {

        private readonly EdaPoints _contour;
        private readonly List<EdaPoints> _holes;
        private EdaRect? _bbox = null;

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="contour">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats interiors.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> contour, params IEnumerable<EdaPoint>[] holes) :
            this(contour, (IEnumerable<EdaPoints>) holes){
        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="contour">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats interiors.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> contour, IEnumerable<IEnumerable<EdaPoint>> holes) {

            _contour = new EdaPoints(contour);

            if (holes != null) {
                _holes = new List<EdaPoints>();
                foreach (var hole in holes)
                    _holes.Add(new EdaPoints(hole));
            }
        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="contour">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats interiors.</param>
        /// 
        public EdaPolygon(EdaPoints contour, params EdaPoints[] holes):
            this(contour, (IEnumerable<EdaPoints>) holes) {

        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="contour">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats interiors.</param>
        /// 
        public EdaPolygon(EdaPoints contour, IEnumerable<EdaPoints> holes) {

            _contour = contour;

            if (holes != null) {
                _holes = new List<EdaPoints>();
                foreach (var hole in holes)
                    _holes.Add(hole);
            }
        }

        /// <summary>
        /// Obte el rectangle envolvent de la llista de punts.
        /// </summary>
        /// <returns>El rectangle envolvent.</returns>
        /// 
        private EdaRect GetBoundingBox() {

            if (_contour == null)
                return new EdaRect(0, 0, 0, 0);

            else {
                int minX = int.MaxValue;
                int minY = int.MaxValue;
                int maxX = int.MinValue;
                int maxY = int.MinValue;

                foreach (var point in _contour) {

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
        /// El bounding-box del poligon.
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
        /// Els punts del poligon.
        /// </summary>
        /// 
        public EdaPoints Contour =>
            _contour;

        /// <summary>
        /// Els fills del poligon.
        /// </summary>
        /// 
        public IEnumerable<EdaPoints> Holes =>
            _holes;
    }
}

