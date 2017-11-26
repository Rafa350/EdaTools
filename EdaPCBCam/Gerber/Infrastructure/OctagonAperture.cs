namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using System;
    using System.Text;
    using System.Globalization;

    internal sealed class OctagonAperture: ApertureBase {

        private readonly double diameter;
        private readonly double drill;

        public OctagonAperture(int id, double diameter, double drill) :
            base(id) {

            this.diameter = diameter;
            this.drill = drill;
        }

        public override string GetDeclarationCommand() {

            StringBuilder sb = new StringBuilder();

            sb.Append("%AAD");
            sb.AppendFormat("{0}", Id);
            sb.Append("P,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", diameter);
            sb.Append("X8");
            sb.Append("X22.5");
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            return sb.ToString();
        }
    }
}
