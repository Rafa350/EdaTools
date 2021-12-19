﻿using System;
using System.Globalization;
using System.Text;

using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Core.Model.IO {

    public static class EdaFormatter {

        private static readonly CultureInfo _ci = CultureInfo.InvariantCulture;

        public static string FormatScalar(int value) =>
            String.Format(_ci, "{0}", value / 1000000.0);

        public static string FormatAngle(EdaAngle value) =>
            String.Format(_ci, "{0}", value.Value / 100.0);

        public static string FormatRatio(EdaRatio value) => 
            String.Format(_ci, "{0}", value.AsPercent);

        public static string FormatPoint(EdaPoint value) =>
            String.Format(_ci, "{0}, {1}", value.X / 1000000.0, value.Y / 1000000.0);

        public static string FormatSize(EdaSize value) =>
            String.Format(_ci, "{0}, {1}", value.Width / 1000000.0, value.Height / 1000000.0);

        public static string FormatLayerSet(EdaLayerSet value) {

            var sb = new StringBuilder();

            bool first = true;
            foreach (var layerId in value) {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(layerId.ToString());
            }

            return sb.ToString();

        }
    }
}
