namespace MikroPic.EdaTools.v1.Cam.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Project {

        private Dictionary<string, Target> targets;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// 
        public Project() {

        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="targets">Llista d'objectius.</param>
        /// 
        public Project(IEnumerable<Target> targets = null) {

            if (targets != null)
                foreach (var target in targets)
                    AddTarget(target);
        }

        /// <summary>
        /// Afegeix un objectiu.
        /// </summary>
        /// <param name="target">L'objectiu a afeigir</param>
        /// 
        public void AddTarget(Target target) {

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if ((targets != null) && targets.ContainsKey(target.Name))
                throw new InvalidOperationException(
                    String.Format("Ya existe un objetivo con el nombbre '{0}' en el proyecto.", target.Name));

            if (targets == null)
                targets = new Dictionary<string, Target>();
            targets.Add(target.Name, target);
        }

        /// <summary>
        /// Elimina un objectiu.
        /// </summary>
        /// <param name="target">L'objectiu a eliminar.</param>
        /// 
        public void RemoveTarget(Target target) {

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if ((targets == null) || !targets.ContainsKey(target.Name))
                throw new InvalidOperationException(
                    String.Format("No se encontro el target '{0}', a eliminar.", target.Name));

            targets.Remove(target.Name);
            if (targets.Count == 0)
                targets = null;
        }

        /// <summary>
        /// Obte un objectiu pel seu nom.
        /// </summary>
        /// <param name="name">Nom de l'objectiu.</param>
        /// <param name="throwOnError">True si cal generar una excepcio en cas d'error.</param>
        /// <returns>L'objectiu trobal, null si no el troba.</returns>
        /// 
        public Target GetTarget(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if ((targets != null) && targets.TryGetValue(name, out Target target))
                return target;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el target '{0}' en el proyecto", name));

            else
                return null;
        }

        /// <summary>
        /// Indica si conte objectius.
        /// </summary>
        /// 
        public bool HasTargets {
            get {
                return targets != null;
            }
        }

        /// <summary>
        /// Obte la col·leccio de noms dels objectius.
        /// </summary>
        /// 
        public IEnumerable<string> TargetNames {
            get {
                return targets?.Keys;
            }
        }

        /// <summary>
        /// Obte la col·leccio d'objectius.
        /// </summary>
        /// 
        public IEnumerable<Target> Targets {
            get {
                return targets?.Values;
            }
        }
    }
}
