using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder;
using MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder.Apertures;

namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber {

    internal sealed class FontDictionary {

        private readonly IList<Macro> _macros = new List<Macro>();
        private readonly IDictionary<char, Aperture> _items = new Dictionary<char, Aperture>();

        public FontDictionary() {
        }

        public void DefineCharacterAperture(Glyph glyph) {

            if (!_items.ContainsKey(glyph.Code)) {

                Macro macro = null;
                _macros.Add(macro);

                Aperture aperture = new MacroAperture(glyph.Code, macro, null);
                _items.Add(glyph.Code, aperture);
            }
        }

        public Aperture GetCharacterAperture(char ch) {

            return _items[ch];
        }

        /// <summary>
        /// Enumera tots els macros definits.
        /// </summary>
        /// 
        public IEnumerable<Macro> Macros =>
            _macros;

        /// <summary>
        /// Enumera totes les apertures definides.
        /// </summary>
        /// 
        public IEnumerable<Aperture> Apertures =>
            _items.Values;
    }
}
