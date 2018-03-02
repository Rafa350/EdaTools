namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un atribut d'un component
    /// </summary>
    public sealed class PartAttribute : IVisitable {

        private readonly string name;
        private string value;
        private Point position;
        private Angle rotation;
        private TextAlign align;
        private bool usePosition;
        private bool useRotation;
        private bool useAlign;
        private bool isVisible;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom del atribut</param>
        /// <param name="value">Valor del atribut.</param>
        /// <param name="isVisible">Indica si es visible.</param>
        /// 
        public PartAttribute(string name, string value, bool isVisible = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
            this.IsVisible = isVisible;
        }

        public PartAttribute(string name, Point position, Angle rotation, TextAlign align, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
            isVisible = true;

            this.position = position;
            usePosition = true;

            this.rotation = rotation;
            useRotation = true;

            this.align = align;
            useAlign = true;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Indica si l'atribut es visible o no.
        /// </summary>
        /// 
        public bool IsVisible {
            get {
                return isVisible;
            }
            set {
                isVisible = value;
            }
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
                usePosition = true;
            }
        }

        /// <summary>
        /// Obte l'angle de rotacio de l'atribut.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
                useRotation = true;
            }
        }

        /// <summary>
        /// Obte l'aliniacio de l'atribut.
        /// </summary>
        /// 
        public TextAlign Align {
            get {
                return align;
            }
            set {
                align = value;
                useAlign = true;
            }
        }

        /// <summary>
        /// Obte el valor de l'atribut.
        /// </summary>
        /// 
        public string Value {
            get {
                return value;
            }
            set {
                this.value = value;
            }
        }

        public bool UsePosition {
            get {
                return usePosition;
            }
        }

        public bool UseRotation {
            get {
                return useRotation;
            }
        }

        public bool UseAlign {
            get {
                return useAlign;
            }
        }
    }
}