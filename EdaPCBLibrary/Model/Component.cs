namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Model.Collections;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;

    public sealed class Component: IVisitable {

        private readonly ElementCollection elements = new ElementCollection();
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
        public Component(string name, IEnumerable<Element> elements) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (elements == null)
                throw new ArgumentNullException("elements");

            this.name = name;
            this.elements.Add(elements);
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
        public ElementCollection Elements {
            get {
                return elements;
            }
        }
    }
}
