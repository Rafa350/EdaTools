using System;

using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una linia.
    /// </summary>
    /// 
    public class LineElement : EdaElement, IEdaConectable {

        public enum CapStyle {
            Round,
            Flat
        }

        private EdaPoint _startPosition;
        private EdaPoint _endPosition;
        private int _thickness;
        private CapStyle _lineCap = CapStyle.Round;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            var points = PolygonBuilder.MakeLineTrace(_startPosition, _endPosition, _thickness, LineCap == CapStyle.Round);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            var points = PolygonBuilder.MakeLineTrace(_startPosition, _endPosition, _thickness + (spacing * 2), _lineCap == CapStyle.Round);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            return new Rect(
                Math.Min(_startPosition.X, _endPosition.X) - _thickness / 2,
                Math.Min(_startPosition.Y, _endPosition.Y) - _thickness / 2,
                Math.Abs(_endPosition.X - _startPosition.X + 1) + _thickness,
                Math.Abs(_endPosition.Y - _startPosition.Y + 1) + _thickness);
        }

        /// <summary>
        /// Obte o asigna la posicio inicial.
        /// </summary>
        /// 
        public EdaPoint StartPosition {
            get => _startPosition;
            set => _startPosition = value;
        }

        /// <summary>
        /// Obte o asigna la posicio final.
        /// </summary>
        /// 
        public EdaPoint EndPosition {
            get => _endPosition;
            set => _endPosition = value;
        }

        /// <summary>
        ///  Obte o asigna l'amplada de linia.
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
        /// Obte o asigna el tipus d'extrem de linia.
        /// </summary>
        /// 
        public CapStyle LineCap {
            get => _lineCap;
            set => _lineCap = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Line;
    }
}

