using System;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un forat circular en la placa.
    /// </summary>
    /// 
    public sealed class EdaCircularHoleElement: EdaHoleElement {

        private EdaPoint _position;

        /// <inheritdoc/>
        /// 
        public override int GetHashCode() =>
            HashCode.Combine(_position, base.GetHashCode());

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            int r = Diameter / 2;
            return new EdaRect(_position.X - r, _position.Y - r, Diameter, Diameter);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            int r = Diameter / 2;
            var points = EdaPointFactory.CreateCircle(Position, r + spacing);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            int r = Diameter / 2;
            var points = EdaPointFactory.CreateCircle(Position, r);
            return new EdaPolygon(points);
        }

        /// <summary>
        /// Obte la posicio.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set => _position = value;
        }
    }
}
