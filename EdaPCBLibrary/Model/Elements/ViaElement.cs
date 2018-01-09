namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public sealed class ViaElement: MultiLayerElement, IPosition, IConected {

        public enum ViaShape {
            Square,
            Octogonal,
            Circular
        }

        public enum ViaType {
            Through,
            Blind,
            Buried
        }

        private const double OAR = 0.125;

        private Point position;
        private double drill;
        private double outerSize;
        private double innerSize;
        private ViaShape shape = ViaShape.Circular;
        private ViaType type = ViaType.Through;

        /// <summary>
        /// Constructor per defecte de l'objecte.
        /// </summary>
        /// 
        public ViaElement():
            base() {

        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany/diametre de la corona.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// <param name="shape">Forma de la corona.</param>
        /// <param name="layers">Capes a les que pertany.</param>
        /// 
        public ViaElement(Point position, IEnumerable<Layer> layers, double size, double drill, ViaShape shape) :
            base(layers) {

            if (position == null)
                throw new ArgumentNullException("position");

            if (size < 0)
                throw new ArgumentOutOfRangeException("size");

            if (drill <= 0)
                throw new ArgumentOutOfRangeException("drill");

            this.position = position;
            this.outerSize = size;
            this.innerSize = size;
            this.drill = drill;
            this.shape = shape;
        }

        /// <summary>
        /// Comprova si partany a la capa.
        /// </summary>
        /// <param name="layer">La capa per verificar.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public override bool IsOnLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            // TODO: Com gestionar though, blind i buried?

            return base.IsOnLayer(layer) ||
                (layer.Id == LayerId.Vias) ||
                (layer.Id == LayerId.Drills);
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
        public override Polygon GetPolygon(double inflate = 0) {

            Polygon polygon;
            switch (shape) {
                case ViaShape.Square:
                    polygon = PolygonBuilder.BuildRectangle(position, new Size((OuterSize / 2) + inflate, (OuterSize / 2) + inflate), 0, 0);
                    break;

                case ViaShape.Octogonal:
                    polygon = PolygonBuilder.BuildRegularPolygon(8, position, (OuterSize / 2) + inflate, 0);
                    break;

                default:
                    polygon = PolygonBuilder.BuildCircle(position, (OuterSize / 2) + inflate);
                    break;
            }

            // Si esta inflat, no genera el forat.
            //
            if (inflate == 0)
                polygon.AddHole(PolygonBuilder.BuildCircle(position, drill / 2));

            return polygon;
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        protected override Rect GetBoundingBox() {

            double size = OuterSize;
            double hSize = size / 2;
            return new Rect(position.X - hSize, position.Y - hSize, size, size);
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
        /// Obte o asigna el diametre del forat
        /// </summary>
        /// 
        public double Drill {
            get {
                return drill;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Drill");

                if (drill != value) {
                    drill = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de la corona de les capes externes.
        /// </summary>
        /// 
        public double OuterSize {
            get {
                return Math.Max(drill + 0.1 + (OAR * 2.0), outerSize);
            }
            set {
                if (outerSize != value) {
                    outerSize = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de la corona de les capes internes.
        /// </summary>
        /// 
        public double InnerSize {
            get {
                return Math.Max(drill + 0.1 + (OAR * 2.0), innerSize);
            }
            set {
                if (innerSize != value) {
                    innerSize = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Obte o asigna la forma exterior. Les interiors sempre son circulars.
        /// </summary>
        /// 
        public ViaShape Shape {
            get {
                return shape;
            }
            set {
                if (shape != value) {
                    shape = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Obte o asigna el tipus de via.
        /// </summary>
        /// 
        public ViaType Type {
            get {
                return type;
            }
            set {
                type = value;
            }
        }
    }
}
