namespace MikroPic.EdaTools.v1.Base.Xml {

    using System;
    using System.Xml;
    using MikroPic.EdaTools.v1.Base.Geometry;

    public static class XmlTypeParser {

        private const double numberMultiplier = 1000000.0;
        private const double angleMultiplier = 100.0;
        private const double ratioMultiplier = 1000.0;

        /// <summary>
        /// Converteix un text a 'Number'
        /// </summary>
        /// <param name="source">El text a convertir.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public static int ParseNumber(string source) {

            return (int)(XmlConvert.ToDouble(source) * numberMultiplier);
        }

        /// <summary>
        /// Converteix un text a 'Point'
        /// </summary>
        /// <param name="source">El text a convertir.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public static Point ParsePoint(string source) {

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            string[] ss = source.Split(',');
            double x = XmlConvert.ToDouble(ss[0]);
            double y = XmlConvert.ToDouble(ss[1]);

            return new Point((int)(x * numberMultiplier), (int)(y * numberMultiplier));
        }

        /// <summary>
        /// Converteix un text a 'Size'
        /// </summary>
        /// <param name="source">El text a convertir.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public static Size ParseSize(string source) {

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            string[] ss = source.Split(',');
            double w = XmlConvert.ToDouble(ss[0]);
            double h = XmlConvert.ToDouble(ss[1]);

            return new Size((int)(w * numberMultiplier), (int)(h * numberMultiplier));
        }

        /// <summary>
        /// Converteix un text a 'Angle'
        /// </summary>
        /// <param name="source">El text a convertir.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public static Angle ParseAngle(string source) {

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            return Angle.FromValue((int)(XmlConvert.ToDouble(source) * angleMultiplier));
        }

        /// <summary>
        /// Converteix un text a 'Ratio'
        /// </summary>
        /// <param name="source">El text a convertir.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public static Ratio ParseRatio(string source) {

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            return Ratio.FromValue((int)(XmlConvert.ToDouble(source) * ratioMultiplier));
        }
    }
}
