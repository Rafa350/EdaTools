namespace MikroPic.EdaTools.v1.Pcb.Model.Collections {

    using System;

    public sealed class LayerCollection: CollectionBase<Layer> {

        public Layer ById(LayerId id) {

            foreach (Layer layer in this)
                if (layer.Id == id)
                    return layer;

            throw new InvalidOperationException("No se encontro la capa con el 'Id' specificado.");
        }
    }
}
