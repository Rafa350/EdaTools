namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    public sealed class Block: IVisitable {

        private readonly HashSet<Element> elements = new HashSet<Element>();
        private readonly Dictionary<string, BlockAttribute> attributes = new Dictionary<string, BlockAttribute>();
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
                if (!this.elements.Add(element))
                    throw new InvalidOperationException("El elemento ya pertenece al bloque.");
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
        }

        public void AddAttribute(BlockAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException("attribute");

            attributes.Add(attribute.Name, attribute);
        }

        public void RemoveAttribute(BlockAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException("attribute");

            attributes.Remove(attribute.Name);
        }

        public BlockAttribute GetAttribute(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            BlockAttribute value;
            if (attributes.TryGetValue(name, out value))
                return value;

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
