namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;
    using System.Collections.Generic;

    public sealed class NetComponent  {

        private readonly string name;
        private List<NetPin> pins;

        public NetComponent(string name) {

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
