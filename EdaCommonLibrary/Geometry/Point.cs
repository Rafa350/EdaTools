namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;
    using System.Xml;
    using MikroPic.EdaTools.v1.Xml;

    /// <summary>
    /// Estructura que representa un punt. Aquesta estructura es inmutable.
    /// </summary>
    /// 
    public struct Point {

        private readonly int x;
        private readonly int y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">Copordinada X</param>
        /// <param name="y">Coordinada Y</param>
        /// 
        public Point(int x, int y) {

            this.x = x;
            this.y = y;
        }

        public static Point Parse(string s) {

            return Parse(s, CultureInfo.CurrentCulture);
        }

        public static Point Parse(string s, IFormatProvider provider) {

            string[] ss = s.Split(',');
            int x = Int32.Parse(ss[0], provider);
            int y = Int32.Parse(ss[1], provider);

            return new Point(x, y);
        }

        /// <summary>
        /// Obte un punt desplaçat.
        /// </summary>
        /// <param name="offsetX">Desplaçament X</param>
        /// <param name="offsetY">Desplaçament Y</param>
        /// <returns>El nou punt resultant.</returns>
        /// 
        public Point Offset(int offsetX, int offsetY) {

            return new Point(x + offsetX, y + offsetY);
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


    public static class PointHelper {

        public static void WriteAttribute(this XmlWriterAdapter wr, string name, Point point) {

            string s = String.Format(
                "{0}, {1}",
                XmlConvert.ToString(point.X / 1000000.0),
                XmlConvert.ToString(point.Y / 1000000.0));
            wr.WriteAttribute(name, s);
        }
    }
}
