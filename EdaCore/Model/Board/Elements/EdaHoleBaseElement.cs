using System;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un forat en la placa.
    /// </summary>
    /// 
    public abstract class EdaHoleBaseElement: EdaElementBase {

        private int _diameter;
        private bool _platted;

        /// <inheritdoc/>
        /// 
        public override int GetHashCode() =>
            HashCode.Combine(_diameter, _platted, base.GetHashCode());

        /// <inheritdoc/>
        /// 
        public override bool IsOnLayer(EdaLayerId layerId) {
            return
                ((layerId == EdaLayerId.Platted) && _platted) ||
                ((layerId == EdaLayerId.Unplatted) && !_platted) ||
                (layerId == EdaLayerId.TopStop) ||
                (layerId == EdaLayerId.BottomStop);
        }

        /// <summary>
        /// Diamtre del forat.
        /// </summary>
        /// 
        public int Diameter {
            get => _diameter;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Diameter));
                _diameter = value;
            }
        }

        /// <summary>
        /// Indica si es platejat o no.
        /// </summary>
        /// 
        public bool Platted {
            get => _platted;
            set => _platted = value;
        }
    }
}
