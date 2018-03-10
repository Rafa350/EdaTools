namespace MikroPic.EdaTools.v1.Pcb.Geometry {

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

        public override string ToString() {

            return String.Format(CultureInfo.CurrentCulture, "X:{0}; Y:{1}", x, y);
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
