namespace MikroPic.EdaTools.v1.Pcb.Model {
    
    using System;

    /// <summary>
    /// Clase que representa una senyal.
    /// </summary>
    public sealed class Signal: IName, IVisitable {

        private string name;
        private int clearance;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom de la senyal.</param>
        /// 
        public Signal(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
        }

        public Signal Clone() {

            return new Signal(name);
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
        /// Obte o asigna el nom de la senyal.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Signal.Name");

                name = value;
            }
        }

        /// <summary>
        /// Obte o asigna la distancia d'aillament minima de la senyal.
        /// </summary>
        /// 
        public int Clearance {
            get {
                return clearance;
            }
            set {
                if (clearance < 0)
                    throw new ArgumentOutOfRangeException("Clearance");
                clearance = value;
            }
        }
    }
}
