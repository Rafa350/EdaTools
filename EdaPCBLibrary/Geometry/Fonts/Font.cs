namespace MikroPic.EdaTools.v1.Pcb.Geometry.Fonts {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    public sealed class Font {

        private readonly Dictionary<char, Glyph> glyphs = new Dictionary<char, Glyph>();
        private readonly double height;

        /// <summary>
        /// Constructor privat de l'objecte
        /// </summary>
        /// <param name="height">Alçada del font.</param>
        /// <param name="glyphs">Llista de figures.</param>
        /// 
        private Font(double height, IEnumerable<Glyph> glyphs) {

            this.height = height;

            foreach (Glyph glyph in glyphs)
                this.glyphs.Add(glyph.Code, glyph);
        }

        /// <summary>
        /// Crea un font des d'un arxiu.
        /// </summary>
        /// <param name="fileName">Nom del fitxer.</param>
        /// <returns>L'objecte 'VectorFont' creat</returns>
        /// 
        public static Font Load(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                return Load(stream);
        }

        /// <summary>
        /// Carrega un font des d'un stream.
        /// </summary>
        /// <param name="stream">En stream.</param>
        /// <returns>L'objecte 'VectorFont'</returns>
        /// 
        public static Font Load(Stream stream) {

            XmlDocument doc = new XmlDocument();
            doc.Load(stream);

            XmlNode fontNode = doc.SelectSingleNode("/resources/fontResource/font");

            double fontHeight = XmlConvert.ToDouble(fontNode.Attributes["height"].Value);
            double ascendent = XmlConvert.ToDouble(fontNode.Attributes["ascent"].Value);
            double descendent = XmlConvert.ToDouble(fontNode.Attributes["descent"].Value);

            List<Glyph> glyphs = new List<Glyph>();
            foreach (XmlNode charNode in fontNode.SelectNodes("char")) {

                string codeStr = charNode.Attributes["code"].Value;
                char code;
                if (codeStr.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                    code = Convert.ToChar(UInt16.Parse(codeStr.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                else
                    code = Convert.ToChar(UInt16.Parse(codeStr));
                double advance = XmlConvert.ToDouble(charNode.Attributes["advance"].Value);

                List<GlyphTrace> traces = new List<GlyphTrace>();
                foreach (XmlNode strokeNode in charNode.SelectNodes("glyph/*")) {

                    string positionStr = strokeNode.Attributes["position"].Value;
                    string[] s = positionStr.Split(',');
                    double x = XmlConvert.ToDouble(s[0]);
                    double y = XmlConvert.ToDouble(s[1]);

                    switch (strokeNode.Name) {
                        case "moveTo":
                            traces.Add(new GlyphTrace(x, y, false));
                            break;

                        case "lineTo":
                            traces.Add(new GlyphTrace(x, y, true));
                            break;
                    }
                }

                glyphs.Add(new Glyph(code, advance, traces));
            }

            return new Font(fontHeight, glyphs);
        }

        /// <summary>
        /// Salva el font en un fitxer
        /// </summary>
        /// <param name="fileName">Nom del fitxer.</param>
        /// 
        public void Save(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                Save(stream);
        }

        /// <summary>
        /// Salva el font en un stream.
        /// </summary>
        /// <param name="stream">El stream.</param>
        /// 
        public void Save(Stream stream) {

            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(stream, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("resources");

            writer.WriteStartElement("fontResource");
            writer.WriteAttributeString("version", "2.1");
            writer.WriteAttributeString("resourceId", "Eda-Tools default stroke font");

            writer.WriteStartElement("font");
            writer.WriteAttributeString("name", "");
            writer.WriteAttributeString("height", "");
            writer.WriteAttributeString("ascend", "");
            writer.WriteAttributeString("descend", "");

            foreach (KeyValuePair<char, Glyph> kv in glyphs) {

                Glyph glyph = kv.Value;

                writer.WriteStartElement("char");
                writer.WriteAttributeString("code", XmlConvert.ToString(glyph.Code));
                writer.WriteAttributeString("advance", XmlConvert.ToString(glyph.Advance));

                writer.WriteStartElement("glyph");

                bool first = true;
                foreach (GlyphTrace trace in glyph.Traces) {
                    if (first || !trace.Stroke)
                        writer.WriteStartElement("moveTo");
                    else
                        writer.WriteStartElement("lineTo");
                    writer.WriteAttributeString("position", String.Format("{0}, {1}", XmlConvert.ToString(trace.X), XmlConvert.ToString(trace.Y)));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        /// <summary>
        /// Obte la figura d'un caracter concret.
        /// </summary>
        /// <param name="code">Codi del caracter.</param>
        /// <returns>La figura.</returns>
        /// 
        public Glyph GetGlyph(char code) {

            Glyph glyph;
            if (glyphs.TryGetValue(code, out glyph))
                return glyph;
            else
                return null;
        }

        public double Height {
            get {
                return height;
            }
        }
    }
}
