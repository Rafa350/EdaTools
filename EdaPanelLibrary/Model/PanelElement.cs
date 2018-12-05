namespace MikroPic.EdaTools.v1.Panel.Model {

    public abstract class PanelElement: IVisitable {

        public abstract void AcceptVisitor(IVisitor visitor);
    }
}
