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
    public sealed partial class EdaComponent : IVisitable<IBoardVisitor>, IName {

        private string _name;
        private string _description;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Identificador del bloc.</param>
        /// <param name="elements">Llista d'elements.</param>
        /// <param name="attributes">Llista d'atributs.</param>
        /// 
        public EdaComponent(string name, IEnumerable<EdaElement> elements = null, IEnumerable<EdaComponentAttribute> attributes = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;

            if (elements != null)
                AddElements(elements);

            if (attributes != null)
                AddAttributes(attributes);
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
        public string Name {
            get => _name;
            set => _name = value;
        }

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
