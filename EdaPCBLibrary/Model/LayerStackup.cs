namespace MikroPic.EdaTools.v1.Pcb.Model.Collections {

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class LayerStackup {

        private readonly IList<Layer> layers = new List<Layer>();
        private readonly IDictionary<Layer, Layer> layerPairs = new Dictionary<Layer, Layer>();

        /// <summary>
        /// Afegeix una capa.
        /// </summary>
        /// <param name="layer">La cala a afeigir.</param>
        /// 
        public void AddLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (layers.Contains(layer))
                throw new InvalidOperationException("La capa ya existe.");

            foreach (Layer l in layers)
                if (l.Name == layer.Name)
                    throw new InvalidOperationException("Ya existe una capa con el mismo nombre.");

            layers.Add(layer);
        }

        /// <summary>
        /// Elimina una capa.
        /// </summary>
        /// <param name="layer">La capa a eliminar.</param>
        /// 
        public void RemoveLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (!layers.Contains(layer))
                throw new InvalidOperationException("La capa no existe.");

            layers.Remove(layer);
        }

        /// <summary>
        /// Asocia una capa amb la seva complementaria de l'altre banda de la placa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <param name="layerPair">La capa complementaria.</param>
        /// 
        public void DefineLayerPair(Layer layer, Layer layerPair) {

            if (layer == null)
                throw new ArgumentException("layer1");

            if (layerPair == null)
                throw new ArgumentException("layer2");

            if (!layers.Contains(layer))
                throw new InvalidOperationException("La capa no existe.");

            if (!layers.Contains(layerPair))
                throw new InvalidOperationException("La capa par no existe.");

            if (layerPairs.ContainsKey(layer))
                throw new InvalidOperationException("Ya existe un par asociado a la capa.");

            layerPairs.Add(layer, layerPair);
        }

        /// <summary>
        /// Obte una capa pel seu nom.
        /// </summary>
        /// <param name="name">El nom de la capa.</param>
        /// <param name="pair">Recupera la capa aparellada.</param>
        /// <returns>La capa.</returns>
        /// 
        public Layer GetLayer(string name, bool pair = false) {

            Layer layer = layers.Single<Layer>(l => l.Name == name);
            if (layer == null)
                throw new InvalidOperationException("La capa no existe.");

            return pair ? GetLayerPair(layer) : layer;
        }

        public Layer GetLayer(LayerId id, bool pair = false) {

            Layer layer = layers.Single<Layer>(l => l.Id == id);
            if (layer == null)
                throw new InvalidOperationException("La capa no existe.");

            return pair ? GetLayerPair(layer) : layer;
        }

        /// <summary>
        /// Obte la capa complementaria d'una capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <param name="thowException">Indica si cal generar una excepcio.</param>
        /// <returns>La seva capa complementaria.</returns>
        /// 
        public Layer GetLayerPair(Layer layer, bool thowException = false) {

            if (layer == null)
                throw new ArgumentException("layer");

            if (!layers.Contains(layer))
                throw new InvalidOperationException("La capa no existe.");

            if (!layerPairs.ContainsKey(layer)) {
                if (thowException)
                    throw new InvalidOperationException("La capa par asociada no existe.");
                else
                    return null;
            }

            return layerPairs[layer];
        }

        /// <summary>
        /// Obte la llista global de capes.
        /// </summary>
        /// 
        public IEnumerable<Layer> Layers {
            get {
                return layers;
            }
        }

        /// <summary>
        /// Obte la llista global de capes de senyal.
        /// </summary>
        /// 
        public IEnumerable<Layer> SignalLayers {
            get {
                return layers.OfType<Layer>();
            }
        }

        /// <summary>
        /// Obte la llista de capes complementaries.
        /// </summary>
        /// 
        public IEnumerable<LayerPair> LayerPairs {
            get {
                List<LayerPair> result = new List<LayerPair>();
                foreach (KeyValuePair<Layer, Layer> pair in layerPairs)
                    result.Add(new LayerPair(pair.Key, pair.Value));
                return result;
            }
        }
    }
}
