using System;
using System.Collections.Generic;
using System.Linq;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed partial class EdaBoard {

        private Dictionary<string, EdaDevice> _devices;

        /// <summary>
        /// Afegeix un component.
        /// </summary>
        /// <param name="device">El component a afeigir.</param>
        /// 
        public void AddDevice(EdaDevice device) {

            if (device == null)
                throw new ArgumentNullException(nameof(device));

            if ((_devices != null) && _devices.ContainsKey(device.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}' ya pertenece a la placa.", device.Name));

            if (_devices == null)
                _devices = new Dictionary<string, EdaDevice>();

            _devices.Add(device.Name, device);
        }

        /// <summary>
        /// Afegeix una coleccio de components.
        /// </summary>
        /// <param name="devices">Els components a afeigir</param>
        /// 
        public void AddDevices(IEnumerable<EdaDevice> devices) {

            if (devices == null)
                throw new ArgumentException(nameof(devices));

            foreach (var device in devices)
                AddDevice(device);
        }

        /// <summary>
        /// Elimina una component de la placa.
        /// </summary>
        /// <param name="device">La peça a eliminar.</param>
        /// 
        public void RemoveDevice(EdaDevice device) {

            if (device == null)
                throw new ArgumentNullException(nameof(device));

            if ((_devices == null) || !_devices.ContainsKey(device.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', no se encontro en la placa.", device.Name));

            _devices.Remove(device.Name);

            if (_devices.Count == 0)
                _devices = null;
        }

        /// <summary>
        /// Obte un component pel seu nom.
        /// </summary>
        /// <param name="name">El nom del component a buscar.</param>
        /// <param name="throwOnError">True si cal generar una exceptio si no el troba.</param>
        /// <returns>El component, o null si no el troba.</returns>
        /// 
        public EdaDevice GetDevice(string name, bool throwOnError = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if ((_devices != null) && _devices.TryGetValue(name, out var device))
                return device;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', no se encontro en esta placa.", name));

            else
                return null;
        }

        /// <summary>
        /// Indica si conte components
        /// </summary>
        /// 
        public bool HasDevices =>
            _devices != null;

        /// <summary>
        /// Obte el nombre de components.
        /// </summary>
        /// 
        public int DeviceCount =>
            _devices == null ? 0 : _devices.Count;

        /// <summary>
        /// Enumera els noms dels components
        /// </summary>
        /// 
        public IEnumerable<string> DeviceNames =>
            _devices == null ? Enumerable.Empty<string>() : _devices.Keys;

        /// <summary>
        /// Obte un enumerador pels components.
        /// </summary>
        /// 
        public IEnumerable<EdaDevice> Devices =>
            _devices == null ? Enumerable.Empty<EdaDevice>() : _devices.Values;
    }
}
