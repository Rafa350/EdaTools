namespace MikroPic.EdaTools.v1.Cam {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Cam.Model;
    using MikroPic.EdaTools.v1.Pcb.Model;

    public abstract class Generator {

        private readonly Target target;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="target">El target.</param>
        /// 
        public Generator(Target target) {

            if (target == null)
                throw new ArgumentNullException("target");

            this.target = target;
        }

        /// <summary>
        /// Genera el fitxer de contingut.
        /// </summary>
        /// <param name="board">La placa a procesar</param>
        /// <param name="outFolder">La carpeta de destinacio del fitxer de sortida</param>
        /// <param name="outPrefix">Preix del nom del fitxer de sortida.</param>
        /// 
        public void Generate(Board board, string outFolder, string outPrefix) {

            string fileName = GetOutputFileName(outFolder, outPrefix);
            Generate(board, fileName);
        }

        protected abstract void Generate(Board board, string fileName);

        private string GetOutputFileName(string outFolder, string outPrefix) {

            Dictionary<string, string> macroDict = new Dictionary<string, string>();
            macroDict.Add("{outPrefix}", outPrefix);
            macroDict.Add("{outFolder}", outFolder);
            return ProcessMacros(target.FileName, macroDict);
        }

        private string ProcessMacros(string str, IDictionary<string, string> macroDict) {

            while (true) { 
                int startIndex = str.IndexOf('{');
                if (startIndex >= 0) {
                    int endIndex = str.IndexOf('}', startIndex);
                    if (endIndex >= 0) {
                        string macroName = str.Substring(startIndex, endIndex - startIndex + 1);
                        if (macroDict.ContainsKey(macroName)) {
                            string macroValue = macroDict[macroName];
                            str = str.Replace(macroName, macroValue);
                        }
                        else
                            str = str.Remove(startIndex, endIndex - startIndex + 1);
                    }
                }
                else
                    break;
            }

            return str;
        }

        /// <summary>
        /// Obte el target.
        /// </summary>
        /// 
        public Target Target {
            get {
                return target;
            }
        }
    }
}
