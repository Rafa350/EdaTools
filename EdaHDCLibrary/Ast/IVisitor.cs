namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public interface IVisitor {

        void Visit(SimpleDeclarationNode node);
        void Visit(ComplexDeclarationNode node);
        void Visit(TypeNode node);
        void Visit(ValueNode node);
        void Visit(IdentifierNode node);
    }
}
