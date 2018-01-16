namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures {

    using System;
    using System.Globalization;
    using System.Text;

    public sealed class ObroundAperture : Aperture {

        private readonly double width;
        private readonly double height;
        private readonly double drill;

        public ObroundAperture(int id, double width, double height, double drill = 0) :
            base(id) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            if (drill >= width || drill >= height)
                throw new ArgumentOutOfRangeException("drill");

            this.width = width;
            this.height = height;
            this.drill = drill;
        }

        protected override string GetCommand() {

            StringBuilder sb = new StringBuilder();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", Id);
            sb.Append("O,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", width);
            sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", height);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            return sb.ToString();
        }

        public double Width {
            get {
                return width;
            }
        }

        public double Height {
            get {
                return height;
            }
        }

        public double Angle {
            get {
                return Angle;
            }
        }
    }
}
