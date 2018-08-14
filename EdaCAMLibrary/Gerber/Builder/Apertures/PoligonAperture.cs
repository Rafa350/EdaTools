namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures {

    using MikroPic.EdaTools.v1.Geometry;
    using System;
    using System.Text;
    using System.Globalization;

    /// <summary>
    /// Clase que representa una apertura poligonal.
    /// </summary>
    public sealed class PoligonAperture : Aperture {

        private readonly int vertex;
        private readonly int diameter;
        private readonly int drill;
        private readonly Angle rotation;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id">Identificador de l'aperture.</param>
        /// <param name="tag">Dades opcionals.</param>
        /// <param name="vertex">Numero de vertex</param>
        /// <param name="diameter">Diametre exterior.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// 
        public PoligonAperture(int id, object tag, int vertex, int diameter, Angle rotation, int drill = 0) :
            base(id, tag) {

            if ((vertex < 3) || (vertex > 12))
                throw new ArgumentOutOfRangeException("vertex");

            if (diameter == 0)
                throw new ArgumentOutOfRangeException("diameter");

            if (drill >= diameter)
                throw new ArgumentOutOfRangeException("drill");

            this.vertex = vertex;
            this.diameter = diameter;
            this.drill = drill;
            this.rotation = rotation;
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
            sb.Append("P,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", diameter / 1000000.0);
            sb.AppendFormat("X{0}", vertex);
            if ((rotation.Degrees > 0) || (drill > 0))
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", rotation.Degrees / 100.0);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill / 1000000.0);
            sb.Append("*%");

            return sb.ToString();
        }

        /// <summary>
        /// Obte el numero de vertex
        /// </summary>
        /// 
        public int Vertex {
            get {
                return vertex;
            }
        }

        /// <summary>
        /// Obte el diametre exterior del poligon.
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

        /// <summary>
        /// Obte l'angle de rotacio.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return rotation;
            }
        }
    }
}

