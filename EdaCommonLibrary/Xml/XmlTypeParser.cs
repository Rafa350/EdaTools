namespace MikroPic.EdaTools.v1.Xml {

    using MikroPic.EdaTools.v1.Geometry;
    using System;
    using System.Xml;

    public static class XmlTypeParser {

        private const double numberMultiplier = 1000000.0;
        private const double angleMultiplier = 100.0;
        private const double ratioMultiplier = 1000.0;

        public static int ParseNumber(string source) {

            return (int)(XmlConvert.ToDouble(source) * numberMultiplier);
        }

        public static Point ParsePoint(string source) {

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");              

            string[] ss = source.Split(',');
            double x = XmlConvert.ToDouble(ss[0]);
            double y = XmlConvert.ToDouble(ss[1]);

            return new Point((int)(x * numberMultiplier), (int)(y * numberMultiplier));
        }

        public static Size ParseSize(string source) {

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            string[] ss = source.Split(',');
            double w = XmlConvert.ToDouble(ss[0]);
            double h = XmlConvert.ToDouble(ss[1]);

            return new Size((int)(w * numberMultiplier), (int)(h * numberMultiplier));
        }

        public static Angle ParseAngle(string source) {

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            return Angle.FromDegrees((int)(XmlConvert.ToDouble(source) * angleMultiplier));
        }

        public static Ratio ParseRatio(string source) {

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            return Ratio.FromPercent((int)(XmlConvert.ToDouble(source) * ratioMultiplier));
        }
    }
}
