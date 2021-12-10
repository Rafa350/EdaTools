using System.Collections;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Colleccio inmutable de punts.
    /// </summary>
    /// 
    public sealed class Points: IEnumerable<Point> {

        private readonly List<Point> _points;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="points">Punt a afeigir.</param>
        /// 
        public Points(IEnumerable<Point> points) {

            _points = new List<Point>(points);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="points">Punt afeigir.</param>
        /// 
        public Points(params Point[] points) {

            _points = new List<Point>(points);
        }

        /// <summary>
        /// Obte un enumerador.
        /// </summary>
        /// <returns>L'enumerador.</returns>
        /// 
        public IEnumerator<Point> GetEnumerator() => 
            _points.GetEnumerator();

        /// <summary>
        /// Obte un enumerador.
        /// </summary>
        /// <returns>L'enumerador.</returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator() => 
            _points.GetEnumerator();

        /// <summary>
        /// Obte el numero d'elements.
        /// </summary>
        /// 
        public int Count =>
            _points.Count;

        /// <summary>
        /// Obte un punt pel seu index.
        /// </summary>
        /// <param name="index">El index.</param>
        /// <returns>El punt en el index especificat.</returns>
        /// 
        public Point this[int index] => 
            _points[index];   
    }
}
