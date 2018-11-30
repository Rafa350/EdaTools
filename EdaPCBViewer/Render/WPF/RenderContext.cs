namespace MikroPic.EdaTools.v1.Designer.Render.WPF {

    using MikroPic.EdaTools.v1.Designer.Render.WPF.Infrastructure;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using System.Collections.Generic;
    using System.Windows.Media;

    public sealed class RenderContext : IRenderContext {

        public ISceneGraph Render(Board board) {

            /*List<LayerId> layerIds = new List<LayerId>();
            layerIds.Add(Layer.BottomNamesId);
            layerIds.Add(Layer.BottomDocumentId);
            layerIds.Add(Layer.BottomGlueId);
            layerIds.Add(Layer.BottomKeepoutId);
            layerIds.Add(Layer.BottomRestrictId);
            layerIds.Add(Layer.BottomPlaceId);
            layerIds.Add(Layer.BottomId);
            layerIds.Add(Layer.ViaRestrictId);
            layerIds.Add(Layer.TopId);
            layerIds.Add(Layer.TopPlaceId);
            layerIds.Add(Layer.TopRestrictId);
            layerIds.Add(Layer.TopKeepoutId);
            layerIds.Add(Layer.TopGlueId);
            layerIds.Add(Layer.TopNamesId);

            layerIds.Add(Layer.PadsId);
            layerIds.Add(Layer.ViasId);
            layerIds.Add(Layer.HolesId);
            layerIds.Add(Layer.TopDocumentId);

            layerIds.Add(Layer.ProfileId);*/

            VisualLayerStack visualLayerStack = VisualLayerStack.CreateDefault();

            VisualDrawer drawer = new VisualDrawer();
            DrawingVisual rootVisual = new DrawingVisual();

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
