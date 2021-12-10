using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una regio poligonal conductora.
    /// </summary>
    /// 
    public sealed class RegionElement : PolygonElement, IConectable {

        private int _clearance;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="filled">True si es ple.</param>
        /// <param name="clearance">Distancia d'aillament.</param>
        /// <param name="segments">Llista de segments.</param>
        /// 
        public RegionElement(LayerSet layerSet, int thickness, bool filled, int clearance, IEnumerable<Segment> segments = null) :
            base(layerSet, thickness, filled, segments) {

            if (clearance < 0)
                throw new ArgumentOutOfRangeException(nameof(clearance));

            _clearance = clearance;

            if (segments != null)
                foreach (Segment segment in segments)
                    Add(segment);
        }

        /// <inheritdoc/>
        /// 
        public override Element Clone() {

            return new RegionElement(LayerSet, Thickness, Filled, _clearance, Segments);
        }

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

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
