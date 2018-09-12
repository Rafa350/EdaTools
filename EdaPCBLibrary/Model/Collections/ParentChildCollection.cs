namespace MikroPic.EdaTools.v1.Pcb.Model.Collections {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Coleccio que mante una relacio pare-fill amb els seus items.
    /// </summary>
    /// <typeparam name="TParent">Tipus del pare.</typeparam>
    /// <typeparam name="TChild">Tipus del fill.</typeparam>
    /// 
    public sealed class ParentChildCollection<TParent, TChild> : Collection<TChild>
        where TParent: class
        where TChild: class, ICollectionChild<TParent> {

        private readonly TParent parent;

        /// <summary>
        /// Constructor de la coleccio.
        /// </summary>
        /// <param name="parent">Pare dels items de la coleccio.</param>
        /// 
        public ParentChildCollection(TParent parent) {

            if (parent == null)
                throw new ArgumentNullException("parent");

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
            child.AssignParent(parent);
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
            child.AssignParent(null);
        }
    }
}
