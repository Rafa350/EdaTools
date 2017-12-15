﻿namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System.Windows;

    public sealed class TextElement: SingleLayerElement {

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

        private double rotate = 0;
        private double height;
        private TextAlign align = TextAlign.MiddleCenter;
        private string value;
        private string name;

        public TextElement():
            base() {

        }

        public TextElement(Point position, Layer layer, double rotate, double height, TextAlign align = TextAlign.MiddleCenter):
            base(position, layer) {

            this.rotate = rotate;
            this.height = height;
            this.align = align;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public double Rotate {
            get {
                return rotate;
            }
            set {
                rotate = value;
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
