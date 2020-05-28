namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public interface IVisitor {

        void Visit(MemberNode node);
        void Visit(EntityNode node);
        void Visit(OptionNode node);
    }
}
