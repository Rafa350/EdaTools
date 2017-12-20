namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

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

        private Point position;
        private double rotation;
        private double height;
        private TextAlign align = TextAlign.MiddleCenter;
        private string value;
        private string name;

        public TextElement():
            base() {
        }

        public TextElement(Point position, Layer layer, double rotation, double height, TextAlign align = TextAlign.MiddleCenter):
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

        public double Rotation {
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
