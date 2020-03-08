namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber {

    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder.Apertures;

    internal sealed class FontDictionary {

        private readonly IList<Macro> macros = new List<Macro>();
        private readonly IDictionary<char, Aperture> items = new Dictionary<char, Aperture>();

        public FontDictionary() {
        }

        public void DefineCharacterAperture(Glyph glyph) {

            if (!items.ContainsKey(glyph.Code)) {

                Macro macro = null;
                macros.Add(macro);

                Aperture aperture = new MacroAperture(glyph.Code, macro, null);
                items.Add(glyph.Code, aperture);
            }
        }

        public Aperture GetCharacterAperture(char ch) {

            return items[ch];
        }

        /// <summary>
        /// Enumera tots els macros definits.
        /// </summary>
        /// 
        public IEnumerable<Macro> Macros =>
            macros;

        /// <summary>
        /// Enumera totes les apertures definides.
        /// </summary>
        /// 
        public IEnumerable<Aperture> Apertures =>
            items.Values;
    }
}
