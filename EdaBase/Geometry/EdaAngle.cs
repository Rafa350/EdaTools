using System;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Clase per representar angles. Els angles son en centesimes 
    /// de grau i el gir positiu es contrari al rellotge.
    /// </summary>
    /// 
    public readonly struct EdaAngle: IEquatable<EdaAngle> {

        public static readonly EdaAngle Zero = new EdaAngle(0);
        public static readonly EdaAngle Deg45 = new EdaAngle(4500);
        public static readonly EdaAngle Deg90 = new EdaAngle(9000);
        public static readonly EdaAngle Deg135 = new EdaAngle(13500);
        public static readonly EdaAngle Deg180 = new EdaAngle(18000);
        public static readonly EdaAngle Deg270 = new EdaAngle(27000);
        public static readonly EdaAngle Deg315 = new EdaAngle(31500);

        private readonly int _value;

        /// <summary>
        /// Constructor privat.
        /// </summary>
        /// <param name="value">El valor de l'angle en centesimes de grau.</param>
        /// 
        private EdaAngle(int value) {

            _value = value % 36000;
        }

        /// <summary>
        /// Crea un angle a partir del seu valor en graus.
        /// </summary>
        /// <param name="value">Valor de l'angle en centesimes de graus.</param>
        /// <returns>L'angle creat.</returns>
        /// 
        public static EdaAngle FromValue(int value) =>
            new EdaAngle(value);

        /// <summary>
        /// Crea un angle a partir del seu valor en graus
        /// </summary>
        /// <param name="deg">El valor de l'angle en graus.</param>
        /// <returns>L'angle creat.</returns>
        /// 
        public static EdaAngle FromDegrees(double deg) =>
            new EdaAngle((int)(deg * 100.0));

        /// <summary>
        /// Crea un angle a partir del seu valor en radiants
        /// </summary>
        /// <param name="rad">El valor de l'angle en radiants.</param>
        /// <returns>L'angle creat.</returns>
        /// 
        public static EdaAngle FromRadiants(double rad) =>
            new EdaAngle((int)((rad * 18000.0) / Math.PI));

        /// <summary>
        /// Test d'igualtat.
        /// </summary>
        /// <param name="other">L'altre objecte a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public bool Equals(EdaAngle other) =>
            _value == other._value;

        /// <summary>
        /// Test d'igualtat.
        /// </summary>
        /// <param name="obj">L'altre objecte.</param>
        /// <returns>True si son iguals, false en cas contrari.</returns>
        /// 
        public override bool Equals(object obj) {

            if (obj is EdaAngle other)
                return Equals(other);
            else
                return false;
        }

        /// <inheritdoc/>
        /// 
        public override int GetHashCode() =>
            _value.GetHashCode();

        /// <inheritdoc/>
        /// 
        public override string ToString() =>
            AsDegrees.ToString();

        public static bool operator ==(EdaAngle a1, EdaAngle a2) =>
            a1._value == a2._value;

        public static bool operator !=(EdaAngle a1, EdaAngle a2) =>
            a1._value != a2._value;

        public static bool operator >(EdaAngle a1, EdaAngle a2) =>
            a1._value > a2._value;

        public static bool operator <(EdaAngle a1, EdaAngle a2) =>
            a1._value < a2._value;

        public static bool operator >=(EdaAngle a1, EdaAngle a2) =>
            a1._value >= a2._value;

        public static bool operator <=(EdaAngle a1, EdaAngle a2) =>
            a1._value <= a2._value;

        public static EdaAngle operator -(EdaAngle a) =>
            new EdaAngle(-a._value);

        public static EdaAngle operator +(EdaAngle a1, EdaAngle a2) =>
            new EdaAngle(a1._value + a2._value);

        public static EdaAngle operator -(EdaAngle a1, EdaAngle a2) =>
            new EdaAngle(a1._value - a2._value);

        public static EdaAngle operator *(EdaAngle a, int v) =>
            new EdaAngle(a._value * v);

        public static EdaAngle operator /(EdaAngle a, int v) =>
            new EdaAngle(a._value / v);

        /// <summary>
        /// Comprova si l'angle es zero.
        /// </summary>
        /// 
        public bool IsZero =>
            _value == 0;

        public bool Is90 =>
            _value == 9000;

        public bool Is180 =>
            _value == 18000;

        public bool Is270 =>
            _value == 27000;

        /// <summary>
        /// Comprova si l'angle es ortogonal respecte els eixos
        /// </summary>
        /// 
        public bool IsOrthogonal =>
            Math.Abs(_value % 9000) == 0;

        /// <summary>
        /// Comprova si l'angle es diagonal respecte els eixos
        /// </summary>
        /// 
        public bool IsDiagonal =>
            Math.Abs(_value % 9000) == 4500;

        /// <summary>
        /// Comprova si l'angle es paral·lel al eix Y
        /// </summary>
        /// 
        public bool IsVertical =>
            Math.Abs(_value % 18000) == 9000;

        /// <summary>
        /// Comprova si l'angle es paral·lel al eix X
        /// </summary>
        /// 
        public bool IsHorizontal =>
            Math.Abs(_value % 18000) == 0;

        /// <summary>
        /// Obte el valor de l'angle en centesimes graus.
        /// </summary>
        /// 
        public int Value =>
            _value;

        /// <summary>
        /// Obte el valor de l'angle en graus
        /// </summary>
        /// 
        public double AsDegrees =>
            _value / 100.0;

        /// <summary>
        /// Obte el valor de l'angle en radiants
        /// </summary>
        /// 
        public double AsRadiants =>
            (_value * Math.PI) / 18000.0;
    }
}
