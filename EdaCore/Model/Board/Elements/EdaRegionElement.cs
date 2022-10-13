using System;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una regio poligonal conductora.
    /// </summary>
    /// 
    public sealed class EdaRegionElement: EdaPolygonElement, IEdaConectable {

        private int _priority;
        private int _clearance;
        private int _minThickness;
        private int _thermalClearance;
        private int _thermalThickness;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// L'amplada del aillament
        /// </summary>
        /// 
        public int Clearance {
            get => _clearance;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Clearance));

                _clearance = value;
            }
        }

        /// <summary>
        /// L'amplada minima de la regio
        /// </summary>
        /// 
        public int MinThickness {
            get => _minThickness;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(MinThickness));

                _minThickness = value;
            }
        }

        /// <summary>
        /// El espaiat dels termals.
        /// </summary>
        /// 
        public int ThermalClearance {
            get => Math.Max(_thermalClearance, _clearance);
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(ThermalClearance));
                _thermalClearance = value;
            }
        }

        /// <summary>
        /// L'amplada dels termals.
        /// </summary>
        /// 
        public int ThermalThickness {
            get => _thermalThickness;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(ThermalThickness));
                _thermalThickness = value;
            }
        }

        /// <summary>
        /// La prioritat
        /// </summary>
        /// 
        public int Priority {
            get => _priority;
            set => _priority = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Region;
    }
}
