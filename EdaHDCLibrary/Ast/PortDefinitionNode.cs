namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public sealed class PortDefinitionNode: DeclarationNode {

        public PortDefinitionNode(string name):
            base(name) {
        }

        public override void AcceptVisitor(IVisitor visitor) {
        }
    }
}
