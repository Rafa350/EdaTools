namespace EdaBoardViewer.Render {

    using MikroPic.EdaTools.v1.Base.Geometry;

    public static class PointExtensions {

        public static Avalonia.Point ToPoint(this EdaPoint point) {

            return new Avalonia.Point(point.X, point.Y);
        }
    }
}
