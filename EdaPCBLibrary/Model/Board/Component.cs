namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Collections;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    public sealed partial class Component : IVisitable, IName, ICollectionKey<String>, ICollectionChild<Board> {

        private Board board;
        private string name;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Identificador del bloc.</param>
        /// <param name="elements">Llista d'elements.</param>
        /// <param name="attributes">Llista d'atributs.</param>
        /// 
        public Component(string name, IEnumerable<Element> elements = null, IEnumerable<ComponentAttribute> attributes = null) {

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
        public Component Clone() {

            Component block = new Component(name);

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
        /// Obte la clau.
        /// </summary>
        /// <returns>La clau.</returns>
        /// <remarks>Implementa ICollectionKey.GetKey()</remarks>
        /// 
        public string GetKey() {

            return name;
        }

        /// <summary>
        /// Asigna l'objecte pare.
        /// </summary>
        /// <param name="board">L'objete pare a asignar.</param>
        /// <remarks>Implementa ICollectionChild.AssignParent(Board)</remarks>
        /// 
        public void AssignParent(Board board) {

            this.board = board;
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
                return board; 
            }
        }
    }
}
