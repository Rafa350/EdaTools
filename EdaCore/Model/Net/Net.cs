namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;

    public sealed partial class Net : INetVisitable {

        public void AcceptVisitor(INetVisitor visitor) {

            visitor.Visit(this);
        }
    }
}
