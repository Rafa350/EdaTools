namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed partial class Library {

        private string name;
        private string description;

        /// <summary>
        /// Obte o asignma el nom de la biblioteca.
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
        /// Obte o asigna la descripcio de la bibnlioteca.
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
