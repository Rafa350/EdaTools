namespace MikroPic.EdaTools.v1.Pcb.Model.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    public abstract class CollectionBase<T>: IEnumerable<T> {

        private readonly List<T> items = new List<T>();

        /// <summary>
        /// Afegeix un item a la col·leccio.
        /// </summary>
        /// <param name="item">L'item a afeigir.</param>
        /// 
        public void Add(T item) {

            if (item == null)
                throw new ArgumentNullException("item");

            if (items.Contains(item))
                throw new InvalidOperationException("El item ya pertenece a la lista.");

            if (!Validate(item))
                throw new InvalidOperationException("No se valido el item para añadir a la llista.");

            items.Add(item);
        }

        /// <summary>
        /// Afegeix diversos items a la col·leccio.
        /// </summary>
        /// <param name="items">Els items a afeigir.</param>
        /// 
        public void Add(IEnumerable<T> items) {

            if (items == null)
                throw new ArgumentNullException("items");

            foreach (T item in items)
                Add(item);
        }

        /// <summary>
        /// Elimina un item de la col·leccio.
        /// </summary>
        /// <param name="item">L'item a eliminar.</param>
        /// 
        public void Remove(T item) {

            if (item == null)
                throw new ArgumentNullException("item");

            if (!items.Contains(item))
                throw new InvalidOperationException("El item no pertenece a la coleecion.");

            items.Remove(item);
        }

        /// <summary>
        /// Elimina tots els items de la col·leccio.
        /// </summary>
        /// 
        public void Clear() {

            while (items != null)
                Remove(items[0]);
        }

        /// <summary>
        /// Comprova si l'tem pertany a la coleccio.
        /// </summary>
        /// <param name="item">L'item a comprovar.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool Contains(T item) {

            if (item == null)
                throw new ArgumentNullException("item");

            return items.Contains(item);
        }

        public IEnumerator<T> GetEnumerator() {

            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {

            return items.GetEnumerator();
        }

        protected virtual bool Validate(T item) {

            return true;
        }

        /// <summary>
        /// Obte un valor boolean que indica si la col·leccio es buida.
        /// </summary>
        /// 
        public bool IsEmpty {
            get {
                return items.Count == 0;
            }
        }
    }
}
