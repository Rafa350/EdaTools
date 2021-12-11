namespace MikroPic.EdaTools.v1.Base.Geometry.Utils {

    using System;

    /// <summary>
    /// Operacions amb arcs.
    /// </summary>
    public static class ArcUtils {

        /// <summary>
        /// Calcula el centre d'un arc.
        /// </summary>
        /// <param name="startPosition">Posicio inicial.</param>
        /// <param name="endPosition">Pocicio final.</param>
        /// <param name="angle">Angle d'apertura.</param>
        /// <returns>El centre.</returns>
        /// 
        public static EdaPoint Center(EdaPoint startPosition, EdaPoint endPosition, EdaAngle angle) {

            double x1 = startPosition.X;
            double y1 = startPosition.Y;

            double x2 = endPosition.X;
            double y2 = endPosition.Y;

            double mx = (x1 + x2) / 2.0;
            double my = (y1 + y2) / 2.0;

            double dx = x2 - x1;
            double dy = y2 - y1;
            double d = Math.Sqrt((dx * dx) + (dy * dy));

            double a = angle.AsRadiants / 2.0;

            double r = Math.Abs(d / 2.0 / Math.Sin(a));
            double s = Math.Abs(r * Math.Cos(a));
            if (a < 0)
                s = -s;
            if (Math.Abs(a) > (Math.PI / 2.0))
                s = -s;

            double cx = mx + s * (y1 - y2) / d;
            double cy = my + s * (x2 - x1) / d;

            return new EdaPoint((int)cx, (int)cy);
        }

        /// <summary>
        /// Calcula l'angle del punt inicial, repecte l'eix X.
        /// </summary>
        /// <param name="startPosition">Punt inicial.</param>
        /// <param name="center">Centre.</param>
        /// <returns>L'angle.</returns>
        /// 
        public static EdaAngle StartAngle(EdaPoint startPosition, EdaPoint center) {

            return EdaAngle.FromRadiants(Math.Atan2(startPosition.Y - center.Y, startPosition.X - center.X));
        }

        /// <summary>
        /// Calcula l'angle del punt final, repecte l'eix X.
        /// </summary>
        /// <param name="startPosition">Punt inicial.</param>
        /// <param name="center">Centre.</param>
        /// <returns>L'angle.</returns>
        /// 
        public static EdaAngle EndAngle(EdaPoint endPosition, EdaPoint center) {

            return EdaAngle.FromRadiants(Math.Atan2(endPosition.Y - center.Y, endPosition.X - center.X));
        }

        public static EdaPoint EndPosition(EdaPoint center, EdaPoint startPosition, EdaAngle angle) {

            double r = angle.AsRadiants;
            double sin = Math.Sin(r);
            double cos = Math.Cos(r);

            double x = startPosition.X;
            double y = startPosition.Y;

            return new EdaPoint(
                center.X + (int)((x * cos) - (y * sin)),
                center.Y + (int)((x * sin) + (y * cos)));
        }

        public static int Radius(EdaPoint startPosition, EdaPoint endPosition, EdaAngle angle) {

            double dx = endPosition.X - startPosition.X;
            double dy = endPosition.Y - startPosition.Y;
            double d = Math.Sqrt((dx * dx) + (dy * dy));

            double a = angle.AsRadiants / 2.0;

            return (int)Math.Abs(d / 2.0 / Math.Sin(a));
        }
    }
}
