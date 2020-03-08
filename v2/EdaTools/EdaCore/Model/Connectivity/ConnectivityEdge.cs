namespace MikroPic.EdaTools.v1.Core.Model.Connectivity {

    using System;

    public sealed class ConnectivityEdge {

        private readonly ConnectivityAnchor anchorA;
        private readonly ConnectivityAnchor anchorB;

        public ConnectivityEdge(ConnectivityAnchor anchorA, ConnectivityAnchor anchorB) {

            if (anchorA == null)
                throw new ArgumentNullException(nameof(anchorA));

            if (anchorB == null)
                throw new ArgumentNullException(nameof(anchorB));

            this.anchorA = anchorA;
            this.anchorB = anchorB;
        }

        public ConnectivityAnchor AnchorA {
            get {
                return anchorA;
            }
        }

        public ConnectivityAnchor AnchorB {
            get {
                return anchorB;
            }
        }
    }
}
