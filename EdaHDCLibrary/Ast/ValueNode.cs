namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public sealed class ValueNode: Node {

        private readonly object value;

        public ValueNode(object value) {

            this.value = value;
        }

        public object Value {
            get {
                return value;
            }
        }
    }
}
