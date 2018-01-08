namespace MikroPic.EdaTools.v1.Pcb.Geometry {

    using System;

    public struct Measure {

        public enum Units {
            Auto,
            Native,
            Millimeters,
            Inches,
        }

        private readonly long value;

        /// <summary>
        /// Contructor del objecte
        /// </summary>
        /// <param name="value">El valor</param>
        /// 
        private Measure(long value) {

            this.value = value;
        }

        /// <summary>
        /// Crea un objecte i inicialitza el valor en milimetres.
        /// </summary>
        /// <param name="value">El valor en milimetres.</param>
        /// <returns>L'objecte creat.</returns>
        /// 
        public static Measure FromMillimeter(double value) {

            return new Measure((long)(value * 1000000000.0));
        }

        /// <summary>
        /// Crea un objecte i inicialitza el valor en polsades.
        /// </summary>
        /// <param name="value">El valor en polsades.</param>
        /// <returns>L'objecte creat.</returns>
        /// 
        public static Measure FroInches(double value) {

            return new Measure((long)(value * 1000000000.0 * 25.4));
        }

        /// <summary>
        /// Retorna el hask del objecte.
        /// </summary>
        /// <returns>El hash.</returns>
        /// 
        public override int GetHashCode() {

            return value.GetHashCode();
        }

        /// <summary>
        /// Comprova si es igual a un altre objecte.
        /// </summary>
        /// <param name="obj">L'alte objecte.</param>
        /// <returns>True si soin iguals.</returns>
        /// 
        public override bool Equals(object obj) {

            if ((obj != null) && (obj is Measure))
                return value == ((Measure)obj).value;
            else
                return false;
        }

        public override string ToString() {

            return String.Format("{0}", value / 1000000000.0);
        }

        /// <summary>
        /// Operador ==
        /// </summary>
        /// <param name="m1">Primer operand.</param>
        /// <param name="m2">Segon operand.</param>
        /// <returns>True si m1 == m2</returns>
        /// 
        public static bool operator ==(Measure m1, Measure m2) {

            return m1.value == m2.value;
        }

        public static bool operator !=(Measure m1, Measure m2) {

            return m1.value != m2.value;
        }

        /// <summary>
        /// Obte el valor en milimetres.
        /// </summary>
        /// 
        public double Millimeters {
            get {
                return value / 1000000000;
            }
        }

        /// <summary>
        /// Obte el valor en polsades.
        /// </summary>
        /// 
        public double Inches {
            get {
                return Millimeters / 25.4;
            }
        }
    }
}
