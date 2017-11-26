namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System;
    using System.Globalization;
    using System.Text;
    using System.IO;

    public enum Units {
        Milimeters,
        Inches
    }

    public enum OperationCode {
        Interpolate = 1,
        Move = 2,
        Flash = 3
    }

    public enum InterpolationMode {
        Linear,
        Circular
    }

    public sealed class GerberBuilder {

        private readonly TextWriter writer;
        private readonly StringBuilder sb = new StringBuilder();

        private int macroIndex = 0;
        private int apertureIndex = 0;
        private int currentAperture = -1;
        private double currentX = 0;
        private double currentY = 0;
        private double currentCX = 0;
        private double currentCY = 0;
        private int precision = 7;
        private int decimals = 4;

        public GerberBuilder(TextWriter writer) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        public void EndFile() {

            writer.WriteLine("M02*");
        }

        public void Escape(string line) {

            writer.WriteLine(line);
        }

        /// <summary>
        /// Afegeix un comentari al fitxer.
        /// </summary>
        /// <param name="line">La linia de comentari.</param>
        /// 
        public void Comment(string line) {

            writer.WriteLine("G04 {0} *", line);
        }

        public void Operation(double x, double y, OperationCode operation) {

            double cx = 0;
            double cy = 0;

            double multiplier = 10 * decimals;

            sb.Clear();
            if (currentX != x) {
                currentX = x;
                sb.AppendFormat("X{0:0000000}", Convert.ToInt32(x * multiplier));
            }
            if (currentY != y) {
                currentY = y;
                sb.AppendFormat("Y{0:0000000}", Convert.ToInt32(y * multiplier));
            }
            if (currentCX != cx) {
                currentCX = cx;
                sb.AppendFormat("I{0:0000000}", Convert.ToInt32(cx * multiplier));
            }
            if (currentCY != cy) {
                currentCY = cy;
                sb.AppendFormat("J{0:0000000}", Convert.ToInt32(cy * multiplier));
            }
            sb.AppendFormat("D{0:00}*", Convert.ToInt32(operation));

            writer.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Defineix un macro.
        /// </summary>
        /// <param name="command">La comanda que defineix el macro.</param>
        /// <returns>El identificador del macro.</returns>
        /// 
        public int DefineMacro(string command) {

            sb.Clear();
            sb.AppendFormat("%AMX{0}*", macroIndex);
            sb.Append(command);
            if (!command.EndsWith("%"))
                sb.Append('%');

            writer.WriteLine(sb.ToString());

            return macroIndex++;
        }

        /// <summary>
        /// Defineix una apertura rectangular.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="drill">Diametre del forar. Zero si no n'hi ha.</param>
        /// <returns>El identificador de l'apertura.</returns>
        /// 
        public int DefineRectangularAperture(double width, double height, double drill = 0) {

            sb.Clear();
            sb.Append("%AAD");
            sb.AppendFormat("{0}", apertureIndex);
            sb.Append("R,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}X{1}", width, height);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            return apertureIndex++;
        }

        /// <summary>
        /// Defineix una apertura circular.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="drill">Diametre del forar. Zero si no n'hi ha.</param>
        /// <returns>El identificador de l'apertura.</returns>
        /// 
        public int DefineCircularAperture(double diameter, double drill = 0) {

            sb.Clear();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", apertureIndex);
            sb.Append("C,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", diameter);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            writer.WriteLine(sb.ToString());

            return apertureIndex++;
        } 

        /// <summary>
        /// Defineix una apertura poligonal.
        /// </summary>
        /// <param name="vertex">Nombre de vertex.</param>
        /// <param name="diameter">Diametre exterior del poligon.</param>
        /// <param name="angle">Angle de rotacio.</param>
        /// <param name="drill">Diametre del forat. Zero si no n'hi ha.</param>
        /// <returns>El identificador de l'apertura.</returns>
        /// 
        public int DefinePoligonalAperture(int vertex, double diameter, double angle, double drill = 0) {

            sb.Append("%AAD");
            sb.AppendFormat("{0}", apertureIndex);
            sb.Append("P,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", diameter);
            sb.AppendFormat("X{0}", vertex);
            sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", angle);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");


            return apertureIndex++;
        }

        /// <summary>
        /// Declara una aperture de macro.
        /// </summary>
        /// <param name="macro">Identificador del macro previament definit.</param>
        /// <param name="args">Llista d'arguments del macro.</param>
        /// <returns>El identificador de l'apertura.</returns>
        /// 
        public int DefineMacroAperture(int macro, params object[] args) {

            sb.Clear();
            sb.Append("%AAD");
            sb.AppendFormat("{0}", apertureIndex);
            sb.AppendFormat("X{0},", macro);

            bool first = true;
            foreach (object arg in args) {
                if (first)
                    first = false;
                else
                    sb.Append('X');
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", arg);
            }
            sb.Append("*%");

            return apertureIndex++;
        }

        public void SelectAperture(int aperture) {

            if (currentAperture != aperture) {
                currentAperture = aperture;
                writer.WriteLine(String.Format("D{0:00}*", aperture));
            }
        }

        public void SetInterpolation(InterpolationMode mode, bool mq, bool ccw) {

            if (mode == InterpolationMode.Linear) 
                writer.WriteLine("G01*");
            else {
                if (ccw)
                    writer.WriteLine("G02*");
                else
                    writer.WriteLine("G03*");
                if (mq)
                    writer.WriteLine("G75*");
                else
                    writer.WriteLine("G75*");
            }

        }

        public void SetPrecision(int precision, int decimals) {

            this.precision = precision;
            this.decimals = decimals;
            writer.WriteLine("%FSLAX{0}{1}Y{0}{1}%*", precision - decimals, decimals);
        }

        /// <summary>
        /// Selecciona les unitats de treball.
        /// </summary>
        /// <param name="units">Les unitats.</param>
        /// 
        public void SetUnits(Units units) {

            writer.WriteLine(String.Format("%MO{0}*%", units == Units.Inches ? "IN" : "MM"));
        }
    }
}
