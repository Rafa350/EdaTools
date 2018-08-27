namespace MikroPic.EdaTools.v1.Cam.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Panel {

        private HashSet<PanelElement> elements;

        public void AddElement(PanelElement element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (elements == null)
                elements = new HashSet<PanelElement>();

            elements.Add(element);
        }

        public bool HasElements {
            get {
                return elements != null;
            }
        }

        public IEnumerable<PanelElement> Elements {
            get {
                if (elements == null)
                    throw new InvalidOperationException("El panel no posee elementos.");

                return elements;
            }
        }
    }
}
