namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

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

    public enum AperturePolarity {
        Clear,
        Dark
    }

    public sealed class GerberBuilder {

        private readonly TextWriter writer;
        private readonly StringBuilder sb = new StringBuilder();

        private sealed class State {
            public int Aperture { get; set; }
            public AperturePolarity AperturePolarity { get; set; }
            public double ApertureAngle { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double CX { get; set; }
            public double CY { get; set; }

            public State() {
                Aperture = -1;
                AperturePolarity = AperturePolarity.Dark;
                ApertureAngle = 0;
                X = 0;
                Y = 0;
                CX = 0;
                CY = 0;
            }
        }

        private int macroIndex = 0;
        private int apertureIndex = 10;
        private State state = new State();
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
            if (state.X != x) {
                state.X = x;
                sb.Append('X');
                sb.Append(FormatNumber(x, precision, decimals));
            }
            if (state.Y != y) {
                state.Y = y;
                sb.Append('Y');
                sb.Append(FormatNumber(y, precision, decimals));
            }
            if (state.CX != cx) {
                state.CX = cx;
                sb.Append('I');
                sb.Append(FormatNumber(cx, precision, decimals));
            }
            if (state.CY != cy) {
                state.CY = cy;
                sb.Append('J');
                sb.Append(FormatNumber(cy, precision, decimals));
            }
            sb.AppendFormat("D{0:00}*", Convert.ToInt32(operation));

            writer.WriteLine(sb.ToString());
        }

        public void DefineAperture(Aperture aperture) {

            if (aperture == null)
                throw new ArgumentNullException("aperture");

            writer.WriteLine(aperture.Command);
        }

        public void DefineApertures(IEnumerable<Aperture> apertures) {

            if (apertures == null)
                throw new ArgumentNullException("apertures");

            foreach (Aperture aperture in apertures)
                DefineAperture(aperture);
        }

        /// <summary>
        /// Definicio d'un macro.
        /// </summary>
        /// <param name="macro">El macro a definir.</param>
        /// 
        public void DefineMacro(Macro macro) {

            if (macro == null)
                throw new ArgumentNullException("macro");

            writer.WriteLine(macro.Command);
        }

        /// <summary>
        /// Definicio d'una col·leccio de macros de macros.
        /// </summary>
        /// <param name="macros">Taula de macros.</param>
        /// 
        public void DefineMacros(IEnumerable<Macro> macros) {

            if (macros == null)
                throw new ArgumentNullException("macros");

            foreach (Macro macro in macros)
                DefineMacro(macro);
        }

        /// <summary>
        /// Selecciona l'apertura.
        /// </summary>
        /// <param name="aperture">L'apertura a seleccionar.</param>
        /// 
        public void SelectAperture(Aperture aperture) {

            if (aperture == null)
                throw new ArgumentNullException("aperture");

            if (state.Aperture != aperture.Id) {
                state.Aperture = aperture.Id;
                writer.WriteLine(String.Format("D{0:00}*", aperture.Id));
            }
        }

        public void SetOffset(double x, double y) {

            writer.WriteLine(String.Format("%OFA{0}B{0}*%", x, y));
        }

        public void SetPolarity(bool positive) {

            writer.WriteLine(String.Format("%IP{0}*%", positive ? "POS" : "NEG"));
        }

        public void SetAperturePolarity(AperturePolarity polarity) {

            if (state.AperturePolarity != polarity) {
                state.AperturePolarity = polarity;
                writer.WriteLine(String.Format("%LP{0}*%", polarity == AperturePolarity.Dark ? "D" : "C"));
            }
        }

        public void SetApertureRotation(double angle) {

            if (state.ApertureAngle != angle) {
                state.ApertureAngle = angle;
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
