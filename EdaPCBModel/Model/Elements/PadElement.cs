namespace MikroPic.EdaTools.v1.Model.Elements {

    using System;
    using System.Windows;

    /// <summary>
    /// Clase base per tots els pads.
    /// </summary>
    public abstract class PadElement: ElementBase {

        private string name;
        private Point position;
        private double rotate;

        /// <summary>
        /// Nom del pad.
        /// </summary>
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        /// <summary>
        /// Obte o asigna la posicio del pad.
        /// </summary>
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        /// <summary>
        /// Obte o asigna la orientacio del pad.
        /// </summary>
        public double Rotate {
            get {
                return rotate;
            }
            set {
                rotate = value;
            }
        }
    }
}
