namespace MikroPic.EdaTools.v1.Geometry {

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
        public static PointInt Center(PointInt startPosition, PointInt endPosition, Angle angle) {

            double x1 = startPosition.X;
            double y1 = startPosition.Y;

            double x2 = endPosition.X;
            double y2 = endPosition.Y;

            double mx = (x1 + x2) / 2.0;
            double my = (y1 + y2) / 2.0;

            double dx = x2 - x1;
            double dy = y2 - y1;
            double d = Math.Sqrt((dx * dx) + (dy * dy));

            double a = angle.Radiants / 2.0;

            double r = Math.Abs(d / 2.0 / Math.Sin(a));
            double s = Math.Abs(r * Math.Cos(a));
            if (a < 0)
                s = -s;
            if (Math.Abs(a) > (Math.PI / 2.0))
                s = -s;

            double cx = mx + s * (y1 - y2) / d;
            double cy = my + s * (x2 - x1) / d;

            return new PointInt((int)cx, (int)cy);
        }

        public static Angle StartAngle(PointInt startPosition, PointInt center) {

            return Angle.FromRadiants(Math.Atan2(startPosition.Y - center.Y, startPosition.X - center.X));
        }

        public static Angle EndAngle(PointInt endPosition, PointInt center) {

            return Angle.FromRadiants(Math.Atan2(endPosition.Y - center.Y, endPosition.X - center.X));
        }

        public static int Radius(PointInt startPosition, PointInt endPosition, Angle angle) {

            double dx = endPosition.X - startPosition.X;
            double dy = endPosition.Y - startPosition.Y;
            double d = Math.Sqrt((dx * dx) + (dy * dy));

            double a = angle.Radiants / 2.0;

            return (int) Math.Abs(d / 2.0 / Math.Sin(a));
        }
    }
}
