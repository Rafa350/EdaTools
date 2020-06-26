namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder {

    using System;
    using MikroPic.EdaTools.v1.Base.Geometry;

    /// <summary>
    /// Clase que gestiona l'estat intern del generador gerber.
    /// </summary>
    internal sealed class State {

        private Aperture _aperture = null;
        private Polarity _aperturePolarity = Polarity.Dark;
        private Angle _apertureAngle = Angle.Zero;
        private double _apertureScale = 0;
        private bool _apertureMirror = false;
        private InterpolationMode _interpolationMode = InterpolationMode.Unknown;
        private int _x = 0;
        private int _y = 0;

        public State() {

        }

        /// <summary>
        /// Selecciona el modus d'interpolacio.
        /// </summary>
        /// <param name="interpolationMode">Modus d'interpolacio.</param>
        /// <returns>True si ha canviat l'estat.</returns>
        /// 
        public bool SetInterpolationMode(InterpolationMode interpolationMode) {

            if (interpolationMode == InterpolationMode.Unknown)
                throw new ArgumentOutOfRangeException(nameof(interpolationMode));

            if (_interpolationMode != interpolationMode) {
                _interpolationMode = interpolationMode;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Asigna el valor a la coordinada X de la posicio actual.
        /// </summary>
        /// <param name="x">El valor de la coordinada.</param>
        /// <returns>True si ha canviat l'estat.</returns>
        /// 
        public bool SetX(int x) {

            if (_x != x) {
                _x = x;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Asigna el valor a la coordinada Y de la posicio actual.
        /// </summary>
        /// <param name="x">El valor de la coordinada.</param>
        /// <returns>True si ha canviat l'estat.</returns>
        /// 
        public bool SetY(int y) {

            if (_y != y) {
                _y = y;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Selecciona l'apertura.
        /// </summary>
        /// <param name="aperture">L'apertura a seleccionar.</param>
        /// <returns>True si ha canviat l'estat.</returns>
        /// 
        public bool SetAperture(Aperture aperture) {

            if (aperture == null)
                throw new ArgumentNullException(nameof(aperture));

            if (_aperture != aperture) {
                _aperture = aperture;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Selecciona l'angle de l'apertura.
        /// </summary>
        /// <param name="apertureAngle">El valor de l'angle en graus.</param>
        /// <returns>True si ha canviat l'estat.</returns>
        /// 
        public bool SetApertureAngle(Angle apertureAngle) {

            if (_apertureAngle != apertureAngle) {
                _apertureAngle = apertureAngle;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Selecciona la polatirat de l'apertura.
        /// </summary>
        /// <param name="aperturePolarity">El valor de la polaritat.</param>
        /// <returns>True si ha canviat l'estat.</returns>
        /// 
        public bool SetAperturePolarity(Polarity aperturePolarity) {

            if (_aperturePolarity != aperturePolarity) {
                _aperturePolarity = aperturePolarity;
                return true;
            }
            else
                return false;
        }

        public bool SetApertureMirror(bool apertureMirror) {

            if (_apertureMirror != apertureMirror) {
                _apertureMirror = apertureMirror;
                return true;
            }
            else
                return false;
        }

        public bool SetApertureScale(double apertureScale) {

            if (_apertureScale != apertureScale) {
                _apertureScale = apertureScale;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Obte el modus d'interpolacio.
        /// </summary>
        /// 
        public InterpolationMode InterpolationMode => InterpolationMode;

        /// <summary>
        /// Obte la coordinada X de la posicio actual.
        /// </summary>
        /// 
        public int X => _x;

        /// <summary>
        /// Obte la coordinada Y de la posicio actual.
        /// </summary>
        /// 
        public int Y => _y;

        /// <summary>
        /// Obte l'apertura seleccionada.
        /// </summary>
        /// 
        public Aperture Aperture => _aperture;

        /// <summary>
        /// Obte l'angle de l'apertura.
        /// </summary>
        /// 
        public Angle ApertureAngle => _apertureAngle;

        /// <summary>
        /// Opte la polaritat de l'apertura.
        /// </summary>
        /// 
        public Polarity AperturePolarity => _aperturePolarity;

        /// <summary>
        /// Opte la reflexio de l'apertura.
        /// </summary>
        /// 
        public bool ApertureMirror => _apertureMirror;

        /// <summary>
        /// Obte l'escala de l'apertura.
        /// </summary>
        /// 
        public double ApertureScale => _apertureScale;
    }
}
