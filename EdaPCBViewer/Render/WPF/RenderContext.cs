namespace MikroPic.EdaTools.v1.Designer.Render.WPF {

    using MikroPic.EdaTools.v1.Designer.Render.WPF.Infrastructure;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using System.Collections.Generic;
    using System.Windows.Media;

    public sealed class RenderContext : IRenderContext {

        public ISceneGraph Render(Board board) {

            List<LayerId> layerIds = new List<LayerId>();
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

            layerIds.Add(Layer.ProfileId);

            VisualDrawer drawer = new VisualDrawer();
            DrawingVisual rootVisual = new DrawingVisual();

            foreach (LayerId layerId in layerIds) {

                Layer layer = board.GetLayer(layerId, false);
                if ((layer != null) && layer.IsVisible) {

                    DrawingVisual layerVisual = new DrawingVisual();
                    rootVisual.Children.Add(layerVisual);
                    layerVisual.Opacity = layer.Color.A / 255.0;

                    RenderVisitor visitor = new RenderVisitor(board, layer, layerVisual, drawer);
                    visitor.Run();
                }
            }

            return new SceneGraph(rootVisual);
        }
    }
}
