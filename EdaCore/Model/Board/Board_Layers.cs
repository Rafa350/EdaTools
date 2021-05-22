using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board {

        private Dictionary<LayerId, Layer> _layers = new Dictionary<LayerId, Layer>();
        private Layer _outlineLayer;
        private Layer _topLayer;
        private Layer _bottomLayer;

        /// <summary>
        /// Afegeix una capa a la placa. L'ordre en que s'afegeixen corresponen a l'apilament fisic de la placa.
        /// </summary>
        /// <param name="layer">La capa a afeigir.</param>
        /// 
        public void AddLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException(nameof(layer));

            if ((_layers != null) && _layers.ContainsKey(layer.Id))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', ya esta asignada a esta placa.", layer.Id));

            if (layer.IsTopCopper)
                _topLayer = layer;
            else if (layer.IsBottomCopper)
                _bottomLayer = layer;
            else if (layer.Function == LayerFunction.Outline) {
                if (_outlineLayer == null)
                    _outlineLayer = layer;
                else
                    throw new InvalidOperationException("Solo puede haber una capa con la funcion 'Outline'");
            }

            if (_layers == null)
                _layers = new Dictionary<LayerId, Layer>();
            _layers.Add(layer.Id, layer);
        }

        /// <summary>
        /// Afegeix diverses capes.
        /// </summary>
        /// <param name="layers">Les capes a afeigir.</param>
        /// 
        public void AddLayers(IEnumerable<Layer> layers) {

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
        public void RemoveLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException(nameof(layer));

            if ((_layers == null) || !_layers.ContainsKey(layer.Id))
                throw new InvalidOperationException(
                    String.Format("No se encontro la capa '{0}'.", layer.Id));

            _layers.Remove(layer.Id);
            if (_layers.Count == 0)
                _layers = null;

            if (_topLayer == layer)
                _topLayer = null;
            else if (_bottomLayer == layer)
                _bottomLayer = null;
            else if (_outlineLayer == layer)
                _outlineLayer = null;
        }

        /// <summary>
        /// Obte una capa pel seu identificador.
        /// </summary>
        /// <param name="id">El identificador de la capa.</param>
        /// <param name="throwOnError">True si cal generar una excepcio si no el troba.</param>
        /// <returns>La capa.</returns>
        /// 
        public Layer GetLayer(LayerId id, bool throwOnError = true) {

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
        /// <returns></returns>
        public IEnumerable<Layer> GetSignalLayers() {

            var signalLayers = new List<Layer>();

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
        public IEnumerable<Element> GetElements(Layer layer, bool includeComponents = true) {

            var list = new List<Element>();

            foreach (var element in Elements)
                if (element.IsOnLayer(layer))
                    list.Add(element);

            if (includeComponents)
                foreach (var component in Components)
                    foreach (var element in component.Elements)
                        if (element.IsOnLayer(layer))
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
                throw new ArgumentNullException(nameof(element));

            var list = new List<Layer>();

            foreach (var id in element.LayerSet) {
                Layer layer = GetLayer(id, false);
                if (layer != null)
                    list.Add(layer);
            }

            return list;
        }

        /// <summary>
        /// Obte la capa superior.
        /// </summary>
        /// 
        public Layer TopLayer =>
            _topLayer;

        /// <summary>
        /// Obte la capa inferior.
        /// </summary>
        /// 
        public Layer BottomLayer =>
           _bottomLayer;

        /// <summary>
        /// Obte la capa de perfil.
        /// </summary>
        /// 
        public Layer OutlineLayer =>
            _outlineLayer;

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
        public IEnumerable<Layer> Layers =>
            _layers?.Values;
    }
}

