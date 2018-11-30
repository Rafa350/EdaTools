namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Collections;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    public sealed partial class Component {

        private ParentChildCollection<Component, Element> elements;
        private KeyCollection<PadElement, String> pads;

        /// <summary>
        /// Afeigeix un element.
        /// </summary>
        /// <param name="element">El element a afeigir.</param>
        /// 
        public void AddElement(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            // Comprova que l'element no estigui afeigit amb anterioritat
            //
            if (element.Block != null)
                throw new InvalidOperationException("El elemento ya pertenece a un bloque.");

            if (element.Board != null)
                throw new InvalidOperationException("El elemento ya pertenece a una placa.");

            // Afegeix l'element a la llista d'elements
            //
            if (elements == null)
                elements = new ParentChildCollection<Component, Element>(this);
            elements.Add(element);

            // Si l'element es un Pad, l'afegeix la la llista de pads.
            //
            PadElement pad = element as PadElement;
            if (pad != null) {
                if (pads == null)
                    pads = new KeyCollection<PadElement, string>();
                pads.Add(pad);
            }
        }

        /// <summary>
        /// Afeigeix una coleccio d'elements.
        /// </summary>
        /// <param name="element">El elements a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<Element> elements) {

            if (elements == null)
                throw new ArgumentNullException("elements");

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
                throw new ArgumentNullException("element");

            // Comprova que l'element estigui en la llista
            //
            if (element.Block != this)
                throw new InvalidOperationException("El elemento no pertenece a esta bloque.");

            // Elimina l'element de la llista d'elements
            //
            elements.Remove(element);
            if (elements.IsEmpty)
                elements = null;

            // Si l'element es un pad, tambe l'elimina de la llista de pads.
            //
            PadElement pad = element as PadElement;
            if (pads != null) {
                pads.Remove(pad);
                if (pads.IsEmpty)
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
        /// <param name="throwOnError">True si dispara una execpcio si no el troba.</param>
        /// <returns>El pad. Null si no el troba.</returns>
        /// 
        public PadElement GetPad(string name, bool throwOnError = true) {

            PadElement pad = pads?.Get(name);

            if ((pad == null) && throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el pad '{0}' en el part '{1}'.", name, this.name));
            else
                return pad;
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
                if (elements == null)
                    throw new InvalidOperationException("El bloque no contiene elementos.");

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
                if (pads == null)
                    throw new InvalidOperationException("El bloque no contiene pads.");

                return pads.Keys;
            }
        }

        /// <summary>
        /// Enumera els pads.
        /// </summary>
        /// 
        public IEnumerable<PadElement> Pads {
            get {
                if (pads == null)
                    throw new InvalidOperationException("El bloque no contiene pads.");

                return pads;
            }
        }
    }
}
