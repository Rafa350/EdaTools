namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public sealed class ValueNode: Node {

        private readonly Type type;
        private readonly object value;

        public ValueNode(object value, Type type) {

            this.value = value;
            this.type = type;
        }

        public override void AcceptVisitor(IVisitor visitor) {
        }
    }
}
