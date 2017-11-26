namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using System;
    using System.Globalization;
    using System.Text;

    internal sealed class CircleAperture: ApertureBase {

        private readonly double diameter;
        private readonly double drill;

        public CircleAperture(int id, double diameter, double drill) : 
            base(id) {

            this.diameter = diameter;
            this.drill = drill;
        }

        public override string GetDeclarationCommand() {

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
    }
}
