using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry.Fonts {

    /// <summary>
    /// Representa la figura d'un caracter. Aquesta clase es inmutable.
    /// </summary>
    /// 
    public sealed class Glyph {

        private readonly char _code;
        private readonly int _advance;
        private readonly int _width;
        private readonly GlyphTrace[] _traces;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="code">Codi del caracter.</param>
        /// <param name="advance">Amplada del caracter.</param>
        /// <param name="traces">Linies que formen la figura.</param>
        /// 
        public Glyph(char code, int advance, GlyphTrace[] traces) {

            _code = code;
            _advance = advance;
            _traces = traces;

            _width = Int32.MinValue;
            foreach (var trace in traces) {
                if (trace.Position.X > _width)
                    _width = trace.Position.X;
            }
        }

        /// <summary>
        /// Obte el codi de la figura.
        /// </summary>
        /// 
        public char Code =>
            _code;

        /// <summary>
        /// Obte l'mplada del caracter
        /// </summary>
        /// 
        public int Width =>
            _width;

        /// <summary>
        /// Obte l'avanç del caracter.
        /// </summary>
        /// 
        public int Advance =>
            _advance;

        /// <summary>
        /// Obte el numero de traços.
        /// </summary>
        /// 
        public int NumTraces =>
            _traces == null ? 0 : _traces.Length;

        /// <summary>
        /// Obte els traços de la figura.
        /// </summary>
        /// 
        public IEnumerable<GlyphTrace>
            Traces => _traces;
    }
}
