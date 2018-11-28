namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public sealed class InstanceDefinitionNode : DeclarationNode {

        public InstanceDefinitionNode(string name):
            base(name) {
        }

        public override void AcceptVisitor(IVisitor visitor) {
        }
    }
}
