using System;

using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un forat no conductor.
    /// </summary>
    /// 
    public sealed class HoleElement : EdaElement, IPosition {

        private EdaPoint _position;
        private int _drill;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            var points = PolygonBuilder.MakeCircle(_position, _drill / 2);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            var points = PolygonBuilder.MakeCircle(_position, (_drill / 2) + spacing);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            return new Rect(_position.X - _drill / 2, _position.Y - _drill / 2, _drill, _drill);
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
        /// El diametre del forat.
        /// </summary>
        /// 
        public int Drill {
            get => _drill;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Drill");

                _drill = value;
            }
        }

        /// <inheritdoc>
        /// 
        public override ElementType ElementType =>
            ElementType.Hole;
    }
}

