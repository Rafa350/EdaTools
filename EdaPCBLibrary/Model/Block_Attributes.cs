namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Model.Collections;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    public sealed partial class Block {

        private KeyCollection<BlockAttribute, String> attributes;

        /// <summary>
        /// Afegeix in atribut.
        /// </summary>
        /// <param name="attribute">L'atribut a afeigir.</param>
        /// 
        public void AddAttribute(BlockAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException("attribute");

            // Comprova que l'atribut no estigui afeigit amb anterioritat
            //
            if ((attributes != null) && attributes.Contains(attribute))
                throw new InvalidOperationException(
                    String.Format("Ya existe un atributo con el nombre '{0}'.", attribute.Name));

            // Afegeix l'atribut a la llista d'atributs
            //
            if (attributes == null)
                attributes = new KeyCollection<BlockAttribute, string>();
            attributes.Add(attribute);
        }

        /// <summary>
        /// Afegeis una col·leccio d'atributs.
        /// </summary>
        /// <param name="attributes">La col·leccio d'atributs.</param>
        /// 
        public void AddAttributes(IEnumerable<BlockAttribute> attributes) {

            if (Attributes == null)
                throw new ArgumentNullException("attributes");

            foreach (BlockAttribute attribute in attributes)
                AddAttribute(attribute);
        }

        /// <summary>
        /// Elimina un atribut.
        /// </summary>
        /// <param name="attribute">L'atribut a eliminar.</param>
        /// 
        public void RemoveAttribute(BlockAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException("attribute");

            // Comprova que l'atribut estigui a la llista
            //
            if ((attributes == null) || !attributes.Contains(attribute))
                throw new InvalidOperationException(
                    String.Format("No se encontro el atributo '{0}'.", attribute.Name));

            // Elimina l'aqtribut de la llista d'atributs
            //
            attributes.Remove(attribute);
            if (attributes.IsEmpty)
                attributes = null;
        }

        /// <summary>
        /// Elimina tots els atributs.
        /// </summary>
        /// 
        public void RemoveAllAttributes() {

            if (attributes != null) {
                attributes.Clear();
                attributes = null;
            }
        }

        /// <summary>
        /// Obte un atribut pel seu nom.
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <returns>L'atribut, null si no el troba.</returns>
        /// 
        public BlockAttribute GetAttribute(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return attributes?.Get(name);
        }

        /// <summary>
        /// Indica si te atributs.
        /// </summary>
        /// 
        public bool HasAttributes {
            get {
                return attributes != null;
            }
        }

        /// <summary>
        /// Enumera els noms dels atributs
        /// </summary>
        /// 
        public IEnumerable<String> AttributeNames {
            get {
                if (attributes == null)
                    throw new InvalidOperationException("El bloque no contiene atributos.");

                return attributes.Keys;
            }
        }

        /// <summary>
        /// Enumera els atributs
        /// </summary>
        /// 
        public IEnumerable<BlockAttribute> Attributes {
            get {
                if (attributes == null)
                    throw new InvalidOperationException("El bloque no contiene atributos.");

                return attributes;
            }
        }
    }
}
