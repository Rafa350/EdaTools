namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public interface IVisitable {

        void AcceptVisitor(IVisitor visitor);
    }
}
