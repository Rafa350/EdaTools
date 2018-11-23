namespace MikroPic.EdaTools.v1.Hdc.Model {

    using System;

    public enum PortType {
        Input,
        Output,
        Bidirectional,
        Passive
    }

    public sealed class Port {

        private string name;
        private PortType portType = PortType.Passive;
        private string pinName;

        public Port(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
        }

        public Port(string name, PortType portType, string pinName) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.portType = portType;
            this.pinName = pinName;
        }

        public string Name {
            get {
                return name;
            }
        }

        public PortType PortType {
            get {
                return portType;
            }
        }

        public string PinName {
            get {
                return pinName;
            }
        }
    }
}
