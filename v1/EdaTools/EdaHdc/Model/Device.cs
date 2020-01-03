namespace MikroPic.EdaTools.v1.Hdc.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Device {

        private string id;
        private List<DevicePin> pins;
        private Dictionary<string, object> attributes;

        public Device(string id) {

            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException("name");

            this.id = id;
        }

        /// <summary>
        /// Afegeix un pin.
        /// </summary>
        /// <param name="pin">El pin a afeigir.</param>
        /// 
        public void AddPin(DevicePin pin) {

            if (pin == null)
                throw new ArgumentNullException("pin");

            if (pins == null)
                pins = new List<DevicePin>();

            pins.Add(pin);
        }

        /// <summary>
        /// Afegeix una col·leccio de pins.
        /// </summary>
        /// <param name="pins">Els pins a afeigir.</param>
        /// 
        public void AddPins(IEnumerable<DevicePin> pins) {

            if (pins == null)
                throw new ArgumentNullException("pins");

            foreach (var pin in pins)
                AddPin(pin);
        }

        public void DefineAttribute(string name, object value = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (attributes == null)
                attributes = new Dictionary<string, object>();
            else {
                if (attributes.ContainsKey(name))
                    throw new InvalidOperationException(String.Format("El atributo '{0}' ya esta definido.", name));
            }

            attributes.Add(name, value);
        }

        public void SetAttribute(string name, object value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if ((attributes == null) || !attributes.ContainsKey(name))
                throw new InvalidOperationException(String.Format("No existe el atributo '{0}'.", name));

            attributes[name] = value;
        }

        public object GetAttribute(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if ((attributes == null) || !attributes.ContainsKey(name))
                throw new InvalidOperationException(String.Format("No existe el atributo '{0}'.", name));

            return attributes[name];
        }

        /// <summary>
        /// Obte el identificador.
        /// </summary>
        /// 
        public string Id {
            get {
                return id;
            }
        }
    }
}
