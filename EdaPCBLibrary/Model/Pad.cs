namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;

    public sealed class Pad {

        private readonly IConected element;

        public Pad(IConected element) {

            if (element == null)
                throw new ArgumentNullException("element");

            this.element = element;
        }

        public IConected Element {
            get {
                return element;
            }
        }
    }
}
