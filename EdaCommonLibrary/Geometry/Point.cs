namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;

    /// <summary>
    /// Estructura que representa un punt
    /// </summary>
    /// 
    public readonly struct Point {

        private readonly int x;
        private readonly int y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">Coordinada X</param>
        /// <param name="y">Coordinada Y</param>
        /// 
        public Point(int x = 0, int y = 0) {

            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Procesa una cadena per crear un objecte 'Point'
        /// </summary>
        /// <param name="s">La cadena a procesar.</param>
        /// <returns>L'objecte 'Point'</returns>
        /// 
        public static Point Parse(string s) {

            return Parse(s, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Procesa un text per crear un objecte 'Point'
        /// </summary>
        /// <param name="s">El text a procesar.</param>
        /// <param name="provider">Objecte proveidor de format.</param>
        /// <returns>L'objecte 'Point'</returns>
        /// 
        public static Point Parse(string s, IFormatProvider provider) {

            try {
                string[] ss = s.Split(',');
                int x = Int32.Parse(ss[0], provider);
                int y = Int32.Parse(ss[1], provider);

                return new Point(x, y);
            }
            catch (Exception ex) {
                throw new InvalidOperationException(
                    String.Format("No se pudo convertir el texto '{0}' a 'Point'.", s), ex);
            }
        }

        /// <summary>
        /// Obte un punt desplaçat.
        /// </summary>
        /// <param name="dx">Desplaçament X</param>
        /// <param name="dy">Desplaçament Y</param>
        /// <returns>El nou punt resultant.</returns>
        /// 
        public Point Offset(int dx, int dy) {

            return new Point(x + dx, y + dy);
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>El resultat de la converssio.</returns>
        /// 
        public override string ToString() {

            return ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converteix a string.
        /// </summary>
        /// <param name="provider">L'objecte proveidor de format.</param>
        /// <returns>El resultat de la converssio.</returns>
        /// 
        public string ToString(IFormatProvider provider) {

            return String.Format(provider, "{0}, {1}", x, y);
        }

        /// <summary>
        /// Retorna el valor hash del objecte.
        /// </summary>
        /// <returns>El valor hash.</returns>
        /// 
        public override int GetHashCode() {

            return (x << 7) ^ y;
        }

        /// <summary>
        /// Operacio de comparacio entrer objectes
        /// </summary>
        /// <param name="obj">L'altre objecte a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public override bool Equals(object obj) {

            if ((obj == null) || !GetType().Equals(obj.GetType())) {
                return false;
            }
            else {
                Point p = (Point)obj;
                return (x == p.x) && (y == p.y);
            }
        }

        /// <summary>
        /// Obte el valor de la coordinada X
        /// </summary>
        /// 
        public int X {
            get {
                return x;
            }
        }

        /// <summary>
        /// Obte el valor de la coordinada Y
        /// </summary>
        /// 
        public int Y {
            get {
                return y;
            }
        }
    }
}
