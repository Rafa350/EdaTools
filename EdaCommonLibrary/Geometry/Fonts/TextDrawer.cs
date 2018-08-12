namespace MikroPic.EdaTools.v1.Geometry.Fonts {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase per generar els glyphs d'una cadena
    /// </summary>
    public class TextDrawer {

        private readonly Font font;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="font">Font</param>
        /// 
        public TextDrawer(Font font) {

            if (font == null)
                throw new ArgumentNullException("font");

            this.font = font;
        }

        /// <summary>
        /// Genera els glyphs d'una cadena de text.
        /// </summary>
        /// <param name="text">El text.</param>
        /// <param name="position">Posicio</param>
        /// <param name="angle">Angle de rotacio.</param>
        /// <param name="align">Aliniacio.</param>
        /// <param name="height">Alçada de lletra.</param>
        /// <returns>La llista de glyphs preparats per dibuixar.</returns>
        /// 
        public IEnumerable<GlyphTrace> Draw(string text, Point position, TextAlign align, int height) {

            List<GlyphTrace> glyphTraces = new List<GlyphTrace>();

            if (!String.IsNullOrEmpty(text)) {

                // Calcula el tamany del text
                //
                int width = 0;
                for (int i = 0; i < text.Length; i++) {
                    Glyph glyph = font.GetGlyph(text[i]);
                    if (glyph != null)
                        width += glyph.Advance;
                }

                // Calcula els offsets en funcio de l'aliniacio
                //
                int offsetX = 0;
                switch (align) {
                    case TextAlign.TopLeft:
                    case TextAlign.MiddleLeft:
                    case TextAlign.BottomLeft:
                        break;

                    case TextAlign.TopCenter:
                    case TextAlign.MiddleCenter:
                    case TextAlign.BottomCenter:
                        offsetX = -width / 2;
                        break;

                    case TextAlign.TopRight:
                    case TextAlign.MiddleRight:
                    case TextAlign.BottomRight:
                        offsetX = -width;
                        break;
                }

                int offsetY = 0;
                switch (align) {
                    case TextAlign.TopLeft:
                    case TextAlign.TopCenter:
                    case TextAlign.TopRight:
                        offsetY = -font.Ascendent;
                        break;

                    case TextAlign.MiddleLeft:
                    case TextAlign.MiddleCenter:
                    case TextAlign.MiddleRight:
                        offsetY = -font.Ascendent / 2;
                        break;

                    case TextAlign.BottomLeft:
                    case TextAlign.BottomCenter:
                    case TextAlign.BottomRight:
                        break;
                }

                int scale = (int)(height / font.Ascendent);

                // Dibuixa text
                //
                int offset = 0;
                for (int i = 0; i < text.Length; i++) {
                    Glyph glyph = font.GetGlyph(text[i]);
                    if (glyph != null) {

                        for (int j = 0; j < glyph.Traces.Length; j++) {

                            bool stroke = glyph.Traces[j].Stroke;
                            Point gp = glyph.Traces[j].Position;
                            Point p = new Point(
                                position.X + ((gp.X + offset + offsetX) * scale),
                                position.Y + ((gp.Y + offsetY) * scale));

                            Trace(p, stroke, j == 0);

                            glyphTraces.Add(new GlyphTrace(p, stroke));
                        }

                        offset += glyph.Advance;
                    }
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
        protected virtual void Trace(Point position, bool stroke, bool first) {

        }
    }
}
