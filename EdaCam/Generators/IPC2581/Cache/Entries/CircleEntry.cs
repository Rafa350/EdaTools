using System;
using MikroPic.EdaTools.v1.Cam.Configuration;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Cache.Entries
{

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

            return HashCode.Combine("Circle", diameter, tag);
        }

        /// <summary>
        /// Obte el diametre.
        /// </summary>
        /// 
        public int Diameter =>
            _diameter;
    }
}
