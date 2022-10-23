using System.Globalization;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Cache.Entries {

    internal sealed class LineDescEntry: DataCacheEntry {

        private int _thickness;
        private EdaLineCap _capStyle;

        public LineDescEntry(int id, string tag, int thickness, EdaLineCap capStyle) :
            base(id, tag) {

            _thickness = thickness;
            _capStyle = capStyle;
        }

        /// <summary>
        /// Obte un identificador unic.
        /// </summary>
        /// <param name="thickness">L'amplada de linia.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'identificador unic.</returns>
        /// 
        public static int GetId(int thickness, EdaLineCap capStyle, string tag) {

            string s = string.Format(CultureInfo.InvariantCulture, "LineDesc;{0};{1};{2}", thickness, capStyle, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte l'emplada de linia.
        /// </summary>
        /// 
        public int Thickness =>
            _thickness;

        /// <summary>
        /// Obte l'estil de l'extrem de linia.
        /// </summary>
        /// 
        public EdaLineCap CapStyle =>
            _capStyle;
    }
}
