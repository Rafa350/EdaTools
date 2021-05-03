using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Interficie pels visitadors de placa
    /// </summary>
    /// 
    public interface IBoardVisitor {

        /// <summary>
        /// Visita una placa.
        /// </summary>
        /// <param name="board">La placa</param>
        /// 
        void Visit(Board board);

        /// <summary>
        /// Visita una llibreria
        /// </summary>
        /// <param name="library">La llibraria</param>
        /// 
        void Visit(Library library);

        /// <summary>
        /// Visita una capa
        /// </summary>
        /// <param name="layer">La capa</param>
        /// 
        void Visit(Layer layer);

        void Visit(Part part);
        void Visit(PartAttribute attribute);
        void Visit(Signal signal);

        /// <summary>
        /// Visita un component
        /// </summary>
        /// <param name="block">El component</param>
        /// 
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
