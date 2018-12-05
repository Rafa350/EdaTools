namespace MikroPic.EdaTools.v1.Panel.Model {

    using MikroPic.EdaTools.v1.Panel.Model.Elements;

    public interface IVisitor {

        void Run();

        void Visit(Panel panel);
        void Visit(MillingElement join);
        void Visit(PlaceElement place);
    }
}
