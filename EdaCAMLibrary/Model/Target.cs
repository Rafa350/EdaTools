namespace MikroPic.EdaTools.v1.Cam.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Target {

        private readonly string fileName;
        private readonly string generatorName;
        private readonly IEnumerable<string> layerNames;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer.</param>
        /// <param name="generatorName">Nom del generador.</param>
        /// <param name="layerNames">Llista de noms de capes a procesar.</param>
        /// 
        public Target(string fileName, string generatorName, IEnumerable<string> layerNames = null) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (String.IsNullOrEmpty(generatorName))
                throw new ArgumentNullException("generatorName");

            this.fileName = fileName;
            this.generatorName = generatorName;
            this.layerNames = layerNames;
        }

        /// <summary>
        /// Obte el nom del fitxer de sortida.
        /// </summary>
        /// 
        public string FileName {
            get {
                return fileName;
            }
        }

        /// <summary>
        /// Obte el nom del generador.
        /// </summary>
        public string GeneratorName {
            get {
                return generatorName;
            }
        }

        /// <summary>
        /// Obte lels noms de les capes a procesar.
        /// </summary>
        /// 
        public IEnumerable<string> LayerNames {
            get {
                return layerNames;
            }
        }
    }
}
