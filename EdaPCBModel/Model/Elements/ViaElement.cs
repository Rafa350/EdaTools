namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class ViaElement: MultiLayerElement {

        public enum ViaShape {
            Square,
            Octogonal,
            Circular
        }

        private const double OAR = 0.125;

        private Point position;
        private double drill;
        private double size;
        private ViaShape shape = ViaShape.Circular;

        public override bool InLayer(Layer layer) {

            if (base.InLayer(layer))
                return true;
            else if (layer.Id == LayerId.Vias)
                return true;
            else
                return false;
        }

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
                return Math.Max(drill + 0.1 + (OAR * 2.0), size);
            }
            set {
                size = value;
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
