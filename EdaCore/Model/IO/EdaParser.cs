using System;
using System.Globalization;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Core.Model.IO {

    public static class EdaParser {

        private static readonly CultureInfo _ci = CultureInfo.InvariantCulture;

        public static int ParseScalar(string source) =>
            (int)(Double.Parse(source, _ci) * 1000000.0);

        public static EdaAngle ParseAngle(string source) =>
            EdaAngle.FromValue((int)(Double.Parse(source, _ci) * 100.0));

        public static EdaRatio ParseRatio(string source) =>
            EdaRatio.FromValue((int)(Double.Parse(source, _ci) * 1000.0));

        public static EdaPoint ParsePoint(string source) {
            try {
                string[] s = source.Split(',');
                double x = Double.Parse(s[0], _ci);
                double y = Double.Parse(s[1], _ci);

                return new EdaPoint((int)(x * 1000000.0), (int)(y * 1000000.0));
            }
            catch (Exception ex) {
                throw new InvalidOperationException(
                    String.Format("No es posible convertir el texto '{0}' a 'EdaPoint'.", source), ex);
            }
        }

        public static EdaSize ParseSize(string source) {
            try {
                string[] s = source.Split(',');
                double width = Double.Parse(s[0], _ci);
                double height = Double.Parse(s[1], _ci);

                return new EdaSize((int)(width * 1000000.0), (int)(height * 1000000.0));
            }
            catch (Exception ex) {
                throw new InvalidOperationException(
                    String.Format("No es posible convertir el texto '{0}' a 'EdaSize'.", source), ex);
            }
        }

        public static EdaLayerSet ParseLayerSet(string source) {

            string[] ss = source.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var layerSet = new EdaLayerSet();
            foreach (var s in ss)
                layerSet.Add(EdaLayerId.Parse(s));

            return layerSet;
        }

    }
}
