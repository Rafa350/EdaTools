namespace MikroPic.EdaTools.v1.Pcb.Geometry.Fonts {

    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    /// <summary>
    /// Representa la figura d'un caracter.
    /// </summary>
    public sealed class Glyph {

        private readonly char code;
        private readonly int advance;
        private readonly List<GlyphTrace> traces;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="code">Codi del caracter.</param>
        /// <param name="advance">Amplada del caracter.</param>
        /// <param name="traces">Linies que formen la figura.</param>
        /// 
        public Glyph(char code, int advance, IEnumerable<GlyphTrace> traces) {

            if (traces == null)
                throw new ArgumentNullException("traces");

            this.code = code;
            this.advance = advance;
            this.traces = new List<GlyphTrace>(traces); 
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
        public IEnumerable<GlyphTrace> Traces {
            get {
                return traces;
            }
        }
    }
}
