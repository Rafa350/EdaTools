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

        public override string ToString() {

            return String.Format("{0}, {1}, {2}, {3}", a, r, g, b);
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
