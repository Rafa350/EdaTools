namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures {

    using System;
    using System.Text;
    using System.Globalization;

    /// <summary>
    /// Clase que representa una apertura circular.
    /// </summary>
    public sealed class CircleAperture : Aperture {

        public readonly double diameter;
        public readonly double drill;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="id">Identificador.</param>
        /// <param name="tag">Dades opcionals.</param>
        /// <param name="diameter">Diametre extern.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// 
        public CircleAperture(int id, object tag, double diameter, double drill = 0) :
            base(id, tag) {

            if (diameter < 0) // C es l'unica que permet tamany zero
                throw new ArgumentOutOfRangeException("diameter");

            if (drill >= diameter)
                throw new ArgumentOutOfRangeException("drill");

            this.diameter = diameter;
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
            sb.Append("C,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", diameter);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            return sb.ToString();
        }

        /// <summary>
        /// Obte el diametre extern.
        /// </summary>
        /// 
        public double Diameter {
            get {
                return diameter;
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
