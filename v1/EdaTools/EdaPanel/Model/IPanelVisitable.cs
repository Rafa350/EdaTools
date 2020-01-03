namespace MikroPic.EdaTools.v1.Panel.Model {

    public interface IPanelVisitable {

        void AcceptVisitor(IPanelVisitor visitor);
    }
}
