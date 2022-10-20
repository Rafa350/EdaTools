using System;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un pad.
    /// </summary>
    /// 
    public abstract class EdaPadElement: EdaElement, IEdaConectable {

        private string _name;
        private EdaPoint _position;
        private EdaAngle _rotation;
        private int _clearance;

        /// <summary>
        /// El nom.
        /// </summary>
        /// 
        public string Name {
            get => _name;
            set {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(Name));
                _name = value;
            }
        }

        /// <summary>
        ///  La posicio del centre geometric del pad.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// L'orientacio del pad.
        /// </summary>
        /// 
        public EdaAngle Rotation {
            get => _rotation;
            set => _rotation = value;
        }


        /// <summary>
        /// Amplada del aillament.
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
    }
}
