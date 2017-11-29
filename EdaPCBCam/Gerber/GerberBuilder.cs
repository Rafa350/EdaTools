namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System;
    using System.Globalization;
    using System.Text;
    using System.IO;

    public enum Units {
        Unknown,
        Milimeters,
        Inches
    }

    public enum OperationCode {
        Interpolate = 1,
        Move = 2,
        Flash = 3
    }

    public enum InterpolationMode {
        Unknown,
        Linear,
        Circular
    }

    public enum CircularInterpolationQuadrant {
        Unknown,
        Single,
        Multiple
    }

    public enum CircularInterpolationDirection {
        Unknown,
        CW,
        CCW
    }

    public sealed class GerberBuilder {

        private readonly TextWriter writer;
        private readonly StringBuilder sb = new StringBuilder();

        private int macroIndex = 0;
        private int apertureIndex = 10;
        private int currentAperture = -1;
        private bool currentAperturePolarity = true;
        private double currentApertureAngle = 0;
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

        /// <summary>
        /// Marca el final del fitxer.
        /// </summary>
        /// 
        public void EndFile() {

            writer.WriteLine("M02*");
        }

        /// <summary>
        /// Afegeix una linia de text directe, sense procesar.
        /// </summary>
        /// <param name="line">El text a afeigir.</param>
        /// 
        public void Escape(string line) {

            writer.WriteLine(line);
        }

        /// <summary>
        /// Afegeix un comentari.
        /// </summary>
        /// <param name="line">La linia de comentari.</param>
        /// 
        public void Comment(string line) {

            writer.WriteLine("G04 {0} *", line);
        }

        /// <summary>
        /// Afegeix un atribut.
        /// </summary>
        /// <param name="attr"></param>
        /// 
        public void Attribute(string attr) {
        }

        public void Operation(double x, double y, OperationCode operation) {

            double cx = 0;
            double cy = 0;

            sb.Clear();
            if (currentX != x) {
                currentX = x;
                sb.Append('X');
                sb.Append(FormatNumber(x, precision, decimals));
            }
            if (currentY != y) {
                currentY = y;
                sb.Append('Y');
                sb.Append(FormatNumber(y, precision, decimals));
            }
            if (currentCX != cx) {
                currentCX = cx;
                sb.Append('I');
                sb.Append(FormatNumber(cx, precision, decimals));
            }
            if (currentCY != cy) {
                currentCY = cy;
                sb.Append('J');
                sb.Append(FormatNumber(cy, precision, decimals));
            }
            sb.AppendFormat("D{0:00}*", Convert.ToInt32(operation));

            writer.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Defineix un macro.
        /// </summary>
        /// <param name="macro">Identificador del macro. -1 si es generara automaticament.</param>
        /// <param name="command">La comanda que defineix el macro.</param>
        /// <returns>El identificador del macro.</returns>
        /// 
        public int DefineMacro(int macro, string command) {

            if (macro < -1)
                throw new ArgumentOutOfRangeException("macro");

            if (String.IsNullOrEmpty(command))
                throw new ArgumentNullException("command");

            if (macro == -1)
                macro = macroIndex++;

            sb.Clear();
            sb.AppendFormat("%AMM{0}*", macro);
            sb.Append(command);
            if (!command.EndsWith("%"))
                sb.Append('%');

            writer.WriteLine(sb.ToString());

            return macro;
        }

        /// <summary>
        /// Defineix una apertura rectangular.
        /// </summary>
        /// <param name="aperture">Identificador de l'apertura. -1 si es genera automaticament.</param>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="drill">Diametre del forar. Zero si no n'hi ha.</param>
        /// <returns>El identificador de l'apertura.</returns>
        /// 
        public int DefineRectangleAperture(int aperture, double width, double height, double drill = 0) {

            if ((aperture < 10) && (aperture != -1))
                throw new ArgumentNullException("aperture");

            if (aperture == -1)
                aperture = apertureIndex++;

            sb.Clear();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", aperture);
            sb.Append("R,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}X{1}", width, height);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            writer.WriteLine(sb.ToString());

            return aperture;
        }

        /// <summary>
        /// Defineix una apertura circular.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="drill">Diametre del forar. Zero si no n'hi ha.</param>
        /// <returns>El identificador de l'apertura.</returns>
        /// 
        public int DefineCircleAperture(int aperture, double diameter, double drill = 0) {

            if ((aperture < 10) && (aperture != -1))
                throw new ArgumentNullException("aperture");

            if (aperture == -1)
                aperture = apertureIndex++;

            sb.Clear();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", aperture);
            sb.Append("C,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", diameter);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            writer.WriteLine(sb.ToString());

            return aperture;
        }

        /// <summary>
        /// Defineix una apertura ovalada.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="drill">Diametre del forar. Zero si no n'hi ha.</param>
        /// <returns>El identificador de l'apertura.</returns>
        /// 
        public int DefineObroundAperture(int aperture, double width, double height, double drill = 0) {

            if ((aperture < 10) && (aperture != -1))
                throw new ArgumentNullException("aperture");

            if (aperture == -1)
                aperture = apertureIndex++;

            sb.Clear();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", aperture);
            sb.Append("O,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", width);
            sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", height);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            writer.WriteLine(sb.ToString());

            return aperture;
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
        public int DefinePoligonAperture(int aperture, int vertex, double diameter, double angle, double drill = 0) {

            if ((aperture < 10) && (aperture != -1))
                throw new ArgumentNullException("aperture");

            if ((vertex < 3) || (vertex > 12))
                throw new ArgumentOutOfRangeException("vertex");

            if (aperture == -1)
                aperture = apertureIndex++;

            sb.Clear();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", aperture);
            sb.Append("P,");
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", diameter);
            sb.AppendFormat("X{0}", vertex);
            if ((angle > 0) || (drill > 0))
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", angle);
            if (drill > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "X{0}", drill);
            sb.Append("*%");

            writer.WriteLine(sb.ToString());

            return aperture;
        }

        /// <summary>
        /// Declara una apertura de macro.
        /// </summary>
        /// <param name="macro">Identificador del macro previament definit.</param>
        /// <param name="args">Llista d'arguments del macro.</param>
        /// <returns>El identificador de l'apertura.</returns>
        /// 
        public int DefineMacroAperture(int aperture, int macro, params object[] args) {

            if ((aperture < 10) && (aperture != -1))
                throw new ArgumentNullException("aperture");

            if (aperture == -1)
                aperture = apertureIndex++;

            sb.Clear();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", aperture);
            sb.AppendFormat("M{0},", macro);

            bool first = true;
            foreach (object arg in args) {
                if (first)
                    first = false;
                else
                    sb.Append('X');
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", arg);
            }
            sb.Append("*%");

            writer.WriteLine(sb.ToString());

            return aperture;
        }

        /// <summary>
        /// Selecciona l'apertura.
        /// </summary>
        /// <param name="aperture">L'apertura a seleccionar.</param>
        /// 
        public void SelectAperture(int aperture) {

            if (aperture < 10)
                throw new ArgumentOutOfRangeException("aperture");

            if (currentAperture != aperture) {
                currentAperture = aperture;
                writer.WriteLine(String.Format("D{0:00}*", aperture));
            }
        }

        public void SetOffset(double x, double y) {

            writer.WriteLine(String.Format("%OFA{0}B{0}*%", x, y));
        }

        public void SetPolarity(bool positive) {

            writer.WriteLine(String.Format("%IP{0}*%", positive ? "POS" : "NEG"));
        }

        public void SetAperturePolarity(bool polarity) {

            if (currentAperturePolarity != polarity) {
                currentAperturePolarity = polarity;
                writer.WriteLine(String.Format("%LP{0}*%", polarity ? "D" : "C"));
            }
        }

        public void SetApertureRotation(double angle) {

            if (currentApertureAngle != angle) {
                currentApertureAngle = angle;
                writer.WriteLine(String.Format("%LR{0}*%", angle));
            }
        }

        /// <summary>
        /// Selecciona interpolacio linial.
        /// </summary>
        public void SetLinealInterpolationMode() {

            writer.WriteLine("G01*");
        }

        /// <summary>
        /// Selecciona interpolacio circular.
        /// </summary>
        /// <param name="quadrant">Quadrant simple o multiple.</param>
        /// <param name="direccio">Direccio dreta o esquerra.</param>
        /// 
        public void SetCircularInterpolationMode(CircularInterpolationQuadrant quadrant, CircularInterpolationDirection direction) {

            if (quadrant == CircularInterpolationQuadrant.Unknown)
                throw new ArgumentOutOfRangeException("quadrant");

            if (direction == CircularInterpolationDirection.Unknown)
                throw new ArgumentOutOfRangeException("direction");

            if (direction == CircularInterpolationDirection.CW)
                writer.WriteLine("G02*");
            else
                writer.WriteLine("G03*");

            if (quadrant == CircularInterpolationQuadrant.Single)
                writer.WriteLine("G74*");
            else
                writer.WriteLine("G75*");
        }

        /// <summary>
        /// Selecciona el format de coordinades.
        /// </summary>
        /// <param name="precision">Numero de digit significatius.</param>
        /// <param name="decimals">Numero de digits decimals.</param>
        /// 
        public void SetCoordinateFormat(int precision, int decimals) {

            if ((precision < 4) || (precision > 9))
                throw new ArgumentOutOfRangeException("precision");

            if ((decimals < 1) || (decimals > precision - 2))
                throw new ArgumentOutOfRangeException("decimals");

            this.precision = precision;
            this.decimals = decimals;
            writer.WriteLine("%FSLAX{0}{1}Y{0}{1}*%", precision - decimals, decimals);
        }

        /// <summary>
        /// Selecciona les unitats de treball.
        /// </summary>
        /// <param name="units">Les unitats.</param>
        /// 
        public void SetUnits(Units units) {

            if (units == Units.Unknown)
                throw new ArgumentOutOfRangeException("units");

            writer.WriteLine(String.Format("%MO{0}*%", units == Units.Inches ? "IN" : "MM"));
        }

        /// <summary>
        /// Formateja un numero.
        /// </summary>
        /// <param name="number">El numero a formatejar.</param>
        /// <param name="precision">Nombre de digits.</param>
        /// <param name="decimals">Nombre de posicions decimals.</param>
        /// <returns>El numero formatejat.</returns>
        /// 
        private static string FormatNumber(double number, int precision, int decimals) {

            number = Math.Round(number * Math.Pow(10, decimals));
            return number.ToString().PadLeft(precision, '0');
        }
    }
}
