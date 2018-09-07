namespace MikroPic.EdaTools.v1.Geometry {

    using MikroPic.EdaTools.v1.Xml;
    using System;
    using System.Globalization;
    using System.Xml;

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
        /// Procesa una cadena per crear un objecte 'Point'
        /// </summary>
        /// <param name="s">La cadena a procesar.</param>
        /// <param name="provider">Objecte proveidor de format.</param>
        /// <returns>L'objecte 'Point'</returns>
        /// 
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

    /// <summary>
    /// Clase que implementa metodes d'ectensio
    /// </summary>
    public static class PointHelper {

        /// <summary>
        /// Escriu un atribut de tipus 'Point'
        /// </summary>
        /// <param name="wr">El objecte XmlWriterAdapter.</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="point">El valor a escriure.</param>
        /// 
        public static void WriteAttribute(this XmlWriterAdapter wr, string name, Point point) {

            string s = String.Format(
                "{0}, {1}",
                XmlConvert.ToString(point.X / 1000000.0),
                XmlConvert.ToString(point.Y / 1000000.0));
            wr.WriteAttribute(name, s);
        }

        public static Point AttributeAsPoint(this XmlReaderAdapter rd, string name) {

            string s = rd.AttributeAsString(name);

            string[] ss = s.Split(',');
            double x = XmlConvert.ToDouble(ss[0]);
            double y = XmlConvert.ToDouble(ss[1]);

            return new Point((int)(x * 1000000.0), (int)(y * 1000000.0));
        }
    }
}
