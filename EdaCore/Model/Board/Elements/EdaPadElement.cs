﻿using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un pad.
    /// </summary>
    /// 
    public abstract class EdaPadElement : EdaElement, IEdaPosition, IEdaRotation, IEdaName, IEdaConectable {

        private string _name;
        private EdaPoint _position;
        private EdaAngle _rotation;

        /// <summary>
        /// Crea el poligon del thermal.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <param name="width">Amplada dels conductors.</param>
        /// <returns>El poligon.</returns>
        /// 
        public abstract EdaPolygon GetThermalPolygon(BoardSide side, int spacing, int width);

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
    }
}
