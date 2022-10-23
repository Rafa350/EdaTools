using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Utils;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un arc.
    /// </summary>
    /// 
    public sealed class EdaArcElement: EdaLineElement, IEdaConectable {

        private EdaAngle _angle;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            if (_angle == EdaAngle.Zero)
                return base.GetPolygon(layerId);
            else {
                var points = EdaPointFactory.CreateArcTrace(Center, Radius, StartAngle, _angle, Thickness, LineCap == EdaLineCap.Round);
                return new EdaPolygon(points);
            }
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            if (_angle == EdaAngle.Zero)
                return base.GetOutlinePolygon(layerId, spacing);
            else {
                var points = EdaPointFactory.CreateArcTrace(Center, Radius, StartAngle, _angle, Thickness + (spacing * 2), LineCap == EdaLineCap.Round);
                return new EdaPolygon(points);
            }
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            var polygon = GetPolygon(layerId);
            return polygon.Bounds;
        }

        /// <summary>
        /// L'angle del arc.
        /// </summary>
        /// 
        public EdaAngle Angle {
            get => _angle;
            set => _angle = value;
        }

        /// <summary>
        /// El centre de l'arc.
        /// </summary>
        /// 
        public EdaPoint Center =>
            ArcUtils.Center(StartPosition, EndPosition, _angle);

        /// <summary>
        /// L'angle inicial del arc.
        /// </summary>
        /// 
        public EdaAngle StartAngle =>
            ArcUtils.StartAngle(StartPosition, Center);

        /// <summary>
        /// L'angle final del arc.
        /// </summary>
        /// 
        public EdaAngle EndAngle =>
            ArcUtils.EndAngle(EndPosition, Center);

        /// <summary>
        /// El radi de l'arc.
        /// </summary>
        /// 
        public int Radius =>
            ArcUtils.Radius(StartPosition, EndPosition, _angle);

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Arc;
    }
}
