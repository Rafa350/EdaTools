namespace MikroPic.EdaTools.v1.PanelEditor.Render.WPF {

    using MikroPic.EdaTools.v1.PanelEditor.Render.WPF.Infrastructure;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System.Collections.Generic;
    using System.Windows.Media;
    using System.Linq;

    public sealed class RenderContext : IRenderContext {

        public ISceneGraph Render(Board board) {

            VisualLayerStack visualLayerStack = VisualLayerStack.CreateDefault();

            VisualDrawer drawer = new VisualDrawer();
            DrawingVisual rootVisual = new DrawingVisual();

            /*List<Element> elements = new List<Element>();
            foreach (var part in board.Parts)
                foreach (var element in part.Elements)
                    elements.Add(element);
            foreach (var element in board.Elements)
                elements.Add(element);

            List<Element> holes = new List<Element>(elements.OfType<HoleElement>());
            */

            // Procesa les capes visuals
            //
            foreach (var visualLayer in visualLayerStack.VisualLayers) {
                if (visualLayer.Visible) {

                    // Procesa les capes de la placa que corresponen a la capa visual
                    //
                    foreach (var layerId in visualLayer.Layers) {

                        Layer layer = board.GetLayer(layerId, false);
                        if (layer != null) {

                            DrawingVisual layerVisual = new DrawingVisual();
                            rootVisual.Children.Add(layerVisual);
                            layerVisual.Opacity = visualLayer.Opacity;

                            RenderVisitor visitor = new RenderVisitor(board, layer, layerVisual, visualLayer.Color, drawer);
                            visitor.Run();
                        }
                    }
                }
            }

            return new SceneGraph(rootVisual);
        }
    }
}
