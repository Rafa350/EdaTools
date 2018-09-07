namespace MikroPic.EdaTools.v1.Pcb.Model.BoardElements {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Geometry.Fonts;
    using System;

    /// <summary>
    /// Clase que representa un text.
    /// </summary>
    /// 
    public sealed class TextElement: BoardElement, IPosition, IRotation {

        private Point position;
        private Angle rotation;
        private int height;
        private int thickness;
        private HorizontalTextAlign horizontalAlign;
        private VerticalTextAlign verticalAlign;
        private string value;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="height">Alçada de lletra.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="horizontalAlign">Aliniacio horitzontal.</param>
        /// <param name="verticalAlign">Aliniacio vertical.</param>
        /// 
        public TextElement(LayerSet layerSet, Point position, Angle rotation, int height, int thickness, 
            HorizontalTextAlign horizontalAlign = HorizontalTextAlign.Left,
            VerticalTextAlign verticalAlign = VerticalTextAlign.Bottom):
            base(layerSet) {

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            if (thickness <= 0)
                throw new ArgumentOutOfRangeException("thickness");

            this.position = position;
            this.rotation = rotation;
            this.height = height;
            this.thickness = thickness;
            this.horizontalAlign = horizontalAlign;
            this.verticalAlign = verticalAlign;
        }

        /// <summary>
        /// Accepta  un visitador.
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

            return null;
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            return null;
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            throw new NotImplementedException();
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
        /// Obte o asigna l'alçada de lletra.
        /// </summary>
        /// 
        public int Height {
            get {
                return height;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Height");

                height = value;
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
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Thickness");

                thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'aliniacio horitzontal del text.
        /// </summary>
        /// 
        public HorizontalTextAlign HorizontalAlign {
            get {
                return horizontalAlign;
            }
            set {
                horizontalAlign = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'aliniacio vertical del text.
        /// </summary>
        /// 
        public VerticalTextAlign VerticalAlign {
            get {
                return verticalAlign;
            }
            set {
                verticalAlign = value;
            }
        }

        /// <summary>
        /// Obte o asigna el valor del text.
        /// </summary>
        /// 
        public string Value {
            get {
                return value;
            }
            set {
                this.value = value;
            }
        }
    }
}
