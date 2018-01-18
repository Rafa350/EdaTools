namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;

    public sealed class Pad: IConectable, IVisitable {

        private readonly string name;
        private readonly IConectable element;

        public Pad(string name, IConectable element) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (element == null)
                throw new ArgumentNullException("element");

            this.name = name;
            this.element = element;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Obte l'element associat
        /// </summary>
        /// 
        public IConectable Element {
            get {
                return element;
            }
        }
    }
}
