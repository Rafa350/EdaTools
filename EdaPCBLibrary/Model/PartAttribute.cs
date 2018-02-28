namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un atribut d'un component
    /// </summary>
    public sealed class PartAttribute: IVisitable {

        private readonly string name;
        private string value;
        private Point position;
        private Angle rotation;
        private TextAlign align;
        private bool isVisible;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom del atribut</param>
        /// <param name="value">Valor del atribut.</param>
        /// 
        public PartAttribute(string name, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
        }

        public PartAttribute(string name, Point position, Angle rotation, TextAlign align, bool isVisible, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.align = align;
            this.isVisible = isVisible;
            this.value = value;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el nom del atribut.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Obte la posicio del atribut.
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

        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }

        public bool IsVisible {
            get {
                return isVisible;
            }
            set {
                isVisible = value;
            }
        }

        public TextAlign Align {
            get {
                return align;
            }
            set {
                align = value;
            }
        }

        public string Value {
            get {
                return value;
            }
            set {
                this.value = value;
            }
        }
    }
}
