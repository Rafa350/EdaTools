namespace MikroPic.EdaTools.v1.Core.Model.Net.Visitors {

    public class DefaultNetVisitor : INetVisitor {

        public virtual void Visit(Net net) {

            if (net.HasParts)
                foreach (var part in net.Parts)
                    part.AcceptVisitor(this);

            if (net.HasSignals)
                foreach (var signal in net.Signals)
                    signal.AcceptVisitor(this);
        }

        public virtual void Visit(NetSignal signal) {

            if (signal.HasConnections)
                foreach (var pin in signal.Connections)
                    pin.AcceptVisitor(this);
        }

        public virtual void Visit(NetConnection connection) {
        }

        public virtual void Visit(NetPart part) {
        }
    }
}
