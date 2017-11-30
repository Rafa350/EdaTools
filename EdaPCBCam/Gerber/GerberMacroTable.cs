namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System;
    using System.Collections.Generic;

    public sealed class GerberMacroTable {

        private readonly List<GerberMacro> macros = new List<GerberMacro>();

        public GerberMacroTable Add(GerberMacro macro) {

            if (macro == null)
                throw new ArgumentNullException("macro");

            if (macros.Contains(macro))
                throw new InvalidOperationException("El macro ya pertenece a la tabla.");

            macros.Add(macro);

            return this;
        }

        public IEnumerable<GerberMacro> Macros {
            get {
                return macros;
            }
        }
    }
}
