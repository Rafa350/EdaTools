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
    public class PolygonElement : EdaElement {

        private readonly List<EdaArcPoint> _segments = new List<EdaArcPoint>();
        private int _thickness;
        private bool _filled;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            var firstPoint = new EdaPoint();
            var prevPoint = new EdaPoint();
            var angle = EdaAngle.Zero;

            var points = new List<EdaPoint>();

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
                    EdaPoint center = ArcUtils.Center(prevPoint, segment.Position, angle);
                    int radius = ArcUtils.Radius(prevPoint, segment.Position, angle);
                    EdaAngle startAngle = ArcUtils.StartAngle(prevPoint, center);
                    points.AddRange(PolygonBuilder.MakeArc(center, radius, startAngle, angle));
                }

                prevPoint = segment.Position;
                angle = segment.Arc;
            }

            if (angle.IsZero)
                points.Add(firstPoint);

            else {
                EdaPoint center = ArcUtils.Center(prevPoint, firstPoint, angle);
                int radius = ArcUtils.Radius(prevPoint, firstPoint, angle);
                EdaAngle startAngle = ArcUtils.StartAngle(prevPoint, center);
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
        /// Afegeix un segment al poligon.
        /// </summary>
        /// <param name="segment">El segment.</param>
        /// 
        public void AddSegment(EdaArcPoint segment) {

            _segments.Add(segment);
        }

        /// <summary>
        /// Afegeix una diversos segments al poligon.
        /// </summary>
        /// <param name="segments">Els segments.</param>
        /// 
        public void AddSegments(IEnumerable<EdaArcPoint> segments) {

            _segments.AddRange(segments);
        }

        /// <summary>
        /// Obte o asigna l'amplada de linia del perfil.
        /// </summary>
        /// 
        public int Thickness {
            get => _thickness;
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
        /// La llista se segments.
        /// </summary>
        /// 
        public IEnumerable<EdaArcPoint> Segments =>
            _segments;

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Polygon;
    }
}
