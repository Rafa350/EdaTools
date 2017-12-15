namespace MikroPic.EdaTools.v1.Pcb.Model {

    public interface IVisitable {

        void AcceptVisitor(IVisitor visitor);
    }
}
