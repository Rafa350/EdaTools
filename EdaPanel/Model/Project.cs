namespace MikroPic.EdaTools.v1.Panel.Model {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Collections;
    using System;
    using System.Collections.Generic;

    public sealed class Project: IPanelVisitable {

        private Size size;
        private Collection<PanelItem> items;

        /// <summary>
        /// Afegeix un element
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void AddElement(PanelItem item) {

            if (item == null)
                throw new ArgumentNullException("item");

            if (items == null)
                items = new Collection<PanelItem>();

            items.Add(item);
        }

        public void  AcceptVisitor(IPanelVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Afegeix una coleccio d'items.
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<PanelItem> item) {

            if (item == null)
                throw new ArgumentNullException("item");

            foreach (var element in item)
                AddElement(element);
        }

        /// <summary>
        /// Indica si te items
        /// </summary>
        /// 
        public bool HasItems {
            get {
                return items != null;
            }
        }

        /// <summary>
        /// Enumera els elements
        /// </summary>
        /// 
        public IEnumerable<PanelItem> Items {
            get {
                return items;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany del panell
        /// </summary>
        /// 
        public Size Size {
            get {
                return size;
            }
            set {
                size = value;
            }
        }
    }
}
