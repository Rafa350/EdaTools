namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;
    using System.Collections.Generic;

    public sealed class RegionElement: SingleLayerElement {

        public struct Segment {

            private Point vertex;
            private double angle;

            public Segment(Point vertex, double angle) {

                this.vertex = vertex;
                this.angle = angle;
            }

            public Point Vertex {
                get { return vertex; }
            }

            public double Angle {
                get { return angle; }
            }
        }

        private readonly List<Segment> segments = new List<Segment>();
        private double thickness = 0;
        private double isolation = 0.15;

        /// <summary>
        /// Constructor per defecte del objecte.
        /// </summary>
        /// 
        public RegionElement():
            base() {

        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layer">Capa a la que pertany.</param>
        /// 
        public RegionElement(Layer layer):
            base(layer) {

        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layer">Capa a la que pertany.</param>
        /// <param name="segments">Llista de segments.</param>
        /// 
        public RegionElement(Layer layer, IEnumerable<Segment> segments) :
            base(layer) {

            foreach (Segment segment in segments)
                Add(segment);
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
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
        /// <param name="vertex">Vertex final de la linia</param>
        /// 
        public void AddLine(Point vertex) {

            Add(new Segment(vertex, 0));
        }

        /// <summary>
        /// Afegeix un arc a la regio.
        /// </summary>
        /// <param name="vertex">Vertec final del arc.</param>
        /// <param name="angle">Angle del arc.</param>
        /// 
        public void AddArc(Point vertex, double angle) {

            Add(new Segment(vertex, angle));
        }

        /// <summary>
        /// Obte o asigna l'amplada de linia.
        /// </summary>
        /// 
        public double Thickness {
            get {
                return thickness;
            }
            set {
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
                    thickness = 0;
            }
        }

        /// <summary>
        /// obte la llista se segments.
        /// </summary>
        /// 
        public IEnumerable<Segment> Segments {
            get {
                return segments;
            }
        }
    }
}
