using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed class LayerSet: IEnumerable<LayerId> {

        private readonly List<LayerId> _items = new List<LayerId>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public LayerSet() {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="layerId">El layer id inicial.</param>
        /// 
        public LayerSet(LayerId layerId) {

            _items.Add(layerId);    
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="layerIds">Els layer id inicials.</param>
        /// 
        public LayerSet(params LayerId[] layerIds) {

            foreach (LayerId layerId in layerIds) 
                _items.Add(layerId);
        }

        /// <summary>
        /// Afegeix un identificador al conjunt.
        /// </summary>
        /// <param name="layerId">El identificador de la capa.</param>
        /// 
        public void Add(LayerId layerId) {

            if (!_items.Contains(layerId))
                _items.Add(layerId);
        }

        public void Remove(LayerId layer) {

            _items.Remove(layer);
        }

        /// <summary>
        /// Comprova si conte el identificador.
        /// </summary>
        /// <param name="layerId">El identificador.</param>
        /// <returns>True si conte l'identificador.</returns>
        /// 
        public bool Contains(LayerId layerId) {

            return _items.Contains(layerId);
        }

        public static LayerSet Parse(string text) {

            string[] ss = text.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

            LayerSet layerSet = new LayerSet();
            foreach(var s in ss) {
                layerSet.Add(LayerId.Parse(s));
            }
            return layerSet;
        }

        public override string ToString() {

            var sb = new StringBuilder();

            bool first = true;
            foreach(var layerId in _items) {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(layerId.ToString());
            }

            return sb.ToString();
        }

        public IEnumerator<LayerId> GetEnumerator() {

            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {

            return _items.GetEnumerator();
        }
    }
}
