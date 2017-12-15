namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class Component: IVisitable {

        private List<ElementBase> elements;
        private static Dictionary<ElementBase, Component> revElements;
        private string name;

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public void Add(ElementBase element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if ((elements != null) && elements.Contains(element))
                throw new InvalidOperationException(
                    String.Format("El elemento ya pertenece al componente '{0}'.", name));

            if (elements == null)
                elements = new List<ElementBase>();
            elements.Add(element);

            if (revElements == null)
                revElements = new Dictionary<ElementBase, Component>();
            revElements.Add(element, this);
        }

        public void Remove(ElementBase element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (elements == null || !elements.Contains(element))
                throw new InvalidOperationException(
                    String.Format("El elemento no pertenece al componente '{0}'.", name));

            elements.Remove(element);
            if (elements.Count == 0)
                elements = null;

            revElements.Remove(element);
            if (revElements.Count == 0)
                revElements = null;
        }

        public static Component ComponentOf(ElementBase element) {

            if ((revElements != null) && revElements.ContainsKey(element))
                return revElements[element];
            else
                return null;
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
