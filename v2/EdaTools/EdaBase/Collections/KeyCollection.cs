namespace MikroPic.EdaTools.v1.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class KeyCollection<TItem, TKey> : IEnumerable<TItem>
        where TItem : class, ICollectionKey<TKey> {

        private readonly Dictionary<TKey, TItem> items = new Dictionary<TKey, TItem>();

        /// <summary>
        /// Afegeig un item a la col·leccio.
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void Add(TItem item) {

            if (items == null)
                throw new ArgumentNullException(nameof(item));

            if (items.ContainsKey(item.GetKey()))
                throw new InvalidOperationException("El item ya pertenece a la coleccion.");

            items.Add(item.GetKey(), item);
        }

        /// <summary>
        /// Elimina un item de la coleccio.
        /// </summary>
        /// <param name="item">El item a eliminar.</param>
        /// 
        public void Remove(TItem item) {

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!items.ContainsKey(item.GetKey()))
                throw new InvalidOperationException("El item no pertenece a la coleccion.");

            items.Remove(item.GetKey());
        }

        /// <summary>
        /// Elimina tots els elements de la coleccio
        /// </summary>
        /// 
        public void Clear() {

            items.Clear();
        }

        /// <summary>
        /// Obte un item.
        /// </summary>
        /// <param name="key">La clau del item.</param>
        /// <returns>El item, o null si no el troba.</returns>
        /// 
        public TItem Get(TKey key) {

            if (items.TryGetValue(key, out TItem item))
                return item;

            else
                return null;
        }

        /// <summary>
        /// Comprova si un item pertany a la coleccio.
        /// </summary>
        /// <param name="child">El item a comprovar.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool Contains(TItem item) {

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return items.ContainsKey(item.GetKey());
        }

        /// <summary>
        /// Obte un enumerador per la coleccio.
        /// </summary>
        /// <returns>L'enumerador.</returns>
        /// 
        public IEnumerator<TItem> GetEnumerator() {

            return ((IEnumerable<TItem>)items.Values).GetEnumerator();
        }

        /// <summary>
        /// Obte un enumerador per la coleccio.
        /// </summary>
        /// <returns>L'enumerador.</returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator() {

            return ((IEnumerable<TItem>)items.Values).GetEnumerator();
        }

        /// <summary>
        /// Enumera les claus de la coleccio
        /// </summary>
        /// 
        public IEnumerable<TKey> Keys {
            get {
                return items.Keys;
            }
        }

        /// <summary>
        /// Obte el numero d'items en la col·leccio.
        /// </summary>
        /// 
        public int Count {
            get {
                return items.Count;
            }
        }

        /// <summary>
        /// Indica si la col·leccio es buida.
        /// </summary>
        /// 
        public bool IsEmpty {
            get {
                return items.Count == 0;
            }
        }
    }
}
