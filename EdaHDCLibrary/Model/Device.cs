namespace MikroPic.EdaTools.v1.Hdc.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Device {

        private string name;
        private List<DevicePin> pins;

        public Device(string name, IEnumerable<DevicePin> pins = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;

            if (pins != null)
                AddPins(pins);
        }

        public void AddPin(DevicePin pin) {

            if (pin == null)
                throw new ArgumentNullException("pin");

            if (pins == null)
                pins = new List<DevicePin>();

            pins.Add(pin);
        }

        public void AddPins(IEnumerable<DevicePin> pins) {

            if (pins == null)
                throw new ArgumentNullException("pins");

            foreach (var pin in pins)
                AddPin(pin);
        }
    }
}
