﻿namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public interface IVisitor {

        void Visit(Board board);
        void Visit(Layer layer);
        void Visit(Part part);
        void Visit(Pad pad);
        void Visit(Parameter parameter);
        void Visit(Signal signal);
        void Visit(Component component);
        void Visit(LineElement line);
        void Visit(ArcElement arc);
        void Visit(RectangleElement rectangle);
        void Visit(CircleElement circle);
        void Visit(ViaElement via);
        void Visit(SmdPadElement pad);
        void Visit(ThPadElement pad);
        void Visit(RegionElement region);
        void Visit(TextElement text);
        void Visit(HoleElement hole);
    }
}
