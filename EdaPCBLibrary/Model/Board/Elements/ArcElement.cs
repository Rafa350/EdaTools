namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;
    using MikroPic.EdaTools.v1.Core.Model.Board;

    /// <summary>
    /// Clase que representa un arc.
    /// </summary>
    public sealed class ArcElement: LineElement, IConectable {

        private Angle angle;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="startPosition">Punt inicial.</param>
        /// <param name="endPosition">Punt final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="angle">Angle del arc.</param>
        /// <param name="lineCap">Extrems de linia.</param>
        /// 
        public ArcElement(LayerSet layerSet, Point startPosition, Point endPosition, int thickness, Angle angle, LineCapStyle lineCap) :
            base(layerSet, startPosition, endPosition, thickness, lineCap) {

            this.angle = angle;
        }

        /// <summary>
        /// Clone l'element
        /// </summary>
        /// <returns>El clon de l'element.</returns>
        /// 
        public override Element Clone() {

            return new ArcElement(LayerSet, StartPosition, EndPosition, Thickness, angle, LineCap);
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Crea el poligon del objecte.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            Point[] points = PolygonBuilder.MakeArcTrace(Center, Radius, StartAngle, angle, Thickness, LineCap == LineCapStyle.Round);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            Point[] points =  PolygonBuilder.MakeArcTrace(Center, Radius, StartAngle, angle, Thickness + (spacing * 2), LineCap == LineCapStyle.Round);
            return new Polygon(points);
        }

        /// <summary>
        /// Obte l'envolvent de l'element
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El rectangle envolvent.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            Polygon polygon = GetPolygon(side);
            return polygon.BoundingBox;
        }

        /// <summary>
        /// Obte o asigna l'angle del arc.
        /// </summary>
        /// 
        public Angle Angle {
            get {
                return angle;
            }
            set {
                angle = value;
            }
        }

        /// <summary>
        /// Obte el centre de l'arc.
        /// </summary>
        /// 
        public Point Center {
            get {
                return ArcUtils.Center(StartPosition, EndPosition, angle);
            }
        }

        /// <summary>
        /// Obte l'angle inicial del arc.
        /// </summary>
        /// 
        public Angle StartAngle {
            get {
                return ArcUtils.StartAngle(StartPosition, Center);
            }
        }

        /// <summary>
        /// Obtel'angle final del arc.
        /// </summary>
        /// 
        public Angle EndAngle {
            get {
                return ArcUtils.EndAngle(EndPosition, Center);
            }
        }

        /// <summary>
        /// Obte el radi de l'arc.
        /// </summary>
        /// 
        public int Radius {
            get {
                return ArcUtils.Radius(StartPosition, EndPosition, angle);
            }
        }

        /// <summary>
        /// Obte el tipus d'element.
        /// </summary>
        /// 
        public override ElementType ElementType {
            get {
                return ElementType.Arc;
            }
        }
    }
}
