namespace MikroPic.EdaTools.v1.Pcb.Model.BoardElements {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure.Polygons;
    using System;

    /// <summary>
    /// Clase que representa un rectangle.
    /// </summary>
    /// 
    public sealed class RectangleElement: BoardElement, IPosition, ISize, IRotation {

        private Point position;
        private Size size;
        private Angle rotation;
        private Ratio roundness;
        private int thickness;
        private bool filled;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="position">Posicio del centre geometric.</param>
        /// <param name="size">Amplada i alçada del rectangle.</param>
        /// <param name="roundness">Factor d'arrodoniment de les cantonades.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="thickness">Amplada de linia. Si es zero, es un rectangle ple.</param>
        /// <param name="filled">True indica si cal omplir el rectangle.</param>
        /// 
        public RectangleElement(LayerSet layerSet, Point position, Size size, Ratio roundness, Angle rotation, int thickness, bool filled) :
            base(layerSet) {

            this.position = position;
            this.size = size;
            this.roundness = roundness;
            this.rotation = rotation;
            this.thickness = thickness;
            this.filled = filled;
        }

        public override BoardElement Clone() {

            return new RectangleElement(LayerSet, position, size, roundness, rotation, thickness, filled);
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
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            if (thickness == 0) {
                Point[] points = PolygonBuilder.BuildRectangle(position, size, Radius, rotation);
                return new Polygon(points);
            }
            else {
                Size outerSize = new Size(size.Width + thickness, size.Height + thickness);
                Point[] outerPoints = PolygonBuilder.BuildRectangle(position, outerSize, Radius, rotation);

                Size innerSize = new Size(size.Width - thickness, size.Height - thickness);
                Point[] innerPoints = PolygonBuilder.BuildRectangle(position, innerSize, Math.Max(0, Radius - thickness), rotation);

                return new Polygon(outerPoints, new Polygon(innerPoints));
            }
        }

        /// <summary>
        /// Crea el poligon exterior del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            Size outerSize = new Size(size.Width + thickness + spacing * 2, size.Height + thickness + spacing * 2);
            Point[] points = PolygonBuilder.BuildRectangle(position, outerSize, Radius, rotation);
            return new Polygon(points);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            double width = size.Width + thickness;
            double height = size.Height + thickness;
            double a = rotation.Radiants;

            int w = (int) (width * Math.Cos(a) + height * Math.Sin(a));
            int h = (int) (width * Math.Sin(a) + height * Math.Cos(a));

            return new Rect(position.X - (w / 2), position.Y - (h / 2), w, h);
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
        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }

        /// <summary>
        /// Obte o asigna el factor d'arrodoniment de les cantonades.
        /// </summary>
        /// 
        public Ratio Roundness {
            get {
                return roundness;
            }
            set {
                roundness = value;
            }
        }

        /// <summary>
        /// Obte el radi de curvatura de les cantonades.
        /// </summary>
        /// 
        public int Radius {
            get {
                return (Math.Min(size.Width, size.Height) * roundness) >> 1;
            }
        }

        /// <summary>
        /// Obte o asigna l'amplada de linia.
        /// </summary>
        /// 
        public int Thickness {
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
                return (thickness == 0) || filled;
            }
            set {
                filled = value;
            }
        }
    }
}
