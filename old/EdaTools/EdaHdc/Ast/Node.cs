namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public abstract class Node: IVisitable {

        public abstract void AcceptVisitor(IVisitor visitor);
    }
}
