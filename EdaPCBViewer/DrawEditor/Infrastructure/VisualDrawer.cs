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

    internal sealed class VisualDrawer {

        private static readonly TextDrawer td;
        private readonly PenCache penCache;
        private readonly BrushCache brushCache;

        /// <summary>
        /// Constructor estatic del objecte.
        /// </summary>
        /// 
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

                Color color = GetLayerColor(layer);

                Pen pen = GetPen(color, line.Thickness, line.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);

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
        public void DrawArcElement(DrawingVisual visual, Layer layer, ArcElement arc) {

            using (DrawingContext dc = visual.RenderOpen()) {

                Color color = GetLayerColor(layer);

                Pen pen = GetPen(color, arc.Thickness, arc.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);

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
        public void DrawRectangleElement(DrawingVisual visual, Layer layer, RectangleElement rectangle) {

            using (DrawingContext dc = visual.RenderOpen()) {

                Color color = GetLayerColor(layer);

                Pen pen = rectangle.Thickness == 0 ? null : GetPen(color, rectangle.Thickness, PenLineCap.Round);
                Brush brush = rectangle.Filled ? GetBrush(color) : null;

                DrawRectangle(dc, pen, brush, rectangle.Position, rectangle.Size, rectangle.Radius, rectangle.Rotation);
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

                Color color = GetLayerColor(layer);

                Pen pen = circle.Thickness == 0 ? null : GetPen(color, circle.Thickness, PenLineCap.Flat);
                Brush brush = circle.Filled ? GetBrush(color) : null;

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
        public void DrawViaElement(DrawingVisual visual, Layer layer, ViaElement via) {

            using (DrawingContext dc = visual.RenderOpen()) {

                Color color = GetLayerColor(layer);
                int size = layer.Side == BoardSide.Inner ? via.InnerSize : via.OuterSize;

                if (via.Shape == ViaElement.ViaShape.Circular) {
                    Pen pen = GetPen(color, (size - via.Drill) / 2, PenLineCap.Flat);
                    DrawCircle(dc, pen, Brushes.Black, via.Position, (size + via.Drill) / 4);
                }
                else {
                    Brush brush = GetBrush(color);
                    Polygon polygon = via.GetPolygon(layer.Side);
                    DrawPolygon(dc, null, brush, polygon);
                    if (polygon.Childs.Length == 1) {
                        Brush holeBrush = GetBrush(Colors.Black);
                        DrawPolygon(dc, null, holeBrush, polygon.Childs[0]);
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
        public void DrawSmdPadElement(DrawingVisual visual, Layer layer, SmdPadElement pad) {

            using (DrawingContext dc = visual.RenderOpen()) {

                Color color = GetLayerColor(layer);

                Brush brush = GetBrush(color);

                DrawRectangle(dc, null, brush, pad.Position, pad.Size, pad.Radius, pad.Rotation);
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

                Color color = GetLayerColor(layer);

                if (pad.Shape == ThPadElement.ThPadShape.Circular) {

                    int size =
                        layer.Side == BoardSide.Top ? pad.TopSize :
                        layer.Side == BoardSide.Bottom ? pad.BottomSize :
                        pad.InnerSize;

                    Pen pen = GetPen(color, (size - pad.Drill) / 2, PenLineCap.Flat);

                    DrawCircle(dc, pen, Brushes.Black, pad.Position, size / 2);
                }

                else {

                    Brush polygonBrush = GetBrush(color);

                    Polygon polygon = pad.GetPolygon(layer.Side);

                    DrawPolygon(dc, null, polygonBrush, polygon);
                    if (polygon.Childs.Length == 1) {
                        Brush holeBrush = GetBrush(Colors.Black);
                        DrawPolygon(dc, null, holeBrush, polygon.Childs[0]);
                    }

                    dc.PushTransform(new ScaleTransform(1, -1, pad.Position.X, pad.Position.Y));

                    Brush textBrush = GetBrush(Colors.Yellow);
                    FormattedText formattedText = new FormattedText(
                        pad.Name, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                        new Typeface("Arial"), 0.5, textBrush);
                    formattedText.TextAlignment = TextAlignment.Center;

                    Point textPosition = new Point(pad.Position.X, pad.Position.Y);
                    dc.DrawText(formattedText, new Point(textPosition.X, textPosition.Y - formattedText.Height / 2));

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

                Color color = GetLayerColor(layer);

                Pen pen = GetPen(color, 0.05 * 1000000.0, PenLineCap.Flat);
                Brush brush = GetBrush(Colors.Black);

                DrawCircle(dc, pen, brush, hole.Position, hole.Drill / 2);
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

                Color color = GetLayerColor(layer);

                Pen pen = GetPen(color, text.Thickness, PenLineCap.Round);

                PartAttributeAdapter paa = new PartAttributeAdapter(part, text);
                Point position = new Point(paa.Position.X, paa.Position.Y);
                Angle rotation = paa.Rotation;

                Matrix m = new Matrix();
                m.Translate(position.X, position.Y);
                m.RotateAt(rotation.Degrees / 100.0, position.X, position.Y);
                dc.PushTransform(new MatrixTransform(m));

                DrawText(dc, pen, new PointInt(0, 0), paa.Height, paa.Align, paa.Value);
                dc.DrawEllipse(Brushes.YellowGreen, null, new Point(0, 0), 0.15, 0.15);

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

                Color color = GetLayerColor(layer);

                Pen pen = region.Thickness > 0 ? GetPen(color, region.Thickness, PenLineCap.Round) : null;
                Brush brush = region.Filled ? GetBrush(color) : null;

                Polygon polygon = layer.Function == LayerFunction.Signal ?
                    board.GetRegionPolygon(region, layer, new Transformation()) :
                    region.GetPolygon(layer.Side);

                DrawPolygon(dc, pen, brush, polygon);
            }
        }

        /// <summary>
        /// Obte el color d'una capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <returns>El color.</returns>
        /// 
        private static Color GetLayerColor(Layer layer) {

            Color color = layer.Color;
            return Color.FromRgb(color.R, color.G, color.B);
        }

        /// <summary>
        /// Obte un pen.
        /// </summary>
        /// <param name="color">Color del pen.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="lineCap">Forma dels extrems de linia.</param>
        /// <returns>El pen.</returns>
        /// 
        private Pen GetPen(Color color, double thickness, PenLineCap lineCap) {

            Brush brush = GetBrush(color);
            return penCache.GetPen(brush, thickness, lineCap);
        }

        /// <summary>
        /// Obte un brush.
        /// </summary>
        /// <param name="color">El color.</param>
        /// <returns>El brush.</returns>
        /// 
        private Brush GetBrush(Color color) {

            return brushCache.GetBrush(color);
        }

        /// <summary>
        /// Primitiva de dibuix de linies
        /// </summary>
        /// <param name="dc">Context de dibuix.</param>
        /// <param name="pen">El pen per dibuixar.</param>
        /// <param name="start">Punt inicial.</param>
        /// <param name="end">Punt final.</param>
        /// 
        private static void DrawLine(DrawingContext dc, Pen pen, PointInt start, PointInt end) {

            dc.DrawLine(
                pen, 
                new Point(start.X, start.Y), 
                new Point(end.X, end.Y));
        }

        /// <summary>
        /// Primitiva de dibuix d'arcs.
        /// </summary>
        /// <param name="dc">Context de dibuix.</param>
        /// <param name="pen">El pen per dibuixar.</param>
        /// <param name="start">Punt inicial.</param>
        /// <param name="end">Punt final.</param>
        /// <param name="radius">Radi de corvatura.</param>
        /// <param name="angle">Angle de l'arc.</param>
        /// 
        private static void DrawArc(DrawingContext dc, Pen pen, PointInt start, PointInt end, int radius, Angle angle) {

            StreamGeometry g = new StreamGeometry();
            using (StreamGeometryContext gc = g.Open()) {
                gc.BeginFigure(
                    new Point(start.X, start.Y), 
                    false, 
                    false);
                gc.ArcTo(
                    new Point(end.X, end.Y),
                    new Size(radius, radius), 
                    angle.Degrees / 100, 
                    Math.Abs(angle.Degrees) > 18000.0, 
                    angle.Degrees < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise, 
                    true, 
                    false);
            }
            g.Freeze();

            dc.DrawGeometry(null, pen, g);
        }

        /// <summary>
        /// Primitiva de dibuix de rectangles.
        /// </summary>
        /// <param name="dc">Context de dibuix.</param>
        /// <param name="pen">El pen per dibuixar.</param>
        /// <param name="brush">El brush per dibuixar.</param>
        /// <param name="centroid">En centroid.</param>
        /// <param name="size">El tamany.</param>
        /// <param name="radius">Radi de corvatura de les cantonades.</param>
        /// <param name="amgle">Angle de rotacio.</param>
        /// 
        private static void  DrawRectangle(DrawingContext dc, Pen pen, Brush brush, PointInt centroid, SizeInt size, int radius, Angle amgle) {

            dc.DrawRoundedRectangle(
                brush, 
                pen,
                new Rect(centroid.X - (size.Width / 2), centroid.Y - (size.Height / 2), size.Width, size.Height),
                radius, 
                radius);
        }

        /// <summary>
        /// Primitiva de dibuix de cercles.
        /// </summary>
        /// <param name="dc">En context de dibuix.</param>
        /// <param name="pen">El pen per dibuixar.</param>
        /// <param name="brush">El brush per dibuixar.</param>
        /// <param name="center">El centre.</param>
        /// <param name="radius">El radi.</param>
        /// 
        private static void DrawCircle(DrawingContext dc, Pen pen, Brush brush, PointInt center, int radius) {

            dc.DrawEllipse(
                brush, 
                pen, 
                new Point(center.X, center.Y), 
                radius, 
                radius);
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
        /// <param name="position">Posicio</param>
        /// <param name="align">Aliniacio</param>
        /// <param name="height">Alçada de lletra.</param>
        /// <param name="text">El text a dibuixar.</param>
        /// 
        private static void DrawText(DrawingContext dc, Pen pen, PointInt position, int height, TextAlign align, string text) {

            IEnumerable<GlyphTrace> glyphTraces = td.Draw(text, position, align, height);

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open()) {

                bool first = true;
                foreach (GlyphTrace glyphTrace in glyphTraces) {
                    Point p = new Point(glyphTrace.Position.X, glyphTrace.Position.Y);
                    if (first) {
                        ctx.BeginFigure(p, false, false);
                        first = false;
                    }
                    else
                        ctx.LineTo(p, glyphTrace.Stroke, true);
                }
            }
            geometry.Freeze();

            dc.DrawGeometry(null, pen, geometry);
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

                Point p;
                PointInt[] points = polygon.Points;

                p = new Point(points[0].X, points[0].Y);
                ctx.BeginFigure(p, true, true);

                for (int i = 1; i < points.Length; i++) {
                    p = new Point(points[i].X, points[i].Y);
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
