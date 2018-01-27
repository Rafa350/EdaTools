namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using System;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Clase que representa una regio poligonal.
    /// </summary>
    public sealed class RegionElement: Element, IConectable {

        public struct Segment {

            private Point position;
            private double angle;

            public Segment(Point position, double angle) {

                this.position = position;
                this.angle = angle;
            }

            public Point Position {
                get { return position; }
            }

            public double Angle {
                get { return angle; }
            }
        }

        private readonly List<Segment> segments = new List<Segment>();
        private double thickness = 0.1;
        private double isolation = 0;

        /// <summary>
        /// Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public RegionElement():
            base() {
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="isolation">Distancia d'aillament.</param>
        /// 
        public RegionElement(double thickness = 0, double isolation = 0):
            base() {

            if (thickness < 0)
                throw new ArgumentOutOfRangeException("thickness");

            if (isolation < 0)
                throw new ArgumentOutOfRangeException("isolation");

            this.thickness = thickness;
            this.isolation = isolation;
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="isolation">D'istancia d'aillament.</param>
        /// <param name="thickness">Amplada de linia del perfil.</param>
        /// <param name="segments">Llista de segments.</param>
        /// 
        public RegionElement(double thickness, double isolation, IEnumerable<Segment> segments) :
            base() {

            this.thickness = thickness;
            this.isolation = isolation;

            foreach (Segment segment in segments)
                Add(segment);
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
        /// Creas el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            Point currentPoint = new Point();

            Point firstPoint = new Point();
            double angle = 0;

            Polygon polygon = new Polygon();
            bool first = true;
            foreach (Segment segment in segments) {

                if (first) {
                    first = false;
                    firstPoint = segment.Position;
                    polygon.AddPoint(segment.Position);
                }

                else {
                 
                    // Tram recte
                    //
                    if (angle == 0)
                        polygon.AddPoint(segment.Position);

                    // Tram circular
                    //
                    else {
                        Point center = ArcUtils.Center(currentPoint, segment.Position, angle);
                        double radius = ArcUtils.Radius(currentPoint, segment.Position, angle);
                        double startAngle = ArcUtils.StartAngle(currentPoint, center);
                        polygon.AddPoints(PolygonBuilder.ArcPoints(center, radius, startAngle, angle, false));
                    }
                }

                currentPoint = segment.Position;
                angle = segment.Angle;
            }

            if (angle == 0)
                polygon.AddPoint(firstPoint);

            else {
                Point center = ArcUtils.Center(currentPoint, firstPoint, angle);
                double radius = ArcUtils.Radius(currentPoint, firstPoint, angle);
                double startAngle = ArcUtils.StartAngle(currentPoint, center);
                polygon.AddPoints(PolygonBuilder.ArcPoints(center, radius, startAngle, angle, false));
            }

            return polygon;
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        ///
        public override Polygon GetPourPolygon(BoardSide side, double spacing) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Afegeix un segment a la regio.
        /// </summary>
        /// <param name="segment">El segment a afeigir.</param>
        /// 
        public void Add(Segment segment) {

            if ((segments.Count == 0) && (segment.Angle != 0))
                throw new InvalidOperationException("En primer segmento no puede ser un arco.");

            segments.Add(segment);
        }

        /// <summary>
        /// Afegeix una linia a la regio.
        /// </summary>
        /// <param name="position">Vertex final de la linia</param>
        /// 
        public void AddLine(Point position) {

            Add(new Segment(position, 0));
        }

        /// <summary>
        /// Afegeix un arc a la regio.
        /// </summary>
        /// <param name="position">Vertec final del arc.</param>
        /// <param name="angle">Angle del arc.</param>
        /// 
        public void AddArc(Point position, double angle) {

            Add(new Segment(position, angle));
        }

        /// <summary>
        /// Obte o asigna l'amplada de linia del perfil.
        /// </summary>
        /// 
        public double Thickness {
            get {
                return thickness;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Tickness");

                thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna la distancia d'aillament.
        /// </summary>
        /// 
        public double Isolation {
            get {
                return isolation;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Isolation");

                isolation = value;
            }
        }

        /// <summary>
        /// Obte o asigna el valor que indica si la regio es dibuixa plena.
        /// </summary>
        /// 
        public bool Filled {
            get {
                return thickness == 0;
            }
            set {
                if (value)
                    Thickness = 0; // Canvia la propietat
            }
        }

        /// <summary>
        /// Obte la llista se segments.
        /// </summary>
        /// 
        public IEnumerable<Segment> Segments {
            get {
                return segments;
            }
        }
    }
}
