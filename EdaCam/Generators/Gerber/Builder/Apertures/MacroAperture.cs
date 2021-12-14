using System;
using System.Globalization;
using System.Text;

namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder.Apertures {

    /// <summary>
    /// Clae que representa una apertura de macro.
    /// </summary>
    public sealed class MacroAperture : Aperture {

        private readonly int[] _args;
        private readonly Macro _macro;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id">Identificador de l'apertura.</param>
        /// <param name="tag">Dades opcionals.</param>
        /// <param name="macro">Macro asignat a l'aperture.</param>
        /// <param name="args">Arguments del macro.</param>
        /// 
        public MacroAperture(int id, object tag, Macro macro, params int[] args) :
            base(id, tag) {

            if (macro == null)
                throw new ArgumentNullException(nameof(macro));

            _macro = macro;
            _args = args;
        }

        /// <summary>
        /// Obte la comanda per definit l'apertura.
        /// </summary>
        /// <returns>La comanda.</returns>
        /// 
        protected override string GetCommand() {

            var sb = new StringBuilder();
            sb.Append("%ADD");
            sb.AppendFormat("{0}", Id);
            sb.AppendFormat("M{0},", _macro.Id);

            if (_args.Length > 0) {
                bool first = true;
                foreach (int arg in _args) {
                    if (first)
                        first = false;
                    else
                        sb.Append('X');
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", arg / 1000000.0);
                }
            }
            sb.Append("*%");

            return sb.ToString();
        }

        /// <summary>
        /// Obte el macro associat a l'apertura.
        /// </summary>
        /// 
        public Macro Macro =>
            _macro;

        /// <summary>
        /// Obte els arguments del macro.
        /// </summary>
        /// 
        public int[] Args =>
            _args;
    }
}