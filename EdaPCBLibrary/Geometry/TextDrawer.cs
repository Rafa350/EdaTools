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
        /// <param name="height">Alçada de lletra.</param>
        /// <returns>La llista de glyphs preparats per dibuixar.</returns>
        /// 
        public void Draw(string text, Point position, TextAlign align, double height) {

            if (String.IsNullOrEmpty(text))
                return;

            // Calcula el tamany del text
            //
            double width = 0;
            for (int i = 0; i < text.Length; i++) {
                Glyph glyph = font.GetGlyph(text[i]);
                if (glyph != null)
                    width += glyph.Advance;
            }

            // Calcula els offsets en funcio de l'aliniacio
            //
            double offsetX = 0;
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

            double offsetY = 0;
            switch (align) {
                case TextAlign.TopLeft:
                case TextAlign.TopCenter:
                case TextAlign.TopRight:
                    offsetY = -font.Height;
                    break;

                case TextAlign.MiddleLeft:
                case TextAlign.MiddleCenter:
                case TextAlign.MiddleRight:
                    offsetY = -font.Height / 2;
                    break;

                case TextAlign.BottomLeft:
                case TextAlign.BottomCenter:
                case TextAlign.BottomRight:
                    break;
            }

            // Calcula la matriu de transformacio
            //
            double scale = height / font.Height;
            Matrix m = new Matrix();
            m.Translate(position.X + offsetX, position.Y + offsetY);
            m.ScaleAt(scale, scale, position.X, position.Y);

            // Dibuixa text
            //
            int offset = 0;
            for (int i = 0; i < text.Length; i++) {
                Glyph glyph = font.GetGlyph(text[i]);
                if (glyph != null) {

                    for (int j = 0; j < glyph.Traces.Length; j++) {

                        GlyphPoint gp = glyph.Traces[j].Position;
                        Point p = new Point(gp.X + offset, gp.Y);

                        Trace(m.Transform(p), glyph.Traces[j].Stroke, j == 0);
                    }

                    offset += glyph.Advance;
                }
            }
        }

        /// <summary>
        /// Dibuixa un segment desde la posicio actual a la posicio final indicada.
        /// </summary>
        /// <param name="position">Posicio final del segment.</param>
        /// <param name="stroke">True si cal dibuixar, false per nomes moure.</param>
        /// <param name="first">True si es el primer punt.</param>
        /// 
        protected abstract void Trace(Point position, bool stroke, bool first);
    }
}
