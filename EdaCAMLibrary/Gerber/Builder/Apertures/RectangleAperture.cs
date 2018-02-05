namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures {

    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Clase que representa una aperture rectangular.
    /// </summary>
    public sealed class RectangleAperture : Aperture {

        private readonly double width;
        private readonly double height;
        private readonly double drill;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id">Identificador de l'apertura.</param>
        /// <param name="tag">Dades opcionals.</param>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// 
        public RectangleAperture(int id, object tag, double width, double height, double drill = 0) :
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
            sb.Append("R,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}X{1}", width, height);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            return sb.ToString();
        }

        /// <summary>
        /// Obte l'asmplada.
        /// </summary>
        /// 
        public double Width {
            get {
                return width;
            }
        }

        /// <summary>
        /// Obte l'alçada.
        /// </summary>
        /// 
        public double Height {
            get {
                return height;
            }
        }

        /// <summary>
        /// Obte el diametre del forat.
        /// </summary>
        /// 
        public double Drill {
            get {
                return drill;
            }
        }
    }
}
