namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    using System;

    /// <summary>
    /// Clase per representar angles
    /// </summary>
    public struct Angle {

        public static readonly Angle Zero = FromDegrees(0.0);
        public static readonly Angle Deg45 = FromDegrees(45.0);
        public static readonly Angle Deg90 = FromDegrees(90.0);
        public static readonly Angle Deg135 = FromDegrees(135.0);
        public static readonly Angle Deg180 = FromDegrees(180.0);
        public static readonly Angle Deg270 = FromDegrees(270.0);
        public static readonly Angle Deg315 = FromDegrees(315.0);

        private readonly double value;

        /// <summary>
        /// Constructor privat.
        /// </summary>
        /// <param name="angle">El valor de l'angle.</param>
        /// 
        private Angle(double angle) {

            value = angle % 360.0;
        }

        /// <summary>
        /// Crea un angle a partir del seu valor en radians
        /// </summary>
        /// <param name="value">El valor de l'angle en radians.</param>
        /// <returns>L'angle creat.</returns>
        /// 
        public static Angle FromRadiants(double value) {

            return new Angle(value * 180.0 / Math.PI);
        }

        /// <summary>
        /// Crea un angle a partir del seu valor en graus.
        /// </summary>
        /// <param name="value">El valor de l'angle en graus.</param>
        /// <returns>L'angle creat.</returns>
        /// 
        public static Angle FromDegrees(double value) {

            return new Angle(value);
        }

        /// <summary>
        /// Test d'igualtat.
        /// </summary>
        /// <param name="obj">L'altre objecte.</param>
        /// <returns>True si son iguals, false en cas contrari.</returns>
        /// 
        public override bool Equals(object obj) {

            if (obj is Angle)
                return value == ((Angle)obj).value;
            else
                return false;
        }

        /// <summary>
        /// Calcula el codi hash del objecte.
        /// </summary>
        /// <returns>El codi hash.</returns>
        /// 
        public override int GetHashCode() {

            return value.GetHashCode();
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>La representacio del valor.</returns>
        /// 
        public override string ToString() {

            return Convert.ToString(value);
        }

        public static bool operator == (Angle a1, Angle a2) {

            return a1.value == a2.value;
        }

        public static bool operator !=(Angle a1, Angle a2) {

            return a1.value != a2.value;
        }

        public static bool operator >(Angle a1, Angle a2) {

            return a1.value > a2.value;
        }

        public static bool operator <(Angle a1, Angle a2) {

            return a1.value < a2.value;
        }

        public static bool operator >=(Angle a1, Angle a2) {

            return a1.value >= a2.value;
        }

        public static bool operator <=(Angle a1, Angle a2) {

            return a1.value <= a2.value;
        }

        public static Angle operator -(Angle a) {

            return new Angle(-a.value);
        }

        public static Angle operator +(Angle a1, Angle a2) {

            return new Angle(a1.value + a2.value);
        }

        public static Angle operator -(Angle a1, Angle a2) {

            return new Angle(a1.value - a2.value);
        }

        public static Angle operator *(Angle a, double v) {

            return new Angle(a.value * v);
        }

        public static Angle operator /(Angle a, double v) {

            return new Angle(a.value / v);
        }

        /// <summary>
        /// Comprova si l'angle es zero.
        /// </summary>
        /// 
        public bool IsZero {
            get {
                return value == 0.0;
            }
        }

        /// <summary>
        /// Comprova si l'angle es ortogonal respecte els eixos
        /// </summary>
        /// 
        public bool IsOrthogonal {
            get {
                return Math.Abs(value % 90) == 0.0;
            }
        }

        /// <summary>
        /// Comprova si l'angle es diagonal respecte els eixos
        /// </summary>
        /// 
        public bool IsDiagonal {
            get {
                return Math.Abs(value % 90) == 45.0;
            }
        }

        /// <summary>
        /// Comprova si l'angle es paral·lel al eix Y
        /// </summary>
        /// 
        public bool IsVertical {
            get {
                return Math.Abs(value % 180) == 90.0;
            }
        }

        /// <summary>
        /// Comprova si l'angle es paral·lel al eix X
        /// </summary>
        /// 
        public bool IsHorizontal {
            get {
                return Math.Abs(value % 180) == 0.0;
            }
        }

        /// <summary>
        /// Obte el valor de l'angle en radians.
        /// </summary>
        /// 
        public double Radiants {
            get {
                return value * Math.PI / 180.0;
            }
        }

        /// <summary>
        /// Obte el valor de l'angle en graus.
        /// </summary>
        /// 
        public double Degrees {
            get {
                return value;
            }
        }
    }
}
