namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections.Generic;

    public sealed partial class Library: IBoardVisitable {

        private string name;
        private string description;

        public Library(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
        }

        public Library(string name, string description, IEnumerable<Component> components) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.description = description;
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
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        /// <summary>
        /// Obte o asigna la descripcio de la biblioteca.
        /// </summary>
        /// 
        public string Description {
            get {
                return description;
            }
            set {
                description = value;
            }
        }
    }
}
