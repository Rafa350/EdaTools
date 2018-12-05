namespace MikroPic.EdaTools.v1.Panel.Model.Visitors {

    using MikroPic.EdaTools.v1.Panel.Model.Elements;

    public class DefaultVisitor: IVisitor {

        public virtual void Run() {
        }

        public virtual void Visit(Panel panel) {

        }

        public virtual void Visit(MillingElement join) {
        }

        public virtual void Visit(PlaceElement place) {
        }
    }
}
