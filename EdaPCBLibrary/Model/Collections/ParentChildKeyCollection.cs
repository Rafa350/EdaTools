namespace MikroPic.EdaTools.v1.Pcb.Model.Collections {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Coleccio que mante una relacio pare-fill amb els seus items.
    /// </summary>
    /// <typeparam name="TParent">Tipus del pare.</typeparam>
    /// <typeparam name="TChild">Tipus del fill.</typeparam>
    /// <typeparam name="TKey">El tipus de la clau.</typeparam>
    /// 
    public sealed class ParentChildKeyCollection<TParent, TChild, TKey> : KeyCollection<TChild, TKey>
        where TParent: class
        where TChild: class, ICollectionKey<TKey>, ICollectionChild<TParent> {

        private static readonly Dictionary<TChild, TParent> parents = new Dictionary<TChild, TParent>();
        private readonly TParent parent;

        /// <summary>
        /// Constructor de la coleccio.
        /// </summary>
        /// <param name="parent">Pare dels items de la coleccio.</param>
        /// 
        public ParentChildKeyCollection(TParent parent) {

            this.parent = parent;
        }

        /// <summary>
        /// Afegeix un item a la coleccio.
        /// </summary>
        /// <param name="child">El item a afeigir.</param>
        /// 
        public new void Add(TChild child) {

            if (child == null)
                throw new ArgumentNullException("child");

            base.Add(child);
            parents.Add(child, parent);
        }

        /// <summary>
        /// Elimina un fill de la coleccio.
        /// </summary>
        /// <param name="child">El item a eliminar.</param>
        /// 
        public new void Remove(TChild child) {

            if (child == null)
                throw new ArgumentNullException("child");

            base.Remove(child);
            parents.Remove(child);
        }

        /// <summary>
        /// Obte el pare d'un item.
        /// </summary>
        /// <param name="child">El item.</param>
        /// <returns>El pare del item.</returns>
        /// 
        public static TParent GetParent(TChild child) {

            if (child == null)
                throw new ArgumentNullException("child");

            if (parents.TryGetValue(child, out TParent parent))
                return parent;

            else
                return null;
        }
    }
}
