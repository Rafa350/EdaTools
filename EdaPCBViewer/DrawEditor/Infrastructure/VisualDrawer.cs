namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Infrastructure {

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
    using SysColor = System.Windows.Media.Color;
    using SysSize = System.Windows.Size;
    using SysPoint = System.Windows.Point;
    using SysRect = System.Windows.Rect;

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
        public void DrawLineElement(DrawingVisual visual, Layer layer, LineElement line) {

            using (DrawingContext dc = visual.RenderOpen()) {

                SysPoint start = new SysPoint(line.StartPosition.X, line.StartPosition.Y);
                SysPoint end = new SysPoint(line.EndPosition.X, line.EndPosition.Y);

                SysColor color = GetLayerColor(layer);
                Pen pen = GetPen(color, line.Thickness, line.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);
                dc.DrawLine(pen, start, end);
            }
        }

        /// <summary>
        /// Dibuixa un arc en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa a dibuixar element.</param>
        /// <param name="line">El element arc.</param>
        /// 
        public void DrawArcElement(DrawingVisual visual, Layer layer, ArcElement arc) {

            using (DrawingContext dc = visual.RenderOpen()) {

                SysPoint start = new SysPoint(arc.StartPosition.X, arc.StartPosition.Y);
                SysPoint end = new SysPoint(arc.EndPosition.X, arc.EndPosition.Y);
                SysSize size = new SysSize(arc.Radius, arc.Radius);
                double angle = arc.Angle.Degrees;

                SysColor color = GetLayerColor(layer);
                Pen pen = GetPen(color, arc.Thickness, arc.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);

                StreamGeometry g = new StreamGeometry();
                using (StreamGeometryContext gc = g.Open()) {
                    gc.BeginFigure(start, false, false);
                    gc.ArcTo(end, size, angle / 100, Math.Abs(angle) > 18000.0,
                        angle < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise, true, false);
                }
                g.Freeze();

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
        public void DrawRectangleElement(DrawingVisual visual, Layer layer, RectangleElement rectangle) {

            using (DrawingContext dc = visual.RenderOpen()) {

                SysRect rect = new SysRect(
                    rectangle.Position.X - (rectangle.Size.Width / 2),
                    rectangle.Position.Y - (rectangle.Size.Height / 2),
                    rectangle.Size.Width,
                    rectangle.Size.Height);

                SysColor color = GetLayerColor(layer);
                Pen pen = rectangle.Thickness == 0 ? null : GetPen(color, rectangle.Thickness, PenLineCap.Round);
                Brush brush = rectangle.Filled ? GetBrush(color) : null;
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
        public void DrawCircleElement(DrawingVisual visual, Layer layer, CircleElement circle) {

            using (DrawingContext dc = visual.RenderOpen()) {

                SysPoint center = new SysPoint(circle.Position.X, circle.Position.Y);
                int radius = circle.Radius;

                SysColor color = GetLayerColor(layer);
                Pen pen = circle.Thickness == 0 ? null : GetPen(color, circle.Thickness, PenLineCap.Flat);
                Brush brush = circle.Filled ? GetBrush(color) : null;
                dc.DrawEllipse(brush, pen, center, radius, radius);
            }
        }

        /// <summary>
        /// Dibuixa una via en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa a dibuixar element.</param>
        /// <param name="via">El element via.</param>
        /// 
        public void DrawViaElement(DrawingVisual visual, Layer layer, ViaElement via) {

            using (DrawingContext dc = visual.RenderOpen()) {

                SysColor color = GetLayerColor(layer);

                if (via.Shape == ViaElement.ViaShape.Circle) {

                    int size = layer.Side == BoardSide.Inner ? via.InnerSize : via.OuterSize;
                    int radius = (size + via.Drill) / 4;
                    SysPoint center = new SysPoint(via.Position.X, via.Position.Y);

                    Pen pen = GetPen(color, (size - via.Drill) / 2, PenLineCap.Flat);
                    dc.DrawEllipse(Brushes.Black, pen, center, radius, radius);
                }

                else {

                    Polygon polygon = via.GetPolygon(layer.Side);

                    Brush brush = GetBrush(color);
                    DrawPolygon(dc, null, brush, polygon);
                    if (polygon.Childs.Length == 1)
                        DrawPolygon(dc, null, Brushes.Black, polygon.Childs[0]);
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
        public void DrawSmdPadElement(DrawingVisual visual, Layer layer, SmdPadElement pad) {

            using (DrawingContext dc = visual.RenderOpen()) {

                SysRect rect = new SysRect(
                    pad.Position.X - (pad.Size.Width / 2),
                    pad.Position.Y - (pad.Size.Height / 2),
                    pad.Size.Width,
                    pad.Size.Height);
                int radius = pad.Radius;

                SysColor color = GetLayerColor(layer);
                Brush brush = GetBrush(color);
                dc.DrawRoundedRectangle(brush, null, rect, radius, radius);
            }
        }

        /// <summary>
        /// Dibuixa un pad throught hole en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="pad">L'element pad.</param>
        /// 
        public void DrawThPadElement(DrawingVisual visual, Layer layer, ThPadElement pad) {

            using (DrawingContext dc = visual.RenderOpen()) {

                SysColor color = GetLayerColor(layer);

                if (pad.Shape == ThPadElement.ThPadShape.Circle) {

                    int size =
                        layer.Side == BoardSide.Top ? pad.TopSize :
                        layer.Side == BoardSide.Bottom ? pad.BottomSize :
                        pad.InnerSize;
                    int radius = size / 2;
                    SysPoint center = new SysPoint(pad.Position.X, pad.Position.Y);

                    Pen pen = GetPen(color, (size - pad.Drill) / 2, PenLineCap.Flat);
                    dc.DrawEllipse(Brushes.Black, pen, center, radius, radius);
                }

                else {

                    Polygon polygon = pad.GetPolygon(layer.Side);

                    Brush polygonBrush = GetBrush(color);
                    DrawPolygon(dc, null, polygonBrush, polygon);
                    if (polygon.Childs.Length == 1) 
                        DrawPolygon(dc, null, Brushes.Black, polygon.Childs[0]);

                    dc.PushTransform(new ScaleTransform(1, -1, pad.Position.X, pad.Position.Y));

                    Brush textBrush = GetBrush(Colors.Yellow);
                    FormattedText formattedText = new FormattedText(
                        pad.Name, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                        new Typeface("Arial"), 0.5, textBrush);
                    formattedText.TextAlignment = TextAlignment.Center;

                    SysPoint textPosition = new SysPoint(pad.Position.X, pad.Position.Y);
                    dc.DrawText(formattedText, new SysPoint(textPosition.X, textPosition.Y - formattedText.Height / 2));

                    dc.Pop();
                }
            }
        }
        

        /// <summary>
        /// Dibuixa un hole en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="pad">L'element hole.</param>
        /// 
        public void DrawHoleElement(DrawingVisual visual, Layer layer, HoleElement hole) {

            using (DrawingContext dc = visual.RenderOpen()) {

                int radius = hole.Drill / 2;
                SysPoint center = new SysPoint(hole.Position.X, hole.Position.Y);

                SysColor color = GetLayerColor(layer);
                Pen pen = GetPen(color, 0.05 * 1000000.0, PenLineCap.Flat);
                dc.DrawEllipse(Brushes.Black, pen, center, radius, radius);
            }
        }

        /// <summary>
        /// Dibuixa un text en un visual.
        /// </summary>
        /// <param name="visual">L'objecte visual.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="pad">L'element text.</param>
        /// 
        public void DrawTextElement(DrawingVisual visual, Layer layer, Part part, TextElement text) {

            using (DrawingContext dc = visual.RenderOpen()) {

                PartAttributeAdapter paa = new PartAttributeAdapter(part, text);
                SysPoint position = new SysPoint(paa.Position.X, paa.Position.Y);
                Angle rotation = paa.Rotation;
                IEnumerable<GlyphTrace> glyphTraces = td.Draw(paa.Value, new Point(0, 0), paa.HorizontalAlign, paa.VerticalAlign, paa.Height);

                Matrix m = new Matrix();
                m.Translate(position.X, position.Y);
                m.RotateAt(rotation.Degrees / 100.0, position.X, position.Y);
                dc.PushTransform(new MatrixTransform(m));

                SysColor color = GetLayerColor(layer);
                Pen pen = GetPen(color, text.Thickness, PenLineCap.Round);
                DrawGlyphs(dc, pen, null, glyphTraces);
                dc.DrawEllipse(Brushes.YellowGreen, null, new SysPoint(0, 0), 0.15 * 1000000.0, 0.15 * 1000000.0);

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
        public void DrawRegionElement(DrawingVisual visual, Layer layer, Board board, RegionElement region) {

            using (DrawingContext dc = visual.RenderOpen()) {

                Polygon polygon = layer.Function == LayerFunction.Signal ?
                    board.GetRegionPolygon(region, layer, new Transformation()) :
                    region.GetPolygon(layer.Side);

                SysColor color = GetLayerColor(layer);
                Pen pen = region.Thickness > 0 ? GetPen(color, region.Thickness, PenLineCap.Round) : null;
                Brush brush = region.Filled ? GetBrush(color) : null;
                DrawPolygon(dc, pen, brush, polygon);
            }
        }

        /// <summary>
        /// Obte el color d'una capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <returns>El color.</returns>
        /// 
        private static SysColor GetLayerColor(Layer layer) {

            Color color = layer.Color;
            return SysColor.FromRgb(color.R, color.G, color.B);
        }

        /// <summary>
        /// Obte un pen.
        /// </summary>
        /// <param name="color">Color del pen.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="lineCap">Forma dels extrems de linia.</param>
        /// <returns>El pen.</returns>
        /// 
        private Pen GetPen(SysColor color, double thickness, PenLineCap lineCap) {

            Brush brush = GetBrush(color);
            return penCache.GetPen(brush, thickness, lineCap);
        }

        /// <summary>
        /// Obte un brush.
        /// </summary>
        /// <param name="color">El color.</param>
        /// <returns>El brush.</returns>
        /// 
        private Brush GetBrush(SysColor color) {

            return brushCache.GetBrush(color);
        }

        /// <summary>
        /// Primitiva de dibuix de poligonns.
        /// </summary>
        /// <param name="dc">El contexte de dibuix.</param>
        /// <param name="pen">El pen per dinuixar.</param>
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
                    SysPoint p = new SysPoint(glyphTrace.Position.X, glyphTrace.Position.Y);
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

                SysPoint p;
                v1.Geometry.Point[] points = polygon.Points;

                p = new SysPoint(points[0].X, points[0].Y);
                ctx.BeginFigure(p, true, true);

                for (int i = 1; i < points.Length; i++) {
                    p = new SysPoint(points[i].X, points[i].Y);
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
