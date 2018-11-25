namespace MikroPic.EdaTools.v1.Hdc.Model {

    using System;

    public sealed class DevicePort: Port {

        private string pinName;

        public DevicePort(string name):
            base(name) {
        }

        public DevicePort(string name, PortType portType, string pinName):
            base(name, portType) {

            if (String.IsNullOrEmpty(pinName))
                throw new ArgumentNullException("pinName");

            this.pinName = pinName;
        }

        /// <summary>
        /// Obte o asigna el nom del pin.
        /// </summary>
        /// 
        public string PinName {
            get {
                return pinName;
            }
            set {
                pinName = value;
            }
        }
    }
}
