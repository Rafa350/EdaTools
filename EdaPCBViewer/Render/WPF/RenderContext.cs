namespace MikroPic.EdaTools.v1.PanelEditor.Render.WPF {

    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.PanelEditor.Render.WPF.Infrastructure;
    using System.Collections.Generic;
    using System.Windows.Media;

    public sealed class RenderContext : IRenderContext {

        public ISceneGraph Render(Board board) {

            VisualLayerStack visualLayerStack = VisualLayerStack.CreateDefault();

            VisualDrawer drawer = new VisualDrawer();
            DrawingVisual rootVisual = new DrawingVisual();

            // Obte els elements de la placa susceptibles a ser visualitzats
            //
            List<Element> elements = new List<Element>();
            foreach (var part in board.Parts)
                foreach (var element in part.Elements)
                    elements.Add(element);
            foreach (var element in board.Elements)
                elements.Add(element);

            // Procesa les capes visuals que son visibles
            //
            foreach (var visualLayer in visualLayerStack.VisualLayers) {
                if (visualLayer.Visible) {

                    HashSet<Element> visibleElements = new HashSet<Element>();

                    // Selecciona els elements que son visibles en aquesta capa visual
                    //
                    foreach (var element in elements) {

                        if (element.ElementType == ElementType.ThPad) {
                            ;
                        }

                        // Seleccio per capa
                        //
                        bool layerOk = false;
                        if (visualLayer.LayerIds == null)
                            layerOk = true;
                        else
                            foreach (var layerId in visualLayer.LayerIds) {
                                if (element.LayerSet.Contains(layerId)) {
                                    layerOk = true;
                                    break;
                                }
                            }

                        // Seleccio per tipus
                        //
                        bool typeOk = false;
                        if (visualLayer.ElementTypes == null)
                            typeOk = true;
                        else
                            foreach (var elementType in visualLayer.ElementTypes) {
                                if (element.ElementType == elementType) {
                                    typeOk = true;
                                    break;
                                }
                            }

                        // Afegeix l'element a la llista de visibles
                        //
                        if (layerOk && typeOk)
                            visibleElements.Add(element);
                    }

                    DrawingVisual layerVisual = new DrawingVisual();
                    rootVisual.Children.Add(layerVisual);
                    layerVisual.Opacity = visualLayer.Opacity;

                    foreach (var layerId in visualLayer.LayerIds) {
                        Layer layer = board.GetLayer(layerId, false);
                        if (layer != null) {
                            RenderVisitor visitor = new RenderVisitor(visibleElements, layer, layerVisual, visualLayer.Color, drawer);
                            board.AcceptVisitor(visitor);
                        }
                    }
                }
            }

            return new SceneGraph(rootVisual);
        }
    }
}
