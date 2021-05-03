using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed partial class Library : IBoardVisitable {

        private string _name;
        private string _description;

        public Library(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
        }

        public Library(string name, string description, IEnumerable<Component> components) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
            _description = description;
            if (components != null)
                AddComponents(components);
        }

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
