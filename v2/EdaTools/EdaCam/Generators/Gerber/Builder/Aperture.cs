namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder {

    /// <summary>
    /// Clase que representa una aperture gerber
    /// </summary>
    public abstract class Aperture {

        private readonly int id;
        private readonly object tag;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="id">Identificador de l'apertura.</param>
        /// <param name="tag">Dades opcionals.</param>
        /// 
        public Aperture(int id, object tag) {

            this.id = id;
            this.tag = tag;
        }

        /// <summary>
        /// Genera la comanda gerber per crear l'apertura.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetCommand();

        /// <summary>
        /// Obte el ID de l'apertura.
        /// </summary>
        /// 
        public int Id {
            get {
                return id;
            }
        }

        /// <summary>
        /// Obte la comanda Gerber per definir l'apertura.
        /// </summary>
        /// 
        public string Command {
            get {
                return GetCommand();
            }
        }

        /// <summary>
        /// Obte les dades opcionals
        /// </summary>
        /// 
        public object Tag {
            get {
                return tag;
            }
        }
    }
}
