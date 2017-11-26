namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using System;
    using System.Globalization;
    using System.Text;

    internal sealed class RoundRectAperture: ApertureBase {

        private readonly double width;
        private readonly double height;
        private readonly double radius;

        public RoundRectAperture(int id, double width, double height, double radius) :
            base(id) {

            this.width = width;
            this.height = height;
            this.radius = radius;
        }

        public override string GetDeclarationCommand() {

            StringBuilder sb = new StringBuilder();

            sb.Append("%AAD");
            sb.AppendFormat("{0}", Id);
            sb.Append("X0,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}X{1}X{2}X{3}", width, height, radius, 0);
            sb.Append("*%");

            return sb.ToString();
        }
    }
}
