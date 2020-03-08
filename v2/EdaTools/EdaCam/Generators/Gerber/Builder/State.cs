namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder {

    using System;
    using MikroPic.EdaTools.v1.Base.Geometry;

    /// <summary>
    /// Clase que gestiona l'estat intern del generador gerber.
    /// </summary>
    internal sealed class State {

        private Aperture aperture = null;
        private Polarity aperturePolarity = Polarity.Dark;
        private Angle apertureAngle = Angle.Zero;
        private double apertureScale = 0;
        private bool apertureMirror = false;
        private InterpolationMode interpolationMode = InterpolationMode.Unknown;
        private int x = 0;
        private int y = 0;

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

            if (this.interpolationMode != interpolationMode) {
                this.interpolationMode = interpolationMode;
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

            if (this.x != x) {
                this.x = x;
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

            if (this.y != y) {
                this.y = y;
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

            if (this.aperture != aperture) {
                this.aperture = aperture;
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

            if (this.apertureAngle != apertureAngle) {
                this.apertureAngle = apertureAngle;
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

            if (this.aperturePolarity != aperturePolarity) {
                this.aperturePolarity = aperturePolarity;
                return true;
            }
            else
                return false;
        }

        public bool SetApertureMirror(bool apertureMirror) {

            if (this.apertureMirror != apertureMirror) {
                this.apertureMirror = apertureMirror;
                return true;
            }
            else
                return false;
        }

        public bool SetApertureScale(double apertureScale) {

            if (this.apertureScale != apertureScale) {
                this.apertureScale = apertureScale;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Obte el modus d'interpolacio.
        /// </summary>
        /// 
        public InterpolationMode InterpolationMode {
            get {
                return interpolationMode;
            }
        }

        /// <summary>
        /// Obte la coordinada X de la posicio actual.
        /// </summary>
        /// 
        public int X {
            get {
                return x;
            }
        }

        /// <summary>
        /// Obte la coordinada Y de la posicio actual.
        /// </summary>
        /// 
        public int Y {
            get {
                return y;
            }
        }

        /// <summary>
        /// Obte l'apertura seleccionada.
        /// </summary>
        /// 
        public Aperture Aperture {
            get {
                return aperture;
            }
        }

        /// <summary>
        /// Obte l'angle de l'apertura.
        /// </summary>
        /// 
        public Angle ApertureAngle {
            get {
                return apertureAngle;
            }
        }

        /// <summary>
        /// Opte la polaritat de l'apertura.
        /// </summary>
        /// 
        public Polarity AperturePolarity {
            get {
                return aperturePolarity;
            }
        }

        /// <summary>
        /// Opte la reflexio de l'apertura.
        /// </summary>
        /// 
        public bool ApertureMirror {
            get {
                return apertureMirror;
            }
        }

        /// <summary>
        /// Obte l'escala de l'apertura.
        /// </summary>
        /// 
        public double ApertureScale {
            get {
                return apertureScale;
            }
        }
    }
}
