namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures {

    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Clase que representa una aperture rectangular.
    /// </summary>
    public sealed class RectangleAperture : Aperture {

        private readonly double width;
        private readonly double height;
        private readonly double drill;

        public RectangleAperture(int id, object tag, double width, double height, double drill = 0) :
            base(id, tag) {

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
            sb.Append("R,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}X{1}", width, height);
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

        public double Drill {
            get {
                return drill;
            }
        }
    }
}
