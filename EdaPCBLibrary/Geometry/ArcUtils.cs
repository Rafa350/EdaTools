namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    using System;
    using System.Windows;

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
        public static Point Center(Point startPosition, Point endPosition, Angle angle) {

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
            if (angle.Degrees > 0)
                return new Point(
                    mx + Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (y1 - y2) / d,
                    my + Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (x2 - x1) / d);

            else
                return new Point(
                    mx - Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (y1 - y2) / d,
                    my - Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (x2 - x1) / d);
        }

        public static Angle StartAngle(Point startPosition, Point center) {

            return Angle.FromRadiants(Math.Atan2(startPosition.Y - center.Y, startPosition.X - center.X));
        }

        public static Angle EndAngle(Point endPosition, Point center) {

            return Angle.FromRadiants(Math.Atan2(endPosition.Y - center.Y, endPosition.X - center.X));
        }

        public static double Radius(Point startPosition, Point endPosition, Angle angle) {

            double dx = endPosition.X - startPosition.X;
            double dy = endPosition.Y - startPosition.Y;
            double length = Math.Sqrt((dx * dx) + (dy * dy));
            return Math.Abs(length / 2.0 / Math.Sin(angle.Radiants / 2.0));
        }
    }
}
