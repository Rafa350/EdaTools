namespace MikroPic.EdaTools.v1.Pcb.Model.BoardElements {

    using System;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;

    /// <summary>
    /// Clase que representa un pad.
    /// </summary>
    /// 
    public abstract class PadElement : BoardElement, IPosition, IRotation, IName, IConectable {

        private string name;
        private Point position;
        private Angle rotation;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom del pad.</param>
        /// <param name="position">Posicio.</param>
        /// 
        public PadElement(string name, Point position, Angle rotation) :
            base() {

            this.name = name;
            this.position = position;
            this.rotation = rotation;
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
                return name;
            }
            set {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Pad.Name");

                name = value;
            }
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre geometric del pad.
        /// </summary>
        /// 
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'orientacio del pad.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }
    }
}
