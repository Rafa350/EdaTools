using Avalonia;
using Avalonia.Media;
using MikroPic.EdaTools.v1.Core.Model.Board;
using SkiaSharp;

namespace EdaBoardViewer.Render {

    public sealed class BoardRenderer {

        private static Color _boardColor = Color.FromArgb(255, 0, 48, 0);
        private readonly DrawingContext context;

        public BoardRenderer(DrawingContext context) {

            this.context = context;
        }

        public void Render(EdaBoard board) {

            var brush = new SolidColorBrush(_boardColor);

            var geometry = board.GetOutlinePolygon().ToGeometry();
            context.DrawGeometry(brush, null, geometry);

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
