namespace EdaBoardViewer.Render {

    using Avalonia;
    using Avalonia.Media;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

    using Rect = MikroPic.EdaTools.v1.Base.Geometry.Rect;

    public sealed class BoardRenderer {

        private readonly DrawingContext context;

        public BoardRenderer(DrawingContext context) {

            this.context = context;
        }

        public void Render(Board board) {

            Rect bbox = board.GetBoundingBox();
            double factor = 100000.0;
            double s = 1.0 / factor;

            Matrix m =
                Matrix.CreateTranslation(0, -bbox.Height) *
                Matrix.CreateScale(s, -s);

            using (context.PushPreTransform(m)) {

                VisualLayerStack visualLayers = VisualLayerStack.CreateDefault();
                foreach (VisualLayer visualLayer in visualLayers.VisualLayers) {
                    if (visualLayer.Visible) {
                        foreach (var layerId in visualLayer.LayerIds) {
                            var layer = board.GetLayer(layerId);
                            if (layer != null) {

                                var visitor = new BoardRenderVisitor(layer, visualLayer, context);
                                visitor.Visit(board);
                            }
                        }
                    }
                }
            }
        }

    }
}
