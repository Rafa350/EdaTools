namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System;

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
        /// Converteix l'objecte a text.
        /// </summary>
        /// <returns>El resultat de la converssio.</returns>
        /// 
        public override string ToString() {

            return String.Format("{0}, {1}", x, y);
        }

        /// <summary>
        /// Converteix un text a 'Point'
        /// </summary>
        /// <param name="source">El text a convertir.</param>
        /// <returns>El resultat de la converssio.</returns>
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
        public override int GetHashCode() {

            return 11 + (x * 7) + y;
        }

        /// <summary>
        /// Operacio de comparacio entrer objectes
        /// </summary>
        /// <param name="obj">L'altre objecte a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public override bool Equals(object obj) {

            if (obj is Point p) 
                return (x == p.x) && (y == p.y);
            else
                return false;
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
