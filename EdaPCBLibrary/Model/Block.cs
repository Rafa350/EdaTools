namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    public sealed class Block : IVisitable, IName {

        private readonly HashSet<Element> elements = new HashSet<Element>();
        private Dictionary<string, PadElement> pads;
        private Dictionary<string, BlockAttribute> attributes;
        private string name;

        /// <summary>
        /// Constructor per defecte de l'objecte.
        /// </summary>
        /// 
        public Block() {
        }

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
                foreach (BlockAttribute attribute in attributes)
                    AddAttribute(attribute);
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

            if (!elements.Add(element))
                throw new InvalidOperationException("El elemento ya pertenece al bloque.");

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

            if (!elements.Contains(element))
                throw new InvalidOperationException("El elemento no pertenece al bloque.");

            elements.Remove(element);

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

            if (pads != null) {
                PadElement pad;
                if (pads.TryGetValue(name, out pad))
                    return pad;
            }

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

            if (attributes != null) {
                BlockAttribute value;
                if (attributes.TryGetValue(name, out value))
                    return value;
            }

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
                name = value;
            }
        }

        /// <summary>
        /// Obte la llista d'elements.
        /// </summary>
        /// 
        public IEnumerable<Element> Elements {
            get {
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
        /// Obte la llista de pads.
        /// </summary>
        /// 
        public IEnumerable<PadElement> Pads {
            get {
                return pads.Values;
            }
        }

        public IEnumerable<string> PadNames {
            get {
                return pads.Keys;
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
        /// Obte els noms dels atributs
        /// </summary>
        /// 
        public IEnumerable<String> AttributeNames {
            get {
                return attributes.Keys;
            }
        }

        /// <summary>
        /// Obte els atributs atributs
        /// </summary>
        /// 
        public IEnumerable<BlockAttribute> Attributes {
            get {
                return attributes.Values;
            }
        }
    }
}
