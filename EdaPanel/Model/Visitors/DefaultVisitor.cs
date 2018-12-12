namespace MikroPic.EdaTools.v1.Panel.Model.Visitors {

    using MikroPic.EdaTools.v1.Panel.Model.Items;

    public class DefaultVisitor: IVisitor {

        public virtual void Run() {
        }

        public virtual void Visit(Project project) {
        }

        public virtual void Visit(CutItem join) {
        }

        public virtual void Visit(PcbItem place) {
        }
    }
}
