namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;

    public sealed class NetConnection : INetVisitable {

        private readonly string partName;
        private readonly string padName;

        public NetConnection(string partName, string padName) {

            if (String.IsNullOrEmpty(partName))
                throw new ArgumentNullException("partName");

            if (String.IsNullOrEmpty(padName))
                throw new ArgumentNullException("padName");

            this.partName = partName;
            this.padName = padName;
        }

        public void AcceptVisitor(INetVisitor visitor) {

            visitor.Visit(this);
        }

        public string PartName {
            get {
                return partName;
            }
        }

        public string PadName {
            get {
                return padName;
            }
        }
    }
}
