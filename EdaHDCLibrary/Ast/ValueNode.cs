namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public sealed class ValueNode: Node {

        private readonly object value;

        public ValueNode(object value) {

            this.value = value;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public object Value {
            get {
                return value;
            }
        }
    }
}
