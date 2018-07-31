namespace MikroPic.EdaTools.v1.Designer.DrawEditor {

    using MikroPic.EdaTools.v1.Designer.DrawEditor.Infrastructure;
    using MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Clase que genera les visuals de la placa.
    /// </summary>
    /// 
    public sealed class VisualGenerator {

        private class RenderTextDrawer: TextDrawer {

            private readonly StreamGeometryContext ctx;

            public RenderTextDrawer(Font font, StreamGeometryContext ctx) : 
                base(font) {

                this.ctx = ctx;
            }

            protected override void Trace(PointInt position, bool stroke, bool first) {

                Point p = new Point(
                    position.X / 1000000.0,
                    position.Y / 1000000.0);

                if (first)
                    ctx.BeginFigure(p, false, false);
                else
                    ctx.LineTo(p, stroke, true);
            }
        }

        /// <summary>
        /// Clase per visitar la placa i generar les visuals.
        /// </summary>
        /// 
        private sealed class RenderVisitor: ElementVisitor {

            private readonly BrushCache brushCache;
            private readonly PenCache penCache;
            private VisualItem parentVisual;

            /// <summary>
            /// Constructor del objecte.
            /// </summary>
            /// <param name="board">La placa a procesar.</param>
            /// <param name="layer">La capa on aplicar el proces.</param>
            /// <param name="rootVisual">El visual arrel.</param>
            /// 
            public RenderVisitor(Board board, Layer layer, VisualItem rootVisual, PenCache penCache, BrushCache brushCache):
                base(board, layer) {

                parentVisual = rootVisual;
                this.penCache = penCache;
                this.brushCache = brushCache;
            }

            /// <summary>
            /// Visita un objecte 'LiniaElement'
            /// </summary>
            /// <param name="line">L'objecte a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                VisualItem visual = new LineVisual(parentVisual, line);

                using (DrawingContext dc = visual.RenderOpen()) {
                    Color color = GetColor(Layer);
                    Pen pen = CreatePen(color, line.Thickness / 1000000.0, 
                        line.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);
                    dc.DrawLine(pen, line.StartPosition.ToPoint(), line.EndPosition.ToPoint());
                }
            }

            /// <summary>
            /// Visita un objecte ArcElement
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {
                
                VisualItem visual = new ArcVisual(parentVisual, arc);

                using (DrawingContext dc = visual.RenderOpen()) {
                    Color color = GetColor(Layer);
                    Pen pen = CreatePen(color, arc.Thickness / 1000000.0, 
                        arc.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);
                    dc.DrawArc(
                        pen, arc.StartPosition.ToPoint(), arc.EndPosition.ToPoint(),
                        new Size(arc.Radius / 1000000.0, arc.Radius / 1000000.0), arc.Angle.Degrees / 100.0);
                }
            }

            /// <summary>
            /// Visita un objecte 'RectangleElement'
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                VisualItem visual = new RectangleVisual(parentVisual, rectangle);

                using (DrawingContext dc = visual.RenderOpen()) {
                    Color color = GetColor(Layer);
                    Pen pen = rectangle.Thickness == 0 ? null : CreatePen(color, rectangle.Thickness / 1000000.0, PenLineCap.Round);
                    Brush brush = rectangle.Filled ? CreateBrush(color) : null;
                    Rect rect = new Rect(
                        (rectangle.Position.X - rectangle.Size.Width / 2) / 1000000.0,
                        (rectangle.Position.Y - rectangle.Size.Height / 2) / 1000000.0,
                        rectangle.Size.Width / 1000000.0,
                        rectangle.Size.Height / 1000000.0);
                    dc.DrawRoundedRectangle(brush, pen, rect, rectangle.Radius / 1000000.0, rectangle.Radius / 1000000.0);
                }
            }

            /// <summary>
            /// Visita un objecte 'CircleElement'.
            /// </summary>
            /// <param name="circle">L'objecte a visitar.</param>
            /// 
            public override void Visit(CircleElement circle) {
                
                VisualItem visual = new CircleVisual(parentVisual, circle);

                using (DrawingContext dc = visual.RenderOpen()) {
                    Color color = GetColor(Layer);
                    Pen pen = circle.Thickness == 0 ? null : CreatePen(color, circle.Thickness / 1000000.0, PenLineCap.Flat);
                    Brush brush = circle.Filled ? CreateBrush(color) : null;
                    dc.DrawEllipse(brush, pen, circle.Position.ToPoint(), circle.Radius / 1000000.0, circle.Radius / 1000000.0);
                }
            }

            /// <summary>
            /// Visita un objecte 'RegionElement'.
            /// </summary>
            /// <param name="region">L'objecte a visitar.</param>
            /// 
            public override void Visit(RegionElement region) {

                VisualItem visual = new VisualItem();
                AddVisual(visual);

                using (DrawingContext dc = visual.RenderOpen()) {

                    Color color = GetColor(Layer);
                    Pen pen = region.Thickness > 0 ? CreatePen(color, region.Thickness / 1000000.0, PenLineCap.Round) : null;
                    Brush brush = region.Filled ? CreateBrush(color) : null;
                    Polygon polygon = Board.GetRegionPolygon(region, Layer, new Transformation());
                    DrawPolygon(dc, pen, brush, polygon);
                }
            }

            /// <summary>
            /// Visita un objecte 'ViaElement'
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                VisualItem visual = new ViaVisual(parentVisual, via);

                using (DrawingContext dc = visual.RenderOpen()) {

                    Color color = GetColor(Layer);

                    if (via.Shape == ViaElement.ViaShape.Circular) {
                        Pen pen = CreatePen(color, (via.OuterSize - via.Drill) / 2000000.0, PenLineCap.Flat);
                        int radius = (via.OuterSize + via.Drill) / 4;
                        dc.DrawEllipse(Brushes.Black, pen, via.Position.ToPoint(), radius / 1000000.0, radius / 1000000.0);
                    }
                    else {
                        Brush brush = CreateBrush(color);
                        Polygon polygon = via.GetPolygon(Layer.Side);
                        DrawPolygon(dc, null, brush, polygon);
                        if (polygon.Childs.Length == 1) {
                            Brush holeBrush = CreateBrush(Colors.Black);
                            DrawPolygon(dc, null, holeBrush, polygon.Childs[0]);
                        }
                    }
                }
            }

            /// <summary>
            /// Visita un objecte 'SmdPadElement'.
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                VisualItem visual = new SmdPadVisual(parentVisual, pad);

                using (DrawingContext dc = visual.RenderOpen()) {
                    Color color = GetColor(Layer);
                    Brush brush = CreateBrush(color);
                    Polygon polygon = pad.GetPolygon(Layer.Side);
                    DrawPolygon(dc, null, brush, polygon);
                }
            }

            /// <summary>
            /// Visita un objecte 'ThPadElement'.
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                VisualItem visual = new VisualItem();
                AddVisual(visual);

                using (DrawingContext dc = visual.RenderOpen()) {

                    Color color = GetColor(Layer);
                    Brush polygonBrush = CreateBrush(color);
                    Polygon polygon = pad.GetPolygon(Layer.Side);
                    DrawPolygon(dc, null, polygonBrush, polygon);
                    if (polygon.Childs.Length == 1) {
                        Brush holeBrush = CreateBrush(Colors.Black);
                        DrawPolygon(dc, null, holeBrush, polygon.Childs[0]);
                    }

                    dc.PushTransform(new ScaleTransform(1, -1, pad.Position.X / 1000000.0, pad.Position.Y / 1000000.0));

                    Brush textBrush = CreateBrush(Colors.Yellow);
                    FormattedText formattedText = new FormattedText(
                        pad.Name, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                        new Typeface("Arial"), 0.5, textBrush);
                    formattedText.TextAlignment = TextAlignment.Center;

                    Point textPosition = new Point(pad.Position.X / 1000000.0, pad.Position.Y / 1000000.0);
                    dc.DrawText(formattedText, new Point(textPosition.X, textPosition.Y - formattedText.Height / 2));

                    dc.Pop();
                }
            }

            /// <summary>
            /// Visita un objecte 'HoleElement'
            /// </summary>
            /// <param name="hole">L'objecte a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                VisualItem visual = new VisualItem();
                AddVisual(visual);

                using (DrawingContext dc = visual.RenderOpen()) {
                    Color color = GetColor(Layer);
                    Pen pen = CreatePen(color, 0.05, PenLineCap.Flat);
                    Brush brush = CreateBrush(Colors.Black);
                    dc.DrawEllipse(brush, pen, hole.Position.ToPoint(), hole.Drill / 2000000.0, hole.Drill / 2000000.0);
                }
            }

            /// <summary>
            /// Visita un objecte de tipus 'TextElement'
            /// </summary>
            /// <param name="text">L'objwecte a visitar.</param>
            /// 
            public override void Visit(TextElement text) {
                
                VisualItem visual = new VisualItem();
                AddVisual(visual);

                using (DrawingContext dc = visual.RenderOpen()) {

                    Color color = GetColor(Layer);
                    Pen pen = CreatePen(color, text.Thickness / 1000000.0, PenLineCap.Round);

                    PartAttributeAdapter paa = new PartAttributeAdapter(Part, text);
                    Point position = paa.Position.ToPoint();
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
            /// Visuta un objecte de tipus 'Part'
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(Part part) {

                VisualItem visual = new VisualItem();
                AddVisual(visual);

                visual.Transform = GetTransform(part);

                VisualItem saveVisual = parentVisual;
                parentVisual = visual;
                try {
                    base.Visit(part);
                }
                finally {
                    parentVisual = saveVisual;
                }
            }

            /// <summary>
            /// Afegeix la visual al seu pare
            /// </summary>
            /// <param name="visual">La visual a afeigir</param>
            /// 
            private void AddVisual(VisualItem visual) {

                parentVisual.Children.Add(visual);
            }

            private Pen CreatePen(Color color, double thickness, PenLineCap lineCap) {

                Brush brush = brushCache.GetBrush(color);
                return penCache.GetPen(brush, thickness, lineCap);
            }

            private Brush CreateBrush(Color color) {

                return brushCache.GetBrush(color);
            }

            /// <summary>
            /// Obte el color pur de la capa.
            /// </summary>
            /// <param name="layer">La capa.</param>
            /// <returns>El color.</returns>
            /// 
            private static Color GetColor(Layer layer) {

                Color color = layer.Color;
                return Color.FromRgb(color.R, color.G, color.B);
            }

            /// <summary>
            /// Obte la transformacio d'un component
            /// </summary>
            /// <param name="part">El component.</param>
            /// <returns>La transformacio.</returns>
            /// 
            private static Transform GetTransform(Part part) {

                Point position = new Point(part.Position.X / 1000000.0, part.Position.Y / 1000000.0);
                double angle = part.Rotation.Degrees / 100.0;

                Matrix m = new Matrix();
                m.Translate(position.X, position.Y);
                m.RotateAt(angle, position.X, position.Y);

                Transform transform = new MatrixTransform(m);
                transform.Freeze();

                return transform;
            }

            /// <summary>
            /// Dibuixa un poligon.
            /// </summary>
            /// <param name="dc">El contexte de dibuix.</param>
            /// <param name="brush">El brush.</param>
            /// <param name="pen">El pen.</param>
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
            /// Dibuixa un text
            /// </summary>
            /// <param name="dc">El contexte de dibuix.</param>
            /// <param name="pen">El pen.</param>
            /// <param name="position">Posicio</param>
            /// <param name="align">Aliniacio</param>
            /// <param name="height">Alçada de lletra.</param>
            /// <param name="text">El text a dibuixar.</param>
            /// 
            private static void DrawText(DrawingContext dc, Pen pen, PointInt position, int height, TextAlign align, string text) {

                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open()) {
                    RenderTextDrawer td = new RenderTextDrawer(font, ctx);
                    td.Draw(text, position, align, height);
                }
                geometry.Freeze();

                dc.DrawGeometry(null, pen, geometry);
            }

            /// <summary>
            /// Dibuixa un poligon en una geometria
            /// </summary>
            /// <param name="ctx">Contexte de la geometria.</param>
            /// <param name="polygon">El poligon a dibuixar.</param>
            /// <param name="level">Nivell del poligon.</param>
            /// 
            private static void StreamPolygon(StreamGeometryContext ctx, Polygon polygon, int level) {

                if (polygon.Points != null) {

                    Point p;
                    PointInt[] points = polygon.Points;

                    p = new Point(
                            (double)points[0].X / 1000000.0,
                            (double)points[0].Y / 1000000.0);
                    ctx.BeginFigure(p, true, true);

                    for (int i = 1; i < points.Length; i++) {
                        p = new Point(
                            (double)points[i].X / 1000000.0, 
                            (double)points[i].Y / 1000000.0);
                        ctx.LineTo(p, true, true);
                    }
                }
                if (polygon.Childs != null && (level < 2))
                    for (int i = 0; i < polygon.Childs.Length; i++)
                        StreamPolygon(ctx, polygon.Childs[i], level + 1);
            }
        }

        private readonly Board board;
        private static readonly Font font;

        static VisualGenerator() {

            FontFactory ff = FontFactory.Instance;
            font = ff.GetFont("Standard");
        }

        /// <summary>
        /// Contructor de la clase. 
        /// </summary>
        /// <param name="board">La placa a procesar.</param>
        /// 
        public VisualGenerator(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        /// <summary>
        /// Crea la visual per renderitzar la placa.
        /// </summary>
        /// <returns>El objecte visual arrel de la placa.</returns>
        /// 
        public VisualItem CreateVisual() {

            BrushCache brushCache = new BrushCache();
            PenCache penCache = new PenCache();

            List<string> layerNames = new List<string>();
            layerNames.Add(Layer.BottomNamesName);
            layerNames.Add(Layer.BottomDocumentName);
            layerNames.Add(Layer.BottomGlueName);
            layerNames.Add(Layer.BottomKeepoutName);
            layerNames.Add(Layer.BottomRestrictName);
            layerNames.Add(Layer.BottomPlaceName);
            layerNames.Add(Layer.BottomName);
            layerNames.Add(Layer.ViaRestrictName);
            layerNames.Add(Layer.TopName);
            layerNames.Add(Layer.TopPlaceName);
            layerNames.Add(Layer.TopRestrictName);
            layerNames.Add(Layer.TopKeepoutName);
            layerNames.Add(Layer.TopGlueName);
            layerNames.Add(Layer.TopNamesName);

            layerNames.Add(Layer.PadsName);
            layerNames.Add(Layer.ViasName);
            layerNames.Add(Layer.HolesName);
            layerNames.Add(Layer.TopDocumentName);

            layerNames.Add(Layer.ProfileName);

            VisualItem boardVisual = new VisualItem();

            foreach (string layerName in layerNames) {

                Layer layer = board.GetLayer(layerName, false);
                if ((layer != null) && layer.IsVisible) {

                    VisualItem layerVisual = new VisualItem();
                    boardVisual.Children.Add(layerVisual);
                    layerVisual.Opacity = layer.Color.ScA;

                    RenderVisitor visitor = new RenderVisitor(board, layer, layerVisual, penCache, brushCache);
                    visitor.Run();
                }
            }

            return boardVisual;
        }
    }
}
