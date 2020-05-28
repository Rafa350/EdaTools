namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;
    using System.Collections.Generic;

    public sealed class NetSignal : INetVisitable {

        private readonly string name;
        private readonly List<NetConnection> connections;

        public NetSignal(string name, IEnumerable<NetConnection> connections) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            this.name = name;
            if (connections != null)
                this.connections = new List<NetConnection>(connections);
        }

        public void AcceptVisitor(INetVisitor visitor) {

            visitor.Visit(this);
        }

        public string Name {
            get {
                return name;
            }
        }

        public bool HasConnections {
            get {
                return connections != null;
            }
        }

        public IEnumerable<NetConnection> Connections {
            get {
                return connections;
            }
        }
    }
}
