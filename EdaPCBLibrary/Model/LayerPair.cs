namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;

    public sealed class LayerPair {

        private readonly Layer layer;
        private readonly Layer complementLayer;

        public LayerPair(Layer layer, Layer complementLayer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (complementLayer == null)
                throw new ArgumentNullException("complementLayer");

            this.layer = layer;
            this.complementLayer = complementLayer;
        }

        public Layer Layer { get { return layer; } }
        public Layer ComplementLayer { get { return complementLayer; } }
    }
}
