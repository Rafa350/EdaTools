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

        private Angle(double angle) {

            value = angle;
        }

        /// <summary>
        /// Crea una copia normalitzada del angle.
        /// </summary>
        /// <returns>L'angle normalitzat.</returns>
        /// 
        public Angle Normalized() {

            return new Angle(Normalize(value));
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

        public override bool Equals(object obj) {

            if (obj is Angle)
                return value == ((Angle)obj).value;
            else
                return false;
        }

        public override int GetHashCode() {

            return value.GetHashCode();
        }

        public override string ToString() {

            return value.ToString();
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

        private static double Normalize(double v) {

            if (v >= 360.0 || v <= -360.0)
                v %= 360.0;

            if (v < 0.0)
                return 360.0 - v;
            else
                return v;
        }

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
                double v = Normalize(value);
                return (v == 0.0) || (v == 90.0) || (v == 180.0) || (value == 270.0);
            }
        }

        /// <summary>
        /// Comprova si l'angle es diagonal respecte els eixos
        /// </summary>
        /// 
        public bool IsDiagonal {
            get {
                double v = Normalize(value);
                return (v == 45.0) || (v == 135.0) || (v == 225.0) || (value == 315.0);
            }
        }

        /// <summary>
        /// Comprova si l'angle es paral·lel al eix Y
        /// </summary>
        /// 
        public bool IsVertical {
            get {
                double v = Normalize(value);
                return (v == 90.0) || (v == 270.0);
            }
        }

        /// <summary>
        /// Comprova si l'angle es paral·lel al eix X
        /// </summary>
        /// 
        public bool IsHorizontal {
            get {
                double v = Normalize(value);
                return (v == 0.0) || (v == 180.0);
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
        /// Obte el valor de l'angle en graus sexagesimals.
        /// </summary>
        /// 
        public double Degrees {
            get {
                return value;
            }
        }
    }
}
