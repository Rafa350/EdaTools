﻿namespace MikroPic.EdaTools.v1.Pcb.Geometry.Fonts {

    /// <summary>
    /// Representa els traços que formen la figura del caracter
    /// </summary>
    public struct GlyphTrace {

        private readonly GlyphPoint position;
        private readonly bool stroke;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio final del traç.</param>
        /// <param name="stroke">Tres si es dibuixa el traç fins a la posicio final.</param>.
        /// 
        public GlyphTrace(GlyphPoint position, bool stroke) {

            this.position = position;
            this.stroke = stroke;
        }

        /// <summary>
        /// Obte la posicio.
        /// </summary>
        /// 
        public GlyphPoint Position {
            get {
                return position;
            }
        }

        /// <summary>
        /// Obte el indicador de dibuix.
        /// </summary>
        /// 
        public bool Stroke {
            get {
                return stroke;
            }
        }
    }
}
