namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using System;
    using System.Text;
    using System.Globalization;

    /// <summary>
    /// Clase que representa una apertura poligonal.
    /// </summary>
    public sealed class PoligonAperture : Aperture {

        private readonly int vertex;
        private readonly double diameter;
        private readonly double drill;
        private readonly Angle angle;

        public PoligonAperture(int id, object tag, int vertex, double diameter, Angle angle, double drill = 0) :
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
            this.angle = angle;
        }

        protected override string GetCommand() {

            StringBuilder sb = new StringBuilder();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", Id);
            sb.Append("P,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", diameter);
            sb.AppendFormat("X{0}", vertex);
            if ((angle.Degrees > 0) || (drill > 0))
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", angle.Degrees);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            return sb.ToString();
        }

        public int Vertex {
            get {
                return vertex;
            }
        }

        public double Diameter {
            get {
                return diameter;
            }
        }

        public double Drill {
            get {
                return drill;
            }
        }

        public Angle Angle {
            get {
                return angle;
            }
        }
    }
}

