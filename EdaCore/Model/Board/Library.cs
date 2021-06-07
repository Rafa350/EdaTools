using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Core.Model.Common;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed partial class Library : IVisitable<IBoardVisitor> {

        private string _name;
        private string _description;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">EWl nom de la llibraria.</param>
        /// 
        public Library(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">El nom de la llibraria.</param>
        /// <param name="description">La descripcio.</param>
        /// <param name="components">Els components a afeigir.</param>
        /// 
        public Library(string name, string description, IEnumerable<Component> components) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
            _description = description;
            if (components != null)
                AddComponents(components);
        }

        /// <inheritdoc/>
        /// 
        public void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna el nom de la biblioteca.
        /// </summary>
        /// 
        public string Name {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Obte o asigna la descripcio de la biblioteca.
        /// </summary>
        /// 
        public string Description {
            get => _description;
            set => _description = value;
        }
    }
}
