using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Panel.Model {

    public sealed class EdaPanel: IEdaPanelVisitable {

        private EdaSize _size;
        private List<EdaPanelItem> _items;

        public void AcceptVisitor(IEdaPanelVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Afegeix un element
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void AddItem(EdaPanelItem item) {

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_items == null)
                _items = new List<EdaPanelItem>();

            _items.Add(item);
        }

        /// <summary>
        /// Afegeix una coleccio d'items.
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void AddItems(IEnumerable<EdaPanelItem> item) {

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
        public IEnumerable<EdaPanelItem> Items =>
            _items;

        /// <summary>
        /// Obte o asigna el tamany del panell
        /// </summary>
        /// 
        public EdaSize Size {
            get => _size;
            set => _size = value;
        }
    }
}
