﻿using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public partial class EdaPart {

        private Dictionary<string, EdaPartAttribute> _attributes;

        /// <summary>
        /// Afegeix un atribut.
        /// </summary>
        /// <param name="attribute">L'atribut a afeigir.</param>
        /// 
        public void AddAttribute(EdaPartAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            if (_attributes == null)
                _attributes = new Dictionary<string, EdaPartAttribute>();
            _attributes.Add(attribute.Name, attribute);
        }

        /// <summary>
        /// Afegeix una coleccio d'atributs
        /// </summary>
        /// <param name="attributes">La coleccio d'atributs.</param>
        /// 
        public void AddAttributes(IEnumerable<EdaPartAttribute> attributes) {

            if (attributes == null)
                throw new ArgumentNullException(nameof(attributes));

            foreach (var attribute in attributes)
                AddAttribute(attribute);
        }

        /// <summary>
        /// Elimina un atribut.
        /// </summary>
        /// <param name="attribute">L'atribut a eliminar.</param>
        /// 
        public void RemoveAttribute(EdaPartAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            _attributes.Remove(attribute.Name);
            if (_attributes.Count == 0)
                _attributes = null;
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <returns>El seu valor. Null si no existeix.</returns>
        /// 
        public EdaPartAttribute GetAttribute(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if ((_attributes != null) && _attributes.TryGetValue(name, out EdaPartAttribute attribute))
                return attribute;

            return null;
        }

        /// <summary>
        /// Indica si conte atributs.
        /// </summary>
        /// 
        public bool HasAttributes =>
            _attributes != null;

        /// <summary>
        /// Obte la llista d'atributs.
        /// </summary>
        /// 
        public IEnumerable<EdaPartAttribute> Attributes =>
            _attributes?.Values;
    }
}
