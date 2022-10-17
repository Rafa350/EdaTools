using System;
using System.Collections.Generic;
using System.Linq;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class EdaBoard {

        private List<EdaElement> _elements;

        /// <summary>
        /// Afeigeix un element.
        /// </summary>
        /// <param name="element">L'element a afeigir.</param>
        /// 
        public void AddElement(EdaElement element) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if ((_elements != null) && _elements.Contains(element))
                throw new InvalidOperationException(
                    String.Format("El elemento ya pertenece a la placa."));

            if (_elements == null)
                _elements = new List<EdaElement>();
            _elements.Add(element);
        }

        /// <summary>
        /// Afegeig diversos elements.
        /// </summary>
        /// <param name="elements">Els elements a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<EdaElement> elements) {

            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            foreach (var element in elements)
                AddElement(element);
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
        public IEnumerable<EdaElement> Elements =>
            _elements == null ? Enumerable.Empty<EdaElement>() : _elements;
    }
}
