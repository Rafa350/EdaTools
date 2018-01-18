namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un rectangle.
    /// </summary>
    public sealed class RectangleElement: Element, IPosition, ISize, IRotation {

        private Point position;
        private Size size;
        private double rotation;
        private double thickness;

        /// <summary>
        ///  Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public RectangleElement(): 
            base() {
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="position">Posicio del centre geometric.</param>
        /// <param name="size">Amplada i alçada del rectangle.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="thickness">Amplada de linia. Si es zero, es un rectangle ple.</param>
        /// 
        public RectangleElement(Point position, Size size, double rotation, double thickness = 0) :
            base() {

            this.position = position;
            this.size = size;
            this.rotation = rotation;
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

            string s = String.Format("{0}${1}${2}${3}${4}${5}${6}",
                GetType().FullName,
                position.X,
                position.Y,
                size.Width,
                size.Height,
                rotation,
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

            if (inflate == 0)
                return PolygonBuilder.BuildRectangle(position, size, 0, rotation);
            else
                return PolygonBuilder.BuildRectangle(position,
                    new Size(size.Width + inflate * 2, size.Height + inflate * 2),
                    inflate, rotation);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        protected override Rect GetBoundingBox() {

            double a = rotation * Math.PI / 180.0;
            double w = size.Width * Math.Cos(a) + size.Height * Math.Sin(a);
            double h = size.Width * Math.Sin(a) + size.Height * Math.Cos(a);

            return new Rect(position.X - w / 2.0, position.Y - h / 2.0, w, h);
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre geometric del rectangle.
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
        /// Obte o asigna el tamany del rectangle.
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
        /// Obte o asigna el indicador de rectangle ple. Es el mateix que Thickness = 0.
        /// </summary>
        /// 
        public bool Filled {
            get {
                return thickness == 0;
            }
            set {
                if (value)
                    Thickness = 0; // Canvia la propietat
            }
        }
    }
}
