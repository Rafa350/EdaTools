﻿using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un pad.
    /// </summary>
    /// 
    public abstract class PadElement : Element, IPosition, IRotation, IName, IConectable {

        private string _name;
        private Point _position;
        private Angle _rotation;
        private bool _stop = false;
        private int _stopMargin = 0;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom del pad.</param>
        /// <param name="position">Posicio.</param>
        /// 
        public PadElement(string name, Point position, Angle rotation) :
            base() {

            _name = name;
            _position = position;
            _rotation = rotation;
        }

        /// <summary>
        /// Crea el poligon del thermal.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <param name="width">Amplada dels conductors.</param>
        /// <returns>El poligon.</returns>
        /// 
        public abstract Polygon GetThermalPolygon(BoardSide side, int spacing, int width);

        /// <summary>
        /// Obte o asigna el nom.
        /// </summary>
        /// 
        public string Name {
            get {
                return _name;
            }
            set {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Pad.Name");

                _name = value;
            }
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre geometric del pad.
        /// </summary>
        /// 
        public Point Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// Obte o asigna l'orientacio del pad.
        /// </summary>
        /// 
        public Angle Rotation {
            get => _rotation;
            set => _rotation = value;
        }

        /// <summary>
        /// Indica si es genera mascara automaticament.
        /// </summary>
        /// 
        public bool Stop {
            get => _stop;
            set => _stop = value;
        }

        /// <summary>
        /// Marge de la mascara de soldadura
        /// </summary>
        /// 
        public int StopMargin {
            get => _stopMargin;
            set => _stopMargin = value;
        }
    }
}
