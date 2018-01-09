namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    using System;
    using System.Globalization;

    /// <summary>
    /// Estructura que representa un angle.
    /// </summary>
    public struct Angle {

        public enum FormatingUnits {
            Auto,
            Native,
            Degrees,
            Radiants
        }

        public static readonly Angle Zero = new Angle(0);

        private readonly int value;

        /// <summary>
        /// Constructor del objecte
        /// </summary>
        /// <param name="value">El valor unitats internes.</param>
        /// 
        private Angle(int value) {

            this.value = value % 36000;
        }

        /// <summary>
        /// Crea un objecte i el inicialitza a un valor en graus.
        /// </summary>
        /// <param name="value">El valor en graus.</param>
        /// <returns>L'objecte creat.</returns>
        /// 
        public static Angle FromDegrees(double value) {

            return new Angle((int)(value * 100.0));
        }

        /// <summary>
        /// Crea un objecte i el inicialitza amb un valor en radiants.
        /// </summary>
        /// <param name="value">El valor en radiants.</param>
        /// <returns>L'objecte creat.</returns>
        /// 
        public static Angle FromRadiants(double value) {

            return new Angle((int)(value * 100.0 * 180.0 / Math.PI));
        }

        /// <summary>
        /// Crea un objecte, analitzant un text.
        /// </summary>
        /// <param name="s">La text a analitzar.</param>
        /// <returns>L'objecte creat.</returns>
        /// 
        public static Angle Parse(string s, FormatingUnits units = FormatingUnits.Auto) {

            double v = Double.Parse(s);
            return new Angle((int)(v * 100.0));
        }

        /// <summary>
        /// Retorna el hash del objecte.
        /// </summary>
        /// <returns>El  hash.</returns>
        /// 
        public override int GetHashCode() {

            return value.GetHashCode();
        }

        /// <summary>
        /// Retorna la representacio textual del valor del objecte.
        /// </summary>
        /// <returns>La representacio.</returns>
        /// 
        public override string ToString() {

            return ToString(CultureInfo.CurrentCulture, FormatingUnits.Native);
        }

        /// <summary>
        /// Retorna la representacio textual del valor del objecte.
        /// </summary>
        /// <param name="provider">Proveidor de format.</param>
        /// <param name="units">Unitats.</param>
        /// <returns>La representacio.</returns>
        /// 
        public string ToString(IFormatProvider provider, FormatingUnits units) {

            switch (units) {
                case FormatingUnits.Native:
                    return String.Format(provider, "{0}", value);

                case FormatingUnits.Degrees:
                    return String.Format(provider, "{0}deg", Degrees);

                case FormatingUnits.Radiants:
                    return String.Format(provider, "{0}rad", Radiants);

                default:
                    throw new InvalidOperationException("Tipo de unidades no valida.");
            }
        }

        /// <summary>
        /// Comprova es igual a un altre objecte.
        /// </summary>
        /// <param name="obj">El objecte a comprobar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public override bool Equals(object obj) {

            if ((obj != null) && (obj is Angle))
                return value == ((Angle)obj).value;
            else
                return false;
        }

        #region Operadors
        /// <summary>
        /// Operador ==
        /// </summary>
        /// <param name="a1">Primer operand.</param>
        /// <param name="a2">Segon operand.</param>
        /// <returns>True si a1 == a2</returns>
        /// 
        public static bool operator ==(Angle a1, Angle a2) {

            return a1.value == a2.value;
        }

        public static bool operator !=(Angle a1, Angle a2) {

            return a1.value != a2.value;
        }

        public static bool operator >(Angle a1, Angle a2) {

            return a1.value > a2.value;
        }

        public static bool operator >=(Angle a1, Angle a2) {

            return a1.value >= a2.value;
        }

        public static bool operator <(Angle a1, Angle a2) {

            return a1.value < a2.value;
        }

        public static bool operator <=(Angle a1, Angle a2) {

            return a1.value <= a2.value;
        }

        public static Angle operator +(Angle a1, Angle a2) {

            return new Angle(a1.value + a2.value);
        }

        public static Angle operator -(Angle a1, Angle a2) {

            return new Angle(a1.value - a2.value);
        }

        public static Angle operator *(Angle a, int n) {

            return new Angle(a.value * n);
        }

        public static Angle operator *(Angle a, double n) {

            return new Angle((int)(a.value * n));
        }

        public static Angle operator /(Angle a, int n) {

            return new Angle(a.value / n);
        }

        public static Angle operator /(Angle a, double n) {

            return new Angle((int)(a.value / n));
        }
        #endregion

        /// <summary>
        /// Comprova si l'angle es zero.
        /// </summary>
        /// 
        public bool IsZero {
            get {
                return value == 0;
            }
        }

        public bool IsPositive {
            get {
                return value >= 0;
            }
        }

        public bool IsNegative {
            get {
                return value < 0;
            }
        }

        /// <summary>
        /// Comprova si l'angle es multiple de 90.
        /// </summary>
        /// 
        public bool IsOrthogonal {
            get {
                return 
                    value == 0 || 
                    value == 18000 || 
                    value == 27000;
            }
        }

        /// <summary>
        /// Comprova si l'angle es multiple de 45.
        /// </summary>
        /// 
        public bool IsDiagonal {
            get {
                return 
                    value == 4500 || 
                    value == 13500 || 
                    value == 22500 || 
                    value == 31500;
            }
        }

        /// <summary>
        /// Obte el valor de l'angle en graus.
        /// </summary>
        /// 
        public double Degrees {
            get {
                return value / 100.0;
            }
        }

        /// <summary>
        /// Obte el valor de l'angle en radiants
        /// </summary>
        /// 
        public double Radiants {
            get {
                return value / 100.0 * Math.PI / 180.0;
            }
        }
    }
}


