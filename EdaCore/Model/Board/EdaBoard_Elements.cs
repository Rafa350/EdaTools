﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class EdaBoard {

        private List<EdaElementBase> _elements;

        /// <summary>
        /// Afeigeix un element.
        /// </summary>
        /// <param name="element">L'element a afeigir.</param>
        /// 
        public void AddElement(EdaElementBase element) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if ((_elements != null) && _elements.Contains(element))
                throw new InvalidOperationException(
                    String.Format("El elemento ya pertenece a esta placa."));

            // Crea la llista si cal.
            //
            if (_elements == null)
                _elements = new List<EdaElementBase>();

            _elements.Add(element);
        }

        /// <summary>
        /// Afegeig diversos elements.
        /// </summary>
        /// <param name="elements">Els elements a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<EdaElementBase> elements) {

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
        public void RemoveElement(EdaElementBase element) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            // Comprova que l'element estigui en la llista
            //
            if ((_elements == null) || !_elements.Contains(element))
                throw new InvalidOperationException("El elemento no pertenece a esta placa.");

            // Elimina l'element de la llista d'elements
            //
            _elements.Remove(element);

            // Elimina la llista si es buida
            //
            if (_elements.Count == 0)
                _elements = null;
        }

        /// <summary>
        /// Indica si conte elements.
        /// </summary>
        /// 
        public bool HasElements =>
            _elements != null;

        /// <summary>
        /// Obte el nombre d'elements.
        /// </summary>
        /// 
        public int ElementCount =>
            _elements == null ? 0 : _elements.Count;

        /// <summary>
        /// Obte un enunerador pels elements.
        /// </summary>
        /// 
        public IEnumerable<EdaElementBase> Elements =>
            _elements == null ? Enumerable.Empty<EdaElementBase>() : _elements;
    }
}
