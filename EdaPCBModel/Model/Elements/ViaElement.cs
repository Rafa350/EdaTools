namespace MikroPic.EdaTools.v1.Model.Elements {

    using System.Windows;

    public sealed class ViaElement: ElementBase {

        public enum ViaShape {
            Square,
            Octogonal,
            Circular
        }

        private static double autosizeFactor = 1.3;

        private Point position;
        private double drill;
        private double size;
        private bool autoSize = false;
        private Layer upper;
        private Layer lower;
        private ViaShape shape = ViaShape.Circular;

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna la posicio de la via.
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
        /// Obte o asigna el diametre del forat
        /// </summary>
        public double Drill {
            get {
                return drill;
            }
            set {
                drill = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de la corona.
        /// </summary>
        public double Size {
            get {
                return autoSize ? size * autosizeFactor : size;
            }
            set {
                size = value;
                autoSize = false;
            }
        }


        /// <summary>
        /// Obte o asigna el indicador de tamany automatic de la corona.
        /// </summary>
        public bool AutoSize {
            get {
                return autoSize;
            }
            set {
                autoSize = value;
            }
        }

        public Layer Lower {
            get {
                return lower;
            }
            set {
                lower = value;
            }
        }

        public Layer Upper {
            get {
                return upper;
            }
            set {
                upper = value;
            }
        }

        /// <summary>
        /// Obte o asigna la forma de la corona exterior. Les interiors sembre son circulars.
        /// </summary>
        public ViaShape Shape {
            get {
                return shape;
            }
            set {
                shape = value;
            }
        }
    }
}
