namespace Eda.PCBViewer.DrawEditor {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public sealed class VisualGenerator {

        private sealed class RenderVisitor: BoardVisitor {

            private readonly Board board;
            private readonly Layer layer;
            private readonly IList<Visual> visuals;
            private MatrixTransform localTransform = null;

            /// <summary>
            /// Constructor del objecte.
            /// </summary>
            /// <param name="board">La placa a procesar.</param>
            /// <param name="layer">La capa on aplicar el proces.</param>
            /// <param name="visuals">Llista de visuals.</param>
            /// 
            public RenderVisitor(Board board, Layer layer, IList<Visual> visuals) {

                this.board = board;
                this.layer = layer;
                this.visuals = visuals;
            }

            /// <summary>
            /// Visita un objecte Part.
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(Part part) {

                localTransform = new MatrixTransform(part.Transformation);
                base.Visit(part);
                localTransform = null;
            }

            /// <summary>
            /// Visita un objecte LiniaElement
            /// </summary>
            /// <param name="line">L'element a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                if (board.IsOnLayer(line, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = CreateBrush(layer.Color);
                        DrawPolygon(dc, brush, null, line.GetPolygon());

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            /// <summary>
            /// Visita un objecte ArcElement
            /// </summary>
            /// <param name="arc">L'element a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                if (board.IsOnLayer(arc, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = CreateBrush(layer.Color);
                        DrawPolygon(dc, brush, null, arc.GetPolygon());

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            /// <summary>
            /// Visita un objecte RectangleElement
            /// </summary>
            /// <param name="rectangle">L'element a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                if (board.IsOnLayer(rectangle, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = CreateBrush(layer.Color);
                        DrawPolygon(dc, brush, null, rectangle.GetPolygon());

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            /// <summary>
            /// Visita un objecte CircleElement.
            /// </summary>
            /// <param name="circle">L'element a visitar.</param>
            /// 
            public override void Visit(CircleElement circle) {

                if (board.IsOnLayer(circle, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = CreateBrush(layer.Color);
                        DrawPolygon(dc, brush, null, circle.GetPolygon());

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            public override void Visit(RegionElement region) {

                if (board.IsOnLayer(region, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Polygon polygon = board.GetRegionPolygon(region, layer, 0.15, Matrix.Identity);


                        Pen pen = null;
                        Brush brush = null;

                        if ((layer.Id == LayerId.Top) || (layer.Id == LayerId.Bottom)) {
                            brush = CreateBrush(layer.Color);
                            pen = CreatePen(layer.Color, 0.2);
                        }
                        else
                            brush = new SolidColorBrush(layer.Color);
                        DrawPolygon(dc, brush, pen, polygon);

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            /// <summary>
            /// Visita un element Via
            /// </summary>
            /// <param name="via">l'element a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                if ((layer.Id == LayerId.Vias) && board.IsOnLayer(via, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Polygon polygon = via.GetPolygon();
                        List<Polygon> polygonHoles = new List<Polygon>(polygon.Childs);

                        Brush polygonBrush = CreateBrush(layer.Color);
                        DrawPolygon(dc, polygonBrush, null, polygon);

                        if (polygonHoles.Count == 1) {
                            Brush holeBrush = CreateBrush(Colors.Black);
                            DrawPolygon(dc, holeBrush, null, polygonHoles[0]);
                        }

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            /// <summary>
            /// Visita un element de tipus SmdPad.
            /// </summary>
            /// <param name="pad">L'element a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                if (board.IsOnLayer(pad, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = CreateBrush(layer.Color);
                        DrawPolygon(dc, brush, null, pad.GetPolygon());

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            /// <summary>
            /// Visita un element de tipus ThPad.
            /// </summary>
            /// <param name="pad">L'element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

               if ((layer.Id == LayerId.Pads) && board.IsOnLayer(pad, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush polygonBrush = CreateBrush(layer.Color);
                        Polygon polygon = pad.GetPolygon();
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

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            /// <summary>
            /// Visita un element Hole
            /// </summary>
            /// <param name="hole">L'element a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                if ((layer.Id == LayerId.Holes) && board.IsOnLayer(hole, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Pen pen = CreatePen(layer.Color, 0.05);
                        Brush brush = CreateBrush(Colors.Black);
                        DrawPolygon(dc, brush, pen, hole.GetPolygon());

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            /// <summary>
            /// Crea un pen
            /// </summary>
            /// <param name="color">Color.</param>
            /// <param name="thickness">Amplada de linia.</param>
            /// <param name="opacity">Opacitat.</param>
            /// <returns>El pen.</returns>
            /// 
            private static Pen CreatePen(Color color, double thickness, double opacity = 1) {

                Brush brush = new SolidColorBrush(color);
                brush.Opacity = opacity;
                brush.Freeze();

                Pen pen = new Pen(brush, thickness);
                pen.StartLineCap = PenLineCap.Round;
                pen.EndLineCap = PenLineCap.Round;
                pen.Freeze();

                return pen;
            }

            /// <summary>
            /// Crea un brush
            /// </summary>
            /// <param name="color">Color.</param>
            /// <param name="opacity">Opacitat.</param>
            /// <returns>El brush.</returns>
            /// 
            private static Brush CreateBrush(Color color, double opacity = 1) {

                Brush brush = new SolidColorBrush(color);
                brush.Opacity = opacity;
                brush.Freeze();

                return brush;
            }

            private static void DrawPolygon(DrawingContext dc, Brush brush, Pen pen, Polygon polygon) {

                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open())
                    StreamPolygon(ctx, polygon, polygon.HasPoints ? 1 : 0);
                geometry.Freeze();
                dc.DrawGeometry(brush, pen, geometry);
            }

            /// <summary>
            /// Dibuixa un poligon en una geometria
            /// </summary>
            /// <param name="ctx">Contexte de la geometria.</param>
            /// <param name="polygon">El poligon a dibuixar.</param>
            /// 
            private static void StreamPolygon(StreamGeometryContext ctx, Polygon polygon, int level) {

                if (polygon.HasPoints) {
                    bool first = true;
                    foreach (Point point in polygon.Points) {
                        if (first) {
                            first = false;
                            ctx.BeginFigure(point, true, true);
                        }
                        else
                            ctx.LineTo(point, true, true);
                    }
                }
                if (polygon.HasChilds && (level < 2))
                    foreach (Polygon child in polygon.Childs)
                        StreamPolygon(ctx, child, level + 1);
            }

        }

        private readonly Board board;

        /// <summary>
        /// Contructor. 
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
        public IEnumerable<Visual> CreateVisuals() {

            List<LayerId> layerIds = new List<LayerId>();
            // layerIds.Add(LayerId.BottomNames);
            // layerIds.Add(LayerId.BottomValues);
            layerIds.Add(LayerId.BottomDocument);
            //layerIds.Add(LayerId.BottomCream);
            layerIds.Add(LayerId.BottomGlue);
            layerIds.Add(LayerId.BottomKeepout);
            layerIds.Add(LayerId.BottomRestrict);
            layerIds.Add(LayerId.BottomPlace);
            layerIds.Add(LayerId.Bottom);
            layerIds.Add(LayerId.ViaRestrict);
            layerIds.Add(LayerId.Top);
            layerIds.Add(LayerId.TopPlace);
            layerIds.Add(LayerId.TopRestrict);
            layerIds.Add(LayerId.TopKeepout);
            layerIds.Add(LayerId.TopGlue);
            //layerIds.Add(LayerId.TopCream);
            layerIds.Add(LayerId.TopDocument);
            //layerIds.Add(LayerId.TopValuest);
            //layerIds.Add(LayerId.TopNames);
            layerIds.Add(LayerId.Vias);
            layerIds.Add(LayerId.Pads);
            layerIds.Add(LayerId.Holes);
            layerIds.Add(LayerId.Profile);

            List<Visual> visuals = new List<Visual>();
            foreach (LayerId layerId in layerIds)
                ProcessLayer(board, layerId, visuals);

            return visuals;
        }

        private void ProcessLayer(Board board, LayerId layerId, IList<Visual> visuals) {

            Layer layer = board.GetLayer(layerId);
            if (layer.IsVisible) {
                RenderVisitor visitor = new RenderVisitor(board, layer, visuals);
                visitor.Visit(board);
            }
        }
    }
}
