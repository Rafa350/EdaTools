using System;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una regio poligonal conductora.
    /// </summary>
    /// 
    public sealed class RegionElement : PolygonElement, IEdaConectable {

        private int _clearance;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna l'amplada del aillament
        /// </summary>
        /// 
        public int Clearance {
            get => _clearance;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Clearance");

                _clearance = value;
            }
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Region;
    }
}
