namespace MikroPic.EdaTools.v1.Model {
    
    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Model.Elements;

    public sealed class Signal: IVisitable {

        private List<ElementBase> elements;
        private string name;

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public void Add(ElementBase element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if ((elements != null) && elements.Contains(element))
                throw new InvalidOperationException(
                    String.Format("El elemento ya pertenece a la señal '{0}'.", name));

            if (elements == null)
                elements = new List<ElementBase>();
            elements.Add(element);
        }

        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        public IEnumerable<ElementBase> Elements {
            get {
                return elements;
            }
        }
    }
}
