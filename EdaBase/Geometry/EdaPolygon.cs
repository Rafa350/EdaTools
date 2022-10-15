using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Objecte que representa un poligon amb forats. Aquesta clase es inmutable.
    /// Els punt van ordenats en direccio contraria al rellotge. El poligon sempre
    /// te un contorm i opcionalment una llista de forats.
    /// Hi ha l'opcio de construir el poligon amb llistes de punts externes. En aquest
    /// cas els punt poden ser modificats, menipulant les llistes.
    /// </summary>
    /// 
    public sealed class EdaPolygon {

        private List<EdaPoint> _outline;
        private List<List<EdaPoint>> _holes;
        private EdaRect? _bounds = null;

        /// <summary>
        /// Construeix un poligon.
        /// </summary>
        /// <param name="outline">El conjunt de punts del contorn.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> outline) {

            if (outline == null)
                throw new ArgumentNullException(nameof(outline));

            Initialize(outline, null);
        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="outline">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats interiors.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> outline, params IEnumerable<EdaPoint>[] holes) {

            if (outline == null)
                throw new ArgumentNullException(nameof(outline));

            Initialize(outline, holes);
        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="outline">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats interiors.</param>
        /// 
        public EdaPolygon(IEnumerable<EdaPoint> outline, IEnumerable<IEnumerable<EdaPoint>> holes) {

            if (outline == null)
                throw new ArgumentNullException(nameof(outline));

            Initialize(outline, holes);
        }

        /// <summary>
        /// Construeix un poligon. 
        /// </summary>
        /// <param name="outline">La llista de punts del contorn.</param>
        /// <remarks>Les llistes son externes al poligon.</remarks>
        /// 
        public EdaPolygon(List<EdaPoint> outline) {

            if (outline == null)
                throw new ArgumentNullException(nameof(outline));

            if (outline.Count < 3)
                throw new ArgumentOutOfRangeException(nameof(outline));

            Initialize(outline, null);
        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="outline">La llista de punts del contorn.</param>
        /// <param name="holes">La llista de forats interiors.</param>
        /// <remarks>Les llistes son externes al poligon.</remarks>
        /// 
        public EdaPolygon(List<EdaPoint> outline, params List<EdaPoint>[] holes) {

            if (outline == null)
                throw new ArgumentNullException(nameof(outline));

            if (outline.Count < 3)
                throw new ArgumentOutOfRangeException(nameof(outline));

            Initialize(outline, holes);
        }

        /// <summary>
        /// Construeix un poligon, amb forats.
        /// </summary>
        /// <param name="outline">La llista de punts del contorn.</param>
        /// <param name="holes">La llista de forats interiors.</param>
        /// <remarks>Les llistes son externes al poligon.</remarks>
        /// 
        public EdaPolygon(List<EdaPoint> outline, IEnumerable<List<EdaPoint>> holes) {

            if (outline == null)
                throw new ArgumentNullException(nameof(outline));

            if (outline.Count < 3)
                throw new ArgumentOutOfRangeException(nameof(outline));

            Initialize(outline, holes);
        }

        /// <summary>
        /// Inicialitzacio.
        /// </summary>
        /// <param name="outline">El conjunt de punts del contorn.</param>
        /// <param name="holes">El conjunt de forats.</param>
        /// 
        private void Initialize(IEnumerable<EdaPoint> outline, IEnumerable<IEnumerable<EdaPoint>> holes) {

            _outline = new List<EdaPoint>(outline);

            if (holes != null) {
                _holes = new List<List<EdaPoint>>();
                foreach (var hole in holes)
                    _holes.Add(new List<EdaPoint>(hole));
            }
        }

        /// <summary>
        /// Inicialitzacio.
        /// </summary>
        /// <param name="outline">La llista punts del contorn.</param>
        /// <param name="holes">La llista punts dels forats.</param>
        /// <remarks>Les llistes son externes al poligon.</remarks>
        /// 
        private void Initialize(List<EdaPoint> outline, IEnumerable<List<EdaPoint>> holes) {

            _outline = outline;

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

            if ((_outline == null) || (_outline.Count < 3))
                return new EdaRect(0, 0, 0, 0);

            else {
                int minX = int.MaxValue;
                int minY = int.MaxValue;
                int maxX = int.MinValue;
                int maxY = int.MinValue;

                foreach (var point in _outline) {

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
        public IEnumerable<EdaPoint> Outline =>
            _outline;

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

