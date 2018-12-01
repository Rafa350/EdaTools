namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;
    using System.Collections.Generic;

    public sealed partial class Net {

        private Dictionary<string, NetElement> elements;

        public void AddElement(NetElement element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (elements == null)
                elements = new Dictionary<string, NetElement>();
            elements.Add(element.Name, element);
        }

        public bool HasElements {
            get {
                return elements != null;
            }
        }

        public IEnumerable<string> ElementNames {
            get {
                return elements?.Keys;
            }
        }

        public IEnumerable<NetElement> Elements {
            get {
                return elements?.Values;
            }
        }
    }
}
