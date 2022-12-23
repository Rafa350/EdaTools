using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Utils;
using NetSerializer.V5.Attributes;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una regio poligonal.
    /// </summary>
    /// 
    public class EdaPolygonElement: EdaElement {

        private IEnumerable<EdaArcPoint> _vertices;
        private int _thickness;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            if (_vertices == null)
                return null;

            else {
                var firstPoint = new EdaPoint();
                var prevPoint = new EdaPoint();
                var angle = EdaAngle.Zero;

                var points = new List<EdaPoint>();

                bool first = true;
                foreach (var segment in _vertices) {

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
                        points.AddRange(EdaPointFactory.CreateArc(center, radius, startAngle, angle));
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
                    points.AddRange(EdaPointFactory.CreateArc(center, radius, startAngle, angle, false));
                }

                return new EdaPolygon(points);
            }
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            if (_vertices == null)
                return null;

            else {
                var polygon = GetPolygon(layerId);
                return (spacing == 0) ? polygon : polygon.Offset(spacing);
            }
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            if (_vertices == null)
                return new EdaRect(0, 0, 0, 0);

            else {
                var polygon = GetPolygon(layerId);
                return polygon.Bounds;
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
                    throw new ArgumentOutOfRangeException(nameof(value));

                _thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna el valor que indica si la regio es dibuixa plena.
        /// </summary>
        /// 
        public bool Filled {
            get => _thickness == 0;
            set {
                if (value)
                    _thickness = 0;
            }
        }

        /// <summary>
        /// La llista de segments.
        /// </summary>
        /// 
        [NetSerializerOptions(Exclude = true)]
        public IEnumerable<EdaArcPoint> Vertices {
            get => _vertices;
            set => _vertices = value;
        }
    }
}
