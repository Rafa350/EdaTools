namespace MikroPic.EdaTools.v1.Hdc.Model {

    using System;
    using System.Collections.Generic;

    public enum PinType {
        Input,
        Output,
        Bidirectional,
        Pasive
    }

    public sealed class DevicePin {

        private string id;
        private string pinName;
        private PinType pinType = PinType.Pasive;

        /// <summary>
        /// Constructor del objecte
        /// </summary>
        /// <param name="id">Identificador del pin.</param>
        /// 
        public DevicePin(string id) {

            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            this.id = id;
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="id">Identificador del pin.</param>
        /// <param name="pinName">Tipus de pin.</param>
        /// <param name="pinType">Nom del pin.</param>
        /// 
        public DevicePin(string id, string pinName, PinType pinType) {

            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException("name");

            this.id = id;
            this.pinName = pinName;
            this.pinType = pinType;
        }

        /// <summary>
        /// Obte o asigna el nom del pin.
        /// </summary>
        /// 
        public string Id {
            get {
                return id;
            }
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

        /// <summary>
        /// Obte o asigna el tipus de pin.
        /// </summary>
        /// 
        public PinType PinType {
            get {
                return pinType;
            }
            set {
                pinType = value;
            }
        }
    }
}
