namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;
    using System.Xml;
    using MikroPic.EdaTools.v1.Xml;

    public struct Size {

        private readonly int width;
        private readonly int height;

        public Size(int width, int height) {

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


    public static class SizeHelper {

        public static void WriteAttribute(this XmlWriterAdapter wr, string name, Size size) {

            string s = String.Format(
                "{0}, {1}",
                XmlConvert.ToString(size.Width / 1000000.0),
                XmlConvert.ToString(size.Height / 1000000.0));
            wr.WriteAttribute(name, s);
        }
    }
}
