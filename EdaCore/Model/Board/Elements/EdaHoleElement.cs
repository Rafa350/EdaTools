using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un forat circular en la placa.
    /// </summary>
    /// 
    public class EdaHoleElement: EdaElement {

        private EdaPoint _position;
        private int _drill;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            return new EdaRect(0, 0, _drill, _drill);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            var points = EdaPoints.CreateCircle(_position, (_drill / 2) + spacing);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            var points = EdaPoints.CreateCircle(_position, _drill);
            return new EdaPolygon(points);
        }

        /// <summary>
        /// La posicio.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set => _position = value;
        }

        /// <inheritdoc/>
        /// 
        public override bool IsOnLayer(EdaLayerId layerId) =>
            layerId == EdaLayerId.Holes;

        /// <summary>
        /// Diamtre del forat.
        /// </summary>
        /// 
        public int Drill {
            get => _drill;
            set => _drill = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType => 
            ElementType.Hole;
    }
}
