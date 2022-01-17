using MikroPic.EdaTools.v1.Base.Geometry;
using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Cam.Model {

    public sealed class Target {

        private readonly string name;
        private string fileName;
        private string generatorName;
        private readonly IEnumerable<string> layerNames;
        private EdaPoint position;
        private EdaAngle rotation;
        private Dictionary<string, TargetOption> options;

        public Target(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            this.name = name;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom del desti.</param>
        /// <param name="fileName">Nom del fitxer.</param>
        /// <param name="generatorName">Nom del generador.</param>
        /// <param name="position">Posicio de la placa.</param>
        /// <param name="rotation">Rotacio de la placa.</param>
        /// <param name="layerNames">Llista de noms de capes a procesar.</param>
        /// <param name="options">Llista d'opcions</param>
        /// 
        public Target(string name, string fileName, string generatorName, EdaPoint position, EdaAngle rotation, IEnumerable<string> layerNames = null, IEnumerable<TargetOption> options = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            if (String.IsNullOrEmpty(generatorName))
                throw new ArgumentNullException(nameof(generatorName));

            this.name = name;
            this.fileName = fileName;
            this.generatorName = generatorName;
            this.position = position;
            this.rotation = rotation;
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
                throw new ArgumentNullException(nameof(name));

            AddOption(new TargetOption(name, value));
        }

        /// <summary>
        /// Afegeix una opcio.
        /// </summary>
        /// <param name="option">L'objecte opcio a afeigir.</param>
        /// 
        public void AddOption(TargetOption option) {

            if (option == null)
                throw new ArgumentNullException(nameof(option));

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
                throw new ArgumentNullException(nameof(name));

            if ((options != null) && options.ContainsKey(name))
                return options[name].Value;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No existe la opcion '{0}'.", name));

            else
                return null;
        }

        /// <summary>
        /// Obte el nom del target.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Obte o asigna el nom del fitxer de sortida.
        /// </summary>
        /// 
        public string FileName {
            get {
                return fileName;
            }
            set {
                fileName = value;
            }
        }

        /// <summary>
        /// Obte o asigna el nom del generador.
        /// </summary>
        /// 
        public string GeneratorName {
            get {
                return generatorName;
            }
            set {
                generatorName = value;
            }
        }

        public EdaPoint Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public EdaAngle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
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
                    throw new InvalidOperationException("No contiene opciones.");

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
                    throw new InvalidOperationException("No contiene opciones.");

                return options.Values;
            }
        }
    }
}
