namespace EdaBoardViewer.Render {

    using Avalonia.Media;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

    public static class PolygonExtensions {

        public static Geometry ToGeometry(this Polygon polygon) {

            StreamGeometry g = new StreamGeometry();
            using (StreamGeometryContext gc = g.Open())
                StreamPolygon(gc, polygon, polygon.Childs == null ? 1 : 0);

            return g;
        }

        private static void StreamPolygon(StreamGeometryContext gc, Polygon polygon, int level) {

            // Procesa el poligon principal
            //
            if (polygon.Points != null) {

                bool first = true;
                foreach (var point in polygon.Points) {
                    if (first) {
                        first = false;
                        gc.BeginFigure(point.ToPoint(), true);
                    }
                    else
                        gc.LineTo(point.ToPoint());
                }
            }

            // Procesa els poligons fills
            //
            if ((polygon.Childs != null) && (level < 2))
                foreach (var child in polygon.Childs)
                    StreamPolygon(gc, child, level + 1);
        }
    }
}
