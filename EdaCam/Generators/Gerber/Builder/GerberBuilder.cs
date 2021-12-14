using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder {

    // Unitats de mesura del fitxer gerber
    //
    public enum Units {
        Unknown,
        Milimeters,
        Inches
    }

    // Modus d'interpolacio
    //
    public enum InterpolationMode {
        Unknown,
        Linear,
        CircularSingleCW,
        CircularSingleCCW,
        CircularMultipleCW,
        CircularMultipleCCW,
    }

    // Direccio de dibuix dels arcs
    //
    public enum ArcDirection {
        CW,
        CCW
    }

    // Modus de dibuix dels arcs
    //
    public enum ArcQuadrant {
        Single,
        Multiple
    }

    // Polaritat dels objectes i regions
    //
    public enum Polarity {
        Clear,
        Dark
    }

    public enum AttributeScope {
        File,
        Aperture,
        Object,
        Delete
    }

    /// <summary>
    /// Generador de codi Gerber.
    /// </summary>
    public sealed class GerberBuilder {

        private readonly TextWriter _writer;
        private readonly StringBuilder _sb = new StringBuilder();
        private readonly State _state = new State();
        private bool _inRegion = false;
        private int _precision = 7;
        private int _decimals = 4;
        private int _offsetX = 0;
        private int _offsetY = 0;
        private EdaAngle _rotation = EdaAngle.Zero;
        private string _fmtTemplate = null;
        private double _fmtScale = 0;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="writer">Escriptor de text per la sortida.</param>
        /// 
        public GerberBuilder(TextWriter writer) {

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            _writer = writer;
        }

        /// <summary>
        /// Marca el final del fitxer.
        /// </summary>
        /// 
        public void EndFile() {

            _writer.WriteLine("M02*");
        }

        /// <summary>
        /// Afegeix una linia de text directe, sense procesar.
        /// </summary>
        /// <param name="line">El text a afeigir.</param>
        /// 
        public void Escape(string line) {

            _writer.WriteLine(line);
        }

        /// <summary>
        /// Afegeix un comentari.
        /// </summary>
        /// <param name="line">La linia de comentari.</param>
        /// 
        public void Comment(string line) {

            _writer.WriteLine("G04 {0} *", line);
        }

        /// <summary>
        /// Afegeix un atribut.
        /// </summary>
        /// <param name="attr"></param>
        /// 
        public void Attribute(AttributeScope scope, string attr = null) {

            char prefix = '\0';

            switch (scope) {
                case AttributeScope.File:
                    prefix = 'F';
                    break;

                case AttributeScope.Aperture:
                    prefix = 'A';
                    break;

                case AttributeScope.Object:
                    prefix = 'O';
                    break;

                case AttributeScope.Delete:
                    prefix = 'D';
                    break;
            }

            if (prefix != '\0')
                _writer.WriteLine(String.Format("%T{0}{1}*%", prefix, attr));
        }

        /// <summary>
        /// Flash d'una apertura en la posicio indicada. La posicio,
        /// passa a ser la posicio actual.
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// 
        public void FlashAt(EdaPoint position) {

            FlashAt(position.X, position.Y);
        }

        /// <summary>
        /// Flash d'una apertura en la posicio indicada. La posicio,
        /// passa a ser la posicio actual.
        /// </summary>
        /// <param name="x">Coordinada X de la posicio.</param>
        /// <param name="y">Coordinada Y de la posicio</param>
        /// 
        public void FlashAt(int x, int y) {

            x += _offsetX;
            y += _offsetY;

            _sb.Clear();
            if (_state.SetX(x)) {
                _sb.Append('X');
                _sb.Append(FormatNumber(x));
            }
            if (_state.SetY(y)) {
                _sb.Append('Y');
                _sb.Append(FormatNumber(y));
            }

            _sb.Append("D03*");
            _writer.WriteLine(_sb.ToString());
        }

        /// <summary>
        /// Mou la posicio actual a les coordinades especificades.
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// 
        public void MoveTo(EdaPoint position) {

            MoveTo(position.X, position.Y);
        }

        /// <summary>
        /// Mou la posicio actual a les coordinades especificades.
        /// </summary>
        /// <param name="x">Coordinada X.</param>
        /// <param name="y">Coordinada Y.</param>
        /// 
        public void MoveTo(int x, int y) {

            x += _offsetX;
            y += _offsetY;

            // Si no hi ha moviment real, no cal fa res
            //
            if ((_state.X != x) || (_state.Y != y)) {

                _sb.Clear();
                if (_state.SetX(x)) {
                    _sb.Append('X');
                    _sb.Append(FormatNumber(x));
                }
                if (_state.SetY(y)) {
                    _sb.Append('Y');
                    _sb.Append(FormatNumber(y));
                }

                _sb.Append("D02*");
                _writer.WriteLine(_sb.ToString());
            }
        }

        /// <summary>
        /// Interpola una linia desde la posicio actual fins la especificada.
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// 
        public void LineTo(EdaPoint position) {

            LineTo(position.X, position.Y);
        }

        /// <summary>
        /// Interpola una linia desde la posicio actual fins la especificada.
        /// </summary>
        /// <param name="x">Coordinada X de la posicio.</param>
        /// <param name="y">Coordinada Y de la posicio.</param>
        /// 
        public void LineTo(int x, int y) {

            x += _offsetX;
            y += _offsetY;

            if (_state.Aperture == null && !_inRegion)
                throw new InvalidOperationException("Apertura no seleccionada.");

            // Si no hi ha movinent real, no cal fer res
            //
            if ((_state.X != x) || (_state.Y != y)) {

                SetInterpolationMode(InterpolationMode.Linear);

                _sb.Clear();
                if (_state.SetX(x)) {
                    _sb.Append('X');
                    _sb.Append(FormatNumber(x));
                }
                if (_state.SetY(y)) {
                    _sb.Append('Y');
                    _sb.Append(FormatNumber(y));
                }

                _sb.Append("D01*");
                _writer.WriteLine(_sb.ToString());
            }
        }

        public void ArcTo(EdaPoint point, EdaPoint center, ArcDirection direction, ArcQuadrant quadrant = ArcQuadrant.Multiple) {

            ArcTo(point.X, point.Y, center.X, center.Y, direction, quadrant);
        }

        public void ArcTo(int x, int y, int cx, int cy, ArcDirection direction, ArcQuadrant quadrant = ArcQuadrant.Multiple) {

            x += _offsetX;
            y += _offsetY;
            cx += _offsetX;
            cy += _offsetY;

            if (_state.Aperture == null)
                throw new InvalidOperationException("Apertura no seleccionada.");

            // Si no hi ha moviment real, no cal fer res
            //
            if ((_state.X != x) || (_state.Y != y)) {

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

                _sb.Clear();
                if (_state.SetX(x)) {
                    _sb.Append('X');
                    _sb.Append(FormatNumber(x));
                }
                if (_state.SetY(y)) {
                    _sb.Append('Y');
                    _sb.Append(FormatNumber(y));
                }
                _sb.Append('I');
                _sb.Append(FormatNumber(cx));
                _sb.Append('J');
                _sb.Append(FormatNumber(cy));

                _sb.AppendFormat("D01*");
                _writer.WriteLine(_sb.ToString());
            }
        }

        /// <summary>
        /// Interpola una polilinia.
        /// </summary>
        /// <param name="points">Sequencia de punts.</param>
        /// 
        public void Polyline(IEnumerable<EdaPoint> points) {

            foreach (EdaPoint point in points)
                LineTo(point);
        }

        /// <summary>
        /// Interpola un poligon. 
        /// </summary>
        /// <param name="points">Sequencia de punts.</param>
        /// 
        public void Polygon(IEnumerable<EdaPoint> points) {

            bool first = true;
            EdaPoint firstPoint = default(EdaPoint);
            foreach (EdaPoint point in points) {
                if (first) {
                    first = false;
                    firstPoint = point;
                    MoveTo(point);
                }
                else
                    LineTo(point);
            }
            LineTo(firstPoint);
        }

        /// <summary>
        /// Defineix una apertura.
        /// </summary>
        /// <param name="aperture">L'apertura a definir.</param>
        /// 
        public void DefineAperture(Aperture aperture) {

            if (aperture == null)
                throw new ArgumentNullException(nameof(aperture));

            _writer.WriteLine(aperture.Command);
        }

        /// <summary>
        /// Defineix una col·leccio d'apertures.
        /// </summary>
        /// <param name="apertures">Les apertures a definir.</param>
        /// 
        public void DefineApertures(IEnumerable<Aperture> apertures) {

            if (apertures == null)
                throw new ArgumentNullException(nameof(apertures));

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
                throw new ArgumentNullException(nameof(macro));

            _writer.WriteLine(macro.Command);
        }

        /// <summary>
        /// Definicio d'una col·leccio de macros.
        /// </summary>
        /// <param name="macros">Taula de macros.</param>
        /// 
        public void DefineMacros(IEnumerable<Macro> macros) {

            if (macros == null)
                throw new ArgumentNullException(nameof(macros));

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
                throw new ArgumentNullException(nameof(aperture));

            if (_state.SetAperture(aperture))
                _writer.WriteLine(String.Format("D{0:00}*", aperture.Id));
        }

        /// <summary>
        /// Inicia una regio.
        /// </summary>
        /// 
        public void BeginRegion() {

            if (_inRegion)
                throw new InvalidOperationException("Ya hay una region abierta.");

            _inRegion = true;
            _writer.WriteLine("G36*");
        }

        /// <summary>
        /// Finalitza una regio.
        /// </summary>
        /// 
        public void EndRegion() {

            if (!_inRegion)
                throw new InvalidOperationException("No hay ninguna region abierta.");

            _writer.WriteLine("G37*");
            _inRegion = false;
        }

        /// <summary>
        /// Dibuixa una regio.
        /// </summary>
        /// <param name="points">La llista de punts que conformen la regio.</param>
        /// 
        public void Region(IEnumerable<EdaPoint> points, bool close = false) {

            bool first = true;
            EdaPoint firstPoint = default(EdaPoint);
            foreach (EdaPoint point in points) {
                if (first) {
                    first = false;
                    firstPoint = point;
                    MoveTo(point);
                }
                else
                    LineTo(point);
            }
            if (close)
                LineTo(firstPoint);
        }

        /// <summary>
        /// Selecciona la polaritat de les apertures.
        /// </summary>
        /// <param name="polarity">Datk o Clear.</param>
        /// 
        public void LoadPolarity(Polarity polarity) {

            if (_state.SetAperturePolarity(polarity))
                _writer.WriteLine(String.Format("%LP{0}*%", polarity == Polarity.Dark ? "D" : "C"));
        }

        /// <summary>
        /// Selecciona l'angle de rotacio de les apertures.
        /// </summary>
        /// <param name="angle">Angle de rotacio.</param>
        /// 
        public void LoadRotation(EdaAngle angle) {

            if (_state.SetApertureAngle(angle))
                _writer.WriteLine(String.Format("%LR{0}*%", (double)angle.Value / 100.0));
        }

        public void LoadMirroring() {

            throw new NotImplementedException();
        }

        public void LoadScaling() {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Selecciona el modus d'interpolacio.
        /// </summary>
        /// <param name="interpolationMode">El modus d'interpolacio.</param>
        /// 
        public void SetInterpolationMode(InterpolationMode interpolationMode) {

            if (_state.SetInterpolationMode(interpolationMode)) {

                switch (interpolationMode) {
                    case InterpolationMode.Linear:
                        _writer.WriteLine("G01*");
                        break;

                    case InterpolationMode.CircularSingleCW:
                    case InterpolationMode.CircularMultipleCW:
                        _writer.WriteLine("G02*");
                        break;

                    case InterpolationMode.CircularSingleCCW:
                    case InterpolationMode.CircularMultipleCCW:
                        _writer.WriteLine("G03*");
                        break;
                }

                switch (interpolationMode) {
                    case InterpolationMode.CircularSingleCW:
                    case InterpolationMode.CircularSingleCCW:
                        _writer.WriteLine("G74*");
                        break;

                    case InterpolationMode.CircularMultipleCW:
                    case InterpolationMode.CircularMultipleCCW:
                        _writer.WriteLine("G75*");
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
                throw new ArgumentOutOfRangeException(nameof(precision));

            if ((decimals < 1) || (decimals > precision - 2))
                throw new ArgumentOutOfRangeException(nameof(decimals));

            this._precision = precision;
            this._decimals = decimals;
            this._fmtTemplate = null;

            _writer.WriteLine("%FSLAX{0}{1}Y{0}{1}*%", precision - decimals, decimals);
        }

        /// <summary>
        /// Selecciona les unitats de treball.
        /// </summary>
        /// <param name="units">Les unitats.</param>
        /// 
        public void SetUnits(Units units) {

            if (units == Units.Unknown)
                throw new ArgumentOutOfRangeException(nameof(units));

            _writer.WriteLine(String.Format("%MO{0}*%", units == Units.Inches ? "IN" : "MM"));
        }

        /// <summary>
        /// Asigna una transformacio de coordinades.
        /// </summary>
        /// <param name="offset">Desplaçament</param>
        /// <param name="rotation">Rotacio respecte el punt especificat com a desplaxament.</param>
        /// 
        public void SetTransformation(EdaPoint offset, EdaAngle rotation) {

            SetTransformation(offset.X, offset.Y, rotation);
        }

        /// <summary>
        /// Asigna una transformacio de coordinades.
        /// </summary>
        /// <param name="offsetX">Desplaçament X</param>
        /// <param name="offsetY">Desplaçament y</param>
        /// <param name="rotation">Rotacio respecte el punt especificat com a desplaxament.</param>
        /// 
        public void SetTransformation(int offsetX, int offsetY, EdaAngle rotation) {

            this._offsetX = offsetX;
            this._offsetY = offsetY;
            this._rotation = rotation;
        }

        /// <summary>
        /// Desactiva la transformacio de coordinades.
        /// </summary>
        /// 
        public void ResetTransformation() {

            _offsetX = 0;
            _offsetY = 0;
            _rotation = EdaAngle.Zero;
        }

        /// <summary>
        /// Formateja un numero al format de coordinades.
        /// </summary>
        /// <param name="number">El numero a formatejar.</param>
        /// <returns>El numero formatejat.</returns>
        /// 
        private string FormatNumber(int number) {

            if (_fmtTemplate == null) {
                _fmtTemplate = String.Format("{{0:{0}}}", new String('0', _precision));
                _fmtScale = Math.Pow(10, _decimals);
            }

            return String.Format(_fmtTemplate, Math.Round(((double)number / 1000000.0) * _fmtScale));
        }
    }
}
