using System;
using System.ComponentModel;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Representa un valor normalitzat a 1, en milesimes d'unitat.
    /// </summary>
    /// 
    [TypeConverter(typeof(EdaRatioConverter))]
    public readonly struct EdaRatio: IEquatable<EdaRatio> {

        public static readonly EdaRatio Zero = new EdaRatio(0);
        public static readonly EdaRatio P25 = new EdaRatio(250);
        public static readonly EdaRatio P50 = new EdaRatio(500);
        public static readonly EdaRatio P75 = new EdaRatio(750);
        public static readonly EdaRatio P100 = new EdaRatio(1000);

        private readonly int _value;

        private EdaRatio(int value) {

            if ((value < 0) || (value > 1000))
                throw new ArgumentOutOfRangeException(nameof(value));

            _value = value;
        }

        public static EdaRatio FromValue(int value) =>
            new EdaRatio(value);

        public static EdaRatio FromPercent(double value) =>
            new EdaRatio((int)(value * 1000.0));

        public static int operator *(int n, EdaRatio r) =>
            (int)((n * (long)r._value) / 1000L);

        public static int operator *(EdaRatio r, int n) =>
            (int)((n * (long)r._value) / 1000L);

        public override int GetHashCode() =>
            _value.GetHashCode();

        public bool Equals(EdaRatio other) =>
            _value == other._value;

        public override bool Equals(object obj) =>
            (obj is EdaRatio other) && Equals(other);

        public static bool operator ==(EdaRatio r1, EdaRatio r2) =>
            r1.Equals(r2);

        public static bool operator !=(EdaRatio r1, EdaRatio r2) =>
            !r1.Equals(r2);

        /// <summary>
        /// Indica si el valor es zero
        /// </summary>
        /// 
        public bool IsZero =>
            _value == 0;

        public bool IsMax =>
            _value == 1000.0;

        public double AsPercent =>
            _value / 1000.0;

        public int Value =>
            _value;
    }
}
