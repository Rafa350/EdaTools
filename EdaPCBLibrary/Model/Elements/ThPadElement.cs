namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un pad throug hole
    /// </summary>
    public sealed class ThPadElement: Element, IPosition, IRotation, IName, IConectable {

        public enum ThPadShape {
            Square,
            Octogonal,
            Circular,
            Oval
        }

        private static double autosizefactor = 1.3;

        private ThPadShape shape = ThPadShape.Circular;
        private string name;
        private Point position;
        private double rotation;
        private double size;
        private bool autoSize = false;
        private double drill;

        /// <summary>
        /// Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public ThPadElement():
            base() {
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Orientacio.</param>
        /// <param name="size">Tamany/diametre del pad.</param>
        /// <param name="shape">Diametre del forat.</param>
        /// <param name="drill">Forma de la corona.</param>
        /// 
        public ThPadElement(string name, Point position, double rotation, double size, ThPadShape shape, double drill):
            base() {

            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.size = size;
            this.drill = drill;
            this.shape = shape;
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

            Polygon polygon;
            switch (shape) {
                case ThPadShape.Square:
                    polygon = PolygonBuilder.BuildRectangle(
                        position, 
                        new Size(size + (inflate * 2), size + (inflate * 2)), 
                        inflate,
                        rotation);
                    break;

                case ThPadShape.Octogonal:
                    polygon = PolygonBuilder.BuildRegularPolygon(
                        8, 
                        position, 
                        (size / 2) + inflate, 
                        rotation);
                    break;

                case ThPadShape.Oval:
                    polygon = PolygonBuilder.BuildRectangle(
                        position, 
                        new Size((size * 2) + (inflate * 2), size + (inflate * 2)), 
                        (size / 2) + inflate,
                        rotation);
                    break;

                default:
                    polygon = PolygonBuilder.BuildCircle(
                        position, 
                        (size / 2) + inflate);
                    break;
            }

            // Si esta inflat, no genera el forat
            //
            if (inflate == 0)
                polygon.AddChild(PolygonBuilder.BuildCircle(position, drill / 2));

            return polygon;
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
        ///  Obte o asigna la posicio.
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
        /// Obte o asigna la forma del pad.
        /// </summary>
        /// 
        public ThPadShape Shape {
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
        /// Obte o asigna el diametre del forat.
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
        /// Obte o asigna el tamany del pad.
        /// </summary>
        /// 
        public double Size {
            get {
                return autoSize ? size * autosizefactor : size;
            }
            set {
                if (size != value) {
                    size = value;
                    autoSize = false;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Asigna o obte el indicador de tamany automatic.
        /// </summary>
        /// 
        public bool AutoSize {
            get {
                return autoSize;
            }
            set {
                if (autoSize != value) {
                    autoSize = value;
                    Invalidate();
                }
            }
        }
    }
}
