namespace MikroPic.EdaTools.v1.Cam.Configuration
{

    internal abstract class DataCacheEntry
    {

        private readonly int _id;
        private readonly string _tag;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Identificador.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public DataCacheEntry(int id, string tag)
        {

            _id = id;
            _tag = tag;
        }

        /// <summary>
        /// Obte l'identificador.
        /// </summary>
        /// 
        public int Id =>
            _id;

        /// <summary>
        /// Obte l'etiqueta.
        /// </summary>
        /// 
        public string Tag =>
            _tag;
    }
}
