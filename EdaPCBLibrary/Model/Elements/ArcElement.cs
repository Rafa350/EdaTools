namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System.Windows;

    /// <summary>
    /// Clase que representa un arc.
    /// </summary>
    public sealed class ArcElement: LineElement, IConectable {

        private Angle angle;

        /// <summary>
        /// Constructor de l'objecte, amb els parametres per defecte.
        /// </summary>
        /// 
        public ArcElement():
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="startPosition">Punt inicial.</param>
        /// <param name="endPosition">Punt final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="angle">Angle del arc.</param>
        /// <param name="lineCap">Extrems de linia.</param>
        /// 
        public ArcElement(Point startPosition, Point endPosition, double thickness, Angle angle, LineCapStyle lineCap) :
            base(startPosition, endPosition, thickness, lineCap) {

            this.angle = angle;
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

            return PolygonBuilder.BuildArcSegment(Center, Radius, StartAngle, angle, Thickness);
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, double spacing) {

            return PolygonBuilder.BuildArcSegment(Center, Radius, StartAngle, angle, Thickness + (spacing * 2));
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
        /// Obte el radi de l'arc.
        /// </summary>
        /// 
        public double Radius {
            get {
                return ArcUtils.Radius(StartPosition, EndPosition, angle);
            }
        }
    }
}
