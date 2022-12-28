namespace MikroPic.EdaTools.v1.Core.Model.Connectivity {

    using System;
    using MikroPic.EdaTools.v1.Core.Model.Board;

    public sealed class ConnectivityItem {

        private readonly EdaElementBase element;

        public ConnectivityItem(EdaElementBase element) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            this.element = element;
        }

        public EdaElementBase Element {
            get {
                return element;
            }
        }
    }
}
