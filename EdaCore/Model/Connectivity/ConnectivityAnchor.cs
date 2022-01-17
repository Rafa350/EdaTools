namespace MikroPic.EdaTools.v1.Core.Model.Connectivity {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using System;
    using System.Collections.Generic;

    public sealed class ConnectivityAnchor {

        private EdaPoint position;
        private List<ConnectivityItem> items;

        public ConnectivityAnchor(EdaPoint position) {

            this.position = position;
        }

        public ConnectivityAnchor(EdaPoint position, IEnumerable<ConnectivityItem> items) {

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            this.position = position;
            this.items = new List<ConnectivityItem>(items);
        }

        public void AddItem(ConnectivityItem item) {

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (items == null)
                items = new List<ConnectivityItem>();

            items.Add(item);
        }

        public void RemoveItem(ConnectivityItem item) {

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            items.Remove(item);

            if (items.Count == 0)
                items = null;
        }

        public void MoveTo(EdaPoint position) {

            this.position = position;
        }
    }
}
