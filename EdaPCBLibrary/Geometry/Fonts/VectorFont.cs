namespace MikroPic.EdaTools.v1.Pcb.Geometry.Fonts {

    using System;
    using System.Collections.Generic;
    using System.IO;

    public sealed class VectorFont {

        private readonly Dictionary<char, Glyph> glyphs = new Dictionary<char, Glyph>();

        private VectorFont(IEnumerable<Glyph> glyphs) {

            foreach (Glyph glyph in glyphs)
                this.glyphs.Add(glyph.Code, glyph);
        }

        public static VectorFont Load(Stream stream) {

            TextReader reader = new StreamReader(stream);
            string line = reader.ReadLine();
            do {
                if (line != null) {
                    line = reader.ReadLine();

                    int symbol = Int32.Parse(line.Substring(0, 4));
                    int numPairs = Int32.Parse(line.Substring(5, 3));
                    int leftPos = line[8] - 'R';
                    int rightPos = line[9] - 'R';
                    for (int pairNum = 1; pairNum < numPairs; pairNum++) {
                    }
                }
            } while (line != null);

            return new VectorFont(null);
        }

        public Glyph GetGlyph(char code) {

            Glyph glyph;
            if (glyphs.TryGetValue(code, out glyph))
                return glyph;
            else
                return null;
        }
    }
}
