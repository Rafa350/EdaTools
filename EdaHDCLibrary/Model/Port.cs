namespace MikroPic.EdaTools.v1.Hdc.Model {

    using System;

    public sealed class Port {

        private string name;

        public Port(string name) { 
        }

        /// <summary>
        /// Obte o asigna el nom del pin.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }
    }
}
