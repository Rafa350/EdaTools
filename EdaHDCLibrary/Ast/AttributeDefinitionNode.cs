namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public sealed class AttributeDefinitionNode: DeclarationNode {

        private readonly Type type;

        public AttributeDefinitionNode(string name, Type type):
            base(name) { 

            this.type = type;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public Type Type {
            get {
                return type;
            }
        }
    }
}
