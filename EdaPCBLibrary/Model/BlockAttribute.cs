namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;

    /// <summary>
    /// Clase que representa un atribut d'un bloc.
    /// </summary>
    public sealed class BlockAttribute: IVisitable {

        private readonly string name;
        private string value;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom de l'atribut.</param>
        /// <param name="value">El valor de l'atribut.</param>
        /// 
        public BlockAttribute(string name, string value = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el nom del atribut
        /// </summary>
        /// 

        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Obte o asigna el valor del atribut
        /// </summary>
        /// 
        public string Value {
            get {
                return value;
            }
            set {
                this.value = value;
            }
        }
    }
}
