namespace MikroPic.EdaTools.v1.Hdc.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Device {

        private string name;

        public Device(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
        }
    }
}
