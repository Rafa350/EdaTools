namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public sealed class PinDefinitionNode: DeclarationNode {

        private readonly string identifier;

        public PinDefinitionNode(string name, string identifier):
            base(name) {

            this.identifier = identifier;
        }

        public override void AcceptVisitor(IVisitor visitor) {
        }

        public string Identifier {
            get {
                return identifier;
            }
        }
    }
}
