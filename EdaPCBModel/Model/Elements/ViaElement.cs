namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class ViaElement: ElementBase {

        public enum ViaShape {
            Square,
            Octogonal,
            Circular
        }

        private static double autosizeFactor = 1.625;

        private Point position;
        private double drill;
        private double size;
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
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Drill");
                drill = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de la corona.
        /// </summary>
        public double Size {
            get {
                return size <= drill ? drill * autosizeFactor : size;
            }
            set {
                size = value;
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
        /// Obte o asigna la forma exterior. Les interiors sempre son circulars.
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
