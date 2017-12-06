namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures {

    using System;
    using System.Globalization;
    using System.Text;

    public sealed class MacroAperture: Aperture {

        private readonly object[] args;
        private readonly Macro macro;

        public MacroAperture(Macro macro, params object[] args) {

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

        public Macro Macro { get { return macro; } }
        public object[] Args { get { return args; } }
    }
}