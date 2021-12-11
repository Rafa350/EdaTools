namespace MikroPic.EdaTools.v1.Panel.Model {

    public interface IEdaPanelVisitable {

        void AcceptVisitor(IEdaPanelVisitor visitor);
    }
}
