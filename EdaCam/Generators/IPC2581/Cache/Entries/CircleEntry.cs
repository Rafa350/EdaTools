using System.Globalization;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Cache.Entries {

    internal class CircleEntry: DataCacheEntry {

        private readonly int _diameter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Identificador.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <param name="diameter">Diametre.</param>
        /// 
        public CircleEntry(int id, string tag, int diameter) :
            base(id, tag) {

            _diameter = diameter;
        }

        /// <summary>
        /// Obte un identificador unic.
        /// </summary>
        /// <param name="diameter">El diametre.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'identificador unic.</returns>
        /// 
        public static int GetId(int diameter, string tag) {

            string s = string.Format(CultureInfo.InvariantCulture, "Circle;{0};{1}", diameter, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte el diametre.
        /// </summary>
        /// 
        public int Diameter =>
            _diameter;
    }
}
