namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public interface IVisitable {

        void AcceptVisitor(IVisitor visitor);
    }
}
