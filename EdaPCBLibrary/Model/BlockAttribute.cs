namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;

    public sealed class BlockAttribute: IVisitable {

        private readonly string name;
        private string value;

        public BlockAttribute(string name, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public string Name {
            get {
                return name;
            }
        }

        public string Value {
            get {
                return value;
            }
            set {
                this.value = value;
            }
        }
    }
}
