namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder.Apertures {

    using System;
    using System.Text;
    using System.Globalization;

    /// <summary>
    /// Clase que representa una apertura circular.
    /// </summary>
    public sealed class CircleAperture : Aperture {

        public readonly int diameter;
        public readonly int drill;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="id">Identificador.</param>
        /// <param name="tag">Dades opcionals.</param>
        /// <param name="diameter">Diametre extern.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// 
        public CircleAperture(int id, object tag, int diameter, int drill = 0) :
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
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", diameter / 1000000.0);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill / 1000000.0);
            sb.Append("*%");

            return sb.ToString();
        }

        /// <summary>
        /// Obte el diametre extern.
        /// </summary>
        /// 
        public int Diameter {
            get {
                return diameter;
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
