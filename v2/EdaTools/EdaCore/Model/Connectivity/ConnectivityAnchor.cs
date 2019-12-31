namespace MikroPic.EdaTools.v1.Core.Model.Connectivity {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using System;
    using System.Collections.Generic;

    public sealed class ConnectivityAnchor {

        private Point position;
        private List<ConnectivityItem> items;

        public ConnectivityAnchor(Point position) {

            this.position = position;
        }

        public ConnectivityAnchor(Point position, IEnumerable<ConnectivityItem> items) {

            if (items == null)
                throw new ArgumentNullException("items");

            this.position = position;
            this.items = new List<ConnectivityItem>(items);
        }

        public void AddItem(ConnectivityItem item) {

            if (item == null)
                throw new ArgumentNullException("item");

            if (items == null)
                items = new List<ConnectivityItem>();

            items.Add(item);
        }

        public void RemoveItem(ConnectivityItem item) {

            if (item == null)
                throw new ArgumentNullException("item");

            items.Remove(item);

            if (items.Count == 0)
                items = null;
        }

        public void MoveTo(Point position) {

            this.position = position;
        }
    }
}
