namespace MikroPic.EdaTools.v1.Core.Model.Conectivity {

    using System;

    public sealed class ConnectivityEdge {

        private readonly ConnectivityAnchor anchorA;
        private readonly ConnectivityAnchor anchorB;

        public ConnectivityEdge(ConnectivityAnchor anchorA, ConnectivityAnchor anchorB) {

            if (anchorA == null)
                throw new ArgumentNullException("anchorA");

            if (anchorB == null)
                throw new ArgumentNullException("anchorB");

            this.anchorA = anchorA;
            this.anchorB = anchorB;
        }

        public ConnectivityAnchor AnchorA {
            get {
                return anchorA;
            }
        }

        public ConnectivityAnchor AnchorB{
            get {
                return anchorB;
            }
        }
    }
}
