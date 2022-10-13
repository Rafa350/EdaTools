using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Objecte que representa un poligon amb forats. Aquesta clase es inmutable.
    /// Els punt van ordenats en direccio contrari al rellotge.
    /// </summary>
    /// 
    public sealed class EdaPolygon {

        private List<EdaPoint> _contour;
        private List<List<EdaPoint>> _holes;
        private EdaRect? _bounds = null;

        /// <summary>
        /// Construeix un poligon.
        /// </summary>
        /// <param name="contour">El conjunt de punts del contorn.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> contour) {

            Initialize(contour, null);
        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="contour">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats interiors.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> contour, params IEnumerable<EdaPoint>[] holes) {

            Initialize(contour, holes);
        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="contour">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats interiors.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> contour, IEnumerable<IEnumerable<EdaPoint>> holes) {

            Initialize(contour, holes);
        }

        /// <summary>
        /// Construeix un poligon.
        /// </summary>
        /// <param name="contour">El conjunt de punts del contorn.</param>
        /// 
        public EdaPolygon(List<EdaPoint> contour) {

            Initialize(contour, null);
        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="contour">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats interiors.</param>
        /// 
        public EdaPolygon(List<EdaPoint> contour, params List<EdaPoint>[] holes) {

            Initialize(contour, holes);
        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="contour">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats interiors.</param>
        /// 
        public EdaPolygon(List<EdaPoint> contour, IEnumerable<List<EdaPoint>> holes) {

            Initialize(contour, holes);
        }

        /// <summary>
        /// Inicialitzacio.
        /// </summary>
        /// <param name="contour">Els punts del contorn.</param>
        /// <param name="holes">Els punts dels forats.</param>
        /// 
        private void Initialize(IEnumerable<EdaPoint> contour, IEnumerable<IEnumerable<EdaPoint>> holes) {

            _contour = new List<EdaPoint>(contour);

            if (holes != null) {
                _holes = new List<List<EdaPoint>>();
                foreach (var hole in holes)
                    _holes.Add(new List<EdaPoint>(hole));
            }
        }

        /// <summary>
        /// Inicialitzacio.
        /// </summary>
        /// <param name="contour">Els punts del contorn.</param>
        /// <param name="holes">Els punts dels forats.</param>
        /// 
        private void Initialize(List<EdaPoint> contour, IEnumerable<List<EdaPoint>> holes) {

            _contour = contour;

            if (holes != null) {
                _holes = new List<List<EdaPoint>>();
                foreach (var hole in holes)
                    _holes.Add(hole);
            }
        }

        /// <summary>
        /// Obte el rectangle envolvent de la llista de punts.
        /// </summary>
        /// <returns>El rectangle envolvent.</returns>
        /// 
        private EdaRect ComputeBounds() {

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
        public EdaRect Bounds {
            get {
                if (!_bounds.HasValue)
                    _bounds = ComputeBounds();
                return _bounds.Value;
            }
        }

        /// <summary>
        /// Els punts del poligon.
        /// </summary>
        /// 
        public IEnumerable<EdaPoint> Contour =>
            _contour;

        /// <summary>
        /// Els fills del poligon.
        /// </summary>
        /// 
        public IEnumerable<IEnumerable<EdaPoint>> Holes =>
            _holes;

        /// <summary>
        /// Retorna true si el poligon te forats.
        /// </summary>
        /// 
        public bool HasHoles =>
            _holes != null;
    }
}

