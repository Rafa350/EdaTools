﻿namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Block: IVisitable {

        private static readonly Dictionary<Element, Block> elementOwners = new Dictionary<Element, Block>();
        private readonly List<Element> elements = new List<Element>();
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
        /// <param name="name">Identificador del component.</param>
        /// <param name="elements">Llista d'elements que formen el component.</param>
        /// 
        public Block(string name, IEnumerable<Element> elements = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;

            if (elements != null)
                foreach (Element element in elements)
                    AddElement(element);
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
        /// Afeigeix un element al component.
        /// </summary>
        /// <param name="element">El element a afeigir.</param>
        /// 
        public void AddElement(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (elements.Contains(element))
                throw new InvalidOperationException("El elemento ya pertenece al componente.");

            if (elementOwners.ContainsKey(element))
                throw new InvalidOperationException("El elemento ya pertenece a otro componente.");

            elements.Add(element);
            elementOwners.Add(element, this);
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
                throw new InvalidOperationException("El elemento no pertenece al componente.");

            elements.Remove(element);
            elementOwners.Remove(element);
        }

        /// <summary>
        /// Obte el component al que pertany el element.
        /// </summary>
        /// <param name="element">El element.</param>
        /// <returns>El component al que pertany.</returns>
        /// 
        public static Block ComponentOf(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            Block component = null;
            if (elementOwners.TryGetValue(element, out component))
                return component;
            else
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
    }
}
