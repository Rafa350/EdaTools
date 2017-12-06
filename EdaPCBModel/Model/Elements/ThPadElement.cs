namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class ThPadElement: ElementBase {

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
        private double rotate;
        private double size;
        private bool autoSize = false;
        private double drill;

        public override bool InLayer(Layer layer) {

            if ((layer.Id == LayerId.Pads) ||
                (layer.Id == LayerId.TopStop) ||
                (layer.Id == LayerId.BottomStop))
                return true;
            else
                return false;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Nom del pad.
        /// </summary>
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        /// <summary>
        /// Obte o asigna la posicio del pad.
        /// </summary>
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        /// <summary>
        /// Obte o asigna la orientacio del pad.
        /// </summary>
        public double Rotate {
            get {
                return rotate;
            }
            set {
                rotate = value;
            }
        }

        public ThPadShape Shape {
            get {
                return shape;
            }
            set {
                shape = value;
            }
        }

        /// <summary>
        /// Obte o asigna el diametre del forat.
        /// </summary>
        public double Drill {
            get {
                return drill;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Drill");
                drill = value;
            }
        }


        /// <summary>
        /// Obte o asigna el tamany del pad.
        /// </summary>
        public double Size {
            get {
                return autoSize ? size * autosizefactor : size;
            }
            set {
                size = value;
                autoSize = false;
            }
        }

        /// <summary>
        /// Asigna o obte el indicador de tamany automatic.
        /// </summary>
        public bool AutoSize {
            get {
                return autoSize;
            }
            set {
                autoSize = value;
            }
        }
    }
}
