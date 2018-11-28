namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public sealed class NetDefinitionNode: DeclarationNode {

        public NetDefinitionNode(string name):
            base(name) {
        }

        public override void AcceptVisitor(IVisitor visitor) {
        }
    }
}
