namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder.Apertures {

    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Clase que representa una aperture rectangular.
    /// </summary>
    public sealed class RectangleAperture : Aperture {

        private readonly int _width;
        private readonly int _height;
        private readonly int _drill;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id">Identificador de l'apertura.</param>
        /// <param name="tag">Dades opcionals.</param>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// 
        public RectangleAperture(int id, object tag, int width, int height, int drill = 0) :
            base(id, tag) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            if (drill >= width || drill >= height)
                throw new ArgumentOutOfRangeException(nameof(drill));

            _width = width;
            _height = height;
            _drill = drill;
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
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}X{1}", _width / 1000000.0, _height / 1000000.0);
            if (_drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", _drill / 1000000.0);
            sb.Append("*%");

            return sb.ToString();
        }

        /// <summary>
        /// Obte l'asmplada.
        /// </summary>
        /// 
        public int Width => _width;

        /// <summary>
        /// Obte l'alçada.
        /// </summary>
        /// 
        public int Height => _height;

        /// <summary>
        /// Obte el diametre del forat.
        /// </summary>
        /// 
        public int Drill => _drill;
    }
}
