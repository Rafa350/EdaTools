namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board {

        private Dictionary<string, Part> _parts;

        /// <summary>
        /// Afegeix un component.
        /// </summary>
        /// <param name="part">El component a afeigir.</param>
        /// 
        public void AddPart(Part part) {

            if (part == null)
                throw new ArgumentNullException(nameof(part));

            if ((_parts != null) && _parts.ContainsKey(part.Name))
                throw new InvalidOperationException(
                    String.Format("El elemento '{0}' ya pertenece a la placa.", part.Name));

            if (!_components.ContainsValue(part.Component))
                throw new InvalidOperationException(
                    String.Format("El el elemento '{0}', hace referencia a un componente '{1}', que no pertenece a la placa.", 
                    part.Name, part.Component.Name));

            if (_parts == null)
                _parts = new Dictionary<string, Part>();
            _parts.Add(part.Name, part);
        }

        /// <summary>
        /// Afegeix una coleccio de components.
        /// </summary>
        /// <param name="parts">Els components a afeigir</param>
        /// 
        public void AddParts(IEnumerable<Part> parts) {

            if (parts == null)
                throw new ArgumentException(nameof(parts));

            foreach (var part in parts)
                AddPart(part);
        }

        /// <summary>
        /// Elimina una component de la placa.
        /// </summary>
        /// <param name="part">La peça a eliminar.</param>
        /// 
        public void RemovePart(Part part) {

            if (part == null)
                throw new ArgumentNullException(nameof(part));

            if ((_parts == null) || !_parts.ContainsKey(part.Name))
                throw new InvalidOperationException(
                    String.Format("El elemento '{0}', no se encontro en la placa.", part.Name));

            _parts.Remove(part.Name);
            if (_parts.Count == 0)
                _parts = null;
        }

        /// <summary>
        /// Obte un component pel seu nom.
        /// </summary>
        /// <param name="name">El nom del component a buscar.</param>
        /// <param name="throwOnError">True si cal generar una exceptio si no el troba.</param>
        /// <returns>El component, o null si no el troba.</returns>
        /// 
        public Part GetPart(string name, bool throwOnError = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if ((_parts != null) && _parts.TryGetValue(name, out var part))
                return part;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', no se encontro en esta placa.", name));

            else
                return null;
        }

        /// <summary>
        /// Indica si conte parts.
        /// </summary>
        /// 
        public bool HasParts => _parts != null;

        /// <summary>
        /// Obte un enumerador pels noms dels components.
        /// </summary>
        /// 
        public IEnumerable<string> PartNames => _parts?.Keys;

        /// <summary>
        /// Obte un enumerador pels components.
        /// </summary>
        /// 
        public IEnumerable<Part> Parts => _parts?.Values;
    }
}
