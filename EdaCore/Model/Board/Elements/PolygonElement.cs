using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Base.Geometry.Utils;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una regio poligonal.
    /// </summary>
    /// 
    public class PolygonElement : Element, ILayer {

        public sealed class Segment {

            private Point _position;
            private Angle _angle;

            public Segment(Point position, Angle angle) {

                _position = position;
                _angle = angle;
            }

            public Point Position {
                get => _position;
                set => _position = value;
            }

            public Angle Angle {
                get => _angle;
                set => _angle = value;
            }
        }

        private readonly List<Segment> _segments = new List<Segment>();
        private LayerId _layerId;
        private int _thickness;
        private bool _filled;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layerId">La capa.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="filled">True si es ple.</param>
        /// <param name="segments">Llista de segments.</param>
        /// 
        public PolygonElement(LayerId layerId, int thickness, bool filled, IEnumerable<Segment> segments = null) :
            base() {

            if (thickness < 0)
                throw new ArgumentOutOfRangeException(nameof(thickness));

            _layerId = layerId;
            _thickness = thickness;
            _filled = filled;

            if (segments != null)
                foreach (var segment in segments)
                    Add(segment);
        }

        /// <inheritdoc/>
        /// 
        public override Element Clone() {

            return new PolygonElement(_layerId, _thickness, _filled, _segments);
        }

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            var firstPoint = new Point();
            var prevPoint = new Point();
            var angle = Angle.Zero;

            var points = new List<Point>();

            bool first = true;
            foreach (var segment in _segments) {

                // Guarda el primer punt, per tancar el poligon
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
                    points.AddRange(PolygonBuilder.MakeArc(center, radius, startAngle, angle));
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
                points.AddRange(PolygonBuilder.MakeArc(center, radius, startAngle, angle, false));
            }

            return new Polygon(points.ToArray());
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            Polygon polygon = GetPolygon(side);
            if (spacing != 0)
                return PolygonProcessor.Offset(polygon, spacing);
            else
                return polygon;
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            Polygon polygon = GetPolygon(side);
            return polygon.BoundingBox;
        }

        /// <summary>
        /// Afegeix un segment a la regio.
        /// </summary>
        /// <param name="segment">El segment a afeigir.</param>
        /// 
        public void Add(Segment segment) {

            _segments.Add(segment);
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

        /// <inheritdoc/>
        /// 
        public override bool IsOnLayer(LayerId layerId) =>
            _layerId == layerId;

        /// <summary>
        /// Obte o asigna la capa.
        /// </summary>
        /// 
        public LayerId LayerId {
            get => _layerId;
            set => _layerId = value;
        }

        /// <summary>
        /// Obte o asigna l'amplada de linia del perfil.
        /// </summary>
        /// 
        public int Thickness {
            get {
                return _thickness;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Tickness");

                _thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna el valor que indica si la regio es dibuixa plena.
        /// </summary>
        /// 
        public bool Filled {
            get => (_thickness == 0) || _filled;
            set => _filled = value;
        }

        /// <summary>
        /// Obte la llista se segments.
        /// </summary>
        /// 
        public IEnumerable<Segment> Segments =>
            _segments;

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType => 
            ElementType.Polygon; 
    }
}
