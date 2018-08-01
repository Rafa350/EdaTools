namespace MikroPic.EdaTools.v1.Designer.DrawEditor {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Contenidor d'items de tipus VisualItem.
    /// </summary>
    /// 
    public abstract class DrawingVisualContainer: FrameworkElement {

        private VisualCollection items;

        /// <summary>
        /// Afegeix un item al contenidor.
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void Add(DrawingVisual item) {

            if (item == null)
                throw new ArgumentNullException("item");

            if ((items != null) && items.Contains(item))
                throw new InvalidOperationException("El item ya pertenece a la coleccion.");

            if (items == null)
                items = new VisualCollection(this);

            items.Add(item);
        }

        /// <summary>
        /// Afegeix diversos items al contenidor.
        /// </summary>
        /// <param name="items">Els items a afeigir.</param>
        /// 
        public void Add(IEnumerable<DrawingVisual> items) {

            if (items == null)
                throw new ArgumentNullException("items");

            foreach (DrawingVisual item in items)
                Add(item);
        }

        /// <summary>
        /// Elimina un item del contenidor.
        /// </summary>
        /// <param name="item">El item a eliminar.</param>
        /// 
        public void Remove(DrawingVisual item) {

            if (item == null)
                throw new ArgumentNullException("item");

            if (items == null || !items.Contains(item))
                throw new InvalidOperationException("El item no pertenece a la coleccion.");

            items.Remove(item);

            if (items.Count == 0)
                items = null;
        }

        /// <summary>
        /// Elimina tots els items del contenidor.
        /// </summary>
        /// 
        public void RemoveAll() {

            if (items != null) {
                while (items.Count > 0)
                    items.Remove(items[0]);
                items = null;
            }
        }

        /// <summary>
        /// Obte el item especificat.
        /// </summary>
        /// <param name="index">Index del item a obtenir.</param>
        /// <returns>El item.</returns>
        /// 
        protected override Visual GetVisualChild(int index) {

            if ((items == null) || (index < 0) || (index >= items.Count))
                throw new ArgumentOutOfRangeException("index");

            return items[index];
        }

        /// <summary>
        /// Obte el numero d'items.
        /// </summary>
        /// <returns>El numero d'items.</returns>
        /// 
        protected override int VisualChildrenCount {
            get {
                return items == null ? 0 : items.Count;
            }
        }
    }
}
