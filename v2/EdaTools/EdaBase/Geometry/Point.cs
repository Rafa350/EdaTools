namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System;

    /// <summary>
    /// Estructura que representa un punt.
    /// </summary>
    /// 
    public readonly struct Point : IEquatable<Point> {

        private readonly int x;
        private readonly int y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">Coordinada X.</param>
        /// <param name="y">Coordinada Y.</param>
        /// 
        public Point(int x = 0, int y = 0) => 
            (this.x, this.y) = (x, y);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="other">El punt a copiar.</param>
        /// 
        public Point(Point other) => 
            (x, y) = (other.x, other.y);

        /// <summary>
        /// Obte un punt desplaçat.
        /// </summary>
        /// <param name="dx">Desplaçament X.</param>
        /// <param name="dy">Desplaçament Y.</param>
        /// <returns>El nou punt resultant.</returns>
        /// 
        public Point Offset(int dx, int dy) =>
            new Point(x + dx, y + dy);

        /// <summary>
        /// Converteix l'objecte a text.
        /// </summary>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public override string ToString() =>
            String.Format("{0}, {1}", x, y);

        /// <summary>
        /// Converteix un text a 'Point'
        /// </summary>
        /// <param name="source">El text a convertir.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public static Point Parse(string source) {

            try {
                string[] s = source.Split(',');
                int x = Int32.Parse(s[0]);
                int y = Int32.Parse(s[1]);

                return new Point(x, y);
            }
            catch (Exception ex) {
                throw new InvalidOperationException(
                    String.Format("No es posible convertir el texto '{0}' a 'Point'.", source), ex);
            }
        }

        /// <summary>
        /// Retorna el valor hash del objecte.
        /// </summary>
        /// <returns>El valor hash.</returns>
        /// 
        public override int GetHashCode() =>
            (x * 371) + (y * 27);

        /// <summary>
        /// Operacio de comparacio entre dos objectes..
        /// </summary>
        /// <param name="other">L'altre objecte a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public bool Equals(Point other) => 
            (x, y) == (other.x, other.y);

        /// <summary>
        /// Operacio de comparacio entrer objectes
        /// </summary>
        /// <param name="obj">L'altre objecte a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public override bool Equals(object obj) {

            if (obj is Point other)
                return Equals(other);
            else
                return false;
        }

        /// <summary>
        /// Operador ==
        /// </summary>
        /// <param name="p1">Primer element a comparar.</param>
        /// <param name="p2">Segon element a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public static bool operator ==(Point p1, Point p2) {

            return p1.Equals(p2);
        }

        /// <summary>
        /// Operador !=
        /// </summary>
        /// <param name="p1">Primer element a comparar.</param>
        /// <param name="p2">Segon element a comparar.</param>
        /// <returns>True si son diferenmts.</returns>
        /// 
        public static bool operator !=(Point p1, Point p2) {

            return !p1.Equals(p2);
        }

        /// <summary>
        /// Obte el valor de la coordinada X
        /// </summary>
        /// 
        public int X => x;

        /// <summary>
        /// Obte el valor de la coordinada Y
        /// </summary>
        /// 
        public int Y => y;
    }
}
