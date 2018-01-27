namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures {

    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Clae que representa una apertura de macro.
    /// </summary>
    public sealed class MacroAperture : Aperture {

        private readonly object[] args;
        private readonly Macro macro;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id">Identificador de l'apertura.</param>
        /// <param name="macro">Macro asignat a l'aperture.</param>
        /// <param name="args">Erguments del macro.</param>
        /// 
        public MacroAperture(int id, Macro macro, params object[] args) :
            base(id) {

            if (macro == null)
                throw new ArgumentNullException("macro");

            this.macro = macro;
            this.args = args;
        }

        protected override string GetCommand() {

            StringBuilder sb = new StringBuilder();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", Id);
            sb.AppendFormat("M{0},", macro.Id);

            if (args.Length > 0) {
                bool first = true;
                foreach (object arg in args) {
                    if (first)
                        first = false;
                    else
                        sb.Append('X');
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", arg);
                }
            }
            sb.Append("*%");

            return sb.ToString();
        }

        public Macro Macro {
            get {
                return macro;
            }
        }

        public object[] Args {
            get {
                return args;
            }
        }
    }
}