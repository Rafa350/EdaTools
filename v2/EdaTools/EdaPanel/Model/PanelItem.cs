namespace MikroPic.EdaTools.v1.Panel.Model {

    public abstract class PanelItem : IPanelVisitable {

        public abstract void AcceptVisitor(IPanelVisitor visitor);
    }
}
