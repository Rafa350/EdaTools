namespace MikroPic.EdaTools.v1.Core.Model.Net.Visitors {

    public class NetDefaultVisitor : INetVisitor {

        public virtual void Visit(Net net) {

            if (net.HasSignals)
                foreach (var signal in net.Signals)
                    signal.AcceptVisitor(this);
        }

        public virtual void Visit(NetSignal signal) {

            if (signal.HasConnections)
                foreach (var pin in signal.Connections)
                    pin.AcceptVisitor(this);
        }

        public virtual void Visit(NetConnection pin) {
        }
    }
}
