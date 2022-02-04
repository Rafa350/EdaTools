using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class EdaBoard {

        private Dictionary<EdaLayerId, EdaLayer> _layers = new Dictionary<EdaLayerId, EdaLayer>();
        private EdaLayer _outlineLayer;

        /// <summary>
        /// Afegeix una capa a la placa. L'ordre en que s'afegeixen corresponen a l'apilament fisic de la placa.
        /// </summary>
        /// <param name="layer">La capa a afeigir.</param>
        /// 
        public void AddLayer(EdaLayer layer) {

            if (layer == null)
                throw new ArgumentNullException(nameof(layer));

            if ((_layers != null) && _layers.ContainsKey(layer.Id))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', ya esta asignada a esta placa.", layer.Id));

            if (layer.Function == LayerFunction.Outline) {
                if (_outlineLayer != null)
                    throw new InvalidOperationException("Solo puede haber una capa con la funcion OutLine.");
                _outlineLayer = layer;
            }

            if (_layers == null)
                _layers = new Dictionary<EdaLayerId, EdaLayer>();
            _layers.Add(layer.Id, layer);
        }

        /// <summary>
        /// Afegeix diverses capes.
        /// </summary>
        /// <param name="layers">Les capes a afeigir.</param>
        /// 
        public void AddLayers(IEnumerable<EdaLayer> layers) {

            if (layers == null)
                throw new ArgumentNullException(nameof(layers));

            foreach (var layer in layers)
                AddLayer(layer);
        }

        /// <summary>
        /// Elimina una capa.
        /// </summary>
        /// <param name="layer">La capa a eliminar.</param>
        /// 
        public void RemoveLayer(EdaLayer layer) {

            if (layer == null)
                throw new ArgumentNullException(nameof(layer));

            if ((_layers == null) || !_layers.ContainsKey(layer.Id))
                throw new InvalidOperationException(
                    String.Format("No se encontro la capa '{0}'.", layer.Id));

            _layers.Remove(layer.Id);
            if (_layers.Count == 0)
                _layers = null;
        }

        /// <summary>
        /// Obte una capa pel seu identificador.
        /// </summary>
        /// <param name="id">El identificador de la capa.</param>
        /// <param name="throwOnError">True si cal generar una excepcio si no el troba.</param>
        /// <returns>La capa.</returns>
        /// 
        public EdaLayer GetLayer(EdaLayerId id, bool throwOnError = true) {

            if ((_layers != null) && _layers.TryGetValue(id, out var layer))
                return layer;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro la capa '{0}'.", id));
            else
                return null;
        }

        /// <summary>
        /// Obte la llista de capes de senyal, ordenades de la capa TOP a la BOTTOM
        /// </summary>
        /// <returns>Les capes.</returns>
        /// 
        public IEnumerable<EdaLayer> GetSignalLayers() {

            var signalLayers = new List<EdaLayer>();

            foreach (var layer in _layers.Values)
                if (layer.Function == LayerFunction.Signal)
                    signalLayers.Add(layer);

            return signalLayers;
        }

        /// <summary>
        /// Obte els elements d'una capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <param name="includeComponents">Indica si cal incluir els elements dels components.</param>
        /// <returns>Els elements.</returns>
        /// 
        public IEnumerable<EdaElement> GetElements(EdaLayer layer, bool includeComponents = true) {

            var list = new List<EdaElement>();

            foreach (var element in Elements)
                if (element.IsOnLayer(layer.Id))
                    list.Add(element);

            if (includeComponents)
                foreach (var component in Components)
                    foreach (var element in component.Elements)
                        if (element.IsOnLayer(layer.Id))
                            list.Add(element);

            return list;
        }

        /// <summary>
        /// Indica si te capes
        /// </summary>
        /// 
        public bool HasLayers =>
            _layers != null;

        /// <summary>
        /// Enumera les capes.
        /// </summary>
        /// 
        public IEnumerable<EdaLayer> Layers =>
            _layers?.Values;

        /// <summary>
        /// Obte la capa del perfil de la placa.
        /// </summary>
        /// 
        public EdaLayer OutlineLayer =>
            _outlineLayer;
    }
}

