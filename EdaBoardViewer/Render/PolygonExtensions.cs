using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace EdaBoardViewer.Render {

    public static class PolygonExtensions {

        public static Geometry ToGeometry(this EdaPolygon polygon) {

            var g = new StreamGeometry();
            using (StreamGeometryContext gc = g.Open()) {
                ToGeometry(gc, polygon);
            }

            return g;
        }

        public static Geometry ToGeometry(this IEnumerable<EdaPolygon> polygons) {

            var g = new StreamGeometry();
            using (StreamGeometryContext gc = g.Open()) {
                foreach (var polygon in polygons)
                    ToGeometry(gc, polygon);
            }

            return g;
        }

        private static void ToGeometry(StreamGeometryContext gc, EdaPolygon polygon) {

            ToGeometry(gc, polygon.Outline);
            if (polygon.HasHoles)
                foreach (var hole in polygon.Holes)
                    ToGeometry(gc, hole);
        }

        private static void ToGeometry(StreamGeometryContext gc, IEnumerable<EdaPoint> points) {

            bool first = true;
            Point firstPoint = default;
            foreach (var point in points) {
                if (first) {
                    firstPoint = point.ToPoint();
                    gc.BeginFigure(firstPoint, true);
                    first = false;
                }
                else
                    gc.LineTo(point.ToPoint());
            }
            gc.LineTo(firstPoint);
        }
    }
}
