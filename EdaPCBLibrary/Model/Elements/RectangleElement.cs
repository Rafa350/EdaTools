namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;

    public sealed class RectangleElement: SingleLayerElement, IPosition, ISize, IRotation {

        private Point position;
        private Size size;
        private double rotation;
        private double thickness;

        /// <summary>
        ///  Constructor por defecte de l'objecte.
        /// </summary>
        /// 
        public RectangleElement(): 
            base() {
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="position">Posicio del centre geometric.</param>
        /// <param name="layer">Capa.</param>
        /// <param name="size">Amplada i alçada del rectangle.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="thickness">Amplada de linia. Si es zero, es un rectangle ple.</param>
        /// 
        public RectangleElement(Point position, Layer layer, Size size, double rotation = 0, double thickness = 0) :
            base(layer) {

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

        protected override Polygon GetPolygon() {

            return PolygonBuilder.Build(this, null, 0);
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
                if (position != value) {
                    position = value;
                    Invalidate();
                }
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
                if (size != value) {
                    size = value;
                    Invalidate();
                }
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
                if (rotation != value) {
                    rotation = value;
                    Invalidate();
                }
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

                if (thickness != value) {
                    thickness = value;
                    Invalidate();
                }
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
