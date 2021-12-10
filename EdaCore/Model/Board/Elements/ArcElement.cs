using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Base.Geometry.Utils;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un arc.
    /// </summary>
    /// 
    public sealed class ArcElement : LineElement, IConectable {

        private Angle _angle;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="startPosition">La posicio inicial.</param>
        /// <param name="endPosition">La posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="angle">Angle del arc.</param>
        /// <param name="lineCap">Extrems de linia.</param>
        /// 
        public ArcElement(LayerSet layerSet, Point startPosition, Point endPosition, int thickness, Angle angle, CapStyle lineCap) :
            base(layerSet, startPosition, endPosition, thickness, lineCap) {

            _angle = angle;
        }

        ///  <inheritdoc/>
        ///  
        public override Element Clone() {

            return new ArcElement(LayerSet, StartPosition, EndPosition, Thickness, _angle, LineCap);
        }

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            var points = PolygonBuilder.MakeArcTrace(Center, Radius, StartAngle, _angle, Thickness, LineCap == CapStyle.Round);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            var points = PolygonBuilder.MakeArcTrace(Center, Radius, StartAngle, _angle, Thickness + (spacing * 2), LineCap == CapStyle.Round);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            var polygon = GetPolygon(side);
            return polygon.BoundingBox;
        }

        /// <summary>
        /// Obte o asigna l'angle del arc.
        /// </summary>
        /// 
        public Angle Angle {
            get => _angle;
            set => _angle = value;
        }

        /// <summary>
        /// Obte el centre de l'arc.
        /// </summary>
        /// 
        public Point Center =>
            ArcUtils.Center(StartPosition, EndPosition, _angle);

        /// <summary>
        /// Obte l'angle inicial del arc.
        /// </summary>
        /// 
        public Angle StartAngle =>
            ArcUtils.StartAngle(StartPosition, Center);
            
        /// <summary>
        /// Obtel'angle final del arc.
        /// </summary>
        /// 
        public Angle EndAngle =>
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
