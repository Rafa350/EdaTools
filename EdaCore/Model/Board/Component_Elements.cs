using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    public sealed partial class Component {

        private List<Element> _elements;
        private Dictionary<string, PadElement> _pads;

        /// <summary>
        /// Afeigeix un element.
        /// </summary>
        /// <param name="element">El element a afeigir.</param>
        /// 
        public void AddElement(Element element) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if ((_elements != null) && _elements.Contains(element))
                throw new InvalidOperationException("El elemento ya pertenece a un componente.");

            if (_elements == null)
                _elements = new List<Element>();
            _elements.Add(element);

            // Si l'element es un Pad, l'afegeix la la llista de pads.
            //
            if (element is PadElement pad) {
                if (_pads == null)
                    _pads = new Dictionary<string, PadElement>();
                _pads.Add(pad.Name, pad);
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
            if ((_elements == null) || !_elements.Contains(element))
                throw new InvalidOperationException("El elemento no pertenece a esta bloque.");

            // Elimina l'element de la llista d'elements
            //
            _elements.Remove(element);
            if (_elements.Count == 0)
                _elements = null;

            // Si l'element es un pad, tambe l'elimina de la llista de pads.
            //
            if (element is PadElement pad) {
                _pads.Remove(pad.Name);
                if (_pads.Count == 0)
                    _pads = null;
            }
        }

        /// <summary>
        /// Elimina tots els elements
        /// </summary>
        /// 
        public void RemoveAllElements() {

            if (_elements != null) {
                _elements.Clear();
                _elements = null;

                if (_pads != null) {
                    _pads.Clear();
                    _pads = null;
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

            if ((_pads != null) && _pads.TryGetValue(name, out PadElement pad))
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
        public bool HasElements =>
            _elements != null;

        /// <summary>
        /// Obte la llista d'elements.
        /// </summary>
        /// 
        public IEnumerable<Element> Elements =>
            _elements;

        /// <summary>
        /// Indica si conte pads.
        /// </summary>
        /// 
        public bool HasPads =>
            _pads != null;

        /// <summary>
        /// Enumera el nom dels pads.
        /// </summary>
        /// 
        public IEnumerable<string> PadNames =>
            _pads?.Keys;

        /// <summary>
        /// Enumera els pads.
        /// </summary>
        /// 
        public IEnumerable<PadElement> Pads =>
            _pads?.Values;
    }
}
