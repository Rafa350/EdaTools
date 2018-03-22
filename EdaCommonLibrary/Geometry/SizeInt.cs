namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;

    public struct SizeInt {

        private readonly int width;
        private readonly int height;

        public SizeInt(int width, int height) {

            this.width = width;
            this.height = height;
        }

        public override string ToString() {

            return String.Format(CultureInfo.CurrentCulture, "{0}; {1}", width, height);
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
