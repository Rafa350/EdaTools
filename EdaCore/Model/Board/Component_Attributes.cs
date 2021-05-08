using System;
using System.Collections.Generic;
using System.Linq;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    public sealed partial class Component {

        private Dictionary<string, ComponentAttribute> _attributes;

        /// <summary>
        /// Afegeix in atribut.
        /// </summary>
        /// <param name="attribute">L'atribut a afeigir.</param>
        /// 
        public void AddAttribute(ComponentAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            // Comprova que l'atribut no estigui afeigit amb anterioritat
            //
            if ((_attributes != null) && _attributes.ContainsKey(attribute.Name))
                throw new InvalidOperationException(
                    String.Format("Ya existe un atributo con el nombre '{0}'.", attribute.Name));

            // Afegeix l'atribut a la llista d'atributs
            //
            if (_attributes == null)
                _attributes = new Dictionary<string, ComponentAttribute>();
            _attributes.Add(attribute.Name, attribute);
        }

        /// <summary>
        /// Afegeis una col·leccio d'atributs.
        /// </summary>
        /// <param name="attributes">La col·leccio d'atributs.</param>
        /// 
        public void AddAttributes(IEnumerable<ComponentAttribute> attributes) {

            if (Attributes == null)
                throw new ArgumentNullException(nameof(attributes));

            foreach (ComponentAttribute attribute in attributes)
                AddAttribute(attribute);
        }

        /// <summary>
        /// Elimina un atribut.
        /// </summary>
        /// <param name="attribute">L'atribut a eliminar.</param>
        /// 
        public void RemoveAttribute(ComponentAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            // Comprova que l'atribut estigui a la llista
            //
            if ((_attributes == null) || !_attributes.ContainsKey(attribute.Name))
                throw new InvalidOperationException(
                    String.Format("No se encontro el atributo '{0}'.", attribute.Name));

            // Elimina l'aqtribut de la llista d'atributs
            //
            _attributes.Remove(attribute.Name);
            if (_attributes.Count == 0)
                _attributes = null;
        }

        /// <summary>
        /// Elimina tots els atributs.
        /// </summary>
        /// 
        public void RemoveAllAttributes() {

            if (_attributes != null) {
                _attributes.Clear();
                _attributes = null;
            }
        }

        /// <summary>
        /// Obte un atribut pel seu nom.
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <returns>L'atribut, null si no el troba.</returns>
        /// 
        public ComponentAttribute GetAttribute(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            return _attributes?[name];
        }

        /// <summary>
        /// Indica si te atributs.
        /// </summary>
        /// 
        public bool HasAttributes =>
            _attributes != null;

        /// <summary>
        /// Enumera els noms dels atributs
        /// </summary>
        /// 
        public IEnumerable<String> AttributeNames =>
            _attributes == null ? Enumerable.Empty<string>() : _attributes.Keys;

        /// <summary>
        /// Enumera els atributs
        /// </summary>
        /// 
        public IEnumerable<ComponentAttribute> Attributes =>
            _attributes == null ? Enumerable.Empty<ComponentAttribute>() : _attributes?.Values;
    }
}
