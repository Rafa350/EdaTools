namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;

    public sealed class Pad: IVisitable {

        private readonly IConectable element;

        public Pad(IConectable element) {

            if (element == null)
                throw new ArgumentNullException("element");

            this.element = element;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public string Name {
            get {
                return "noname";
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
