namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public abstract class DeclarationNode: Node {

        private TypeNode type;
        private IdentifierNode identifier;

        public DeclarationNode(TypeNode type, IdentifierNode identifier) {

            if (type == null)
                throw new ArgumentNullException("type");

            if (identifier == null)
                throw new ArgumentNullException("identifier");

            this.type = type;
            this.identifier = identifier;
        }

        public TypeNode Type {
            get {
                return type;
            }
        }

        public IdentifierNode Identifier {
            get {
                return identifier;
            }
        }
    }
}
