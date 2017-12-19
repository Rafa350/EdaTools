namespace MikroPic.EdaTools.v1.Pcb.Model {
    
    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class Signal: IVisitable {

        private List<Element> elements;
        private List<Terminal> terminals;
        private string name;

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public void Add(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if ((elements != null) && elements.Contains(element))
                throw new InvalidOperationException(
                    String.Format("El elemento ya pertenece a la señal '{0}'.", name));

            if (elements == null)
                elements = new List<Element>();
            elements.Add(element);
        }

        public void Add(Terminal pad) {

            if (pad == null)
                throw new ArgumentNullException("pad");

            if ((terminals != null) && terminals.Contains(pad))
                throw new InvalidOperationException(
                    String.Format("El pad ya pertenece a la señal '{0}'.", name));

            if (terminals == null)
                terminals = new List<Terminal>();
            terminals.Add(pad);
        }

        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        public IEnumerable<Element> Elements {
            get {
                return elements;
            }
        }

        public IEnumerable<Terminal> Terminals {
            get {
                return terminals;
            }
        }
    }
}
