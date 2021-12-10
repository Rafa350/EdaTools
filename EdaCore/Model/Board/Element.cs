using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Model.Common;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Identifica el tipus d'element
    /// </summary>
    /// 
    public enum ElementType {
        Line,
        Arc,
        Rectangle,
        Circle,
        Polygon,
        Text,
        SmdPad,
        ThPad,
        SlotPad,
        Region,
        Via,
        Hole
    }

    /// <summary>
    /// Clase base per tots els elements de la placa.
    /// </summary>
    /// 
    public abstract class Element : IVisitable<IBoardVisitor> {

        private readonly LayerSet _layerSet;

        /// <summary>
        /// Constructor del element.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes en el que es present.</param>
        /// 
        protected Element(LayerSet layerSet) {

            _layerSet = layerSet;
        }

        /// <summary>
        /// Obte una copia en profunditat de l'objecte.
        /// </summary>
        /// <returns>La copia de l'objecte.</returns>
        /// 
        public abstract Element Clone();

        /// <inheritdoc/>
        /// 
        public abstract void AcceptVisitor(IBoardVisitor visitor);

        /// <summary>
        /// Obte el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public abstract Polygon GetPolygon(BoardSide side);

        /// <summary>
        /// Obte el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>El poligon.</returns>
        /// 
        public abstract Polygon GetOutlinePolygon(BoardSide side, int spacing);

        /// <summary>
        /// Obte el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public abstract Rect GetBoundingBox(BoardSide side);

        /// <summary>
        /// Indica si l'element es present en una capa.
        /// </summary>
        /// <param name="layerId">La capa.</param>
        /// <returns>True si es present.</returns>
        /// 
        public virtual bool IsOnLayer(LayerId layerId) =>
            _layerSet.Contains(layerId);

        /// <summary>
        /// El conjun de capes on es present l'element.
        /// </summary>
        /// 
        public LayerSet LayerSet =>
            _layerSet;

        /// <summary>
        /// Obte el identificador del tipus d'element
        /// </summary>
        /// 
        public abstract ElementType ElementType { get; }
    }
}
