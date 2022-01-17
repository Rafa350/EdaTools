using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
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
        public override EdaPolygon GetPolygon(BoardSide side) {

            var points = EdaPoints.CreateArcTrace(Center, Radius, StartAngle, _angle, Thickness, LineCap == CapStyle.Round);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(BoardSide side, int spacing) {

            var points = EdaPoints.CreateArcTrace(Center, Radius, StartAngle, _angle, Thickness + (spacing * 2), LineCap == CapStyle.Round);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(BoardSide side) {

            var polygon = GetPolygon(side);
            return polygon.BoundingBox;
        }

        /// <summary>
        /// Obte o asigna l'angle del arc.
        /// </summary>
        /// 
        public EdaAngle Angle {
            get => _angle;
            set => _angle = value;
        }

        /// <summary>
        /// Obte el centre de l'arc.
        /// </summary>
        /// 
        public EdaPoint Center =>
            ArcUtils.Center(StartPosition, EndPosition, _angle);

        /// <summary>
        /// Obte l'angle inicial del arc.
        /// </summary>
        /// 
        public EdaAngle StartAngle =>
            ArcUtils.StartAngle(StartPosition, Center);

        /// <summary>
        /// Obtel'angle final del arc.
        /// </summary>
        /// 
        public EdaAngle EndAngle =>
            ArcUtils.EndAngle(EndPosition, Center);

        /// <summary>
        /// Obte el radi de l'arc.
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
