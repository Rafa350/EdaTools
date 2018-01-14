namespace Eda.PCBViewer.DrawEditor {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    public sealed class VisualGenerator {

        private class RenderItem {
            public Part Part { get; set; }
            public Element Element { get; set; }
        }

        private sealed class RenderVisitor: BoardVisitor {

            private readonly Board board;
            private readonly Layer layer;
            private readonly IList<Visual> visuals;
            private MatrixTransform localTransform = null;

            public RenderVisitor(Board board, Layer layer, IList<Visual> visuals) {

                this.board = board;
                this.layer = layer;
                this.visuals = visuals;
            }

            public override void Visit(Part part) {

                localTransform = new MatrixTransform(part.Transformation);
                base.Visit(part);
                localTransform = null;
            }

            public override void Visit(LineElement line) {

                if (board.IsOnLayer(line, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = BrushCache.Instance.GetBrush(layer.Color);
                        dc.DrawPolygon(brush, null, line.Polygon);

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            public override void Visit(ArcElement arc) {

                if (board.IsOnLayer(arc, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = BrushCache.Instance.GetBrush(layer.Color);
                        dc.DrawPolygon(brush, null, arc.Polygon);

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            public override void Visit(RectangleElement rectangle) {

                if (board.IsOnLayer(rectangle, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = BrushCache.Instance.GetBrush(layer.Color);
                        dc.DrawPolygon(brush, null, rectangle.Polygon);

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            public override void Visit(CircleElement circle) {

                if (board.IsOnLayer(circle, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = BrushCache.Instance.GetBrush(layer.Color);
                        dc.DrawPolygon(brush, null, circle.Polygon);

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            public override void Visit(RegionElement region) {

                if (layer.Id == LayerId.TopPlace && board.IsOnLayer(region, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = BrushCache.Instance.GetBrush(layer.Color);
                        dc.DrawPolygon(brush, null, region.Polygon);

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            public override void Visit(ViaElement via) {

                if ((layer.Id == LayerId.Vias) && board.IsOnLayer(via, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Polygon polygon = via.Polygon;
                        List<Polygon> polygonHoles = new List<Polygon>(polygon.Childs);

                        Brush polygonBrush = BrushCache.Instance.GetBrush(layer.Color);
                        dc.DrawPolygon(polygonBrush, null, polygon);

                        if (polygonHoles.Count == 1) {
                            Brush holeBrush = BrushCache.Instance.GetBrush(Colors.Black);
                            dc.DrawPolygon(holeBrush, null, polygonHoles[0]);
                        }

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (board.IsOnLayer(pad, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = BrushCache.Instance.GetBrush(layer.Color);
                        dc.DrawPolygon(brush, null, pad.Polygon);

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            public override void Visit(ThPadElement pad) {

               if ((layer.Id == LayerId.Pads) && board.IsOnLayer(pad, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Brush brush = BrushCache.Instance.GetBrush(layer.Color);
                        dc.DrawPolygon(brush, null, pad.Polygon);

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }

            public override void Visit(HoleElement hole) {

                if ((layer.Id == LayerId.Holes) && board.IsOnLayer(hole, layer)) {

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen()) {

                        if (localTransform != null)
                            dc.PushTransform(localTransform);

                        Pen pen = PenCache.Instance.GetPen(layer.Color, 0.05);
                        Brush brush = BrushCache.Instance.GetBrush(Colors.Black);
                        dc.DrawPolygon(brush, pen, hole.Polygon);

                        if (localTransform != null)
                            dc.Pop();
                    }

                    visuals.Add(visual);
                }
            }
        }

        private readonly Board board;


        /// <summary>
        /// Contructor. 
        /// </summary>
        /// <param name="board">La placa a procesar.</param>
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

            List<Visual> visuals = new List<Visual>();

            // Procesa la placa, capa a capa
            //
            //ProcessLayer(board, LayerId.BottomNames, visualList);
            //ProcessLayer(board, LayerId.BottomValues, visualList);
            ProcessLayer(board, LayerId.BottomDocument, visuals);
            //ProcessLayer(board, LayerId.BottomCream, visuals);
            ProcessLayer(board, LayerId.BottomGlue, visuals);
            ProcessLayer(board, LayerId.BottomKeepout, visuals);
            ProcessLayer(board, LayerId.BottomRestrict, visuals);
            ProcessLayer(board, LayerId.BottomPlace, visuals);
            ProcessLayer(board, LayerId.Bottom, visuals);
            ProcessLayer(board, LayerId.ViaRestrict, visuals);
            ProcessLayer(board, LayerId.Top, visuals);
            ProcessLayer(board, LayerId.TopPlace, visuals);
            ProcessLayer(board, LayerId.TopRestrict, visuals);
            ProcessLayer(board, LayerId.TopKeepout, visuals);
            ProcessLayer(board, LayerId.TopGlue, visuals);
            //ProcessLayer(board, LayerId.TopCream, visuals);
            ProcessLayer(board, LayerId.TopDocument, visuals);
            //ProcessLayer(board, LayerId.TopValues, visualList);
            //ProcessLayer(board, LayerId.TopNames, visualList);
            ProcessLayer(board, LayerId.Vias, visuals);
            ProcessLayer(board, LayerId.Pads, visuals);
            ProcessLayer(board, LayerId.Holes, visuals);
            ProcessLayer(board, LayerId.Profile, visuals);

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
