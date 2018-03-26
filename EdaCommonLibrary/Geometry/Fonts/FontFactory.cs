namespace MikroPic.EdaTools.v1.Geometry.Fonts {

    using System.Collections.Generic;

    public sealed class FontFactory {

        private Dictionary<string, Font> fontCache;
        private static FontFactory instance;

        private FontFactory() {

            fontCache = new Dictionary<string, Font>();
        }

        public Font GetFont(string fontName) {

            Font font;
            if (!fontCache.TryGetValue(fontName, out font)) {
                font = Font.Load(@"Data\font.xml");
                fontCache.Add(font.Name, font);
            }
            return font;
        }

        public static FontFactory Instance {
            get {
                if (instance == null)
                    instance = new FontFactory();
                return instance;
            }
        }
    }
}
