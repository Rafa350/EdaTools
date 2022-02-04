using System.Collections;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed class EdaLayerSet: IEnumerable<EdaLayerId> {

        private readonly List<EdaLayerId> _items = new List<EdaLayerId>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public EdaLayerSet() {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="layerId">El layer id inicial.</param>
        /// 
        public EdaLayerSet(EdaLayerId layerId) {

            _items.Add(layerId);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="layerIds">Els layer id inicials.</param>
        /// 
        public EdaLayerSet(params EdaLayerId[] layerIds) {

            _items.AddRange(layerIds);
        }

        /// <summary>
        /// Afegeix un identificador al conjunt.
        /// </summary>
        /// <param name="layerId">El identificador de la capa.</param>
        /// 
        public void Add(EdaLayerId layerId) {

            if (!_items.Contains(layerId))
                _items.Add(layerId);
        }

        /// <summary>
        /// Elimina un identificador del conjunt.
        /// </summary>
        /// <param name="layer">El identificador a eliminar.</param>
        /// 
        public void Remove(EdaLayerId layer) {

            _items.Remove(layer);
        }

        /// <summary>
        /// Comprova si conte el identificador.
        /// </summary>
        /// <param name="layerId">El identificador.</param>
        /// <returns>True si conte l'identificador.</returns>
        /// 
        public bool Contains(EdaLayerId layerId) {

            return _items.Contains(layerId);
        }

        public IEnumerable<EdaLayerId> Items =>
            _items;

        public IEnumerator<EdaLayerId> GetEnumerator() {

            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {

            return _items.GetEnumerator();
        }
    }
}
