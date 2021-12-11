﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    public sealed partial class EdaComponent {

        private List<EdaElement> _elements;

        /// <summary>
        /// Afeigeix un element.
        /// </summary>
        /// <param name="element">El element a afeigir.</param>
        /// 
        public void AddElement(EdaElement element) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if ((_elements != null) && _elements.Contains(element))
                throw new InvalidOperationException("El elemento ya pertenece a un componente.");

            if (_elements == null)
                _elements = new List<EdaElement>();
            _elements.Add(element);
        }

        /// <summary>
        /// Afeigeix una coleccio d'elements.
        /// </summary>
        /// <param name="element">El elements a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<EdaElement> elements) {

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
        public void RemoveElement(EdaElement element) {

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
        }

        /// <summary>
        /// Elimina tots els elements
        /// </summary>
        /// 
        public void RemoveAllElements() {

            if (_elements != null) {
                _elements.Clear();
                _elements = null;
            }
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
        public IEnumerable<EdaElement> Elements =>
            _elements == null ? Enumerable.Empty<EdaElement>() : _elements;
    }
}
