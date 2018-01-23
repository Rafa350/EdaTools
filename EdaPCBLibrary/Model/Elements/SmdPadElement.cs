namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un pad superficial
    /// </summary>
    public sealed class SmdPadElement: PadElement, IRotation {

        private Size size;
        private double rotation;
        private double roundnes;

        /// <summary>
        /// Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public SmdPadElement():
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom del pad.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="roundnes">Percentatge d'arrodoniment de les cantonades.</param>
        /// 
        public SmdPadElement(string name, Point position, Size size, double rotation, double roundnes) :
            base(name, position) {

            this.size = size;
            this.rotation = rotation;
            this.roundnes = roundnes;
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon() {

            return PolygonBuilder.BuildRectangle(Position, Size, Radius, rotation);
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPourPolygon(double spacing) {

            return PolygonBuilder.BuildRectangle(Position,
                new Size(size.Width + (spacing * 2), size.Height + (spacing * 2)), Radius + spacing, rotation);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox() {

            double a = rotation * Math.PI / 180.0;
            double w = size.Width * Math.Cos(a) + size.Height * Math.Sin(a);
            double h = size.Width * Math.Sin(a) + size.Height * Math.Cos(a);

            return new Rect(Position.X - w / 2.0, Position.Y - h / 2.0, w, h);

        }

        /// <summary>
        /// Obte o asigna l'orientacio del pad.
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
        /// Obte o asigna el tamany del pad.
        /// </summary>
        /// 
        public Size Size {
            get {
                return size;
            }
            set {
                size = value;
            }
        }

        /// <summary>
        /// Obte o asigna el factor d'arrodoniment de les cantonades del pad.
        /// </summary>
        /// 
        public double Roundnes {
            get {
                return roundnes;
            }
            set {
                if (value < 0 || roundnes > 1)
                    throw new ArgumentOutOfRangeException("Roundness");

                roundnes = value;
            }
        }

        /// <summary>
        /// Obte el radi de curvatura de les cantonades.
        /// </summary>
        /// 
        public double Radius {
            get {
                return Math.Min(size.Width, size.Height) * Roundnes / 2;
            }
        }
    }
}
