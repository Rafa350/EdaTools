namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public interface IVisitor {

        void Visit(PinDefinitionNode node);
        void Visit(AttributeDefinitionNode node);
        void Visit(PortDefinitionNode node);
        void Visit(NetDefinitionNode node);
        void Visit(DeviceDeclarationNode node);
        void Visit(ModuleDeclarationNode node);
        void Visit(ValueNode node);
    }
}
