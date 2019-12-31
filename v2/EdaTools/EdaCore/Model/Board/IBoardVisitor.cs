namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

    public interface IBoardVisitor {

        void Visit(Board board);
        void Visit(Library library);
        void Visit(Layer layer);
        void Visit(Part part);
        void Visit(PartAttribute attribute);
        void Visit(Signal signal);
        void Visit(Component block);
        void Visit(ComponentAttribute attribute);
        void Visit(LineElement line);
        void Visit(ArcElement arc);
        void Visit(RectangleElement rectangle);
        void Visit(CircleElement circle);
        void Visit(ViaElement via);
        void Visit(SmdPadElement pad);
        void Visit(ThPadElement pad);
        void Visit(SlotElement pad);
        void Visit(RegionElement region);
        void Visit(TextElement text);
        void Visit(HoleElement hole);
    }
}
