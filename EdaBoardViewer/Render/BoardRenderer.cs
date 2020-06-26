namespace EdaBoardViewer.Render {

    using Avalonia.Media;
    using MikroPic.EdaTools.v1.Core.Model.Board;

    public sealed class BoardRenderer {

        private readonly DrawingContext context;

        public BoardRenderer(DrawingContext context) {

            this.context = context;
        }

        public void Render(Board board) {

            VisualLayerStack visualLayers = VisualLayerStack.CreateDefault();
            foreach (VisualLayer visualLayer in visualLayers.VisualLayers) {
                if (visualLayer.Visible) {
                    foreach (var layerId in visualLayer.LayerIds) {
                        var layer = board.GetLayer(layerId, false);
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
