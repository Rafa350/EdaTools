namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    public sealed class TextElement: SingleLayerElement, IPosition, IRotation {

        public enum TextAlign {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight,
        }

        private System.Windows.Point position;
        private Angle rotation;
        private double height;
        private TextAlign align = TextAlign.MiddleCenter;
        private string value;
        private string name;

        public TextElement():
            base() {
        }

        public TextElement(System.Windows.Point position, Layer layer, Angle rotation, double height, TextAlign align = TextAlign.MiddleCenter):
            base(layer) {

            this.position = position;
            this.rotation = rotation;
            this.height = height;
            this.align = align;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(double inflate = 0) {

            throw new System.NotImplementedException();
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
        ///  Obte o asigna la posicio del centre del cercle.
        /// </summary>
        /// 
        public System.Windows.Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }

        public double Height {
            get {
                return height;
            }
            set {
                height = value;
            }
        }

        public TextAlign Align {
            get {
                return align;
            }
            set {
                align = value;
            }
        }

        public string Name {
            get {
                return name;
            }
            set {
                this.name = value;
            }
        }

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
