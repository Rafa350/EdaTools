namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;
    using System.Collections.Generic;

    public sealed partial class Net : INetVisitable {

        public Net(IEnumerable<NetSignal> signals) {

            if (signals == null)
                throw new ArgumentNullException(nameof(signals));

            InitializeSignals(signals);
        }

        public void AcceptVisitor(INetVisitor visitor) {

            visitor.Visit(this);
        }
    }
}
