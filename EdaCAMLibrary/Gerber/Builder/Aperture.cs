namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder {

    /// <summary>
    /// Clase que representa una aperture gerber
    /// </summary>
    public abstract class Aperture {

        private readonly int id;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// 
        public Aperture(int id) {

            this.id = id;
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
    }
}
