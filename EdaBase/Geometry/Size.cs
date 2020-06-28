﻿namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System;

    /// <summary>
    /// Estructura que representa un tamany d'una superficie rectangular.
    /// </summary>
    public readonly struct Size: IEquatable<Size> {

        private readonly int width;
        private readonly int height;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Açada.</param>
        /// 
        public Size(int width = 0, int height = 0) {

            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Converteix l'objecte a text.
        /// </summary>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public override string ToString() =>
            String.Format("{0}, {1}", width, height);

        /// <summary>
        /// Converteix un texte a 'Size'
        /// </summary>
        /// <param name="source">El texte.</param>
        /// <returns>El resultat de la converssio.</returns>
        /// 
        public static Size Parse(string source) {

            try {
                string[] s = source.Split(',');
                int width = Int32.Parse(s[0]);
                int height = Int32.Parse(s[1]);
                return new Size(width, height);
            }
            catch (Exception ex) {
                throw new InvalidOperationException(String.Format("No es posible convertir el texto '{0}' a 'Size'.", source), ex);
            }
        }

        /// <summary>
        /// Obte el codi hash del objecte.
        /// </summary>
        /// <returns>El codi hash</returns>
        /// 
        public override int GetHashCode() =>
            (width * 7) * (height * 531);

        /// <summary>
        /// Operacio d'igualtat entre dos objectes.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True si son iguals.</returns>
        /// 
        public bool Equals(Size other) =>
            (width, height) == (other.width, other.height);

        /// <summary>
        /// Comprovacio d'igualtat entre dos objectes.
        /// </summary>
        /// <param name="obj">L'altre objecte.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public override bool Equals(object obj) {

            if (obj is Size other)
                return Equals(other);
            else
                return false;
        }

        public static bool operator ==(Size s1, Size s2) =>
            s1.Equals(s2);

        public static bool operator !=(Size s1, Size s2) =>
            !s1.Equals(s2);

        /// <summary>
        /// Obte l'amplada.
        /// </summary>
        /// 
        public int Width => width;

        /// <summary>
        /// Obte l'alçada
        /// </summary>
        /// 
        public int Height => height;
    }
}