using System.Linq;
using Avalonia.Media;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace EdaBoardViewer.Render {

    public static class PolygonExtensions {

        public static Geometry ToGeometry(this EdaPolygon polygon) {

            var g = new StreamGeometry();
            using (StreamGeometryContext gc = g.Open())
                StreamPolygon(gc, polygon, polygon.Holes == null ? 1 : 0);

            return g;
        }

        private static void StreamPolygon(StreamGeometryContext gc, EdaPolygon polygon, int level) {

            // Procesa el poligon principal
            //
            if (polygon.Contour != null) {

                EdaPoint[] points = polygon.Contour.ToArray();

                gc.BeginFigure(points[0].ToPoint(), true);
                for (int i = 1; i < points.Length; i++)
                    gc.LineTo(points[i].ToPoint());
                gc.LineTo(points[0].ToPoint());
            }

            // Procesa els poligons fills
            //
            if ((polygon.Holes != null) && (level < 2))
                foreach (var child in polygon.Holes)
                    StreamPolygon(gc, child, level + 1);
        }
    }
}
