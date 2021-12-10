using System;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Percent value.
    /// </summary>
    /// 
    public readonly struct Ratio: IEquatable<Ratio> {

        public static readonly Ratio Zero = new Ratio(0);
        public static readonly Ratio P25 = new Ratio(250);
        public static readonly Ratio P50 = new Ratio(500);
        public static readonly Ratio P75 = new Ratio(750);
        public static readonly Ratio P100 = new Ratio(1000);

        private readonly int _value;

        private Ratio(int value) {

            if ((value < 0) || (value > 1000))
                throw new ArgumentOutOfRangeException(nameof(value));

            _value = value;
        }

        public static Ratio FromValue(int value) =>
            new Ratio(value);

        public static int operator *(int n, Ratio r) => 
            (int)((n * (long)r._value) / 1000L);

        public static int operator *(Ratio r, int n) => 
            (int)((n * (long)r._value) / 1000L);

        public override int GetHashCode() => 
            _value * 11337793;

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>La representacio del valor.</returns>
        /// 
        public override string ToString() => _value.ToString();

        public static Ratio Parse(string s) =>
            new Ratio(Int32.Parse(s));

        public bool Equals(Ratio other) => 
            _value == other._value;

        public override bool Equals(object obj) =>
            (obj is Ratio other) && Equals(other);

        public static bool operator ==(Ratio r1, Ratio r2) => 
            r1.Equals(r2);

        public static bool operator !=(Ratio r1, Ratio r2) => 
            !r1.Equals(r2);

        /// <summary>
        /// Indica si el valor es zero
        /// </summary>
        /// 
        public bool IsZero => 
            _value == 0;

        public bool IsMax => 
            _value == 1000.0;

        public int Value => _value;
    }
}
