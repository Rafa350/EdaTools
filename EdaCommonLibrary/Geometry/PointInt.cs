namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;

    /// <summary>
    /// Estructura que representa un punt. Aquesta estructura es inmutable.
    /// </summary>
    public struct PointInt {

        private readonly int x;
        private readonly int y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">Copordinada X</param>
        /// <param name="y">Coordinada Y</param>
        /// 
        public PointInt(int x, int y) {

            this.x = x;
            this.y = y;
        }

        public static PointInt Parse(string s) {

            return Parse(s, CultureInfo.CurrentCulture);
        }

        public static PointInt Parse(string s, IFormatProvider provider) {

            string[] ss = s.Split(',');
            int x = Int32.Parse(ss[0], provider);
            int y = Int32.Parse(ss[1], provider);

            return new PointInt(x, y);
        }

        public override string ToString() {

            return ToString(CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider provider) {

            return String.Format(provider, "{0}, {1}", x, y);
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
