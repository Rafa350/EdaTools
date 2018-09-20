namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;

    /// <summary>
    /// Estruxctura que representa un tamany d'una superficie rectangular.
    /// </summary>
    public readonly struct Size {

        private readonly int width;
        private readonly int height;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Açada.</param>
        /// 
        public Size(int width = 0, int height = 0) {

            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Converteix l'objecte a text.
        /// </summary>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public override string ToString() {

            return String.Format("{0}, {1}", width, height);
        }

        /// <summary>
        /// Obte l'amplada.
        /// </summary>
        /// 
        public int Width {
            get {
                return width;
            }
        }

        /// <summary>
        /// Obte l'aláda
        /// </summary>
        /// 
        public int Height {
            get {
                return height;
            }
        }
    }
}