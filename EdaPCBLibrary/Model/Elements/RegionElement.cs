﻿namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure.Polygons;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa una regio poligonal.
    /// </summary>
    public sealed class RegionElement: Element, IConectable {

        public class Segment {

            private Point position;
            private Angle angle;

            public Segment() {

            }

            public Segment(Point position, Angle angle) {

                this.position = position;
                this.angle = angle;
            }

            public Point Position {
                get {
                    return position;
                }
                set {
                    position = value;
                }
            }

            public Angle Angle {
                get {
                    return angle;
                }
                set {
                    angle = value;
                }
            }
        }

        private readonly List<Segment> segments = new List<Segment>();
        private int thickness;
        private bool filled;
        private int clearance;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="filled">True si es ple.</param>
        /// <param name="clearance">Distancia d'aillament.</param>
        /// 
        public RegionElement(LayerSet layerSet, int thickness, bool filled, int clearance):
            base(layerSet) {

            if (thickness < 0)
                throw new ArgumentOutOfRangeException("thickness");

            if (clearance < 0)
                throw new ArgumentOutOfRangeException("clearance");

            this.thickness = thickness;
            this.filled = filled;
            this.clearance = clearance;
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="filled">True si es ple.</param>
        /// <param name="clearance">Distancia d'aillament.</param>
        /// <param name="segments">Llista de segments.</param>
        /// 
        public RegionElement(LayerSet layerSet, int thickness, bool filled, int clearance, IEnumerable<Segment> segments) :
            base(layerSet) {

            if (thickness < 0)
                throw new ArgumentOutOfRangeException("thickness");

            if (clearance < 0)
                throw new ArgumentOutOfRangeException("clearance");

            this.thickness = thickness;
            this.filled = filled;
            this.clearance = clearance;

            foreach (Segment segment in segments)
                Add(segment);
        }

        public override Element Clone() {

            RegionElement region = new RegionElement(LayerSet, thickness, filled, clearance);
            foreach (var segment in segments)
                region.Add(new Segment(segment.Position, segment.Angle));
            return region;
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

            Point firstPoint = new Point();
            Point prevPoint = new Point();
            Angle angle = Angle.Zero;

            List<Point> points = new List<Point>();

            bool first = true;
            foreach (Segment segment in segments) {

                // Guarda el prinper punt, per tancar el poligon
                //
                if (first) {
                    first = false;
                    firstPoint = segment.Position;
                }

                // Tram recte
                //
                if (angle.IsZero)
                    points.Add(segment.Position);

                // Tram circular
                //
                else {
                    Point center = ArcUtils.Center(prevPoint, segment.Position, angle);
                    int radius = ArcUtils.Radius(prevPoint, segment.Position, angle);
                    Angle startAngle = ArcUtils.StartAngle(prevPoint, center);
                    points.AddRange(PolygonBuilder.BuildArc(center, radius, startAngle, angle));
                }

                prevPoint = segment.Position;
                angle = segment.Angle;
            }

            if (angle.IsZero)
                points.Add(firstPoint);

            else {
                Point center = ArcUtils.Center(prevPoint, firstPoint, angle);
                int radius = ArcUtils.Radius(prevPoint, firstPoint, angle);
                Angle startAngle = ArcUtils.StartAngle(prevPoint, center);
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

            return PolygonProcessor.Offset(GetPolygon(side), spacing);
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

            segments.Add(segment);
        }

        /// <summary>
        /// Afegeix una linia a la regio.
        /// </summary>
        /// <param name="position">Vertex final de la linia</param>
        /// 
        public void AddLine(Point position) {

            Add(new Segment(position, Angle.Zero));
        }

        /// <summary>
        /// Afegeix un arc a la regio.
        /// </summary>
        /// <param name="position">Vertec final del arc.</param>
        /// <param name="angle">Angle del arc.</param>
        /// 
        public void AddArc(Point position, Angle angle) {

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
        /// Obte o asigna l'amplada del aillament
        /// </summary>
        /// 
        public int Clearance {
            get {
                return clearance;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Clearance");

                clearance = value;
            }
        }

        /// <summary>
        /// Obte o asigna el valor que indica si la regio es dibuixa plena.
        /// </summary>
        /// 
        public bool Filled {
            get {
                return (thickness == 0) || filled;
            }
            set {
                filled = value;
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
