namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;

    public sealed class Component: IVisitable {

        private static readonly Dictionary<IComponentElement, Component> elementOwners = new Dictionary<IComponentElement, Component>();
        private readonly List<IComponentElement> elements = new List<IComponentElement>();
        private string name;

        /// <summary>
        /// Constructor per defecte de l'objecte.
        /// </summary>
        /// 
        public Component() {

        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Identificador del component.</param>
        /// <param name="elements">Llista d'elements que formen el component.</param>
        /// 
        public Component(string name, IEnumerable<Element> elements = null) {

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
        public void AddElement(IComponentElement element) {

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
        public void RemoveElement(IComponentElement element) {

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
        public static Component ComponentOf(IComponentElement element) {

            if (element == null)
                throw new ArgumentNullException("element");

            Component component = null;
            if (elementOwners.TryGetValue(element, out component))
                return component;
            else
                return null;
        }

        /// <summary>
        /// Obte la placa a la que pertany el component
        /// </summary>
        /// 
        public Board Board {
            get {
                return Board.BoardOf(this);
            }
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
        /// Indica si conte elements.
        /// </summary>
        /// 
        public bool HasElements {
            get {
                return elements.Count > 0;
            }
        }

        /// <summary>
        /// Obte la llista d'elements.
        /// </summary>
        /// 
        public IEnumerable<IComponentElement> Elements {
            get {
                return elements;
            }
        }
    }
}
