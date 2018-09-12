namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Model.Collections;
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board {

        // Elements
        private ParentChildCollection<Board, Element> elements;

        /// <summary>
        /// Afeigeix un element.
        /// </summary>
        /// <param name="element">L'element a afeigir.</param>
        /// 
        public void AddElement(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (element.Board != null)
                throw new InvalidOperationException("El elemento ya pertenece a una placa.");

            if (element.Block != null)
                throw new InvalidOperationException("El elemento ya pertenece a un bloque.");

            if (elements == null)
                elements = new ParentChildCollection<Board, Element>(this);
            elements.Add(element);
        }

        /// <summary>
        /// Afegeig una col·leccio d'elements.
        /// </summary>
        /// <param name="elements">Els elements a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<Element> elements) {

            if (elements == null)
                throw new ArgumentNullException("elements");

            foreach (var element in elements)
                AddElement(element);
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
        /// Obte un enunerador pels elements.
        /// </summary>
        /// 
        public IEnumerable<Element> Elements {
            get {
                return elements;
            }
        }
    }
}
