namespace MikroPic.EdaTools.v1.Xml {

    using MikroPic.EdaTools.v1.Geometry;
    using System;
    using System.Xml;

    public static class XmlTypeFormater {

        private const double numberDivider = 1000000.0;
        private const double angleDivider = 100.0;
        private const double ratioDivider = 1000.0;

        /// <summary>
        /// Formateja un valor numeric
        /// </summary>
        /// <param name="value">El valor a formatejar.</param>
        /// <returns>El text formatejar.</returns>
        /// 
        public static string FormatNumber(int value) {

            return XmlConvert.ToString(value / numberDivider);
        }

        /// <summary>
        /// Formateja un valor 'Point'
        /// </summary>
        /// <param name="point">El valor a formatejar.</param>
        /// <returns>El text formatejar.</returns>
        /// 
        public static string FormatPoint(in Point point) {

            return String.Format(
                "{0}, {1}",
                XmlConvert.ToString(point.X / numberDivider),
                XmlConvert.ToString(point.Y / numberDivider));
        }

        /// <summary>
        /// Formateja un valor 'Size'
        /// </summary>
        /// <param name="point">El valor a formatejar.</param>
        /// <returns>El text formatejar.</returns>
        /// 
        public static string FormatSize(in Size size) {

            return String.Format(
                "{0}, {1}",
                XmlConvert.ToString(size.Width / numberDivider),
                XmlConvert.ToString(size.Height / numberDivider));
        }

        /// <summary>
        /// Formateha un valor 'Angle'.
        /// </summary>
        /// <param name="angle">El valor a formatejar.</param>
        /// <returns>El text formatejar.</returns>
        /// 
        public static string FormatAngle(Angle value) {

            return XmlConvert.ToString(value.Degrees / angleDivider);
        }

        /// <summary>
        /// Formateja un valor 'Ratio'
        /// </summary>
        /// <param name="ratio">El valor a formatejar.</param>
        /// <returns>El text formatejar.</returns>
        /// 
        public static string FormatRatio(Ratio value) {

            return XmlConvert.ToString(value.Percent / ratioDivider);
        }

        /// <summary>
        /// Formateja un valor 'Color'
        /// </summary>
        /// <param name="color">El valor a formatejar.</param>
        /// <returns>El text formatejar.</returns>
        /// 
        public static string FormatColor(Color color) {

            return String.Format(
                "{0}, {1}, {2}, {3}",
                color.A,
                color.R,
                color.G,
                color.B);
        }
    }
}
