﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace MikroPic.EdaTools.v1.Base.Geometry.Fonts {

    public sealed class Font {

        private readonly Dictionary<char, Glyph> _glyphs = new Dictionary<char, Glyph>();
        private readonly string _name;
        private readonly int _ascendent;
        private readonly int _height;

        /// <summary>
        /// Constructor privat de l'objecte
        /// </summary>
        /// <param name="name">Nom del font.</param>
        /// <param name="height">Alçada del font.</param>
        /// <param name="ascendent">Ascendent del font.</param>
        /// <param name="glyphs">Llista de figures.</param>
        /// 
        private Font(string name, int height, int ascendent, IEnumerable<Glyph> glyphs) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
            _height = height;
            _ascendent = ascendent;

            foreach (var glyph in glyphs)
                _glyphs.Add(glyph.Code, glyph);
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

            var doc = new XmlDocument();
            doc.Load(stream);

            XmlNode fontNode = doc.SelectSingleNode("/resources/fontResource/font");

            string name = fontNode.Attributes["name"].Value;
            int height = XmlConvert.ToInt32(fontNode.Attributes["height"].Value);
            int ascendent = XmlConvert.ToInt32(fontNode.Attributes["ascent"].Value);
            int descendent = XmlConvert.ToInt32(fontNode.Attributes["descent"].Value);

            var glyphs = new List<Glyph>();
            foreach (XmlNode charNode in fontNode.SelectNodes("char")) {

                string codeStr = charNode.Attributes["code"].Value;
                char code;
                if (codeStr.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                    code = Convert.ToChar(UInt16.Parse(codeStr.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                else
                    code = Convert.ToChar(UInt16.Parse(codeStr));
                int advance = XmlConvert.ToInt32(charNode.Attributes["advance"].Value);

                List<GlyphTrace> traces = new List<GlyphTrace>();
                foreach (XmlNode strokeNode in charNode.SelectNodes("glyph/*")) {

                    string positionStr = strokeNode.Attributes["position"].Value;
                    string[] s = positionStr.Split(',');
                    int x = XmlConvert.ToInt32(s[0]);
                    int y = XmlConvert.ToInt32(s[1]);
                    EdaPoint position = new EdaPoint(x, y);

                    switch (strokeNode.Name) {
                        case "moveTo":
                            traces.Add(new GlyphTrace(position, false));
                            break;

                        case "lineTo":
                            traces.Add(new GlyphTrace(position, true));
                            break;
                    }
                }

                glyphs.Add(new Glyph(code, advance, traces.ToArray()));
            }

            return new Font(name, height, ascendent, glyphs);
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

            foreach (KeyValuePair<char, Glyph> kv in _glyphs) {

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
                    writer.WriteAttributeString("position", String.Format("{0}, {1}", XmlConvert.ToString(trace.Position.X), XmlConvert.ToString(trace.Position.Y)));
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
            if (_glyphs.TryGetValue(code, out glyph))
                return glyph;
            else
                return null;
        }

        /// <summary>
        /// Obte el nom del font.
        /// </summary>
        /// 
        public string Name {
            get {
                return _name;
            }
        }

        /// <summary>
        /// Obte l'alçada del font.
        /// </summary>
        /// 
        public int Height {
            get {
                return _height;
            }
        }

        /// <summary>
        /// Obte l'ascendent del font.
        /// </summary>
        /// 
        public int Ascendent {
            get {
                return _ascendent;
            }
        }
    }
}
