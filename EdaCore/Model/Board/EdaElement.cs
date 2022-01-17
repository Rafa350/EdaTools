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
    public abstract class EdaElement: IEdaVisitable<IEdaBoardVisitor> {

        private EdaLayerSet _layerSet;

        /// <inheritdoc/>
        /// 
        public abstract void AcceptVisitor(IEdaBoardVisitor visitor);

        /// <summary>
        /// Obte el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public abstract EdaPolygon GetPolygon(BoardSide side);

        /// <summary>
        /// Obte el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>El poligon.</returns>
        /// 
        public abstract EdaPolygon GetOutlinePolygon(BoardSide side, int spacing);

        /// <summary>
        /// Obte el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public abstract EdaRect GetBoundingBox(BoardSide side);

        /// <summary>
        /// Indica si l'element es present en una capa.
        /// </summary>
        /// <param name="layerId">La capa.</param>
        /// <returns>True si es present.</returns>
        /// 
        public virtual bool IsOnLayer(EdaLayerId layerId) =>
            _layerSet == null ? false : _layerSet.Contains(layerId);

        /// <summary>
        /// El conjun de capes on es present l'element.
        /// </summary>
        /// 
        public EdaLayerSet LayerSet {
            get {
                if (_layerSet == null)
                    _layerSet = new EdaLayerSet();
                return _layerSet;
            }
            set => _layerSet = value;
        }

        /// <summary>
        /// Obte el identificador del tipus d'element
        /// </summary>
        /// 
        public abstract ElementType ElementType { get; }
    }
}
