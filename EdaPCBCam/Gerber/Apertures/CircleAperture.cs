namespace MikroPic.EdaTools.v1.Cam.Gerber.Apertures {

    using System;
    using System.Text;
    using System.Globalization;

    public sealed class CircleAperture: Aperture {

        public readonly double diameter;
        public readonly double drill;

        public CircleAperture(double diameter, double drill = 0) {

            if (diameter <= 0)
                throw new ArgumentOutOfRangeException("diameter");

            if (drill >= diameter)
                throw new ArgumentOutOfRangeException("drill");

            this.diameter = diameter;
            this.drill = drill;
        }

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

        public double Diameter { get { return diameter; } }
        public double Drill { get { return drill; } }
    }
}
