namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;

    /// <summary>
    /// Clase que representa un color ARGB
    /// </summary>
    /// 
    public readonly struct Color {

        private readonly byte a;
        private readonly byte r;
        private readonly byte g;
        private readonly byte b;

        public Color(byte r, byte g, byte b) {

            this.a = 255;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color(byte a, byte r, byte g, byte b) {

            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color(Color other) {

            a = other.a;
            r = other.r;
            g = other.g;
            b = other.b;
        }

        public static Color Parse(string s) {

            return Parse(s, CultureInfo.CurrentCulture);
        }

        public static Color Parse(string s, IFormatProvider provider) {

            string[] ss = s.Split(',');
            byte a = Byte.Parse(ss[0], provider);
            byte r = Byte.Parse(ss[1], provider);
            byte g = Byte.Parse(ss[2], provider);
            byte b = Byte.Parse(ss[3], provider);

            return new Color(a, r, g, b);
        }

        public override string ToString() {

            return ToString(CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider provider) {

            return String.Format(provider, "{0}, {1}, {2}, {3}", a, r, g, b);
        }

        /// <summary>
        /// Obte el valor de la component A
        /// </summary>
        /// 
        public byte A {
            get {
                return a;
            }
        }

        /// <summary>
        /// Obte el valor de la component R.
        /// </summary>
        /// 
        public byte R {
            get {
                return r;
            }
        }

        /// <summary>
        /// Obte el valor de la component G.
        /// </summary>
        /// 
        public byte G {
            get {
                return g;
            }
        }

        /// <summary>
        /// Obte el valor de la component B.
        /// </summary>
        /// 
        public byte B {
            get {
                return b;
            }
        }
    }
}
