namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Clase que representa una regio poligonal.
    /// </summary>
    public sealed class RegionElement: Element, IConectable {

        public struct Segment {

            private PointInt position;
            private Angle angle;

            public Segment(PointInt position, Angle angle) {

                this.position = position;
                this.angle = angle;
            }

            public PointInt Position {
                get {
                    return position;
                }
            }

            public Angle Angle {
                get {
                    return angle;
                }
            }
        }

        private readonly List<Segment> segments = new List<Segment>();
        private int thickness = 100000;

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
        /// 
        public RegionElement(int thickness = 0):
            base() {

            if (thickness < 0)
                throw new ArgumentOutOfRangeException("thickness");

            this.thickness = thickness;
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="isolation">D'istancia d'aillament.</param>
        /// <param name="segments">Llista de segments.</param>
        /// 
        public RegionElement(int thickness, IEnumerable<Segment> segments) :
            base() {

            this.thickness = thickness;

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

            PointInt currentPoint = new PointInt();

            PointInt firstPoint = new PointInt();
            Angle angle = Angle.Zero;

            List<PointInt> points = new List<PointInt>();

            bool first = true;
            foreach (Segment segment in segments) {

                if (first) {
                    first = false;
                    firstPoint = segment.Position;
                    points.Add(segment.Position);
                }

                else {
                 
                    // Tram recte
                    //
                    if (angle.IsZero)
                        points.Add(segment.Position);

                    // Tram circular
                    //
                    else {
                        PointInt center = ArcUtils.Center(currentPoint, segment.Position, angle);
                        int radius = ArcUtils.Radius(currentPoint, segment.Position, angle);
                        Angle startAngle = ArcUtils.StartAngle(currentPoint, center);
                        points.AddRange(PolygonBuilder.BuildArc(center, radius, startAngle, angle));
                    }
                }

                currentPoint = segment.Position;
                angle = segment.Angle;
            }

            if (angle.IsZero)
                points.Add(firstPoint);

            else {
                PointInt center = ArcUtils.Center(currentPoint, firstPoint, angle);
                int radius = ArcUtils.Radius(currentPoint, firstPoint, angle);
                Angle startAngle = ArcUtils.StartAngle(currentPoint, center);
                points.AddRange(PolygonBuilder.BuildArc(center, radius, startAngle, angle, false));
            }

            return new Polygon(points.ToArray());
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        ///
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override RectInt GetBoundingBox(BoardSide side) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Afegeix un segment a la regio.
        /// </summary>
        /// <param name="segment">El segment a afeigir.</param>
        /// 
        public void Add(Segment segment) {

            segments.Add(segment);
        }

        /// <summary>
        /// Afegeix una linia a la regio.
        /// </summary>
        /// <param name="position">Vertex final de la linia</param>
        /// 
        public void AddLine(PointInt position) {

            Add(new Segment(position, Angle.Zero));
        }

        /// <summary>
        /// Afegeix un arc a la regio.
        /// </summary>
        /// <param name="position">Vertec final del arc.</param>
        /// <param name="angle">Angle del arc.</param>
        /// 
        public void AddArc(PointInt position, Angle angle) {

            Add(new Segment(position, angle));
        }

        /// <summary>
        /// Obte o asigna l'amplada de linia del perfil.
        /// </summary>
        /// 
        public int Thickness {
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
