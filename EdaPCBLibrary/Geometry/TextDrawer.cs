namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Fonts;
    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Clase per generar els glyphs d'una cadena
    /// </summary>
    public abstract class TextDrawer {

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
        /// <param name="height">A´çada de lletra.</param>
        /// <returns>La llista de glyphs preparats per dibuixar.</returns>
        /// 
        public void Draw(string text, Point position, TextAlign align, double height) {

            // Calcula el tamany del text
            //
            double width = 0;
            foreach (char ch in text) {
                Glyph glyph = font.GetGlyph(ch);
                if (glyph != null)
                    width += glyph.Advance;
            }

            // Calcula els offsets en funcio de l'aliniacio
            //
            double offsetX = 0;
            double offsetY = 0;
            switch (align) {
                case TextAlign.TopLeft:
                    break;

                case TextAlign.TopCenter:
                    offsetX = -width / 2;
                    break;

                case TextAlign.TopRight:
                    offsetX = -width;
                    break;
            }

            // Calcula la matriu de transformacio
            //
            double scale = height / font.Height;
            Matrix m = new Matrix();
            m.Translate(position.X + offsetX, position.Y + offsetY);
            m.Scale(scale, scale);

            // Dibuixa text
            //
            double offset = 0;
            foreach (char ch in text) {
                Glyph glyph = font.GetGlyph(ch);
                if (glyph != null) {

                    bool first = true;
                    foreach (GlyphTrace glyphTrace in glyph.Traces) {

                        Point p = new Point(glyphTrace.Position.X + offset, glyphTrace.Position.Y);
                        Trace(m.Transform(p), glyphTrace.Stroke, first);

                        first = false;
                    }

                    offset += glyph.Advance;
                }
            }
        }

        protected abstract void Trace(Point position, bool stroke, bool first);
    }
}
