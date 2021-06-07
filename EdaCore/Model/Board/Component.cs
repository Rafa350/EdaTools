using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Core.Model.Board.IO.Serializers;
using MikroPic.EdaTools.v1.Core.Model.Common;
using NetSerializer.Attributes;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    [NetSerializer(typeof(ComponentSerializer), AliasName = "Component")]
    public sealed partial class Component : IVisitable<IBoardVisitor>, IName {

        private readonly string _name;
        private string _description;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Identificador del bloc.</param>
        /// <param name="elements">Llista d'elements.</param>
        /// <param name="attributes">Llista d'atributs.</param>
        /// 
        public Component(string name, IEnumerable<Element> elements = null, IEnumerable<ComponentAttribute> attributes = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;

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

            Component component = new Component(_name);

            if (_elements != null)
                foreach (var element in Elements)
                    component.AddElement(element.Clone());

            if (_attributes != null)
                foreach (var attribute in Attributes)
                    component.AddAttribute(attribute.Clone());

            return component;
        }

        /// <inheritdoc/>
        /// 
        public void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna el identificador del component.
        /// </summary>
        /// 
        public string Name =>
            _name;

        /// <summary>
        /// Obte o asigna la descripcio.
        /// </summary>
        /// 
        public string Description {
            get => _description;
            set => _description = value;
        }
    }
}
