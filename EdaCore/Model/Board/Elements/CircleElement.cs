using System;

using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un cercle.
    /// </summary>
    /// 
    public sealed class CircleElement : EdaElement, IEdaPosition {

        private EdaPoint _position;
        private int _radius;
        private int _thickness;
        private bool _filled;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            if (Filled) {
                var points = PolygonBuilder.MakeCircle(_position, _radius);
                return new Polygon(points);
            }
            else {
                var outerPoints = PolygonBuilder.MakeCircle(_position, _radius + (_thickness / 2));
                var innerPoints = PolygonBuilder.MakeCircle(_position, _radius - (_thickness / 2));
                return new Polygon(outerPoints, new Polygon(innerPoints));
            }
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            var points = PolygonBuilder.MakeCircle(_position, _radius + (_thickness / 2) + spacing);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            int r = _radius + (_thickness / 2);
            return new Rect(_position.X - r, _position.Y - r, r + r, r + r);
        }

        /// <summary>
        ///  La posicio del centre del cercle.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// El radi del cercle.
        /// </summary>
        /// 
        public int Radius {
            get => _radius;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Radius");

                _radius = value;
            }
        }

        /// <summary>
        /// Diametre del cercle.
        /// </summary>
        /// 
        public int Diameter {
            get => _radius * 2;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Diameter");

                _radius = value / 2;
            }
        }

        /// <summary>
        /// L'amplada de linia.
        /// </summary>
        /// 
        public int Thickness {
            get => _thickness;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Thickness");

                _thickness = value;
            }
        }

        /// <summary>
        /// Indicador de cercle ple.
        /// </summary>
        /// 
        public bool Filled {
            get => (_thickness == 0) || _filled;
            set => _filled = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Circle;
    }
}
