namespace MikroPic.EdaTools.v1.Base.Geometry.Fonts {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Base.Geometry;

    /// <summary>
    /// Clase per generar els glyphs d'una cadena
    /// </summary>
    public class TextDrawer {

        private readonly Font _font;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="font">Font</param>
        /// 
        public TextDrawer(Font font) {

            if (font == null)
                throw new ArgumentNullException(nameof(font));

            _font = font;
        }

        /// <summary>
        /// Genera els glyphs d'una cadena de text.
        /// </summary>
        /// <param name="text">El text.</param>
        /// <param name="position">Posicio</param>
        /// <param name="angle">Angle de rotacio.</param>
        /// <param name="horizontalAlign">Aliniacio horitzontal.</param>
        /// <param name="verticalAlign">Aliniacio vertical.</param>
        /// <param name="height">Alçada de lletra.</param>
        /// <returns>La llista de glyphs preparats per dibuixar.</returns>
        /// 
        public IEnumerable<GlyphTrace> Draw(string text, EdaPoint position, HorizontalTextAlign horizontalAlign,
            VerticalTextAlign verticalAlign, int height) {

            var glyphTraces = new List<GlyphTrace>();

            if (!String.IsNullOrEmpty(text)) {

                // Calcula el tamany del text
                //
                int width = 0;
                for (int i = 0; i < text.Length; i++) {
                    var glyph = _font.GetGlyph(text[i]);
                    if (glyph != null)
                        width += (i == text.Length - 1) ? glyph.Width : glyph.Advance;
                }

                // Calcula els offsets en funcio de l'aliniacio
                //
                int offsetX = 0;
                switch (horizontalAlign) {
                    case HorizontalTextAlign.Left:
                        break;

                    case HorizontalTextAlign.Center:
                        offsetX = -width / 2;
                        break;

                    case HorizontalTextAlign.Right:
                        offsetX = -width;
                        break;
                }

                int offsetY = 0;
                switch (verticalAlign) {
                    case VerticalTextAlign.Top:
                        offsetY = -_font.Ascendent;
                        break;

                    case VerticalTextAlign.Middle:
                        offsetY = -_font.Ascendent / 2;
                        break;

                    case VerticalTextAlign.Bottom:
                        break;
                }

                int scale = (int)(height / _font.Ascendent);

                // Dibuixa text
                //
                int offset = 0;
                for (int i = 0; i < text.Length; i++) {
                    
                    TraceStartGlyph();
                    
                    Glyph glyph = _font.GetGlyph(text[i]);
                    if (glyph != null) {

                        if (glyph.Traces != null) {

                            bool first = true;
                            foreach (var trace in glyph.Traces) {

                                var p = new EdaPoint(
                                    position.X + ((trace.Position.X + offset + offsetX) * scale),
                                    position.Y + ((trace.Position.Y + offsetY) * scale));

                                Trace(p, trace.Stroke, first);
                                glyphTraces.Add(new GlyphTrace(p, trace.Stroke));

                                first = false;
                            }
                        }

                        offset += glyph.Advance;
                    }

                    TraceEndGlyph();
                }
            }

            return glyphTraces;
        }

        /// <summary>
        /// Dibuixa un segment desde la posicio actual a la posicio final indicada.
        /// </summary>
        /// <param name="position">Posicio final del segment.</param>
        /// <param name="stroke">True si cal dibuixar, false per nomes moure.</param>
        /// <param name="first">True si es el primer punt.</param>
        /// 
        protected virtual void Trace(EdaPoint position, bool stroke, bool first) {

        }

        protected virtual void TraceStartGlyph() {

        }

        protected virtual void TraceEndGlyph() {

        }
    }
}
