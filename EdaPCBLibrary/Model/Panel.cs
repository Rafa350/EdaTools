namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Panel {

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
    }
}
