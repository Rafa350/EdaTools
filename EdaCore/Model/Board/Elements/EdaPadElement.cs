﻿using System;
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
        private int _maskClearance;
        private bool _maskEnabled = true;

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
        /// La posicio del centre geometric del pad.
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
        /// L'espai entre el pad i altres conductors.
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
        /// L'espai entre el pad i la mascara de soldadura.
        /// </summary>
        /// 
        public int MaskClearance {
            get => _maskClearance;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(MaskClearance));
                _maskClearance = value;
            }
        }

        /// <summary>
        /// Activa o desactiva la mascara de soldadura.
        /// </summary>
        /// 
        public bool MaskEnabled {
            get => _maskEnabled;
            set => _maskEnabled = value;
        }
    }
}
