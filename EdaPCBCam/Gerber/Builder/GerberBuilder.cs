namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.IO;
    using System.Text;

    public enum Units {
        Unknown,
        Milimeters,
        Inches
    }

    public enum InterpolationMode {
        Unknown,
        Linear,
        CircularSingleCW,
        CircularSingleCCW,
        CircularMultipleCW,
        CircularMultipleCCW,
    }

    public enum ArcDirection {
        CW,
        CCW
    }

    public enum ArcQuadrant {
        Single,
        Multiple
    }

    public enum Polarity {
        Clear,
        Dark
    }

    public sealed class GerberBuilder {

        private readonly TextWriter writer;
        private readonly StringBuilder sb = new StringBuilder();       
        private readonly State state = new State();
        private bool inRegion = false;
        private int precision = 7;
        private int decimals = 4;
        private string fmt;

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

            writer.WriteLine(String.Format("%TF{0}*%", attr));
        }

        public void FlashAt(Point point) {

            FlashAt(point.X, point.Y);
        }

        /// <summary>
        /// Flash d'una apertura en la posicio indicada. La posicio,
        /// passa a ser la posicio actual.
        /// </summary>
        /// <param name="x">Coordinada X de la posicio.</param>
        /// <param name="y">Coordinada Y de la posicio</param>
        /// 
        public void FlashAt(double x, double y) {

            sb.Clear();
            if (state.SetX(x)) { 
                sb.Append('X');
                sb.Append(FormatNumber(x));
            }
            if (state.SetY(y)) {
                sb.Append('Y');
                sb.Append(FormatNumber(y));
            }

            sb.Append("D03*");
            writer.WriteLine(sb.ToString());
        }

        public void MoveTo(Point point) {

            MoveTo(point.X, point.Y);
        }

        /// <summary>
        /// Mou la posicio actual a les coordinades especificades.
        /// </summary>
        /// <param name="x">Coordinada X.</param>
        /// <param name="y">Coordinada Y.</param>
        /// 
        public void MoveTo(double x, double y) {

            // Si no hi ha moviment real, no cal fa res
            //
            if ((state.X != x) || (state.Y != y)) {

                sb.Clear();
                if (state.SetX(x)) {
                    sb.Append('X');
                    sb.Append(FormatNumber(x));
                }
                if (state.SetY(y)) {
                    sb.Append('Y');
                    sb.Append(FormatNumber(y));
                }

                sb.Append("D02*");
                writer.WriteLine(sb.ToString());
            }
        }

        public void LineTo(Point point) {

            LineTo(point.X, point.Y);
        }

        /// <summary>
        /// Interpola una linia desde la posicio actual fins la especificada.
        /// </summary>
        /// <param name="x">Coordinada X.</param>
        /// <param name="y">Coordinada Y.</param>
        /// 
        public void LineTo(double x, double y) {

            if (state.Aperture == null && !inRegion)
                throw new InvalidOperationException("Apertura no seleccionada.");

            // Si no hi ha movinent real, no cal fer res
            //
            if ((state.X != x) || (state.Y != y)) {

                SetInterpolationMode(InterpolationMode.Linear);

                sb.Clear();
                if (state.SetX(x)) {
                    sb.Append('X');
                    sb.Append(FormatNumber(x));
                }
                if (state.SetY(y)) {
                    sb.Append('Y');
                    sb.Append(FormatNumber(y));
                }

                sb.Append("D01*");
                writer.WriteLine(sb.ToString());
            }
        }

        public void ArcTo(double x, double y, double cx, double cy, ArcDirection direction, ArcQuadrant quadrant = ArcQuadrant.Multiple) {

            if (state.Aperture == null)
                throw new InvalidOperationException("Apertura no seleccionada.");

            // Si no hi ha moviment real, no cal fer res
            //
            if ((state.X != x) || (state.Y != y)) {

                if (direction == ArcDirection.CW) {
                    if (quadrant == ArcQuadrant.Single)
                        SetInterpolationMode(InterpolationMode.CircularSingleCW);
                    else
                        SetInterpolationMode(InterpolationMode.CircularMultipleCW);
                }
                else {
                    if (quadrant == ArcQuadrant.Single)
                        SetInterpolationMode(InterpolationMode.CircularSingleCCW);
                    else
                        SetInterpolationMode(InterpolationMode.CircularMultipleCCW);
                }

                sb.Clear();
                if (state.SetX(x)) {
                    sb.Append('X');
                    sb.Append(FormatNumber(x));
                }
                if (state.SetY(y)) {
                    sb.Append('Y');
                    sb.Append(FormatNumber(y));
                }
                if (state.SetCX(cx)) {
                    sb.Append('I');
                    sb.Append(FormatNumber(cx));
                }
                if (state.SetCY(cy)) {
                    sb.Append('J');
                    sb.Append(FormatNumber(cy));
                }

                sb.AppendFormat("D01*");
                writer.WriteLine(sb.ToString());
            }
        }

        /// <summary>
        /// Defineix una apertura.
        /// </summary>
        /// <param name="aperture">L'apertura a definir.</param>
        /// 
        public void DefineAperture(Aperture aperture) {

            if (aperture == null)
                throw new ArgumentNullException("aperture");

            writer.WriteLine(aperture.Command);
        }

        /// <summary>
        /// Defineix una col·leccio d'apertures.
        /// </summary>
        /// <param name="apertures">Les apertures a definir.</param>
        /// 
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
        /// Definicio d'una col·leccio de macros.
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

            if (state.SetAperture(aperture))
                writer.WriteLine(String.Format("D{0:00}*", aperture.Id));
        }

        /// <summary>
        /// Inicia una regio.
        /// </summary>
        /// 
        public void BeginRegion() {

            if (inRegion)
                throw new InvalidOperationException("Ya hay una region abierta.");

            inRegion = true;
            writer.WriteLine("G36*");
        }

        /// <summary>
        /// Finalitza una regio.
        /// </summary>
        /// 
        public void EndRegion() {

            if (!inRegion)
                throw new InvalidOperationException("No hay ninguna region abierta.");

            writer.WriteLine("G37*");
            inRegion = false;
        }

        public void Region(IEnumerable<Point> points) {

            bool first = true;
            foreach (Point point in points) {
                if (first) {
                    first = false;
                    MoveTo(point);
                }
                else
                    LineTo(point);
            }
        }

        public void SetOffset(double x, double y) {

            writer.WriteLine(String.Format("%OFA{0}B{0}*%", x, y));
        }

        public void SetPolarity(bool positive) {

            writer.WriteLine(String.Format("%IP{0}*%", positive ? "POS" : "NEG"));
        }

        public void LoadPolarity(Polarity polarity) {

            if (state.SetAperturePolarity(polarity)) 
                writer.WriteLine(String.Format("%LP{0}*%", polarity == Polarity.Dark ? "D" : "C"));
        }

        public void LoadRotation(double angle) {

            if (state.SetApertureAngle(angle))
                writer.WriteLine(String.Format("%LR{0}*%", angle));
        }

        public void LoadMirroring() {

        }

        public void LoadScaling() {

        }

        /// <summary>
        /// Selecciona el modus d'interpolacio.
        /// </summary>
        public void SetInterpolationMode(InterpolationMode interpolationMode) {

            if (state.SetInterpolationMode(interpolationMode)) {

                switch(interpolationMode) {
                    case InterpolationMode.Linear:
                        writer.WriteLine("G01*");
                        break;

                    case InterpolationMode.CircularSingleCW:
                    case InterpolationMode.CircularMultipleCW:
                        writer.WriteLine("G02*");
                        break;

                    case InterpolationMode.CircularSingleCCW:
                    case InterpolationMode.CircularMultipleCCW:
                        writer.WriteLine("G03*");
                        break;
                }

                switch (interpolationMode) {
                    case InterpolationMode.CircularSingleCW:
                    case InterpolationMode.CircularSingleCCW:
                        writer.WriteLine("G74*");
                        break;

                    case InterpolationMode.CircularMultipleCW:
                    case InterpolationMode.CircularMultipleCCW:
                        writer.WriteLine("G75*");
                        break;
                }
            }
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

            fmt = String.Format("{{0:{0}}}", new String('0', precision));

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
        /// <returns>El numero formatejat.</returns>
        /// 
        private string FormatNumber(double number) {

            for (int i = 0; i < decimals; i++)
                number *= 10;
            number = Math.Round(number);

            return String.Format(fmt, number);
        }
    }
}
