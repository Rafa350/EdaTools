namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System.Collections.Generic;

    public sealed class ComplexDeclarationNode: DeclarationNode {

        private readonly IEnumerable<DeclarationNode> declarations;

        public ComplexDeclarationNode(TypeNode type, IdentifierNode identifier, IEnumerable<DeclarationNode> declarations):
            base(type, identifier) {

            this.declarations = declarations;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public IEnumerable<DeclarationNode> Declarations {
            get {
                return declarations;
            }
        }
    }
}
