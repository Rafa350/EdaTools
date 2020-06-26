namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;

    /// <summary>
    /// Clase que representa una senyal.
    /// </summary>
    public sealed class Signal : IName, IBoardVisitable {

        private readonly string _name;
        private int _clearance = 150000;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom de la senyal.</param>
        /// 
        public Signal(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
        }

        /// <summary>
        /// Clona l'objecte.
        /// </summary>
        /// <param name="name">El nom del clon.</param>
        /// <returns>El clon de l'objecte.</returns>
        /// 
        public Signal Clone(string name) {

            return new Signal(name);
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna el nom de la senyal.
        /// </summary>
        /// 
        public string Name => _name;

        /// <summary>
        /// Obte o asigna la distancia d'aillament minima de la senyal.
        /// </summary>
        /// 
        public int Clearance {
            get {
                return _clearance;
            }
            set {
                if (_clearance < 0)
                    throw new ArgumentOutOfRangeException("Clearance");
                _clearance = value;
            }
        }
    }
}
