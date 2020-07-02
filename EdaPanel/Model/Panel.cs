namespace MikroPic.EdaTools.v1.Panel.Model {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Collections;

    public sealed class Panel : IPanelVisitable {

        private Size _size;
        private Collection<PanelItem> _items;

        public void AcceptVisitor(IPanelVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Afegeix un element
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void AddItem(PanelItem item) {

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_items == null)
                _items = new Collection<PanelItem>();

            _items.Add(item);
        }

        /// <summary>
        /// Afegeix una coleccio d'items.
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void AddItems(IEnumerable<PanelItem> item) {

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            foreach (var element in item)
                AddItem(element);
        }

        /// <summary>
        /// Indica si te items
        /// </summary>
        /// 
        public bool HasItems => 
            _items != null;

        /// <summary>
        /// Enumera els elements
        /// </summary>
        /// 
        public IEnumerable<PanelItem> Items => _items;

        /// <summary>
        /// Obte o asigna el tamany del panell
        /// </summary>
        /// 
        public Size Size {
            get {
                return _size;
            }
            set {
                _size = value;
            }
        }
    }
}
