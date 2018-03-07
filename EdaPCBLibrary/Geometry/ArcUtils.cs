namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using System;

    /// <summary>
    /// Operacions amb arcs.
    /// </summary>
    internal static class ArcUtils {

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

            // Calcula el punt central
            //
            double mx = (x1 + x2) / 2.0;
            double my = (y1 + y2) / 2.0;

            // Calcula la distancia entre els dos punts.
            //
            double d = Math.Sqrt(Math.Pow(x2 - x1, 2.0) + Math.Pow(y2 - y1, 2.0));

            // Calcula el radi
            //
            double r = Math.Abs((d / 2.0) / Math.Sin(angle.Radiants / 2.0));

            // Calcula el centre
            //
            double cx, cy;
            if (angle.Degrees > 0) {
                cx = mx + Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (y1 - y2) / d;
                cy = my + Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (x2 - x1) / d;
            }

            else {
                cx = mx - Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (y1 - y2) / d;
                cy = my - Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (x2 - x1) / d;
            }

            return new PointInt((int) cx, (int) cy);
        }

        public static Angle StartAngle(PointInt startPosition, PointInt center) {

            return Angle.FromRadiants(Math.Atan2(startPosition.Y - center.Y, startPosition.X - center.X));
        }

        public static Angle EndAngle(PointInt endPosition, PointInt center) {

            return Angle.FromRadiants(Math.Atan2(endPosition.Y - center.Y, endPosition.X - center.X));
        }

        public static int Radius(PointInt startPosition, PointInt endPosition, Angle angle) {

            int dx = endPosition.X - startPosition.X;
            int dy = endPosition.Y - startPosition.Y;
            double length = Math.Sqrt((dx * dx) + (dy * dy));
            return (int) Math.Abs(length / 2.0 / Math.Sin(angle.Radiants / 2.0));
        }
    }
}
