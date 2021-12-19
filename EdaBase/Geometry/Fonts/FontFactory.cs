namespace MikroPic.EdaTools.v1.Base.Geometry.Fonts {

    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public sealed class FontFactory {

        private readonly Dictionary<string, Font> _fontCache;
        private static FontFactory _instance;

        private FontFactory() {

            _fontCache = new Dictionary<string, Font>();
        }

        public Font GetFont(string fontName) {

            Font font;

            if (!_fontCache.TryGetValue(fontName, out font)) {

                Assembly assembly = Assembly.GetExecutingAssembly();
                string location = assembly.Location;
                string path = Path.Combine(Path.GetDirectoryName(location), @"Data\font.xml");

                font = Font.Load(path);
                _fontCache.Add(font.Name, font);
            }
            return font;
        }

        public static FontFactory Instance {
            get {
                if (_instance == null)
                    _instance = new FontFactory();
                return _instance;
            }
        }
    }
}
