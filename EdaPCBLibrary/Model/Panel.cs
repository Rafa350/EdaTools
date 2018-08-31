namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Geometry;

    public sealed class Panel: ISize {

        private Size size;
        private HashSet<PanelElement> elements;

        /// <summary>
        /// Afegeix un element
        /// </summary>
        /// <param name="element">L'element a afeigir.</param>
        /// 
        public void AddElement(PanelElement element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (elements == null)
                elements = new HashSet<PanelElement>();

            elements.Add(element);
        }

        /// <summary>
        /// Afegeix una coleccio d'elements.
        /// </summary>
        /// <param name="elements">Ele elements a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<PanelElement> elements) {

            if (elements == null)
                throw new ArgumentNullException("elements");

            foreach (var element in elements)
                AddElement(element);
        }

        /// <summary>
        /// Indica si te elements
        /// </summary>
        /// 
        public bool HasElements {
            get {
                return elements != null;
            }
        }

        /// <summary>
        /// Enumera els elements
        /// </summary>
        /// 
        public IEnumerable<PanelElement> Elements {
            get {
                if (elements == null)
                    throw new InvalidOperationException("El panel no posee elementos.");

                return elements;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany del panell
        /// </summary>
        /// 
        public Size Size {
            get {
                return size;
            }
            set {
                size = value;
            }
        }
    }
}
