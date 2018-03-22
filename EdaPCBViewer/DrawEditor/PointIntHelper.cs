namespace MikroPic.EdaTools.v1.Designer.DrawEditor {

    using System.Windows;
    using MikroPic.EdaTools.v1.Geometry;

    public static class PointIntHelper {

        public static Point ToPoint(this PointInt point) {

            return new Point((double)point.X / 1000000.0, (double)point.Y/1000000.0);
        }
    }
}
