namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure;
    using System;

    /// <summary>
    /// Clase que representa un rectangle.
    /// </summary>
    public sealed class RectangleElement: Element, IPosition, ISize, IRotation {

        private PointInt position;
        private SizeInt size;
        private Angle rotation;
        private int thickness;

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
        public RectangleElement(PointInt position, SizeInt size, Angle rotation, int thickness = 0) :
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
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            PointInt[] points = PolygonBuilder.BuildRectangle(position, size, 0, rotation);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            PointInt[] points = PolygonBuilder.BuildRectangle(position,
                new SizeInt(size.Width + spacing * 2, size.Height + spacing * 2),
                spacing, rotation);
            return new Polygon(points);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override RectInt GetBoundingBox(BoardSide side) {

            double a = rotation.Radiants;
            int w = (int) (size.Width * Math.Cos(a) + size.Height * Math.Sin(a));
            int h = (int) (size.Width * Math.Sin(a) + size.Height * Math.Cos(a));

            return new RectInt(position.X - (w / 2), position.Y - (h / 2), w, h);
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre geometric del rectangle.
        /// </summary>
        /// 
        public PointInt Position {
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
        public SizeInt Size {
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
                return thickness == 0;
            }
            set {
                if (value)
                    thickness = 0; 
            }
        }
    }
}
