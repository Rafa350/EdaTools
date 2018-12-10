namespace MikroPic.EdaTools.v1.Panel.Model {

    public abstract class ProjectItem: IVisitable {

        public abstract void AcceptVisitor(IVisitor visitor);
    }
}
