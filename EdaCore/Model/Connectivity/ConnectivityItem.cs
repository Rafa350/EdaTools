namespace MikroPic.EdaTools.v1.Core.Model.Connectivity {

    using System;
    using MikroPic.EdaTools.v1.Core.Model.Board;

    public sealed class ConnectivityItem {

        private readonly EdaElement element;

        public ConnectivityItem(EdaElement element) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            this.element = element;
        }

        public EdaElement Element {
            get {
                return element;
            }
        }
    }
}
