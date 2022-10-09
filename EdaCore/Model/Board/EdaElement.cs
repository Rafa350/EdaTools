using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Core.Model.Common;

namespace MikroPic.EdaTools.v1.Core.Model.Board
{

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
        CircleHole,
        LineHole,
        ArcHole
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
        /// Obte el poligon del element en una capa.
        /// </summary>
        /// <param name="layerId">Identificador de la capa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public abstract EdaPolygon GetPolygon(EdaLayerId layerId);

        /// <summary>
        /// Obte el poligon espaiat del element.
        /// </summary>
        /// <param name="layerId">Identificador de la capa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>El poligon.</returns>
        /// 
        public abstract EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing);

        /// <summary>
        /// Obte el bounding box del element.
        /// </summary>
        /// <param name="layerId">Identificador de la capa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public abstract EdaRect GetBoundingBox(EdaLayerId layerId);

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
