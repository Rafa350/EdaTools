namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;

    public sealed class NetPin {

        private readonly string name;

        public NetPin(string name) {

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
