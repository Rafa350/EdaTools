namespace MikroPic.EdaTools.v1.Hdc.Model {

    using System;

    public enum PortType {
        Input,
        Output,
        Bidirectional,
        Pasive
    }

    public abstract class Port {

        private string name;
        private PortType portType = PortType.Pasive;

        public Port(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
        }

        public Port(string name, PortType portType) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.portType = portType;
        }

        /// <summary>
        /// Obte o asigna el nom del port.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Name");
                name = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tipus de port.
        /// </summary>
        /// 
        public PortType PortType {
            get {
                return portType;
            }
            set {
                portType = value;
            }
        }
    }
}
