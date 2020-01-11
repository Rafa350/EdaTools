namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System;

    /// <summary>
    /// Clase per representar angles. Els angles son en centesimes 
    /// de grau i el gir es considera contrari al rellotge.
    /// </summary>
    /// 
    public readonly struct Angle : IEquatable<Angle> {

        public static readonly Angle Zero = new Angle(0);
        public static readonly Angle Deg45 = new Angle(4500);
        public static readonly Angle Deg90 = new Angle(9000);
        public static readonly Angle Deg135 = new Angle(13500);
        public static readonly Angle Deg180 = new Angle(18000);
        public static readonly Angle Deg270 = new Angle(27000);
        public static readonly Angle Deg315 = new Angle(31500);

        private readonly int value;

        /// <summary>
        /// Constructor privat.
        /// </summary>
        /// <param name="value">El valor de l'angle en centesimes de grau.</param>
        /// 
        private Angle(int value) {

            this.value = value % 36000;
        }

        /// <summary>
        /// Crea un angle a partir del seu valor en graus.
        /// </summary>
        /// <param name="value">Valor de l'angle en centesimes de graus.</param>
        /// <returns>L'angle creat.</returns>
        /// 
        public static Angle FromValue(int value) {

            return new Angle(value);
        }

        /// <summary>
        /// Crea un angle a partir del seu valor en graus
        /// </summary>
        /// <param name="deg">El valor de l'angle en graus.</param>
        /// <returns>L'angle creat.</returns>
        /// 
        public static Angle FromDegrees(double deg) {

            return new Angle((int)(deg * 100.0));
        }

        /// <summary>
        /// Crea un angle a partir del seu valor en radiants
        /// </summary>
        /// <param name="rad">El valor de l'angle en radiants.</param>
        /// <returns>L'angle creat.</returns>
        /// 
        public static Angle FromRadiants(double rad) {

            return new Angle((int)(rad * 18000.0 / Math.PI));
        }

        /// <summary>
        /// Test d'igualtat.
        /// </summary>
        /// <param name="other">L'altre objecte a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public bool Equals(Angle other) {

            return value == other.value;
        }

        /// <summary>
        /// Test d'igualtat.
        /// </summary>
        /// <param name="obj">L'altre objecte.</param>
        /// <returns>True si son iguals, false en cas contrari.</returns>
        /// 
        public override bool Equals(object obj) {

            if (obj is Angle other)
                return Equals(other);
            else
                return false;
        }

        /// <summary>
        /// Calcula el codi hash del objecte.
        /// </summary>
        /// <returns>El codi hash.</returns>
        /// 
        public override int GetHashCode() {

            return value;
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>La representacio del valor.</returns>
        /// 
        public override string ToString() {

            return value.ToString();
        }

        public static Angle Parse(string s) {

            return Angle.FromValue(Int32.Parse(s));
        }

        public static bool operator ==(Angle a1, Angle a2) {

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

        public static Angle operator *(Angle a, int v) {

            return new Angle(a.value * v);
        }

        public static Angle operator /(Angle a, int v) {

            return new Angle(a.value / v);
        }

        /// <summary>
        /// Comprova si l'angle es zero.
        /// </summary>
        /// 
        public bool IsZero {
            get {
                return value == 0;
            }
        }

        public bool Is90 {
            get {
                return value == 9000;
            }
        }

        public bool Is180 {
            get {
                return value == 18000;
            }
        }

        public bool Is270 {
            get {
                return value == 27000;
            }
        }

        /// <summary>
        /// Comprova si l'angle es ortogonal respecte els eixos
        /// </summary>
        /// 
        public bool IsOrthogonal {
            get {
                return Math.Abs(value % 9000) == 0;
            }
        }

        /// <summary>
        /// Comprova si l'angle es diagonal respecte els eixos
        /// </summary>
        /// 
        public bool IsDiagonal {
            get {
                return Math.Abs(value % 9000) == 4500;
            }
        }

        /// <summary>
        /// Comprova si l'angle es paral·lel al eix Y
        /// </summary>
        /// 
        public bool IsVertical {
            get {
                return Math.Abs(value % 18000) == 9000;
            }
        }

        /// <summary>
        /// Comprova si l'angle es paral·lel al eix X
        /// </summary>
        /// 
        public bool IsHorizontal {
            get {
                return Math.Abs(value % 18000) == 0;
            }
        }

        /// <summary>
        /// Obte el valor de l'angle en centesimes graus.
        /// </summary>
        /// 
        public int Value {
            get {
                return value;
            }
        }

        /// <summary>
        /// Obte el valor de l'angle en graus
        /// </summary>
        /// 
        public double ToDegrees {
            get {
                return value / 100.0;
            }
        }

        /// <summary>
        /// Obte el valor de l'angle en radiants
        /// </summary>
        /// 
        public double ToRadiants {
            get {
                return value * Math.PI / 18000.0;
            }
        }
    }
}
