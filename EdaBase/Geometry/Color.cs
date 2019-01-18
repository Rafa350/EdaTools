namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System;

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

        public override string ToString() {

            return String.Format("{0}, {1}, {2}, {3}", a, r, g, b);
        }

        public static Color Parse(string source) {

            try {
                string[] s = source.Split(',');
                byte a = Byte.Parse(s[0]);
                byte r = Byte.Parse(s[1]);
                byte g = Byte.Parse(s[2]);
                byte b = Byte.Parse(s[3]);
                return new Color(a, r, g, b);
            }
            catch (Exception ex) {
                throw new InvalidOperationException(String.Format("No es posible convertir el texto '{0}' a 'Color'.", source), ex);
            }
        }

        public override int GetHashCode() {

            return (a << 24) | (r << 16) | (g << 8) + b;
        }

        public override bool Equals(object obj) {

            if (obj is Color c)
                return (c.a == a) && (c.r == r) && (c.g == g) && (c.b == b);
            else
                return false;
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
