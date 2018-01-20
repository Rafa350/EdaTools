namespace Eda.PCBViewer.DrawEditor {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    
    public static class DrawingContextHelper {

        /// <summary>
        /// Dibuixa un anell circular.
        /// </summary>
        /// <param name="dc">Contexte de dibuix.</param>
        /// <param name="brush">La brotxa.</param>
        /// <param name="pen">El pincell.</param>
        /// <param name="center">El centre del pad.</param>
        /// <param name="size">Tamany del pad.</param>
        /// <param name="drill">Diametre del forat.</param>
        public static void DrawCircularRing(this DrawingContext dc, Brush brush, Pen pen, Point center, double size, double drill) {

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open()) {

                bool isStroked = pen != null;

                double oRadius = size / 2;
                Size oSize = new Size(oRadius, oRadius);

                ctx.BeginFigure(new Point(center.X - oRadius, center.Y), true, true);
                ctx.ArcTo(new Point(center.X + oRadius, center.Y), oSize, 180, true, SweepDirection.Clockwise, isStroked, true);
                ctx.ArcTo(new Point(center.X - oRadius, center.Y), oSize, 180, true, SweepDirection.Clockwise, isStroked, true);

                double iRadius = drill / 2;
                Size iSize = new Size(iRadius, iRadius);

                ctx.BeginFigure(new Point(center.X - iRadius, center.Y), true, true);
                ctx.ArcTo(new Point(center.X + iRadius, center.Y), iSize, 180, true, SweepDirection.Clockwise, isStroked, true);
                ctx.ArcTo(new Point(center.X - iRadius, center.Y), iSize, 180, true, SweepDirection.Clockwise, isStroked, true);
            }
            geometry.Freeze();

            dc.DrawGeometry(brush, pen, geometry);
        }

        /// <summary>
        /// Dibuixa un anell cuadrat.
        /// </summary>
        /// <param name="dc">Contexte de dibuix.</param>
        /// <param name="brush">La brotxa.</param>
        /// <param name="pen">El pincell.</param>
        /// <param name="center">El centre del pad.</param>
        /// <param name="size">Tamany del pad.</param>
        /// <param name="drill">Diametre del forat.</param>
        public static void DrawSquareRing(this DrawingContext dc, Brush brush, Pen pen, Point center, double size, double drill) {

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open()) {

                bool isStroked = pen != null;

                double delta = size / 2;

                ctx.BeginFigure(new Point(center.X - delta, center.Y - delta), true, true);
                ctx.LineTo(new Point(center.X + delta, center.Y - delta), isStroked, true);
                ctx.LineTo(new Point(center.X + delta, center.Y + delta), isStroked, true);
                ctx.LineTo(new Point(center.X - delta, center.Y + delta), isStroked, true);
                ctx.LineTo(new Point(center.X - delta, center.Y - delta), isStroked, true); 

                double iRadius = drill / 2;
                Size iSize = new Size(iRadius, iRadius);

                ctx.BeginFigure(new Point(center.X - iRadius, center.Y), true, true);
                ctx.ArcTo(new Point(center.X + iRadius, center.Y), iSize, 180, true, SweepDirection.Clockwise, isStroked, true);
                ctx.ArcTo(new Point(center.X - iRadius, center.Y), iSize, 180, true, SweepDirection.Clockwise, isStroked, true);
            }
            geometry.Freeze();

            dc.DrawGeometry(brush, pen, geometry);
        }

        /// <summary>
        /// Dibuixa un anell octogonal.
        /// </summary>
        /// <param name="dc">Contexte de dibuix.</param>
        /// <param name="brush">La brotxa.</param>
        /// <param name="pen">El pincell.</param>
        /// <param name="center">El centre del pad.</param>
        /// <param name="size">Tamany del pad.</param>
        /// <param name="drill">Diametre del forat.</param>
        public static void DrawOctogonalRing(this DrawingContext dc, Brush brush, Pen pen, Point center, double size, double drill) {

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open()) {

                bool isStroked = pen != null;

                double iRadius = drill / 2;
                Size iSize = new Size(iRadius, iRadius);

                ctx.BeginFigure(new Point(center.X - iRadius, center.Y), true, true);
                ctx.ArcTo(new Point(center.X + iRadius, center.Y), iSize, 180, true, SweepDirection.Clockwise, isStroked, true);
                ctx.ArcTo(new Point(center.X - iRadius, center.Y), iSize, 180, true, SweepDirection.Clockwise, isStroked, true);

                double oRadius = (size / 2) / Math.Cos(22.5 * Math.PI / 180);
                Point point = Rotate(22.5, new Point(center.X, center.Y + oRadius), center);
                ctx.BeginFigure(point, true, true);
                for (int i = 1; i < 8; i++) {
                    point = Rotate(45, point, center);
                    ctx.LineTo(point, isStroked, true);
                }
            }
            geometry.Freeze();

            dc.DrawGeometry(brush, pen, geometry);
        }

        /// <summary>
        /// Dibuixa un anell ovalat.
        /// </summary>
        /// <param name="dc">Contexte de dibuix.</param>
        /// <param name="brush">La brotxa.</param>
        /// <param name="pen">El pincell.</param>
        /// <param name="center">El centre del pad.</param>
        /// <param name="size">Tamany del pad.</param>
        /// <param name="drill">Diametre del forat.</param>
        public static void DrawOvalRing(this DrawingContext dc, Brush brush, Pen pen, Point center, double size, double drill) {

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open()) {

                bool isStroked = pen != null;

                double delta = size / 2;
                Size oSize = new Size(delta, delta);

                ctx.BeginFigure(new Point(center.X - delta, center.Y - delta), true, true);
                ctx.LineTo(new Point(center.X + delta, center.Y - delta), isStroked, true);
                ctx.ArcTo(new Point(center.X + delta, center.Y + delta), oSize, 180, true, SweepDirection.Clockwise, isStroked, true);
                ctx.LineTo(new Point(center.X - delta, center.Y + delta), isStroked, true);
                ctx.ArcTo(new Point(center.X - delta, center.Y - delta), oSize, 180, true, SweepDirection.Clockwise, isStroked, true);

                double iRadius = drill / 2;
                Size iSize = new Size(iRadius, iRadius);

                ctx.BeginFigure(new Point(center.X - iRadius, center.Y), true, true);
                ctx.ArcTo(new Point(center.X + iRadius, center.Y), iSize, 180, true, SweepDirection.Clockwise, isStroked, true);
                ctx.ArcTo(new Point(center.X - iRadius, center.Y), iSize, 180, true, SweepDirection.Clockwise, isStroked, true);
            }
            geometry.Freeze();

            dc.DrawGeometry(brush, pen, geometry);
        }

        public static void DrawPolygon(this DrawingContext dc, Brush brush, Pen pen, Polygon polygon) {

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open()) {

                bool isStroked = pen != null;
                StreamPolygon(ctx, polygon, isStroked);
                if (polygon.HasChilds)
                    foreach (Polygon hole in polygon.Childs)
                        StreamPolygon(ctx, hole, isStroked);
            }
            geometry.Freeze();

            dc.DrawGeometry(brush, pen, geometry);
        }

        private static void StreamPolygon(StreamGeometryContext ctx, Polygon polygon, bool isStroked) {

            if (polygon.HasPoints) {
                bool first = true;
                foreach (Point point in polygon.Points) {
                    if (first) {
                        first = false;
                        ctx.BeginFigure(point, true, true);
                    }
                    else
                        ctx.LineTo(point, isStroked, true);
                }
            }
        }

        private static Point Rotate(double angle, Point point, Point center) {

            double a = angle * Math.PI / 180;
            return new Point (
                center.X + Math.Cos(a) * (point.X - center.X) - Math.Sin(a) * (point.Y - center.Y),
                center.Y + Math.Sin(a) * (point.X - center.X) + Math.Cos(a) * (point.Y - center.Y));
        }
    }
}
