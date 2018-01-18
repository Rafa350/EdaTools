﻿namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un text.
    /// </summary>
    public sealed class TextElement: Element, IPosition, IRotation {

        public enum TextAlign {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight,
        }

        private Point position;
        private double rotation;
        private double height;
        private TextAlign align = TextAlign.MiddleCenter;
        private string value;
        private string name;

        /// <summary>
        /// Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public TextElement():
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="height">Alçada de lletra.</param>
        /// <param name="align">Alineacio respecte la posicio.</param>
        /// 
        public TextElement(Point position, double rotation, double height, TextAlign align = TextAlign.MiddleCenter):
            base() {

            this.position = position;
            this.rotation = rotation;
            this.height = height;
            this.align = align;
        }

        /// <summary>
        /// Accepta  un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Calcula el numero de serie del element.
        /// </summary>
        /// <returns>El numero de serie.</returns>
        /// 
        protected override int GetSerial() {

            string s = String.Format("{0}${1}${2}${3}${4}${5}",
                GetType().FullName,
                position.X,
                position.Y,
                rotation,
                height,
                align);
            return s.GetHashCode();
        }
        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(double inflate = 0) {

            return new Polygon();
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        protected override Rect GetBoundingBox() {

            throw new NotImplementedException();
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre del cercle.
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
        /// Obte o asigna l'angle de rotacio.
        /// </summary>
        /// 
        public double Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'alçada de lletra.
        /// </summary>
        /// 
        public double Height {
            get {
                return height;
            }
            set {
                height = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'aliniacio del text.
        /// </summary>
        /// 
        public TextAlign Align {
            get {
                return align;
            }
            set {
                align = value;
            }
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
        /// Obte o asigna el valor del text.
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
    }
}
