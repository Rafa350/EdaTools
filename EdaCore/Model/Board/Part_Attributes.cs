namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections.Generic;

    public partial class Part {

        private Dictionary<string, PartAttribute> attributes;

        /// <summary>
        /// Afegeix un atribut.
        /// </summary>
        /// <param name="attribute">L'atribut a afeigir.</param>
        /// 
        public void AddAttribute(PartAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            if (attributes == null)
                attributes = new Dictionary<string, PartAttribute>();
            attributes.Add(attribute.Name, attribute);
        }

        /// <summary>
        /// Afegeix una coleccio d'atributs
        /// </summary>
        /// <param name="attributes">La coleccio d'atributs.</param>
        /// 
        public void AddAttributes(IEnumerable<PartAttribute> attributes) {

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
        public void RemoveAttribute(PartAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            attributes.Remove(attribute.Name);
            if (attributes.Count == 0)
                attributes = null;
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <returns>El seu valor. Null si no existeix.</returns>
        /// 
        public PartAttribute GetAttribute(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if ((attributes != null) && attributes.TryGetValue(name, out PartAttribute attribute))
                return attribute;

            return null;
        }

        /// <summary>
        /// Indica si conte atributs.
        /// </summary>
        /// 
        public bool HasAttributes {
            get {
                return attributes != null;
            }
        }

        /// <summary>
        /// Obte la llista d'atributs.
        /// </summary>
        /// 
        public IEnumerable<PartAttribute> Attributes {
            get {
                return attributes?.Values;
            }
        }
    }
}
