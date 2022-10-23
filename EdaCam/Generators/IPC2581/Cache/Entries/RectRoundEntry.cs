using System;
using System.Globalization;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Cache.Entries {

    internal class RectRoundEntry: DataCacheEntry {

        private readonly EdaSize _size;
        private readonly EdaRatio _ratio;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Identificador.</param>
        /// <param name="tag">Etioqueta.</param>
        /// <param name="size">El tamany.</param>
        /// <param name="ratio">El percentatge de curvatura de les cantonades.</param>
        /// 
        public RectRoundEntry(int id, string tag, EdaSize size, EdaRatio ratio) :
            base(id, tag) {

            _size = size;
            _ratio = ratio;
        }

        /// <summary>
        /// Obte un identificador unic.
        /// </summary>
        /// <param name="size">El tamany</param>
        /// <param name="ratio">El ratio de curvatura.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'identificador unic.</returns>
        /// 
        public static int GetId(EdaSize size, EdaRatio ratio, string tag) {

            string s = string.Format(CultureInfo.InvariantCulture, "RectRound;{0};{1};{2};{3}", size.Width, size.Height, ratio.AsPercent, tag);
            return s.GetHashCode();
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
        /// Obte el radi de curvatura de les cantonades.
        /// </summary>
        /// 
        public int Radius =>
            (Math.Min(_size.Width, _size.Height) * _ratio) / 2;
    }
}
