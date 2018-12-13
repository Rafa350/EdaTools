namespace MikroPic.EdaTools.v1.Panel.Model.Visitors {

    using MikroPic.EdaTools.v1.Panel.Model.Items;

    public abstract class DefaultVisitor: IVisitor {

        public abstract void Run();

        public virtual void Visit(Project project) {

            foreach (var item in project.Items)
                item.AcceptVisitor(this);
        }

        public virtual void Visit(CutItem join) {
        }

        public virtual void Visit(PcbItem place) {
        }
    }
}
