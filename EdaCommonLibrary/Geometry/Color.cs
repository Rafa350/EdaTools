namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;

    public struct Color {

        private byte a;
        private byte r;
        private byte g;
        private byte b;

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

        public byte A {
            get {
                return a;
            }
        }

        public byte R {
            get {
                return r;
            }
        }

        public byte G {
            get {
                return g;
            }
        }

        public byte B {
            get {
                return b;
            }
        }
    }
}
