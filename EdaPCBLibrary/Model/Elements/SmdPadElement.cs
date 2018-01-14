namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un pad superficial
    /// </summary>
    public sealed class SmdPadElement: Element, IPosition, IRotation, IName, IConectable {

        private string name;
        private Point position;
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
            base() {

            this.name = name;
            this.position = position;
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
        /// <param name="inflate">Increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(double inflate = 0) {

            return PolygonBuilder.BuildRectangle(position,
                new Size(size.Width + (inflate * 2), size.Height + (inflate * 2)), Radius + inflate, rotation);
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
                if (position != value) {
                    position = value;
                    Invalidate();
                }
            }
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
                if (rotation != value) {
                    rotation = value;
                    Invalidate();
                }
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
                if (size != value) {
                    size = value;
                    Invalidate();
                }
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

                if (roundnes != value) {
                    roundnes = value;
                    Invalidate();
                }
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
