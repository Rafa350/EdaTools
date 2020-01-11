namespace MikroPic.EdaTools.v1.Core.Model.Connectivity {

    using System;
    using MikroPic.EdaTools.v1.Core.Model.Board;

    public sealed class ConnectivityItem {

        private readonly Element element;

        public ConnectivityItem(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            this.element = element;
        }

        public Element Element {
            get {
                return element;
            }
        }
    }
}
