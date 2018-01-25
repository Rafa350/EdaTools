namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    using System;
    using System.Windows;

    internal static class ArcUtils {

        public static Point Center(Point startPosition, Point endPosition, double angle) {

            double rAngle = angle * Math.PI / 180.0;
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
            double r = Math.Abs((d / 2.0) / Math.Sin(rAngle / 2.0));

            // Calcula el centre
            //
            if (angle > 0)
                return new Point(
                    mx + Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (y1 - y2) / d,
                    my + Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (x2 - x1) / d);

            else
                return new Point(
                    mx - Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (y1 - y2) / d,
                    my - Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (x2 - x1) / d);
        }

        public static double StartAngle(Point startPosition, Point center) {

            double a = Math.Atan2(startPosition.Y - center.Y, startPosition.X - center.X);
            return a * 180.0 / Math.PI;
        }

        public static double Radius(Point startPosition, Point endPosition, double angle) {

            double dx = endPosition.X - startPosition.X;
            double dy = endPosition.Y - startPosition.Y;
            double length = Math.Sqrt((dx * dx) + (dy * dy));
            double a = angle * Math.PI / 180.0;
            return Math.Abs(length / 2.0 / Math.Sin(a / 2.0));
        }
    }
}
