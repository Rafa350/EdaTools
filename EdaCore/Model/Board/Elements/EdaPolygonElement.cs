using System;
using System.Collections.Generic;

using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Base.Geometry.Utils;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una regio poligonal.
    /// </summary>
    /// 
    public class EdaPolygonElement : EdaElement {

        private IEnumerable<EdaArcPoint> _segments;
        private int _thickness;
        private bool _filled;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(BoardSide side) {

            if (_segments == null)
                return null;

            else {
                var firstPoint = new EdaPoint();
                var prevPoint = new EdaPoint();
                var angle = EdaAngle.Zero;

                var points = EdaPoints.Create();

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
                        points.AddPoint(segment.Position);

                    // Tram circular
                    //
                    else {
                        EdaPoint center = ArcUtils.Center(prevPoint, segment.Position, angle);
                        int radius = ArcUtils.Radius(prevPoint, segment.Position, angle);
                        EdaAngle startAngle = ArcUtils.StartAngle(prevPoint, center);
                        points.AddArcPoints(center, radius, startAngle, angle);
                    }

                    prevPoint = segment.Position;
                    angle = segment.Arc;
                }

                if (angle.IsZero)
                    points.AddPoint(firstPoint);

                else {
                    EdaPoint center = ArcUtils.Center(prevPoint, firstPoint, angle);
                    int radius = ArcUtils.Radius(prevPoint, firstPoint, angle);
                    EdaAngle startAngle = ArcUtils.StartAngle(prevPoint, center);
                    points.AddArcPoints(center, radius, startAngle, angle, false);
                }

                return new EdaPolygon(points);
            }
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(BoardSide side, int spacing) {

            if (_segments == null)
                return null;

            else {
                EdaPolygon polygon = GetPolygon(side);
                if (spacing != 0)
                    return PolygonProcessor.Offset(polygon, spacing);
                else
                    return polygon;
            }
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(BoardSide side) {

            if (_segments == null)
                return new EdaRect(0, 0, 0, 0);

            else {
                EdaPolygon polygon = GetPolygon(side);
                return polygon.BoundingBox;
            }
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
        /// La llista de segments.
        /// </summary>
        /// 
        public IEnumerable<EdaArcPoint> Segments {
            get => _segments;
            set => _segments = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Polygon;
    }
}
