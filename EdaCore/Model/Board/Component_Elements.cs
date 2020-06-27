namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    public sealed partial class Component {

        private List<Element> elements;
        private Dictionary<string, PadElement> pads;

        /// <summary>
        /// Afeigeix un element.
        /// </summary>
        /// <param name="element">El element a afeigir.</param>
        /// 
        public void AddElement(Element element) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if ((elements != null) && elements.Contains(element))
                throw new InvalidOperationException("El elemento ya pertenece a un componente.");

            if (elements == null)
                elements = new List<Element>();
            elements.Add(element);

            // Si l'element es un Pad, l'afegeix la la llista de pads.
            //
            if (element is PadElement pad) {
                if (pads == null)
                    pads = new Dictionary<string, PadElement>();
                pads.Add(pad.Name, pad);
            }
        }

        /// <summary>
        /// Afeigeix una coleccio d'elements.
        /// </summary>
        /// <param name="element">El elements a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<Element> elements) {

            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            foreach (var element in elements)
                AddElement(element);
        }

        /// <summary>
        /// Elimina un element del component.
        /// </summary>
        /// <param name="element">El element a eliminar.</param>
        /// 
        public void RemoveElement(Element element) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            // Comprova que l'element estigui en la llista
            //
            if ((elements == null) || !elements.Contains(element))
                throw new InvalidOperationException("El elemento no pertenece a esta bloque.");

            // Elimina l'element de la llista d'elements
            //
            elements.Remove(element);
            if (elements.Count == 0)
                elements = null;

            // Si l'element es un pad, tambe l'elimina de la llista de pads.
            //
            if (element is PadElement pad) {
                pads.Remove(pad.Name);
                if (pads.Count == 0)
                    pads = null;
            }
        }

        /// <summary>
        /// Elimina tots els elements
        /// </summary>
        /// 
        public void RemoveAllElements() {

            if (elements != null) {
                elements.Clear();
                elements = null;

                if (pads != null) {
                    pads.Clear();
                    pads = null;
                }
            }
        }

        /// <summary>
        /// Obte un pad pel seu nom.
        /// </summary>
        /// <param name="name">El nom del pad.</param>
        /// <param name="throwOnError">True si dispara una excepcio si no el troba.</param>
        /// <returns>El pad. Null si no el troba.</returns>
        /// 
        public PadElement GetPad(string name, bool throwOnError = true) {

            if ((pads != null) && pads.TryGetValue(name, out PadElement pad))
                return pad;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el pad '{0}' en el componente '{1}'.", name, this._name));

            else
                return null;
        }

        /// <summary>
        /// Indica si conte elements.
        /// </summary>
        /// 
        public bool HasElements {
            get {
                return elements != null;
            }
        }

        /// <summary>
        /// Obte la llista d'elements.
        /// </summary>
        /// 
        public IEnumerable<Element> Elements {
            get {
                return elements;
            }
        }

        /// <summary>
        /// Indica si conte pads.
        /// </summary>
        /// 
        public bool HasPads {
            get {
                return pads != null;
            }
        }

        /// <summary>
        /// Enumera el nom dels pads.
        /// </summary>
        /// 
        public IEnumerable<string> PadNames {
            get {
                return pads?.Keys;
            }
        }

        /// <summary>
        /// Enumera els pads.
        /// </summary>
        /// 
        public IEnumerable<PadElement> Pads {
            get {
                return pads?.Values;
            }
        }
    }
}
