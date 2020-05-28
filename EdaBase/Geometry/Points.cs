namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Colleccio inmutable de punts.
    /// </summary>
    /// 
    public sealed class Points: IEnumerable<Point> {

        private readonly List<Point> points;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="points">Punt a afeigir.</param>
        /// 
        public Points(IEnumerable<Point> points) {

            this.points = new List<Point>(points);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="points">Punt afeigir.</param>
        /// 
        public Points(params Point[] points) {

            this.points = new List<Point>(points);
        }

        /// <summary>
        /// Obte un enumerador.
        /// </summary>
        /// <returns>L'enumerador.</returns>
        /// 
        public IEnumerator<Point> GetEnumerator() => 
            points.GetEnumerator();

        /// <summary>
        /// Obte un enumerador.
        /// </summary>
        /// <returns>L'enumerador.</returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator() => 
            points.GetEnumerator();

        /// <summary>
        /// Obte el numero d'elements.
        /// </summary>
        /// 
        public int Count =>
            points.Count;

        /// <summary>
        /// Obte un punt pel seu index.
        /// </summary>
        /// <param name="index">El index.</param>
        /// <returns>El punt en el index especificat.</returns>
        /// 
        public Point this[int index] => 
            points[index];   
    }
}
