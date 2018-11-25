namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public sealed class SimpleDeclarationNode: DeclarationNode {

        private readonly ValueNode value;

        public SimpleDeclarationNode(TypeNode type, IdentifierNode identifier, ValueNode value):
            base(type, identifier) {

            if (value == null)
                throw new ArgumentNullException("value");

            this.value = value;
        }

        public ValueNode Value {
            get {
                return value;
            }
        }
    }
}
