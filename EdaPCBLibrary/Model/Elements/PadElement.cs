namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;

    /// <summary>
    /// Clase que representa un pad.
    /// </summary>
    /// 
    public abstract class PadElement : Element, IPosition, IName, IConectable {

        private string name;
        private Point position;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom del pad.</param>
        /// <param name="position">Posicio.</param>
        /// 
        public PadElement(string name, Point position) :
            base() {

            this.name = name;
            this.position = position;
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
    }
}
