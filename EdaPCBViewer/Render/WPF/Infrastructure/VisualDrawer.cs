namespace MikroPic.EdaTools.v1.PanelEditor.Render.WPF.Infrastructure {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Core.Infrastructure;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;
    using Point = MikroPic.EdaTools.v1.Base.Geometry.Point;
    using WinColor = System.Windows.Media.Color;
    using WinPoint = System.Windows.Point;
    using WinRect = System.Windows.Rect;
    using WinSize = System.Windows.Size;

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

                WinPoint p1 = new WinPoint(line.StartPosition.X, line.StartPosition.Y);
                WinPoint p2 = new WinPoint(line.EndPosition.X, line.EndPosition.Y);

                WinColor c = WinColor.FromRgb(color.R, color.G, color.B);
                Pen pen = GetPen(c, line.Thickness, line.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);

                dc.DrawLine(pen, p1, p2);
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

                WinPoint p1 = new WinPoint(arc.StartPosition.X, arc.StartPosition.Y);
                WinPoint p2 = new WinPoint(arc.EndPosition.X, arc.EndPosition.Y);
                WinSize size = new WinSize(arc.Radius, arc.Radius);
                double a = arc.Angle.Degrees;

                StreamGeometry g = new StreamGeometry();
                using (StreamGeometryContext gc = g.Open()) {
                    gc.BeginFigure(p1, false, false);
                    gc.ArcTo(p2, size, a / 100.0, Math.Abs(a) > 18000.0,
                        a < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise, true, false);
                }
                g.Freeze();

                WinColor c = WinColor.FromRgb(color.R, color.G, color.B);
                Pen pen = GetPen(c, arc.Thickness, arc.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);
                dc.DrawGeometry(null, pen, g);
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

                WinRect rect = new WinRect(
                    rectangle.Position.X - (rectangle.Size.Width / 2),
                    rectangle.Position.Y - (rectangle.Size.Height / 2),
                    rectangle.Size.Width,
                    rectangle.Size.Height);

                WinColor c = WinColor.FromRgb(color.R, color.G, color.B);
                Pen pen = rectangle.Thickness == 0 ? null : GetPen(c, rectangle.Thickness, PenLineCap.Round);
                Brush brush = rectangle.Filled ? GetBrush(c) : null;

                if (rectangle.Radius == 0)
                    dc.DrawRectangle(brush, pen, rect);
                else
                    dc.DrawRoundedRectangle(brush, pen, rect, rectangle.Radius, rectangle.Radius);
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

                WinPoint center = new WinPoint(circle.Position.X, circle.Position.Y);

                WinColor c = WinColor.FromRgb(color.R, color.G, color.B);
                Pen pen = circle.Thickness == 0 ? null : GetPen(c, circle.Thickness, PenLineCap.Flat);
                Brush brush = circle.Filled ? GetBrush(c) : null;

                dc.DrawEllipse(brush, pen, center, circle.Radius, circle.Radius);
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

                WinPoint center = new WinPoint(via.Position.X, via.Position.Y);

                if (layer.Function == LayerFunction.Mechanical) {
                    WinColor c = WinColor.FromRgb(color.R, color.G, color.B);
                    Pen pen = GetPen(c, 0.05 * 1000000.0, PenLineCap.Flat);
                    dc.DrawEllipse(Brushes.Black, pen, center, via.Drill / 2, via.Drill / 2);
                }

                else {
                    WinColor c = WinColor.FromRgb(color.R, color.G, color.B);

                    if (via.Shape == ViaElement.ViaShape.Circle) {

                        int size = layer.Id.Side == BoardSide.Inner ? via.InnerSize : via.OuterSize;
                        int radius = (size + via.Drill) / 4;

                        Pen pen = GetPen(c, (size - via.Drill) / 2, PenLineCap.Flat);
                        dc.DrawEllipse(Brushes.Black, pen, center, radius, radius);
                    }

                    else {

                        Polygon polygon = via.GetPolygon(layer.Id.Side);

                        Brush brush = GetBrush(c);
                        DrawPolygon(dc, null, brush, polygon);
                        dc.DrawEllipse(Brushes.Black, null, center, via.Drill / 2, via.Drill / 2);
                    }
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

                WinRect rect = new WinRect(
                    pad.Position.X - (pad.Size.Width / 2),
                    pad.Position.Y - (pad.Size.Height / 2),
                    pad.Size.Width,
                    pad.Size.Height);

                WinColor c = WinColor.FromRgb(color.R, color.G, color.B);
                Brush brush = GetBrush(c);
                if (pad.Radius == 0)
                    dc.DrawRectangle(brush, null, rect);
                else
                    dc.DrawRoundedRectangle(brush, null, rect, pad.Radius, pad.Radius);
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

                WinPoint center = new WinPoint(pad.Position.X, pad.Position.Y);

                WinColor c = WinColor.FromRgb(color.R, color.G, color.B);

                if (pad.Shape == ThPadElement.ThPadShape.Circle) {

                    int size =
                        layer.Id.Side == BoardSide.Top ? pad.TopSize :
                        layer.Id.Side == BoardSide.Bottom ? pad.BottomSize :
                        pad.InnerSize;
                    int radius = size / 2;

                    Pen pen = GetPen(c, (size - pad.Drill) / 2, PenLineCap.Flat);
                    dc.DrawEllipse(Brushes.Black, pen, center, radius, radius);
                }

                else {

                    Polygon polygon = pad.GetPolygon(layer.Id.Side);

                    Brush brush = GetBrush(c);
                    DrawPolygon(dc, null, brush, polygon);
                    dc.DrawEllipse(Brushes.Black, null, center, pad.Drill / 2, pad.Drill / 2);
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

                WinPoint center = new WinPoint(hole.Position.X, hole.Position.Y);

                WinColor c = WinColor.FromRgb(color.R, color.G, color.B);
                Pen pen = GetPen(c, 0.05 * 1000000.0, PenLineCap.Flat);
                dc.DrawEllipse(Brushes.Black, pen, center, hole.Drill / 2, hole.Drill / 2);
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

                WinColor c = WinColor.FromRgb(color.R, color.G, color.B);
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

                WinColor c = WinColor.FromRgb(color.R, color.G, color.B);
                Pen pen = region.Thickness > 0 ? GetPen(c, region.Thickness, PenLineCap.Round) : null;
                Brush brush = region.Filled ? GetBrush(c) : null;
                DrawPolygon(dc, pen, brush, polygon);
            }
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
    }
}
