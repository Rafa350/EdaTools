namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;

    public readonly struct Size {

        private readonly int width;
        private readonly int height;

        public Size(int width = 0, int height = 0) {

            this.width = width;
            this.height = height;
        }

        public override string ToString() {

            return ToString(CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider provider) {

            return String.Format(provider, "{0}; {1}", width, height);
        }

        public int Width {
            get {
                return width;
            }
        }

        public int Height {
            get {
                return height;
            }
        }
    }
}