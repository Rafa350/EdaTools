namespace MikroPic.EdaTools.v1.Panel.Model {

    public abstract class EdaPanelItem: IEdaPanelVisitable {

        public abstract void AcceptVisitor(IEdaPanelVisitor visitor);
    }
}
