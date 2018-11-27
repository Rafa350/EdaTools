namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public interface IVisitor {

        void Visit(DevicePinDeclarationNode node);
        void Visit(DeviceAttributeDeclarationNode node);
        void Visit(ModulePortDeclarationNode node);
        void Visit(ModuleNetDeclarationNode node);
        void Visit(DeviceDeclarationNode node);
        void Visit(ModuleDeclarationNode node);
        void Visit(ValueNode node);
    }
}
