namespace MikroPic.EdaTools.v1.Cam.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Target {

        private readonly string fileName;
        private readonly string generatorName;
        private readonly IEnumerable<string> layerNames;
        private Dictionary<string, TargetOption> options;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer.</param>
        /// <param name="generatorName">Nom del generador.</param>
        /// <param name="layerNames">Llista de noms de capes a procesar.</param>
        /// 
        public Target(string fileName, string generatorName, IEnumerable<string> layerNames = null, IEnumerable<TargetOption> options = null) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (String.IsNullOrEmpty(generatorName))
                throw new ArgumentNullException("generatorName");

            this.fileName = fileName;
            this.generatorName = generatorName;
            this.layerNames = layerNames;

            if (options != null)
                foreach (var option in options)
                    AddOption(option);
        }

        /// <summary>
        /// Afegeix una opcio.
        /// </summary>
        /// <param name="name">Nom de l'opcio.</param>
        /// <param name="value">Valor de l'opcio.</param>
        /// 
        public void AddOption(string name, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            AddOption(new TargetOption(name, value));
        }

        /// <summary>
        /// Afegeix una opcio.
        /// </summary>
        /// <param name="option">L'objecte opcio a afeigir.</param>
        /// 
        public void AddOption(TargetOption option) {

            if (option == null)
                throw new ArgumentNullException("option");

            if (options == null)
                options = new Dictionary<string, TargetOption>();

            options.Add(option.Name, option);
        }

        /// <summary>
        /// Obte el valor d'una opcio.
        /// </summary>
        /// <param name="name">Nom de l'opcio.</param>
        /// <param name="throwOnError">True si cal generer una exepcio en cas d'error.</param>
        /// <returns>El valor de l'opcio, o null si no la troba.</returns>
        /// 
        public string GetOptionValue(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if ((options != null) && options.ContainsKey(name))
                return options[name].Value;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No existe la opcion '{0}'.", name));

            else
                return null;
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
        /// 
        public string GeneratorName {
            get {
                return generatorName;
            }
        }

        /// <summary>
        /// Indica si conte capes
        /// </summary>
        /// 
        public bool HasLayers {
            get {
                return layerNames != null;
            }
        }

        /// <summary>
        /// Obte els noms de les capes a procesar.
        /// </summary>
        /// 
        public IEnumerable<string> LayerNames {
            get {
                return layerNames;
            }
        }

        /// <summary>
        /// Indica si te opcions.
        /// </summary>
        /// 
        public bool HasOptions {
            get {
                return options != null;
            }
        }

        /// <summary>
        /// Enumera els noms de totes les opcions.
        /// </summary>
        /// 
        public IEnumerable<string> OptionNames {
            get {
                if (options == null)
                    throw new InvalidOperationException("No contiene optiones.");

                return options.Keys;
            }
        }

        /// <summary>
        /// Enumera totes les opcions.
        /// </summary>
        /// 
        public IEnumerable<TargetOption> Options {
            get {
                if (options == null)
                    throw new InvalidOperationException("No contiene optiones.");

                return options.Values;
            }
        }
    }
}
