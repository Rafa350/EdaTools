namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    public sealed class Block : IVisitable, IName {

        private HashSet<Element> elements;
        private Dictionary<string, PadElement> pads;
        private Dictionary<string, BlockAttribute> attributes;
        private string name;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Identificador del bloc.</param>
        /// <param name="elements">Llista d'elements.</param>
        /// <param name="attributes">Llista d'atributs.</param>
        /// 
        public Block(string name, IEnumerable<Element> elements = null, IEnumerable<BlockAttribute> attributes = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;

            if (elements != null)
                AddElements(elements);

            if (attributes != null)
                AddAttributes(attributes);
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Afeigeix un element.
        /// </summary>
        /// <param name="element">El element a afeigir.</param>
        /// 
        public void AddElement(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (elements == null)
                elements = new HashSet<Element>();

            if (!elements.Add(element))
                throw new InvalidOperationException("El elemento ya pertenece al bloque.");

            // Si l'element es un Pad, l'afegeix la la llista de pads.
            //
            PadElement pad = element as PadElement;
            if (pad != null) {
                if (pads == null)
                    pads = new Dictionary<string, PadElement>();
                pads.Add(pad.Name, pad);
            }
        }

        /// <summary>
        /// Afeigeix una coleccio d'elements.
        /// </summary>
        /// <param name="element">El elements a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<Element> elements) {

            if (elements == null)
                throw new ArgumentNullException("elements");

            foreach (Element element in elements)
                AddElement(element);
        }

        /// <summary>
        /// Elimina un element del component.
        /// </summary>
        /// <param name="element">El element a eliminar.</param>
        /// 
        public void RemoveElement(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if ((elements == null) || !elements.Contains(element))
                throw new InvalidOperationException("El elemento no pertenece al bloque.");

            elements.Remove(element);
            if (elements.Count == 0)
                elements = null;

            // Si l'element es un pad, tambe l'elimina de la llista de pads.
            //
            PadElement pad = element as PadElement;
            if (pads != null) {
                pads.Remove(pad.Name);
                if (pads.Count == 0)
                    pads = null;
            }
        }

        /// <summary>
        /// Obte un pad pel seu nom.
        /// </summary>
        /// <param name="name">El nom del pad.</param>
        /// <param name="throwOnError">True si dispara una execpcio si no el troba.</param>
        /// <returns>El pad. Null si no el troba.</returns>
        /// 
        public PadElement GetPad(string name, bool throwOnError = true) {

            if ((pads != null) && pads.TryGetValue(name, out PadElement pad))
                return pad;

            if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el pad '{0}' en el part '{1}'.", name, this.name));

            return null;
        }

        /// <summary>
        /// Afegeix in atribut.
        /// </summary>
        /// <param name="attribute">L'atribut a afeigir.</param>
        /// 
        public void AddAttribute(BlockAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException("attribute");

            if (attributes == null)
                attributes = new Dictionary<string, BlockAttribute>();

            attributes.Add(attribute.Name, attribute);
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

            attributes.Remove(attribute.Name);

            if (attributes.Count == 0)
                attributes = null;
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

            if ((attributes != null) && attributes.TryGetValue(name, out BlockAttribute attribute))
                return attribute;

            return null;
        }

        /// <summary>
        /// Obte o asigna el identificador del component.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Block.Name");

                name = value;
            }
        }

        /// <summary>
        /// Indica si conte elements.
        /// </summary>
        /// 
        public bool HasElements {
            get {
                return elements != null;
            }
        }

        /// <summary>
        /// Obte la llista d'elements.
        /// </summary>
        /// 
        public IEnumerable<Element> Elements {
            get {
                if (elements == null)
                    throw new InvalidOperationException("El bloque no contiene elementos.");

                return elements;
            }
        }

        /// <summary>
        /// Indica si conte pads.
        /// </summary>
        /// 
        public bool HasPads {
            get {
                return pads != null;
            }
        }

        /// <summary>
        /// Enumera el nom dels pads.
        /// </summary>
        /// 
        public IEnumerable<string> PadNames {
            get {
                if (pads == null)
                    throw new InvalidOperationException("El bloque no contiene pads.");

                return pads.Keys;
            }
        }

        /// <summary>
        /// Enumera els pads.
        /// </summary>
        /// 
        public IEnumerable<PadElement> Pads {
            get {
                if (pads == null)
                    throw new InvalidOperationException("El bloque no contiene pads.");

                return pads.Values;
            }
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

                return attributes.Values;
            }
        }
    }
}
