namespace MikroPic.EdaTools.v1.Base.Geometry.Fonts {

    /// <summary>
    /// Representa els traços que formen la figura del caracter. Aquesta clase
    /// es inmutable.
    /// </summary>
    public struct GlyphTrace {

        private readonly Point _position;
        private readonly bool _stroke;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio final del traç.</param>
        /// <param name="stroke">True si es dibuixa el traç fins a la posicio final.</param>.
        /// 
        public GlyphTrace(Point position, bool stroke) {

            _position = position;
            _stroke = stroke;
        }

        /// <summary>
        /// Obte la posicio.
        /// </summary>
        /// 
        public Point Position => _position;

        /// <summary>
        /// Obte el indicador de dibuix.
        /// </summary>
        /// 
        public bool Stroke => _stroke;
    }
}
