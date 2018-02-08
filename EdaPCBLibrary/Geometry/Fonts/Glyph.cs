﻿namespace MikroPic.EdaTools.v1.Pcb.Geometry.Fonts {

    using System.Collections.Generic;

    /// <summary>
    /// Representa la figura d'un caracter.
    /// </summary>
    public sealed class Glyph {

        private readonly char code;
        private readonly double width;
        private readonly List<GlyphTrace> traces;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="code">Codi del caracter.</param>
        /// <param name="advance">Amplada del caracter.</param>
        /// <param name="traces">Linies que formen la figura.</param>
        /// 
        public Glyph(char code, double advance, IEnumerable<GlyphTrace> traces) {

            this.code = code;
            this.width = advance;
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
        public double Advance {
            get {
                return width;
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
