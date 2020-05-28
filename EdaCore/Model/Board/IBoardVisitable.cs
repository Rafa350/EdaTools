namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public interface IBoardVisitable {

        void AcceptVisitor(IBoardVisitor visitor);
    }
}
