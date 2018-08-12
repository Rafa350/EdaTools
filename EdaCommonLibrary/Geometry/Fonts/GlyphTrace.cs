namespace MikroPic.EdaTools.v1.Geometry.Fonts {

    using MikroPic.EdaTools.v1.Geometry;

    /// <summary>
    /// Representa els traços que formen la figura del caracter. Aquesta clase
    /// es inmutable.
    /// </summary>
    public struct GlyphTrace {

        private readonly Geometry.Point position;
        private readonly bool stroke;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio final del traç.</param>
        /// <param name="stroke">Tres si es dibuixa el traç fins a la posicio final.</param>.
        /// 
        public GlyphTrace(Point position, bool stroke) {

            this.position = position;
            this.stroke = stroke;
        }

        /// <summary>
        /// Obte la posicio.
        /// </summary>
        /// 
        public Point Position {
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
