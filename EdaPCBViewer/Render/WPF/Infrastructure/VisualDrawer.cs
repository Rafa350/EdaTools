namespace MikroPic.EdaTools.v1.Designer.Render.WPF.Infrastructure {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Geometry.Color;
    using Size = MikroPic.EdaTools.v1.Geometry.Size;
    using Point = MikroPic.EdaTools.v1.Geometry.Point;
    using Rect = MikroPic.EdaTools.v1.Geometry.Rect;
    using WinColor = System.Windows.Media.Color;
    using WinSize = System.Windows.Size;
    using WinPoint = System.Windows.Point;
    using WinRect = System.Windows.Rect;

    internal sealed class VisualDrawer {

        private static readonly TextDrawer td;
        private readonly PenCache penCache;
        private readonly BrushCache brushCache;

        static VisualDrawer() {

            FontFactory ff = FontFactory.Instance;
            Font font = ff.GetFont("Standard");
            td = new TextDrawer(font);
        }

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// 
        public VisualDrawer() {

            penCache = new PenCache();
            brushCache = new BrushCache();
        }

        /// <summary>
        /// Dibuixa una linia en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa a dibuixar element.</param>
        /// <param name="line">El element linia.</param>
        /// 
        public void DrawLineElement(DrawingVisual visual, Layer layer, LineElement line, Color color) {

            using (DrawingContext dc = visual.RenderOpen()) {

                WinColor c = GetLayerColor(color);
                Pen pen = GetPen(c, line.Thickness, line.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);
                DrawLine(dc, pen, line.StartPosition, line.EndPosition);
            }
        }

        /// <summary>
        /// Dibuixa un arc en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa a dibuixar element.</param>
        /// <param name="line">El element arc.</param>
        /// 
        public void DrawArcElement(DrawingVisual visual, Layer layer, ArcElement arc, Color color) {

            using (DrawingContext dc = visual.RenderOpen()) {

                WinColor c = GetLayerColor(color);
                Pen pen = GetPen(c, arc.Thickness, arc.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);
                DrawArc(dc, pen, arc.StartPosition, arc.EndPosition, arc.Radius, arc.Angle);
            }
        }

        /// <summary>
        /// Dibuixa un rectangle en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa a dibuixar element.</param>
        /// <param name="line">El element rectangle.</param>
        /// 
        public void DrawRectangleElement(DrawingVisual visual, Layer layer, RectangleElement rectangle, Color color) {

            using (DrawingContext dc = visual.RenderOpen()) {

                WinColor c = GetLayerColor(color);
                Pen pen = rectangle.Thickness == 0 ? null : GetPen(c, rectangle.Thickness, PenLineCap.Round);
                Brush brush = rectangle.Filled ? GetBrush(c) : null;
                DrawRectangle(dc, pen, brush, rectangle.Position, rectangle.Size, rectangle.Radius);
            }
        }

        /// <summary>
        /// Dibuixa un cercle en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa a dibuixar element.</param>
        /// <param name="circle">El element cercle.</param>
        /// 
        public void DrawCircleElement(DrawingVisual visual, Layer layer, CircleElement circle, Color color) {

            using (DrawingContext dc = visual.RenderOpen()) {

                WinColor c = GetLayerColor(color);
                Pen pen = circle.Thickness == 0 ? null : GetPen(c, circle.Thickness, PenLineCap.Flat);
                Brush brush = circle.Filled ? GetBrush(c) : null;
                DrawCircle(dc, pen, brush, circle.Position, circle.Radius);
            }
        }

        /// <summary>
        /// Dibuixa una via en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa a dibuixar element.</param>
        /// <param name="via">El element via.</param>
        /// 
        public void DrawViaElement(DrawingVisual visual, Layer layer, ViaElement via, Color color) {

            using (DrawingContext dc = visual.RenderOpen()) {

                WinColor c = GetLayerColor(color);

                if (via.Shape == ViaElement.ViaShape.Circle) {

                    int size = layer.Id.Side == BoardSide.Inner ? via.InnerSize : via.OuterSize;
                    int radius = (size + via.Drill) / 4;

                    Pen pen = GetPen(c, (size - via.Drill) / 2, PenLineCap.Flat);
                    DrawCircle(dc, pen, Brushes.Black, via.Position, radius);
                }

                else {

                    Polygon polygon = via.GetPolygon(layer.Id.Side);

                    WinPoint center = new WinPoint(via.Position.X, via.Position.Y);

                    Brush brush = GetBrush(c);
                    DrawPolygon(dc, null, brush, polygon);
                    DrawCircle(dc, null, Brushes.Black, via.Position, via.Drill / 2);
                }
            }
        }

        /// <summary>
        /// Dibuixa un pad smd en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="pad">L'element pad.</param>
        /// 
        public void DrawSmdPadElement(DrawingVisual visual, Layer layer, SmdPadElement pad, Color color) {

            using (DrawingContext dc = visual.RenderOpen()) {

                WinColor c = GetLayerColor(color);
                Brush brush = GetBrush(c);
                DrawRectangle(dc, null, brush, pad.Position, pad.Size, pad.Radius);
            }
        }

        /// <summary>
        /// Dibuixa un pad throught hole en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="pad">L'element pad.</param>
        /// 
        public void DrawThPadElement(DrawingVisual visual, Layer layer, ThPadElement pad, Color color) {

            using (DrawingContext dc = visual.RenderOpen()) {

                WinColor c = GetLayerColor(color);

                if (pad.Shape == ThPadElement.ThPadShape.Circle) {

                    int size =
                        layer.Id.Side == BoardSide.Top ? pad.TopSize :
                        layer.Id.Side == BoardSide.Bottom ? pad.BottomSize :
                        pad.InnerSize;
                    int radius = size / 2;

                    Pen pen = GetPen(c, (size - pad.Drill) / 2, PenLineCap.Flat);
                    DrawCircle(dc, pen, Brushes.Black, pad.Position, radius);
                }

                else {

                    Polygon polygon = pad.GetPolygon(layer.Id.Side);

                    Brush brush = GetBrush(c);
                    DrawPolygon(dc, null, brush, polygon);
                    DrawCircle(dc, null, Brushes.Black, pad.Position, pad.Drill / 2);
                }
                
                /*dc.PushTransform(new ScaleTransform(1, -1, pad.Position.X, pad.Position.Y));

                Brush textBrush = GetBrush(Colors.Yellow);
                FormattedText formattedText = new FormattedText(
                    pad.Name, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                    new Typeface("Arial"), 50, textBrush);
                formattedText.TextAlignment = TextAlignment.Center;

                WinPoint textPosition = new WinPoint(pad.Position.X, pad.Position.Y);
                dc.DrawText(formattedText, new WinPoint(textPosition.X, textPosition.Y - formattedText.Height / 2));

                dc.Pop();
                */
            }
        }
        

        /// <summary>
        /// Dibuixa un hole en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="pad">L'element hole.</param>
        /// 
        public void DrawHoleElement(DrawingVisual visual, Layer layer, HoleElement hole, Color color) {

            using (DrawingContext dc = visual.RenderOpen()) {

                WinColor c = GetLayerColor(color);
                Pen pen = GetPen(c, 0.05 * 1000000.0, PenLineCap.Flat);
                DrawCircle(dc, pen, Brushes.Black, hole.Position, hole.Drill / 2);
            }
        }

        /// <summary>
        /// Dibuixa un text en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="pad">L'element text.</param>
        /// 
        public void DrawTextElement(DrawingVisual visual, Layer layer, Part part, TextElement text, Color color) {

            using (DrawingContext dc = visual.RenderOpen()) {

                PartAttributeAdapter paa = new PartAttributeAdapter(part, text);
                WinPoint position = new WinPoint(paa.Position.X, paa.Position.Y);
                Angle rotation = paa.Rotation;
                IEnumerable<GlyphTrace> glyphTraces = td.Draw(paa.Value, new Point(0, 0), paa.HorizontalAlign, paa.VerticalAlign, paa.Height);

                Matrix m = new Matrix();
                m.Translate(position.X, position.Y);
                m.RotateAt(rotation.Degrees / 100.0, position.X, position.Y);
                dc.PushTransform(new MatrixTransform(m));

                WinColor c = GetLayerColor(color);
                Pen pen = GetPen(c, text.Thickness, PenLineCap.Round);
                DrawGlyphs(dc, pen, null, glyphTraces);
                dc.DrawEllipse(Brushes.YellowGreen, null, new WinPoint(0, 0), 0.15 * 1000000.0, 0.15 * 1000000.0);

                dc.Pop();
            }
        }

        /// <summary>
        /// Dibuixa una regio en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="pad">L'element region.</param>
        /// 
        public void DrawRegionElement(DrawingVisual visual, Layer layer, Board board, RegionElement region, Color color) {

            using (DrawingContext dc = visual.RenderOpen()) {

                Polygon polygon = layer.Function == LayerFunction.Signal ?
                    board.GetRegionPolygon(region, layer, new Transformation()) :
                    region.GetPolygon(layer.Id.Side);

                WinColor c = GetLayerColor(color);
                Pen pen = region.Thickness > 0 ? GetPen(c, region.Thickness, PenLineCap.Round) : null;
                Brush brush = region.Filled ? GetBrush(c) : null;
                DrawPolygon(dc, pen, brush, polygon);
            }
        }

        /// <summary>
        /// Obte el color d'una capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <returns>El color.</returns>
        /// 
        private static WinColor GetLayerColor(Color color) {

            return WinColor.FromRgb(color.R, color.G, color.B);
        }

        /// <summary>
        /// Obte un pen.
        /// </summary>
        /// <param name="color">Color del pen.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="lineCap">Forma dels extrems de linia.</param>
        /// <returns>El pen.</returns>
        /// 
        private Pen GetPen(WinColor color, double thickness, PenLineCap lineCap) {

            Brush brush = GetBrush(color);
            return penCache.GetPen(brush, thickness, lineCap);
        }

        /// <summary>
        /// Obte un brush.
        /// </summary>
        /// <param name="color">El color.</param>
        /// <returns>El brush.</returns>
        /// 
        private Brush GetBrush(WinColor color) {

            return brushCache.GetBrush(color);
        }

        /// <summary>
        /// Primitiva de dibuix de linies
        /// </summary>
        /// <param name="dc">Context de dibuix.</param>
        /// <param name="pen">El per per dibuixar.</param>
        /// <param name="start">Punt d'inici.</param>
        /// <param name="end">Put final.</param>
        /// 
        private static void DrawLine(DrawingContext dc, Pen pen, Point start, Point end) {

            WinPoint p1 = new WinPoint(start.X, start.Y);
            WinPoint p2 = new WinPoint(end.X, end.Y);
            dc.DrawLine(pen, p1, p2);
        }

        /// <summary>
        /// Primitiva de dibuix d'arcs
        /// </summary>
        /// <param name="dc">Context de dibuix.</param>
        /// <param name="pen">El per per dibuixar.</param>
        /// <param name="start">Punt d'inici.</param>
        /// <param name="end">Put final.</param>
        /// <param name="radius">Radi del arc.</param>
        /// <param name="angle">Angle de l'arc.</param>
        /// 
        private static void DrawArc(DrawingContext dc, Pen pen, Point start, Point end, int radius, Angle angle) {

            WinPoint p1 = new WinPoint(start.X, start.Y);
            WinPoint p2 = new WinPoint(end.X, end.Y);
            WinSize size = new WinSize(radius, radius);
            double a = angle.Degrees;

            StreamGeometry g = new StreamGeometry();
            using (StreamGeometryContext gc = g.Open()) {
                gc.BeginFigure(p1, false, false);
                gc.ArcTo(p2, size, a / 100, Math.Abs(a) > 18000.0,
                    a < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise, true, false);
            }
            g.Freeze();

            dc.DrawGeometry(null, pen, g);
        }

        /// <summary>
        /// Primitiva de dibuix de rectangles.
        /// </summary>
        /// <param name="dc">El contexte de dibuix.</param>
        /// <param name="pen">El pen per dibuixar.</param>
        /// <param name="brush">El brush per dibuixar</param>
        /// <param name="position">El centroid del rectangle.</param>
        /// <param name="size">El tamany.</param>
        /// <param name="radius">El radi de corvatura de les cantonades.</param>
        /// 
        private static void DrawRectangle(DrawingContext dc, Pen pen, Brush brush, Point position, Size size, int radius) {

            WinRect rect = new WinRect(
                position.X - (size.Width / 2),
                position.Y - (size.Height / 2),
                size.Width,
                size.Height);
            dc.DrawRoundedRectangle(brush, pen, rect, radius, radius);
        }

        /// <summary>
        /// Primitiva de dibuix de cercles
        /// </summary>
        /// <param name="dc">El contexte de dibuix.</param>
        /// <param name="pen">El pen per dibuixar.</param>
        /// <param name="brush">El brush per dibuixar.</param>
        /// <param name="position">Posicio</param>
        /// <param name="radius">Radi</param>
        /// 
        private static void DrawCircle(DrawingContext dc, Pen pen, Brush brush, Point position, int radius) {

            dc.DrawEllipse(brush, pen, new WinPoint(position.X, position.Y), radius, radius);
        }

        /// <summary>
        /// Primitiva de dibuix de poligonns.
        /// </summary>
        /// <param name="dc">El contexte de dibuix.</param>
        /// <param name="pen">El pen per dibuixar.</param>
        /// <param name="brush">El brush per dibuixar.</param>
        /// <param name="polygon">El poligon a dibuixar.</param>
        /// 
        private static void DrawPolygon(DrawingContext dc, Pen pen, Brush brush, Polygon polygon) {

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
                StreamPolygon(ctx, polygon, polygon.Childs == null ? 1 : 0);
            geometry.Freeze();

            dc.DrawGeometry(brush, pen, geometry);
        }

        /// <summary>
        /// Primitiva de dibuix de textos
        /// </summary>
        /// <param name="dc">El contexte de dibuix.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="brush">El brush.</param>
        /// <param name="glyphTraces">Els traços a dibuixar.</param>
        /// 
        private static void DrawGlyphs(DrawingContext dc, Pen pen, Brush brush, IEnumerable<GlyphTrace> glyphTraces) {

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open()) {

                bool first = true;
                foreach (GlyphTrace glyphTrace in glyphTraces) {
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

            dc.DrawGeometry(brush, pen, geometry);
        }

        /// <summary>
        /// Ompla una geometria amb un poligon.
        /// </summary>
        /// <param name="ctx">Contexte de la geometria.</param>
        /// <param name="polygon">El poligon a dibuixar.</param>
        /// <param name="level">Nivell del poligon.</param>
        /// 
        private static void StreamPolygon(StreamGeometryContext ctx, Polygon polygon, int level) {

            // Procesa el poligon principal
            //
            if (polygon.Points != null) {

                WinPoint p;
                v1.Geometry.Point[] points = polygon.Points;

                p = new WinPoint(points[0].X, points[0].Y);
                ctx.BeginFigure(p, true, true);

                for (int i = 1; i < points.Length; i++) {
                    p = new WinPoint(points[i].X, points[i].Y);
                    ctx.LineTo(p, true, true);
                }
            }

            // Procesa els poligons fills
            //
            if (polygon.Childs != null && (level < 2))
                for (int i = 0; i < polygon.Childs.Length; i++)
                    StreamPolygon(ctx, polygon.Childs[i], level + 1);
        }
    }
}
