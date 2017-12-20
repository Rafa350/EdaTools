namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class CircleElement: SingleLayerElement, IPosition {

        private Point position;
        private double radius;
        private double thickness;

        /// <summary>
        ///  Constructor por defecte de l'objecte.
        /// </summary>
        /// 
        public CircleElement():
            base() {

        }

        public CircleElement(Point position, Layer layer, double radius, double thickness = 0) :
            base(layer) {

            this.position = position;
            this.radius = radius;
            this.thickness = thickness;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
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
        /// Obte o asigna el radi del cercle.
        /// </summary>
        /// 
        public double Radius {
            get {
                return radius;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Radius");
                radius = value;
            }
        }

        /// <summary>
        /// Obte o asigna el diametre del cercle.
        /// </summary>
        /// 
        public double Diameter {
            get {
                return radius * 2;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Diameter");
                radius = value / 2;
            }
        }

        public double Thickness {
            get {
                return thickness;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Thickness");
                thickness = value;
            }
        }

        public bool Filled {
            get {
                return thickness == 0;
            }
            set {
                if (value)
                    thickness = 0;
            }
        }
    }
}
