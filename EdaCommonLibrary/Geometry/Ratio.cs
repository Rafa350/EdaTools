﻿namespace MikroPic.EdaTools.v1.Geometry {

    using System;
    using System.Globalization;
    using System.Xml;
    using MikroPic.EdaTools.v1.Xml;

    public struct Ratio {

        public static readonly Ratio Zero = new Ratio(0);
        public static readonly Ratio P25 = new Ratio(250);
        public static readonly Ratio P50 = new Ratio(500);
        public static readonly Ratio P75 = new Ratio(750);
        public static readonly Ratio P100 = new Ratio(1000);

        private readonly int value;

        private Ratio(int value) {

            if ((value < 0) || (value > 1000))
                throw new ArgumentOutOfRangeException("value");

            this.value = value;
        }

        public static Ratio FromPercent(int value) {

            return new Ratio(value);
        }

        public static int operator *(int n, Ratio r) {

            return (int)((n * (long)r.value) / 1000L);
        }

        public static int operator *(Ratio r, int n) {

            return (int)((n * (long)r.value) / 1000L);
        }

        public override int GetHashCode() {

            return value;
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>La representacio del valor.</returns>
        /// 
        public override string ToString() {

            return ToString(CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider provider) {

            return String.Format(provider, "{0}", value);
        }

        /// <summary>
        /// Indica si el valor es zero
        /// </summary>
        /// 
        public bool IsZero {
            get {
                return value == 0;
            }
        }

        public bool IsMax {
            get {
                return value == 1000.0;
            }
        }

        public int Percent {
            get {
                return value;
            }
        }
    }


    public static class RatioHelper {

        public static void WriteAttribute(this XmlWriterAdapter wr, string name, Ratio ratio) {

            string s = XmlConvert.ToString(ratio.Percent / 1000.0);
            wr.WriteAttribute(name, s);
        }
    }
}
