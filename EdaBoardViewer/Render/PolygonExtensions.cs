namespace EdaBoardViewer.Render {

    using System.Linq;

    using Avalonia.Media;

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

    public static class PolygonExtensions {

        public static Geometry ToGeometry(this EdaPolygon polygon) {

            var g = new StreamGeometry();
            using (StreamGeometryContext gc = g.Open())
                StreamPolygon(gc, polygon, polygon.Childs == null ? 1 : 0);

            return g;
        }

        private static void StreamPolygon(StreamGeometryContext gc, EdaPolygon polygon, int level) {

            // Procesa el poligon principal
            //
            if (polygon.Points != null) {

                EdaPoint[] points = polygon.Points.ToArray();

                gc.BeginFigure(points[0].ToPoint(), true);
                for (int i = 1; i < points.Length; i++)
                    gc.LineTo(points[i].ToPoint());
                gc.LineTo(points[0].ToPoint());
            }

            // Procesa els poligons fills
            //
            if ((polygon.Childs != null) && (level < 2))
                foreach (var child in polygon.Childs)
                    StreamPolygon(gc, child, level + 1);
        }
    }
}
