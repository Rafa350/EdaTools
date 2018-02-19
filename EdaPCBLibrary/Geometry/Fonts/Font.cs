namespace MikroPic.EdaTools.v1.Pcb.Geometry.Fonts {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    public sealed class Font {

        private readonly Dictionary<char, Glyph> glyphs = new Dictionary<char, Glyph>();

        /// <summary>
        /// Constructor privat de l'objecte
        /// </summary>
        /// <param name="glyphs">Llista de figures.</param>
        /// 
        private Font(IEnumerable<Glyph> glyphs) {

            foreach (Glyph glyph in glyphs)
                this.glyphs.Add(glyph.Code, glyph);
        }

        /// <summary>
        /// Crera un font des d'un arxiu.
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

            List<Glyph> glyphs = new List<Glyph>();

            XmlDocument doc = new XmlDocument();
            doc.Load(stream);

            XmlNode fontNode = doc.SelectSingleNode("/resources/fontResource/font");

            foreach (XmlNode charNode in fontNode.SelectNodes("char")) {

                char code = Char.Parse(charNode.Attributes["char"].Value);
                double left = XmlConvert.ToDouble(charNode.Attributes["left"].Value);
                double top = XmlConvert.ToDouble(charNode.Attributes["top"].Value);
                double width = XmlConvert.ToDouble(charNode.Attributes["width"].Value);
                double height = XmlConvert.ToDouble(charNode.Attributes["height"].Value);
                double advance = XmlConvert.ToDouble(charNode.Attributes["advance"].Value);

                List<GlyphTrace> traces = new List<GlyphTrace>();
                foreach (XmlNode strokeNode in charNode.SelectNodes("glyph/*")) {

                    string positionStr = strokeNode.Attributes["position"].Value;

                    switch (strokeNode.Name) {
                        case "moveTo":
                            break;

                        case "lineTo":
                            break;
                    }

                }

                Glyph glyph = new Glyph(code, advance, traces);
            }

            return new Font(glyphs);
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
            writer.WriteAttributeString("resourceId", "Hershey-Gothic-Simplex");

            writer.WriteStartElement("font");
            writer.WriteAttributeString("name", "");
            writer.WriteAttributeString("height", "");
            writer.WriteAttributeString("ascend", "");
            writer.WriteAttributeString("descend", "");

            foreach (KeyValuePair<char, Glyph> kv in glyphs) {

                Glyph glyph = kv.Value;

                writer.WriteStartElement("char");
                writer.WriteAttributeString("code", XmlConvert.ToString(glyph.Code));
                writer.WriteAttributeString("left", "");
                writer.WriteAttributeString("top", "");
                writer.WriteAttributeString("width", "");
                writer.WriteAttributeString("height", "");
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
    }
}
