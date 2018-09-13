namespace MikroPic.EdaTools.v1.Geometry {

    using MikroPic.EdaTools.v1.Xml;
    using System;
    using System.Globalization;
    using System.Xml;

    /// <summary>
    /// Estructura que representa un punt
    /// </summary>
    /// 
    public struct Point {

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
    /// Clase que implementa metodes d'extenssio
    /// </summary>
    public static class PointHelper {

        /// <summary>
        /// Escriu un atribut de tipus 'Point'
        /// </summary>
        /// <param name="wr">L'objecte XmlWriterAdapter.</param>
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

        /// <summary>
        /// Llegeix un atribut i el converteix a 'Point'
        /// </summary>
        /// <param name="rd">L'objecte XmlReaderAdapter.</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <returns>El valor lleigit.</returns>
        /// 
        public static Point AttributeAsPoint(this XmlReaderAdapter rd, string name) {

            string s = rd.AttributeAsString(name);

            string[] ss = s.Split(',');
            double x = XmlConvert.ToDouble(ss[0]);
            double y = XmlConvert.ToDouble(ss[1]);

            return new Point((int)(x * 1000000.0), (int)(y * 1000000.0));
        }
    }
}
