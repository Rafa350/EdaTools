using System;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un forat en forma de linia en la placa.
    /// </summary>
    /// 
    public class EdaLinearHoleElement: EdaHoleElement {

        private EdaPoint _startPosition;
        private EdaPoint _endPosition;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            int r = Diameter / 2;
            return new EdaRect(
                Math.Min(_startPosition.X, _endPosition.X) - r,
                Math.Min(_startPosition.Y, _endPosition.Y) - r,
                Math.Abs(_endPosition.X - _startPosition.X + 1) + Diameter,
                Math.Abs(_endPosition.Y - _startPosition.Y + 1) + Diameter);
        }


        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            var points = EdaPointFactory.CreateLineTrace(_startPosition, _endPosition, Diameter + (spacing * 2), true);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            var points = EdaPointFactory.CreateLineTrace(_startPosition, _endPosition, Diameter, true);
            return new EdaPolygon(points);
        }

        /// <summary>
        /// Obte la posicio inicial.
        /// </summary>
        /// 
        public EdaPoint StartPosition {
            get => _startPosition;
            set => _startPosition = value;
        }

        /// <summary>
        /// Obte la posicio final.
        /// </summary>
        /// 
        public EdaPoint EndPosition {
            get => _endPosition;
            set => _endPosition = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.LineHole;
    }
}
