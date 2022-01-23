using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Interficie pels visitadors de placa
    /// </summary>
    /// 
    public interface IEdaBoardVisitor {

        /// <summary>
        /// Visita una placa.
        /// </summary>
        /// <param name="board">La placa</param>
        /// 
        void Visit(EdaBoard board);

        /// <summary>
        /// Visita una llibreria
        /// </summary>
        /// <param name="library">La llibraria</param>
        /// 
        void Visit(EdaLibrary library);

        /// <summary>
        /// Visita una capa
        /// </summary>
        /// <param name="layer">La capa</param>
        /// 
        void Visit(EdaLayer layer);

        void Visit(EdaPart part);
        void Visit(EdaPartAttribute attribute);
        void Visit(EdaSignal signal);

        /// <summary>
        /// Visita un component
        /// </summary>
        /// <param name="block">El component</param>
        /// 
        void Visit(EdaComponent block);

        /// <summary>
        /// Visita un atribut de component.
        /// </summary>
        /// <param name="attribute">L'atribut.</param>
        /// 
        void Visit(EdaComponentAttribute attribute);

        /// <summary>
        /// Visita un element de tipus linia.
        /// </summary>
        /// <param name="line">La linia.</param>
        /// 
        void Visit(EdaLineElement line);

        /// <summary>
        /// Visita un element de tipus arc.
        /// </summary>
        /// <param name="arc">L'arc a visitar.</param>
        /// 
        void Visit(EdaArcElement arc);

        /// <summary>
        /// Visita un element de tipus rectangle.
        /// </summary>
        /// <param name="rectangle">El rectangle.</param>
        /// 
        void Visit(EdaRectangleElement rectangle);

        /// <summary>
        /// Visita un element de tipus cercle.
        /// </summary>
        /// <param name="circle">El cercle.</param>
        /// 
        void Visit(EdaCircleElement circle);

        /// <summary>
        /// Visita un element de tipus poligon.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// 
        void Visit(EdaPolygonElement polygon);

        void Visit(EdaViaElement via);

        void Visit(EdaSmdPadElement pad);
        void Visit(EdaThPadElement pad);
        void Visit(EdaRegionElement region);
        void Visit(EdaTextElement text);
    }
}
