namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;

    public sealed class LayerPair {

        private readonly Layer layer;
        private readonly Layer pairLayer;

        public LayerPair(Layer layer, Layer pairLayer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (pairLayer == null)
                throw new ArgumentNullException("pairLayer");

            this.layer = layer;
            this.pairLayer = pairLayer;
        }

        public Layer Layer { get { return layer; } }
        public Layer PairLayer { get { return pairLayer; } }
    }
}
