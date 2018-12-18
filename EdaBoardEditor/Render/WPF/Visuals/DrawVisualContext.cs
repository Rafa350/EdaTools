namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;
    using Point = MikroPic.EdaTools.v1.Base.Geometry.Point;
    using WinColor = System.Windows.Media.Color;
    using WinPoint = System.Windows.Point;
    using WinRect = System.Windows.Rect;
    using WinSize = System.Windows.Size;

    public sealed class DrawVisualContext {

        private readonly DrawingContext context;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="context">El context de dibuix.</param>
        /// 
        public DrawVisualContext(DrawingContext context) {

            this.context = context;
        }

        /// <summary>
        /// Dibuixa una linia.
        /// </summary>
        /// <param name="pen">El pen.</param>
        /// <param name="p1">Punt inicial.</param>
        /// <param name="p2">Punt final.</param>
        /// 
        public void DrawLine(Pen pen, Point p1, Point p2) {

            context.DrawLine(pen, new WinPoint(p1.X, p1.Y), new WinPoint(p2.X, p2.Y));
        }

        /// <summary>
        /// Dibuixa un arc.
        /// </summary>
        /// <param name="pen">El pen.</param>
        /// <param name="p1">Punt inicial.</param>
        /// <param name="p2">Punt final.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="angle">Angle de l'arc.</param>
        /// 
        public void DrawArc(Pen pen, Point p1, Point p2, int radius, Angle angle) {

            WinPoint wp1 = new WinPoint(p1.X, p1.Y);
            WinPoint wp2 = new WinPoint(p2.X, p2.Y);
            WinSize size = new WinSize(radius, radius);
            double a = angle.Degrees;

            StreamGeometry g = new StreamGeometry();
            using (StreamGeometryContext gc = g.Open()) {
                gc.BeginFigure(wp1, false, false);
                gc.ArcTo(wp2, size, a / 100.0, Math.Abs(a) > 18000.0,
                    a < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise, true, false);
            }
            g.Freeze();

            context.DrawGeometry(null, pen, g);
        }

        /// <summary>
        /// Dibuixa un rectangle.
        /// </summary>
        /// <param name="brush">El brush.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="centroid">Posicio del rectangle.</param>
        /// <param name="size">Tamany del rectangle.</param>
        /// 
        public void DrawRectangle(Brush brush, Pen pen, Point centroid, Size size) {

            WinRect r = new WinRect(centroid.X - size.Width / 2, centroid.Y - size.Height / 2, size.Width, size.Height);
            context.DrawRectangle(brush, pen, r);
        }

        /// <summary>
        /// Dibuixa un rectangle arrodonit.
        /// </summary>
        /// <param name="brush">El brush.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="centroid">Posicio del rectangle.</param>
        /// <param name="size">Tamany del rectangle.</param>
        /// <param name="radiusX">El radi X dels cantons.</param>
        /// <param name="radiusY">El radi Y dels cantons.</param>
        /// 
        public void DrawRoundedRectangle(Brush brush, Pen pen, Point centroid, Size size, int radiusX, int radiusY) {

            WinRect r = new WinRect(centroid.X - size.Width / 2, centroid.Y - size.Height / 2, size.Width, size.Height);
            context.DrawRoundedRectangle(brush, pen, r, radiusX, radiusY);
        }

        /// <summary>
        /// Dibuixa una elipse.
        /// </summary>
        /// <param name="brush">El brush.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="center">En centre de l'el·lipse.</param>
        /// <param name="radiusX">El radi del eix X.</param>
        /// <param name="radiusY">El redi del eix Y.</param>
        /// 
        public void DrawEllipse(Brush brush, Pen pen, Point center, int radiusX, int radiusY) {

            context.DrawEllipse(brush, pen, new WinPoint(center.X, center.Y), radiusX, radiusY);
        }

        /// <summary>
        /// Dibuixa un poligon.
        /// </summary>
        /// <param name="brush">El brush.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="polygon">El poligon.</param>
        /// 
        public void DrawPolygon(Brush brush, Pen pen, Polygon polygon) {

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
                StreamPolygon(ctx, polygon, polygon.Childs == null ? 1 : 0);
            geometry.Freeze();

            context.DrawGeometry(brush, pen, geometry);
        }

        /// <summary>
        /// Dibuixa un glif
        /// </summary>
        /// <param name="brush">El brush.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="glyphTraces">Els gliphs</param>
        /// 
        public void DrawGlyphs(Brush brush, Pen pen, IEnumerable<GlyphTrace> glyphTraces) {

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open()) {

                bool first = true;
                foreach (var glyphTrace in glyphTraces) {
                    WinPoint p = new WinPoint(glyphTrace.Position.X, glyphTrace.Position.Y);
                    if (first) {
                        ctx.BeginFigure(p, false, false);
                        first = false;
                    }
                    else
                        ctx.LineTo(p, glyphTrace.Stroke, true);
                }
            }
            geometry.Freeze();

            context.DrawGeometry(brush, pen, geometry);
        }

        private static void StreamPolygon(StreamGeometryContext ctx, Polygon polygon, int level) {

            // Procesa el poligon principal
            //
            if (polygon.Points != null) {

                WinPoint point = new WinPoint(polygon.Points[0].X, polygon.Points[0].Y);
                ctx.BeginFigure(point, true, true);

                WinPoint[] points = new WinPoint[polygon.Points.Length - 1];
                for (int i = 1; i < polygon.Points.Length; i++) {
                    points[i - 1].X = polygon.Points[i].X;
                    points[i - 1].Y = polygon.Points[i].Y;
                }
                ctx.PolyLineTo(points, true, true);
            }

            // Procesa els poligons fills
            //
            if (polygon.Childs != null && (level < 2))
                for (int i = 0; i < polygon.Childs.Length; i++)
                    StreamPolygon(ctx, polygon.Childs[i], level + 1);
        }

        /// <summary>
        /// Obte un brush
        /// </summary>
        /// <param name="color">El color.</param>
        /// <returns>El brush obtingut.</returns>
        /// 
        public Brush GetBrush(Color color) {

            Brush brush = new SolidColorBrush(WinColor.FromRgb(color.R, color.G, color.B));
            brush.Freeze();

            return brush;
        }

        /// <summary>
        /// Obte un pen.
        /// </summary>
        /// <param name="color">El color.</param>
        /// <param name="thickness">L'amplada de linia.</param>
        /// <param name="lineCap">Els finals de linia.</param>
        /// <returns>El pen obtingut.</returns>
        /// 
        public Pen GetPen(Color color, int thickness, PenLineCap lineCap) {

            Pen pen = new Pen(GetBrush(color), thickness);
            pen.StartLineCap = lineCap;
            pen.EndLineCap = lineCap;
            pen.LineJoin = PenLineJoin.Round;
            pen.Freeze();

            return pen;
        }

        /// <summary>
        /// Empeny una transformacio en la pila.
        /// </summary>
        /// <param name="m">La transformacio.</param>
        /// 
        public void PushTransform(MatrixTransform m) {

            context.PushTransform(m);
        }

        /// <summary>
        /// Estira un objecte de la pila.
        /// </summary>
        /// 
        public void Pop() {

            context.Pop();
        }

        /// <summary>
        /// Obte el context de dibuix.
        /// </summary>
        /// 
        public DrawingContext Context {
            get {
                return context;
            }
        }
    }
}
