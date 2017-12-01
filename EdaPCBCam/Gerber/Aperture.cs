namespace MikroPic.EdaTools.v1.Cam.Gerber {

    public abstract class Aperture {

        private static int __id = 10;
        private readonly int id;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// 
        public Aperture() {

            this.id = __id++;
        }

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
