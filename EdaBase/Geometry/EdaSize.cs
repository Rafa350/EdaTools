using System;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Estructura que representa un tamany d'una superficie rectangular en milionesimes de la unitat.
    /// </summary>
    /// 
    public readonly struct EdaSize : IEquatable<EdaSize> {

        private readonly int _width;
        private readonly int _height;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Açada.</param>
        /// 
        public EdaSize(int width = 0, int height = 0) {

            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;
        }

        /// <summary>
        /// Converteix l'objecte a text.
        /// </summary>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public override string ToString() =>
            String.Format("{0}; {1}", _width.ToString(), _height.ToString());

        /// <summary>
        /// Converteix un texte a 'Size'
        /// </summary>
        /// <param name="source">El texte.</param>
        /// <returns>El resultat de la converssio.</returns>
        /// 
        public static EdaSize Parse(string source) {

            try {
                string[] s = source.Split(';');
                double width = Double.Parse(s[0]);
                double height = Double.Parse(s[1]);

                return new EdaSize((int)width, (int)height);
            }
            catch (Exception ex) {
                throw new InvalidOperationException(String.Format("No es posible convertir el texto '{0}' a 'EdaSize'.", source), ex);
            }
        }

        /// <summary>
        /// Obte el codi hash del objecte.
        /// </summary>
        /// <returns>El codi hash</returns>
        /// 
        public override int GetHashCode() =>
            (_width * 7) * (_height * 531);

        /// <summary>
        /// Operacio d'igualtat entre dos objectes.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True si son iguals.</returns>
        /// 
        public bool Equals(EdaSize other) =>
            (_width, _height) == (other._width, other._height);

        /// <summary>
        /// Comprovacio d'igualtat entre dos objectes.
        /// </summary>
        /// <param name="obj">L'altre objecte.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public override bool Equals(object obj) =>
            (obj is EdaSize other) && Equals(other);

        public static bool operator ==(EdaSize s1, EdaSize s2) =>
            s1.Equals(s2);

        public static bool operator !=(EdaSize s1, EdaSize s2) =>
            !s1.Equals(s2);

        /// <summary>
        /// Obte l'amplada.
        /// </summary>
        /// 
        public int Width => _width;

        /// <summary>
        /// Obte l'alçada
        /// </summary>
        /// 
        public int Height => _height;
    }
}