namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un cercle.
    /// </summary>
    public sealed class CircleElement: Element, IPosition {

        private Point position;
        private double radius;
        private double thickness;

        /// <summary>
        ///  Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public CircleElement():
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <param name="thickness">Amplada de linia. Zero indica que es un disc.</param>
        /// 
        public CircleElement(Point position, double radius, double thickness = 0) :           
            base() {

            this.position = position;
            this.radius = radius;
            this.thickness = thickness;
        }

        /// <summary>
        /// Accepta un visitador del objecte.
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

            string s = String.Format("{0}${1}${2}${3}${4}",
                GetType().FullName,
                position.X,
                position.Y,
                radius,
                thickness);
            return s.GetHashCode();
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="inflate">Increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(double inflate = 0) {

            return PolygonBuilder.BuildCircle(position, radius + inflate);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox() {

            return new Rect(position.X - radius, position.Y - radius, radius + radius, radius + radius);
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

        /// <summary>
        /// Obte o asigna l'amplada de linia.
        /// </summary>
        /// 
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

        /// <summary>
        /// Obte o asigna el indicador de cercle ple.
        /// </summary>
        /// 
        public bool Filled {
            get {
                return thickness == 0;
            }
            set {
                if (value)
                    Thickness = 0; // Canvia la propietat, no el camp
            }
        }
    }
}
