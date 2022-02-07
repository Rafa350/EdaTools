using Avalonia;
using Avalonia.Media;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace EdaBoardViewer.Render {

    public sealed class BoardRenderer {

        private static Color _boardColor = Color.FromArgb(255, 0, 32, 0);
        private readonly DrawingContext context;

        public BoardRenderer(DrawingContext context) {

            this.context = context;
        }

        public void Render(EdaBoard board) {

            var r = board.GetBoundingBox();
            var brush = new SolidColorBrush(_boardColor);
            context.FillRectangle(brush, new Rect(r.X, r.Y, r.Width, r.Height));

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
