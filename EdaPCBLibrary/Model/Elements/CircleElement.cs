namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;

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

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="layer">Capa.</param>
        /// <param name="radius">Radi.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// 
        public CircleElement(Point position, Layer layer, double radius, double thickness = 0) :
            base(layer) {

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
        /// Crea el poligon del element.
        /// </summary>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon() {

            Polygon polygon = PolygonBuilder.BuildCircle(position, radius);

            return polygon;
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
                if (position != value) {
                    position = value;
                    Invalidate();
                }
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

                if (radius != value) {
                    radius = value;
                    Invalidate();
                }
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

                double v = value / 2;
                if (radius != v) {
                    radius = v;
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
        /// Obte o asigna el indicador de cercle ple.
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
