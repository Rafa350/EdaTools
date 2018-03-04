namespace MikroPic.EdaTools.v1.Pcb.Geometry.Fonts {

    using System;

    /// <summary>
    /// Representa la figura d'un caracter. Aquesta clase es inmutable.
    /// </summary>
    public sealed class Glyph {

        private readonly char code;
        private readonly int advance;
        private readonly GlyphTrace[] traces;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="code">Codi del caracter.</param>
        /// <param name="advance">Amplada del caracter.</param>
        /// <param name="traces">Linies que formen la figura.</param>
        /// 
        public Glyph(char code, int advance, GlyphTrace[] traces) {

            this.code = code;
            this.advance = advance;
            this.traces = traces; 
        }

        /// <summary>
        /// Obte el codi de la figura.
        /// </summary>
        /// 
        public char Code {
            get {
                return code;
            }
        }

        /// <summary>
        /// Obte l'amplada.
        /// </summary>
        /// 
        public int Advance {
            get {
                return advance;
            }
        }

        /// <summary>
        /// Obte els traços de la figura.
        /// </summary>
        /// 
        public GlyphTrace[] Traces {
            get {
                return traces;
            }
        }
    }
}
