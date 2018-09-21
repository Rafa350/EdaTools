namespace MikroPic.EdaTools.v1.Panel.Model {

    public interface IVisitable {

        void AcceptVisitor(IVisitor visitor);
    }
}
