namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System;

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

        private readonly int value;

        private Ratio(int value) {

            if ((value < 0) || (value > 1000))
                throw new ArgumentOutOfRangeException(nameof(value));

            this.value = value;
        }

        public static Ratio FromValue(int value) {

            return new Ratio(value);
        }

        public static int operator *(int n, Ratio r) {

            return (int)((n * (long)r.value) / 1000L);
        }

        public static int operator *(Ratio r, int n) {

            return (int)((n * (long)r.value) / 1000L);
        }

        public override int GetHashCode() =>
            value * 11337793;

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>La representacio del valor.</returns>
        /// 
        public override string ToString() =>
            value.ToString();

        public static Ratio Parse(string s) {

            return new Ratio(Int32.Parse(s));
        }

        public bool Equals(Ratio other) =>
            value == other.value;

        public override bool Equals(object obj) {

            if (obj is Ratio other)
                return Equals(other);
            else
                return false;
        }

        public static bool operator ==(Ratio r1, Ratio r2) =>
            r1.Equals(r2);

        public static bool operator !=(Ratio r1, Ratio r2) =>
            !r1.Equals(r2);

        /// <summary>
        /// Indica si el valor es zero
        /// </summary>
        /// 
        public bool IsZero => 
            value == 0;

        public bool IsMax =>
                value == 1000.0;

        public int Value => value;
    }
}
