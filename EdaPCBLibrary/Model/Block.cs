namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Collections;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    public sealed class Block : IVisitable, IName, IKey<String> {

        private ParentChildCollection<Block, Element> elements;
        private KeyCollection<PadElement, String> pads;
        private KeyCollection<BlockAttribute, String> attributes;
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
        /// Clona l'objecte en profunditat.
        /// </summary>
        /// <returns>El clon de l'objecte.</returns>
        /// 
        public Block Clone() {

            Block block = new Block(name);

            if (elements != null)
                foreach (var element in elements)
                    block.AddElement(element.Clone());

            if (attributes != null)
                foreach (var attribute in attributes)
                    block.AddAttribute(attribute.Clone());

            return block;
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

            // Comprova que l'element no estigui afeigit amb anterioritat
            //
            if (element.Block != null)
                throw new InvalidOperationException("El elemento ya pertenece a un bloque.");

            // Afegeix l'element a la llista d'elements
            //
            if (elements == null)
                elements = new ParentChildCollection<Block, Element>(this);
            elements.Add(element);

            // Si l'element es un Pad, l'afegeix la la llista de pads.
            //
            PadElement pad = element as PadElement;
            if (pad != null) {
                if (pads == null)
                    pads = new KeyCollection<PadElement, string>();
                pads.Add(pad);
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

            foreach (var element in elements)
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

            // Comprova que l'element estigui en la llista
            //
            if (element.Block != this)
                throw new InvalidOperationException("El elemento no pertenece a esta bloque.");

            // Elimina l'element de la llista d'elements
            //
            elements.Remove(element);
            if (elements.IsEmpty)
                elements = null;

            // Si l'element es un pad, tambe l'elimina de la llista de pads.
            //
            PadElement pad = element as PadElement;
            if (pads != null) {
                pads.Remove(pad);
                if (pads.IsEmpty)
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

            PadElement pad = pads?.Get(name);

            if ((pad == null) && throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el pad '{0}' en el part '{1}'.", name, this.name));
            else
                return pad;
        }

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
        /// Obte el bloc al que pertany l'element
        /// </summary>
        /// <param name="element">L'element.</param>
        /// <returns>El bloc al que pertany, null si no pertany a cap.</returns>
        /// 
        internal static Block GetBlock(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            return ParentChildCollection<Block, Element>.GetParent(element);
        }

        /// <summary>
        /// Obte la clau.
        /// </summary>
        /// <returns>La clau.</returns>
        /// <remarks>Implementa IKey.GetKey()</remarks>
        /// 
        public string GetKey() {

            return name;
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
        /// Obte la placa a la que pertany.
        /// </summary>
        /// 
        public Board Board {
            get {
                return Board.GetBoard(this); 
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

                return pads;
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

                return attributes;
            }
        }
    }
}
