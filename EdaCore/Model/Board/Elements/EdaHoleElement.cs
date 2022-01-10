using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un forat no conductor.
    /// </summary>
    /// 
    public sealed class EdaHoleElement : EdaElement, IEdaPosition {

        private EdaPoint _position;
        private int _drill;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(BoardSide side) {

            var points = EdaPoints.CreateCircle(_position, _drill / 2);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(BoardSide side, int spacing) {

            var points = EdaPoints.CreateCircle(_position, (_drill / 2) + spacing);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(BoardSide side) {

            return new EdaRect(_position.X - _drill / 2, _position.Y - _drill / 2, _drill, _drill);
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
                    throw new ArgumentOutOfRangeException(nameof(Drill));

                _drill = value;
            }
        }

        /// <inheritdoc>
        /// 
        public override ElementType ElementType =>
            ElementType.Hole;
    }
}

