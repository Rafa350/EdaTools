namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board {

        private Dictionary<LayerId, Layer> layers = new Dictionary<LayerId, Layer>();
        private Layer outlineLayer;

        /// <summary>
        /// Afegeix una capa a la placa. L'ordre en que s'afegeixen corresponen a l'apilament fisic de la placa.
        /// </summary>
        /// <param name="layer">La capa a afeigir.</param>
        /// 
        public void AddLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if ((layers != null) && layers.ContainsKey(layer.Id))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', ya esta asignada a esta placa.", layer.Id.FullName));

            if (layers == null)
                layers = new Dictionary<LayerId, Layer>();
            layers.Add(layer.Id, layer);

            if (layer.Function == LayerFunction.Outline) {
                if (outlineLayer == null)
                    outlineLayer = layer;
                else
                    throw new InvalidOperationException("Solo puede haber una capa con la funcion 'Outline'");
            }
        }

        /// <summary>
        /// Afegeix diverses capes.
        /// </summary>
        /// <param name="layers">Les capes a afeigir.</param>
        /// 
        public void AddLayers(IEnumerable<Layer> layers) {

            if (layers == null)
                throw new ArgumentNullException("layers");

            foreach (var layer in layers)
                AddLayer(layer);
        }

        /// <summary>
        /// Elimina una capa.
        /// </summary>
        /// <param name="layer">La capa a eliminar.</param>
        /// 
        public void RemoveLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if ((layers == null) || !layers.ContainsKey(layer.Id))
                throw new InvalidOperationException(
                    String.Format("No se encontro la capa '{0}'.", layer.Id.FullName));

            layers.Remove(layer.Id);
            if (layers.Count == 0)
                layers = null;

            if (outlineLayer == layer)
                outlineLayer = null;
        }

        /// <summary>
        /// Obte una capa pel seu nom
        /// </summary>
        /// <param name="layerId">L'identificador de la capa.</param>
        /// <param name="throwOnError">True si cal generar una excepcio si no el troba.</param>
        /// <returns>La capa.</returns>
        /// 
        public Layer GetLayer(LayerId layerId, bool throwOnError = true) {

            if ((layers != null) && layers.TryGetValue(layerId, out var layer)) 
                return layer;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro la capa '{0}'.", layerId.FullName));
            else
                return null;
        }

        /// <summary>
        /// Obte la llista de capes de senyal, ordenades de la capa TOP a la BOTTOM
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Layer> GetSignalLayers() {

            List<Layer> signalLayers = new List<Layer>();
            foreach (var layer in layers.Values) {
                if (layer.Function == LayerFunction.Signal)
                    signalLayers.Add(layer);
            }
            return signalLayers;
        }

        /// <summary>
        /// Obte els elements d'una capa.
        /// </summary>
        /// <param name="layerId">Identificador de la capa.</param>
        /// <param name="includeComponents">Indica si cal incluir els elements dels components.</param>
        /// <returns>Els elements.</returns>
        /// 
        public IEnumerable<Element> GetElements(LayerId layerId, bool includeComponents = true) {

            List<Element> list = new List<Element>();

            foreach (var element in Elements)
                if (element.IsOnLayer(layerId))
                    list.Add(element);

            if (includeComponents)
                foreach (var component in Components)
                    foreach (var element in component.Elements)
                        if (element.IsOnLayer(layerId))
                            list.Add(element);

            return list;
        }

        /// <summary>
        /// Obte les capes d'un element.
        /// </summary>
        /// <param name="element">L'element.</param>
        /// <returns>Les capes.</returns>
        /// 
        public IEnumerable<Layer> GetLayers(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            List<Layer> list = new List<Layer>();
            foreach (var layerId in element.LayerSet) {
                Layer layer = GetLayer(layerId, false);
                if (layer != null)
                    list.Add(layer);
            }

            return list;
        }

        /// <summary>
        /// Obte la capa de perfil.
        /// </summary>
        /// 
        public Layer OutlineLayer {
            get {
                return outlineLayer;
            }
        }

        /// <summary>
        /// Enumera les capes.
        /// </summary>
        /// 
        public IEnumerable<Layer> Layers {
            get {
                return layers?.Values;
            }
        }
    }
}
