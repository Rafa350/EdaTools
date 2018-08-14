namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures {

    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Clase que representa una aperture eliptica.
    /// </summary>
    public sealed class ObroundAperture : Aperture {

        private readonly int width;
        private readonly int height;
        private readonly int drill;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id">Identificador de l'apertura.</param>
        /// <param name="tag">Dades opcionals.</param>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// 
        public ObroundAperture(int id, object tag, int width, int height, int drill = 0) :
            base(id, tag) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            if (drill >= width || drill >= height)
                throw new ArgumentOutOfRangeException("drill");

            this.width = width;
            this.height = height;
            this.drill = drill;
        }

        /// <summary>
        /// Obte la comanda per definir l'apertura.
        /// </summary>
        /// <returns>La comanda.</returns>
        /// 
        protected override string GetCommand() {

            StringBuilder sb = new StringBuilder();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", Id);
            sb.Append("O,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", width / 1000000.0);
            sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", height / 1000000.0);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill / 1000000.0);
            sb.Append("*%");

            return sb.ToString();
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
        /// Obte l'alçada.
        /// </summary>
        ///
        public int Height {
            get {
                return height;
            }
        }

        /// <summary>
        /// Obte el diametre del forat.
        /// </summary>
        /// 
        public int Drill {
            get {
                return drill;
            }
        }
    }
}
