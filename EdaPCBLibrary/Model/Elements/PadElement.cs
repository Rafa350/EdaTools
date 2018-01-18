namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public abstract class PadElement : Element, IPosition, IName, IConectable {

        private string name;
        private Point position;

        public PadElement():
            base() {

        }

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
        /// Obte o asigna el nom.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
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
