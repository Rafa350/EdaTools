namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;

    public sealed class NetElement {

        private readonly string name;

        public NetElement(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
        }

        public string Name {
            get {
                return name;
            }
        }
    }
}
