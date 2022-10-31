using Avalonia.Media;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace EdaBoardViewer.Render {

    public sealed class BoardRenderer {

        private static Color _boardColor = Color.FromArgb(255, 0, 48, 0);

        private readonly EdaBoard _board;

        private PolygonCache _polygonCache;

        public BoardRenderer(EdaBoard board) {

            _board = board;
            _polygonCache = new PolygonCache();
        }

        public void Render(DrawingContext context) {

            var brush = new SolidColorBrush(_boardColor);

            var geometry = _board.GetOutlinePolygon().ToGeometry();
            context.DrawGeometry(brush, null, geometry);

            VisualLayerStack visualLayers = VisualLayerStack.CreateDefault();
            foreach (VisualLayer visualLayer in visualLayers.VisualLayers) {
                if (visualLayer.Visible) {
                    foreach (var layerId in visualLayer.LayerIds) {
                        var layer = _board.GetLayer(layerId, false);
                        if (layer != null) {
                            var visitor = new BoardRenderVisitor(layer, visualLayer, context, _polygonCache);
                            visitor.Visit(_board);
                        }
                    }
                }
            }
        }
    }
}
