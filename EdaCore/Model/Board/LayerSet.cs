using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public struct LayerSet : IEnumerable<LayerId> {

        private readonly HashSet<LayerId> _set;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="ids">Identificadors.</param>
        /// 
        public LayerSet(params LayerId[] ids) {

            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            _set = new HashSet<LayerId>(ids);
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="ids">Identificadors.</param>
        /// 
        public LayerSet(IEnumerable<LayerId> ids) {

            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            _set = new HashSet<LayerId>(ids);
        }

        /// <summary>
        /// Constructor de copia.
        /// </summary>
        /// <param name="other">El conjunt a copiar.</param>
        /// 
        public LayerSet(LayerSet other) {

            _set = new HashSet<LayerId>(other);
        }

        /// <summary>
        /// Constructor privat
        /// </summary>
        /// <param name="set">Contingut.</param>
        /// 
        private LayerSet(HashSet<LayerId> set) {

            _set = set;
        }

        /// <summary>
        /// Comprova si un identificador pertany al conjunt.
        /// </summary>
        /// <param name="id">Identificador.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool Contains(LayerId id) =>
            _set.Contains(id);

        /// <summary>
        /// Obte un objecte que es la unio d'altres dos.
        /// </summary>
        /// <param name="set1">Primer conjunt.</param>
        /// <param name="set2">Segon conjunt.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static LayerSet operator +(LayerSet set1, LayerSet set2) =>
            new LayerSet(set1._set.Union(set2._set));

        /// <summary>
        /// Obte un conjunt que es la unio d'altres dos.
        /// </summary>
        /// <param name="set">Conjunt.</param>
        /// <param name="id">Identificador.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static LayerSet operator +(LayerSet set, LayerId id) {

            if (set._set == default)
                return new LayerSet(id);
            else if (set.Contains(id))
                return new LayerSet(set);
            else {
                var s = new HashSet<LayerId>(set._set);
                s.Add(id);
                return new LayerSet(s);
            }
        }

        /// <summary>
        /// Obte un conjunt que es la diferencia d'altres dos.
        /// </summary>
        /// <param name="set">Conjunt.</param>
        /// <param name="id">Itentificador.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static LayerSet operator -(LayerSet set, LayerId id) {

            var s = new HashSet<LayerId>(set);
            s.Remove(id);
            return new LayerSet(s);
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public override string ToString() {

            var sb = new StringBuilder();
            bool first = true;
            foreach (var id in _set) {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(id);
            }
            return sb.ToString();
        }


        public static LayerSet Parse(string s) {

            if (String.IsNullOrEmpty(s))
                throw new ArgumentNullException(nameof(s));

            var ss = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            LayerSet layerSet = new LayerSet();
            foreach (var sss in ss)
                layerSet += LayerId.Get(sss);
            return layerSet;
        }

        public IEnumerator<LayerId> GetEnumerator() =>
            _set == default ? null : _set.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _set == default ? null : _set.GetEnumerator();

        /// <summary>
        /// Obte l'indicador de conjunt buit.
        /// </summary>
        /// 
        public bool IsEmpty =>
            (_set == default) || (_set.Count == 0);
    }
}