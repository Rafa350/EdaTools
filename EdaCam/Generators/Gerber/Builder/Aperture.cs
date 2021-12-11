namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder {

    /// <summary>
    /// Clase que representa una aperture gerber
    /// </summary>
    public abstract class Aperture {

        private readonly int _id;
        private readonly object _tag;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="id">Identificador de l'apertura.</param>
        /// <param name="tag">Dades opcionals.</param>
        /// 
        public Aperture(int id, object tag) {

            _id = id;
            _tag = tag;
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
        public int Id =>
            _id;

        /// <summary>
        /// Obte la comanda Gerber per definir l'apertura.
        /// </summary>
        /// 
        public string Command =>
            GetCommand();

        /// <summary>
        /// Obte les dades opcionals
        /// </summary>
        /// 
        public object Tag =>
            _tag;
    }
}
