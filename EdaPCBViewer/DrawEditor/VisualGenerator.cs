namespace Eda.PCBViewer.DrawEditor {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Linq;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Clase que genera les visuals de la placa.
    /// </summary>
    public sealed class VisualGenerator {

        private class RenderTextDrawer: TextDrawer {

            private readonly StreamGeometryContext ctx;

            public RenderTextDrawer(Font font, StreamGeometryContext ctx) : 
                base(font) {

                this.ctx = ctx;
            }

            protected override void Trace(Point position, bool stroke, bool first) {

                if (first)
                    ctx.BeginFigure(position, false, false);
                else
                    ctx.LineTo(position, stroke, true);
            }
        }

        /// <summary>
        /// Clase per visitar la placa i generar les visuals.
        /// </summary>
        private sealed class RenderVisitor: ElementVisitor {

            private readonly Dictionary<Color, Brush> brushCache = new Dictionary<Color, Brush>();
            private readonly Dictionary<Tuple<Color, double>, Pen> penCache = new Dictionary<Tuple<Color, double>, Pen>();
            private DrawingVisual parentVisual;

            /// <summary>
            /// Constructor del objecte.
            /// </summary>
            /// <param name="board">La placa a procesar.</param>
            /// <param name="layer">La capa on aplicar el proces.</param>
            /// <param name="rootVisual">El visual arrel.</param>
            /// 
            public RenderVisitor(Board board, Layer layer, DrawingVisual rootVisual):
                base(board, layer) {

                parentVisual = rootVisual;
            }

            /// <summary>
            /// Visita un objecte 'LiniaElement'
            /// </summary>
            /// <param name="line">L'objecte a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen()) {

                    if (Part != null)
                        dc.PushTransform(GetTransform(Part));

                    Color color = GetColor(Layer);
                    Brush brush = CreateBrush(color);
                    Polygon polygon = line.GetPolygon(Layer.Side);
                    DrawPolygon(dc, brush, null, polygon);

                    if (Part != null)
                        dc.Pop();
                }

                AddVisual(visual);
            }

            /// <summary>
            /// Visita un objecte ArcElement
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen()) {

                    if (Part != null)
                        dc.PushTransform(GetTransform(Part));

                    Color color = GetColor(Layer);
                    Brush brush = CreateBrush(color);
                    Polygon polygon = arc.GetPolygon(Layer.Side);
                    DrawPolygon(dc, brush, null, polygon);

                    if (Part != null)
                        dc.Pop();
                }

                AddVisual(visual);
            }

            /// <summary>
            /// Visita un objecte 'RectangleElement'
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen()) {

                    if (Part != null)
                        dc.PushTransform(GetTransform(Part));

                    Color color = GetColor(Layer);
                    Brush brush = rectangle.Filled ? CreateBrush(color) : null;
                    Pen pen = rectangle.Filled ? null : CreatePen(color, rectangle.Thickness);
                    Polygon polygon = rectangle.GetPolygon(Layer.Side);
                    DrawPolygon(dc, brush, pen, polygon);

                    if (Part != null)
                        dc.Pop();
                }

                AddVisual(visual);
            }

            /// <summary>
            /// Visita un objecte 'CircleElement'.
            /// </summary>
            /// <param name="circle">L'objecte a visitar.</param>
            /// 
            public override void Visit(CircleElement circle) {

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen()) {

                    if (Part != null)
                        dc.PushTransform(GetTransform(Part));

                    Color color = GetColor(Layer);
                    Brush brush = circle.Filled ? CreateBrush(color) : null;
                    Pen pen = circle.Filled ? null : CreatePen(color, circle.Thickness);
                    Polygon polygon = circle.GetPolygon(Layer.Side);
                    DrawPolygon(dc, brush, pen, polygon);

                    if (Part != null)
                        dc.Pop();
                }

                AddVisual(visual);
            }

            /// <summary>
            /// Visita un objecte 'RegionElement'.
            /// </summary>
            /// <param name="region">L'objecte a visitar.</param>
            /// 
            public override void Visit(RegionElement region) {

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen()) {

                    if (Part != null)
                        dc.PushTransform(GetTransform(Part));

                    Color color = GetColor(Layer);
                    bool isSignalLayer = (Layer.Name == Layer.TopName) || (Layer.Name == Layer.BottomName);
                    Pen pen = isSignalLayer ? CreatePen(color, region.Thickness) : null;
                    Brush brush = CreateBrush(color);
                    Polygon polygon = Board.GetRegionPolygon(region, Layer, 0.15, Matrix.Identity);
                    DrawPolygon(dc, brush, pen, polygon);

                    if (Part != null)
                        dc.Pop();
                }

                AddVisual(visual);
            }

            /// <summary>
            /// Visita un objecte 'ViaElement'
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen()) {

                    if (Part != null)
                        dc.PushTransform(GetTransform(Part));

                    Color color = GetColor(Layer);
                    Brush polygonBrush = CreateBrush(color);
                    Polygon polygon = via.GetPolygon(Layer.Side);
                    DrawPolygon(dc, polygonBrush, null, polygon);

                    List<Polygon> polygonHoles = new List<Polygon>(polygon.Childs);
                    if (polygonHoles.Count == 1) {
                        Brush holeBrush = CreateBrush(Colors.Black);
                        DrawPolygon(dc, holeBrush, null, polygonHoles[0]);
                    }

                    if (Part != null)
                        dc.Pop();
                }

                AddVisual(visual);
            }

            /// <summary>
            /// Visita un objecte 'SmdPadElement'.
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen()) {

                    if (Part != null)
                        dc.PushTransform(GetTransform(Part));

                    Color color = GetColor(Layer);
                    Brush brush = CreateBrush(color);
                    Polygon polygon = pad.GetPolygon(Layer.Side);
                    DrawPolygon(dc, brush, null, polygon);

                    if (Part != null)
                        dc.Pop();
                }

                AddVisual(visual);
            }

            /// <summary>
            /// Visita un objecte 'ThPadElement'.
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen()) {

                    if (Part != null)
                        dc.PushTransform(GetTransform(Part));

                    Color color = GetColor(Layer);
                    Brush polygonBrush = CreateBrush(color);
                    Polygon polygon = pad.GetPolygon(Layer.Side);
                    DrawPolygon(dc, polygonBrush, null, polygon);

                    List<Polygon> polygonHoles = new List<Polygon>(polygon.Childs);
                    if (polygonHoles.Count == 1) {
                        Brush holeBrush = CreateBrush(Colors.Black);
                        DrawPolygon(dc, holeBrush, null, polygonHoles[0]);
                    }

                    dc.PushTransform(new ScaleTransform(1, -1, pad.Position.X, pad.Position.Y));
                    Brush textBrush = CreateBrush(Colors.Yellow);
                    FormattedText formattedText = new FormattedText(
                        pad.Name, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                        new Typeface("Arial"), 0.5, textBrush);
                    formattedText.TextAlignment = TextAlignment.Center;
                    dc.DrawText(formattedText, new Point(pad.Position.X, pad.Position.Y - formattedText.Height / 2));
                    dc.Pop();

                    if (Part != null)
                        dc.Pop();
                }

                AddVisual(visual);
            }

            /// <summary>
            /// Visita un objecte 'HoleElement'
            /// </summary>
            /// <param name="hole">L'objecte a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen()) {

                    if (Part != null)
                        dc.PushTransform(GetTransform(Part));

                    Color color = GetColor(Layer);
                    Pen pen = CreatePen(color, 0.05);
                    Brush brush = CreateBrush(Colors.Black);
                    DrawPolygon(dc, brush, pen, hole.GetPolygon(Layer.Side));

                    if (Part != null)
                        dc.Pop();
                }

                AddVisual(visual);
            }

            /// <summary>
            /// Visita un objecte de tipus 'TextElement'
            /// </summary>
            /// <param name="text">L'objwecte a visitar.</param>
            /// 
            public override void Visit(TextElement text) {

                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen()) {

                    if (Part != null)
                        dc.PushTransform(GetTransform(Part));

                    Color color = GetColor(Layer);
                    Pen pen = CreatePen(color, text.Thickness);

                    Point position = text.Position;
                    Angle rotation = Angle.Zero;
                    TextAlign align = TextAlign.TopLeft;
                    string value = text.Value;

                    if (Part != null && value.StartsWith(">")) {
                        PartAttribute pa = Part.GetAttribute(value.Substring(1));
                        if (pa != null) {
                            value = value.Replace(value, pa.Value);
                            position = pa.Position;
                            rotation = pa.Rotation;
                            align = pa.Align;
                        }
                    }

                    Matrix m = new Matrix();
                    m.Translate(position.X, position.Y);
                    m.Rotate(rotation.Degrees);
                    dc.PushTransform(new MatrixTransform(m));

                    DrawText(dc, pen, new Point(0, 0), text.Height, align, value);
                    dc.DrawEllipse(Brushes.YellowGreen, null, new Point(0, 0), 0.15, 0.15);

                    dc.Pop();

                    if (Part != null)
                        dc.Pop();
                }

                AddVisual(visual);
            }

            /// <summary>
            /// Visuta un objecte de tipus 'Part'
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(Part part) {

                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);

                DrawingVisual saveVisual = parentVisual;
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
            private void AddVisual(DrawingVisual visual) {

                parentVisual.Children.Add(visual);
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
            /// Obte la transformacio d'un component.
            /// </summary>
            /// <param name="part">El component.</param>
            /// <returns>La transformacio.</returns>
            /// 
            private static Transform GetTransform(Part part) {

                Transform transaform = new MatrixTransform(part.Transformation);
                transaform.Freeze();

                return transaform;
            }

            /// <summary>
            /// Crea un pen
            /// </summary>
            /// <param name="color">Color.</param>
            /// <param name="thickness">Amplada de linia.</param>
            /// <returns>El pen.</returns>
            /// 
            private Pen CreatePen(Color color, double thickness) {

                Pen pen;

                if (!penCache.TryGetValue(new Tuple<Color, double>(color, thickness), out pen)) {

                    Brush brush = CreateBrush(color);

                    pen = new Pen(brush, thickness);
                    pen.StartLineCap = PenLineCap.Round;
                    pen.EndLineCap = PenLineCap.Round;
                    pen.Freeze();

                    penCache.Add(new Tuple<Color, double>(color, thickness), pen);
                }

                return pen;
            }

            /// <summary>
            /// Crea un brush
            /// </summary>
            /// <param name="color">Color.</param>
            /// <returns>El brush.</returns>
            /// 
            private Brush CreateBrush(Color color) {

                Brush brush;

                if (!brushCache.TryGetValue(color, out brush)) {

                    brush = new SolidColorBrush(color);
                    brush.Freeze();

                    brushCache.Add(color, brush);
                }

                return brush;
            }

            /// <summary>
            /// Dibuixa un poligon.
            /// </summary>
            /// <param name="dc">El contexte de dibuix.</param>
            /// <param name="brush">El brush.</param>
            /// <param name="pen">El pen.</param>
            /// <param name="polygon">El poligon a dibuixar.</param>
            /// 
            private static void DrawPolygon(DrawingContext dc, Brush brush, Pen pen, Polygon polygon) {

                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open())
                    StreamPolygon(ctx, polygon, polygon.HasPoints ? 1 : 0);
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
            private static void DrawText(DrawingContext dc, Pen pen, Point position, double height, TextAlign align, string text) {

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

                if (polygon.HasPoints) {
                    List<Point> points = new List<Point>(polygon.Points);
                    ctx.BeginFigure(points[0], true, true);
                    ctx.PolyLineTo(points.Skip(1).ToList<Point>(), true, true);
                }
                if (polygon.HasChilds && (level < 2))
                    foreach (Polygon child in polygon.Childs)
                        StreamPolygon(ctx, child, level + 1);
            }

            /// <summary>
            /// Dibuixa un text en una geometria
            /// </summary>
            /// <param name="ctx">Contexte de dibuix.</param>
            /// <param name="text">El text a dibuixar.</param>
            /// 
            private static void StreamText(StreamGeometryContext ctx, double height, string text) {

                double delta = 0;
                double scale = height / font.Height; 
                foreach (char ch in text) {
                    Glyph glyph = font.GetGlyph(ch);
                    if (glyph != null) {
                        bool first = true;
                        foreach (GlyphTrace trace in glyph.Traces) {
                            Point p = new Point(scale *(trace.Position.X + delta), scale * trace.Position.Y);
                            if (first) {
                                first = false;
                                ctx.BeginFigure(p, false, false);
                            }
                            else
                                ctx.LineTo(p, trace.Stroke, true);
                        }
                        delta += glyph.Advance;
                    }
                }
            }
        }

        private readonly Board board;
        private static readonly Font font;

        static VisualGenerator() {

            font = Font.Load(@"..\..\..\Data\font.xml");
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
        /// Genera totes les visuals per representasr la placa.
        /// </summary>
        /// <returns>Un enumerador de les visuals creades.</returns>
        /// 
        public Visual CreateVisuals() {

            List<string> layerNames = new List<string>();
            layerNames.Add(Layer.BottomNamesName);
            // layerIds.Add(LayerId.BottomValues);
            layerNames.Add(Layer.BottomDocumentName);
            //layerIds.Add(LayerId.BottomCream);
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
            //layerIds.Add(LayerId.TopCream);
            layerNames.Add(Layer.TopDocumentName);
            //layerIds.Add(LayerId.TopValues);
            layerNames.Add(Layer.TopNamesName);
            layerNames.Add(Layer.PadsName);
            layerNames.Add(Layer.ViasName);
            layerNames.Add(Layer.HolesName);
            layerNames.Add(Layer.TopDocumentName);
            layerNames.Add(Layer.ProfileName);

            DrawingVisual boardVisual = new DrawingVisual();
            foreach (string layerName in layerNames) {

                Layer layer = board.GetLayer(layerName);
                if (layer.IsVisible) {

                    DrawingVisual layerVisual = new DrawingVisual();
                    boardVisual.Children.Add(layerVisual);
                    layerVisual.Opacity = layer.Color.ScA;

                    RenderVisitor visitor = new RenderVisitor(board, layer, layerVisual);
                    visitor.Run();
                }
            }

            return boardVisual;
        }
    }
}
