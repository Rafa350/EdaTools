namespace EdaBoardViewer.Render {

    using Avalonia.Media;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

    public static class PolygonExtensions {

        public static Geometry ToGeometry(this Polygon polygon) {

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
                StreamPolygon(ctx, polygon, polygon.Childs == null ? 1 : 0);


            return geometry;
        }

        private static void StreamPolygon(StreamGeometryContext ctx, Polygon polygon, int level) {

            // Procesa el poligon principal
            //
            if (polygon.Points != null) {

                Avalonia.Point point = polygon.Points[0].ToPoint();
                ctx.BeginFigure(point, true);

                for (int i = 1; i < polygon.Points.Length; i++)
                    ctx.LineTo(polygon.Points[i].ToPoint());
            }

            // Procesa els poligons fills
            //
            if (polygon.Childs != null && (level < 2))
                for (int i = 0; i < polygon.Childs.Length; i++)
                    StreamPolygon(ctx, polygon.Childs[i], level + 1);
        }
    }
}
