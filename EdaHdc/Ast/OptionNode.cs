namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public sealed class OptionNode: Node {

        private readonly string name;
        private readonly object value;

        public OptionNode(string name, object value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("null");

            this.name = name;
            this.value = value;
        }

        public override void AcceptVisitor(IVisitor visitor) {
        }

        public string Name {
            get {
                return name;
            }
        }

        public object Value {
            get {
                return value;
            }
        }
    }
}
