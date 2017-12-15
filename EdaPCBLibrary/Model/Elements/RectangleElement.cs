namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class RectangleElement: SingleLayerElement {

        private Size size;
        private double rotate;
        private double thickness;

        public RectangleElement(): 
            base() {

        }

        public RectangleElement(Point position, Layer layer, Size size, double rotate = 0, double thickness = 0) :
            base(position, layer) {

            this.size = size;
            this.rotate = rotate;
            this.thickness = thickness;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public Size Size {
            get {
                return size;
            }
            set {
                size = value;
            }
        }

        public double Rotate {
            get {
                return rotate;
            }
            set {
                rotate = value;
            }
        }

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
