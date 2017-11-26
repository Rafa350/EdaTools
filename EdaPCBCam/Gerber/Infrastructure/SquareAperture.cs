namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using System;
    using System.Globalization;
    using System.Text;

    internal class SquareAperture: ApertureBase {

        private readonly double size;
        private readonly double drill;

        public SquareAperture(int id, double size, double drill) :
            base(id) {

            this.size = size;
            this.drill = drill;
        }

        public override string GetDeclarationCommand() {

            StringBuilder sb = new StringBuilder();

            sb.Append("%AAD");
            sb.AppendFormat("{0}", Id);
            sb.Append("R,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}X{0}", size);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            return sb.ToString();
        }
    }
}
