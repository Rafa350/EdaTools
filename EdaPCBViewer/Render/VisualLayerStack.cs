namespace MikroPic.EdaTools.v1.Designer.Render {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Geometry;
    using System;
    using System.Collections.Generic;

    public sealed class VisualLayerStack {

        private readonly List<VisualLayer> layers = new List<VisualLayer>();

        public void Add(VisualLayer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (layers.Contains(layer))
                throw new InvalidOperationException("Ya existe esta capa en la lista.");

            layers.Add(layer);
        }

        public IEnumerable<VisualLayer> VisualLayers {
            get {
                return layers;
            }
        }

        public static VisualLayerStack CreateDefault() {

            VisualLayerStack vls = new VisualLayerStack();

            vls.Add(new VisualLayer("Bottom", new LayerSet(Layer.BottomId), true, new Color(204, 0, 0, 255)));
            vls.Add(new VisualLayer("Top", new LayerSet(Layer.TopId), true, new Color(153, 255, 0, 0)));

            vls.Add(new VisualLayer("TopPlace", new LayerSet(Layer.TopPlaceId), true, new Color(204, 211, 211, 211)));
            vls.Add(new VisualLayer("TopDocument", new LayerSet(Layer.TopDocumentId), true, new Color(204, 160, 160, 160)));
            vls.Add(new VisualLayer("TopNames", new LayerSet(Layer.TopNamesId), true, new Color(255, 211, 211, 211)));

            vls.Add(new VisualLayer("Drills", new LayerSet(Layer.DrillsId), true, new Color(255, 0, 0, 0)));

            return vls;
        }
    }
}
