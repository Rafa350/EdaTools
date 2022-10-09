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

        /// <summary>
        /// Visita un 'Part'
        /// </summary>
        /// <param name="part">El part.</param>
        /// 
        void Visit(EdaPart part);

        /// <summary>
        /// Visita un atribut d'un part.
        /// </summary>
        /// <param name="attribute">L'atribut.</param>
        /// 
        void Visit(EdaPartAttribute attribute);

        /// <summary>
        /// Visita un senyal.
        /// </summary>
        /// <param name="signal">El senyal.</param>
        /// 
        void Visit(EdaSignal signal);

        /// <summary>
        /// Visita un component
        /// </summary>
        /// <param name="component">El component</param>
        /// 
        void Visit(EdaComponent component);

        /// <summary>
        /// Visita un atribut de component.
        /// </summary>
        /// <param name="attribute">L'atribut.</param>
        /// 
        void Visit(EdaComponentAttribute attribute);

        /// <summary>
        /// Visita un element de tipus 'Line'.
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaLineElement element);

        /// <summary>
        /// Visita un element de tipus 'Arc'.
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaArcElement element);

        /// <summary>
        /// Visita un element de tipus 'Rectangle'.
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaRectangleElement element);

        /// <summary>
        /// Visita un element de tipus 'Circle'.
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaCircleElement element);

        /// <summary>
        /// Visita un element de tipus poligon.
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaPolygonElement element);

        /// <summary>
        /// Visita un element de tipus 'Via'
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaViaElement element);

        /// <summary>
        /// Visita un element de tipus 'SmdPad'
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaSmdPadElement element);

        /// <summary>
        /// Visita un element de tipus 'ThPad'
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaThPadElement element);

        /// <summary>
        /// Visita un element de tipus 'Region'
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaRegionElement element);

        /// <summary>
        /// Visita un element de tipus 'Text'
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaTextElement element);

        /// <summary>
        /// Visita un element de tipus 'CircleHole'
        /// </summary>
        /// <param name="element">L'element.</param>
        /// 
        void Visit(EdaCircularHoleElement element);

        /// <summary>
        /// Visita un element de tipus 'LineHole'
        /// </summary>
        /// <param name="circleHole">L'element.</param>
        /// 
        void Visit(EdaLinearHoleElement element);
    }
}
