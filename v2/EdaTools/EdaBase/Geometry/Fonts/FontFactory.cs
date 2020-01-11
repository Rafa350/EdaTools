namespace MikroPic.EdaTools.v1.Base.Geometry.Fonts {

    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public sealed class FontFactory {

        private Dictionary<string, Font> fontCache;
        private static FontFactory instance;

        private FontFactory() {

            fontCache = new Dictionary<string, Font>();
        }

        public Font GetFont(string fontName) {

            Font font;

            if (!fontCache.TryGetValue(fontName, out font)) {

                Assembly assembly = Assembly.GetExecutingAssembly();
                string location = assembly.Location;
                string path = Path.Combine(Path.GetDirectoryName(location), @"Data\font.xml");

                font = Font.Load(path);
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
