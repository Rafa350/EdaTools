namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using MikroPic.EdaTools.v1.Pcb.Model.Collections;

    /// <summary>
    /// Clase que representa un atribut d'un bloc.
    /// </summary>
    public sealed class BlockAttribute: IVisitable, IKey<String> {

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

        /// <summary>
        /// Clone l'objecte.
        /// </summary>
        /// <returns>El nou objecte clonat.</returns>
        /// 
        public BlockAttribute Clone() {

            return new BlockAttribute(name, value);
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el valor de la clau;
        /// </summary>
        /// <returns>El valor de la clau.</returns>
        /// <remarks>Implementa el metode IKey.GetKey()</remarks>
        /// 
        public string GetKey() {

            return name;
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
