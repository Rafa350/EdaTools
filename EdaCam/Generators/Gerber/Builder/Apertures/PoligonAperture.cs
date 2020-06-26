namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder.Apertures {

    using System;
    using System.Globalization;
    using System.Text;
    using MikroPic.EdaTools.v1.Base.Geometry;

    /// <summary>
    /// Clase que representa una apertura poligonal.
    /// </summary>
    public sealed class PoligonAperture : Aperture {

        private readonly int _vertex;
        private readonly int _diameter;
        private readonly int _drill;
        private readonly Angle _rotation;

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
                throw new ArgumentOutOfRangeException(nameof(vertex));

            if (diameter == 0)
                throw new ArgumentOutOfRangeException(nameof(diameter));

            if (drill >= diameter)
                throw new ArgumentOutOfRangeException(nameof(drill));

            _vertex = vertex;
            _diameter = diameter;
            _drill = drill;
            _rotation = rotation;
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
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", _diameter / 1000000.0);
            sb.AppendFormat("X{0}", _vertex);
            if ((_rotation.Value > 0) || (_drill > 0))
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", _rotation.Value / 100.0);
            if (_drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", _drill / 1000000.0);
            sb.Append("*%");

            return sb.ToString();
        }

        /// <summary>
        /// Obte el numero de vertex
        /// </summary>
        /// 
        public int Vertex => _vertex;

        /// <summary>
        /// Obte el diametre exterior del poligon.
        /// </summary>
        /// 
        public int Diameter => _diameter;

        /// <summary>
        /// Obte el diametre del forat.
        /// </summary>
        /// 
        public int Drill => _drill;

        /// <summary>
        /// Obte l'angle de rotacio.
        /// </summary>
        /// 
        public Angle Rotation => _rotation;
    }
}

