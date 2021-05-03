using System;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa un atribut d'un bloc.
    /// </summary>
    public sealed class ComponentAttribute : IBoardVisitable {

        private readonly string _name;
        private string _value;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom de l'atribut.</param>
        /// <param name="value">El valor de l'atribut.</param>
        /// 
        public ComponentAttribute(string name, string value = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
            _value = value;
        }

        /// <summary>
        /// Clone l'objecte.
        /// </summary>
        /// <returns>El nou objecte clonat.</returns>
        /// 
        public ComponentAttribute Clone() {

            return new ComponentAttribute(_name, _value);
        }

        public void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el nom del atribut
        /// </summary>
        /// 
        public string Name =>
            _name;

        /// <summary>
        /// Obte o asigna el valor del atribut
        /// </summary>
        /// 
        public string Value {
            get => _value;
            set => _value = value;
        }
    }
}
