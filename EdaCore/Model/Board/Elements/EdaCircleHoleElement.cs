using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un forat circular en la placa.
    /// </summary>
    /// 
    public sealed class EdaCircleHoleElement: EdaHoleElement {

        private EdaPoint _position;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            int radius = Diameter / 2;
            return new EdaRect(_position.X - radius, _position.Y - radius, Diameter, Diameter);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            int radius = Diameter / 2;
            EdaPoints points = EdaPoints.CreateCircle(Position, radius + spacing);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            int radius = Diameter / 2;
            EdaPoints points = EdaPoints.CreateCircle(Position, radius);
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

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.CircleHole;
    }
}
