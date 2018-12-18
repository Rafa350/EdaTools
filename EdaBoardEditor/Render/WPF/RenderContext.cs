namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF {

    using MikroPic.EdaTools.v1.Core.Model.Board;
    using System.Windows.Media;

    public sealed class RenderContext : IRenderContext {

        public ISceneGraph Render(Board board) {

            VisualLayerStack visualLayerStack = VisualLayerStack.CreateDefault();

            DrawingVisual rootVisual = new DrawingVisual();

            // Procesa les capes visuals que son visibles
            //
            foreach (var visualLayer in visualLayerStack.VisualLayers) {
                if (visualLayer.Visible) {

                    DrawingVisual layerVisual = new DrawingVisual();
                    rootVisual.Children.Add(layerVisual);
                    layerVisual.Opacity = visualLayer.Opacity;

                    foreach (var layerId in visualLayer.LayerIds) {
                        Layer layer = board.GetLayer(layerId, false);
                        if (layer != null) {
                            RenderVisitor visitor = new RenderVisitor(layer, layerVisual, visualLayer);
                            board.AcceptVisitor(visitor);
                        }
                    }
                }
            }

            return new SceneGraph(rootVisual);
        }
    }
}
