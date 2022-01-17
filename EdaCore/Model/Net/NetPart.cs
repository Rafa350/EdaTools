namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;

    public sealed class NetPart: INetVisitable {

        private readonly string name;

        public NetPart(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            this.name = name;
        }

        public void AcceptVisitor(INetVisitor visitor) {

            visitor.Visit(this);
        }

        public string Name {
            get {
                return name;
            }
        }
    }
}
