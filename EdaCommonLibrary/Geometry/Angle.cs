namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;
    using System.Xml;
    using MikroPic.EdaTools.v1.Xml;

    /// <summary>
    /// Clase per representar angles. Els angles son en centesimes 
    /// de grau i el gir es considera contrari al rellotge.
    /// </summary>
    /// 
    public struct Angle {

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
        /// <param name="angle">El valor de l'angle en centesimes de grau.</param>
        /// 
        private Angle(int angle) {

            value = angle % 36000;
        }

        /// <summary>
        /// Crea un angle a partir del seu valor en graus.
        /// </summary>
        /// <param name="deg">Valor de l'angle en centesimes de graus.</param>
        /// <returns>L'angle creat.</returns>
        /// 
        public static Angle FromDegrees(int deg) {

            return new Angle(deg);
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

            return value;
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>La representacio del valor.</returns>
        /// 
        public override string ToString() {

            return String.Format(CultureInfo.CurrentCulture, "{0}", value);
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
        public int Degrees {
            get {
                return value;
            }
        }

        /// <summary>
        /// Obte el valor de l'angle en radiants
        /// </summary>
        /// 
        public double Radiants {
            get {
                return value * Math.PI / 18000.0;
            }
        }
    }


    /// <summary>
    /// Clase que implementa metodes d'extensio.
    /// </summary>
    /// 
    public static class AngleHelper {

        /// <summary>
        /// Escriu un atribut de tipus 'Angle'.
        /// </summary>
        /// <param name="wr">L'objecte 'XmlWriterAdapter'</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="angle">El valor a escriure.</param>
        /// 
        public static void WriteAttribute(this XmlWriterAdapter wr, string name, Angle angle) {

            string s = XmlConvert.ToString(angle.Degrees / 100.0);
            wr.WriteAttribute(name, s);
        }

        public static Angle AttributeAsAngle(this XmlReaderAdapter rd, string name) {

            double v = rd.AttributeAsDouble(name);
            return Angle.FromDegrees((int)(v * 100.0));
        }
    }
}
