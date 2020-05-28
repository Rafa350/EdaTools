namespace MikroPic.EdaTools.v1.Cam.Generators {

    using System;
    using System.Collections.Generic;

    public sealed class GeneratorOptions {

        private readonly Dictionary<string, string> options = new Dictionary<string, string>();

        /// <summary>
        /// Afegeix una opcio a la llista.
        /// </summary>
        /// <param name="name">Nom de l'opcio.</param>
        /// <param name="value">Valor de l'opcio.</param>
        /// 
        public void Add(string name, string value) {

            if (String.IsNullOrEmpty("name"))
                throw new ArgumentNullException(nameof(name));

            options.Add(name, value);
        }

        /// <summary>
        /// Comprova si existeix la opcio.
        /// </summary>
        /// <param name="name">El nom de l'opcio.</param>
        /// <returns>True si l'opcio existeix.</returns>
        /// 
        public bool HasOption(string name) {

            if (String.IsNullOrEmpty("name"))
                throw new ArgumentNullException(nameof(name));

            return options.ContainsKey(name);
        }

        public string GetOption(string name) {

            if (String.IsNullOrEmpty("name"))
                throw new ArgumentNullException(nameof(name));

            if (options.TryGetValue(name, out string value))
                return value;
            else
                return null;
        }
    }
}
