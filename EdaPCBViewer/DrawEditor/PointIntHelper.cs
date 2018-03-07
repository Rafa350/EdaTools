namespace MikroPic.EdaTools.v1.Designer.DrawEditor {

    using System.Windows;
    using MikroPic.EdaTools.v1.Pcb.Geometry;

    public static class PointIntHelper {

        public static Point ToPoint(this PointInt point) {

            return new Point(point.X, point.Y);
        }
    }
}
