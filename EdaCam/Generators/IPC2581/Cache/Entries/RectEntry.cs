using System;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Cache.Entries {

    internal class RectEntry: DataCacheEntry {

        private readonly EdaSize _size;
        private readonly EdaRatio _ratio;
        private readonly bool _flat;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Identificador.</param>
        /// <param name="tag">Etioqueta.</param>
        /// <param name="size">El tamany.</param>
        /// <param name="ratio">El percentatge de curvatura de les cantonades.</param>
        /// <param name="flat">True si les cantonades son planes</param>
        /// 
        public RectEntry(int id, string tag, EdaSize size, EdaRatio ratio, bool flat) :
            base(id, tag) {

            _size = size;
            _ratio = ratio;
            _flat = flat;
        }

        /// <summary>
        /// Obte un identificador unic.
        /// </summary>
        /// <param name="size">El tamany</param>
        /// <param name="ratio">El ratio de curvatura.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'identificador unic.</returns>
        /// 
        public static int GetId(EdaSize size, EdaRatio ratio, bool flat, string tag) {

            return HashCode.Combine("RectRound", size.Width, size.Height, ratio.AsPercent, flat, tag);
        }

        /// <summary>
        /// Obte el tamany.
        /// </summary>
        /// 
        public EdaSize Size =>
            _size;

        /// <summary>
        /// Obte el percentatge de curvatura de les cantonades.
        /// </summary>
        /// 
        public EdaRatio Ratio =>
            _ratio;

        /// <summary>
        /// Obte el indicador de cantonades planes.
        /// </summary>
        /// 
        public bool Flat =>
            _flat;

        /// <summary>
        /// Obte el radi de curvatura de les cantonades.
        /// </summary>
        /// 
        public int Radius =>
            (Math.Min(_size.Width, _size.Height) * _ratio) / 2;
    }
}
