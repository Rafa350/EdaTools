namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Collections.Generic;
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

        /// <summary>
        /// Constructor per defecte de l'objecte.
        /// </summary>
        /// 
        public ViaElement():
            base() {

        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany/diametre de la corona.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// <param name="shape">Forma de la corona.</param>
        /// 
        public ViaElement(Point position, double size, double drill, ViaShape shape):
            base() {

            if (position == null)
                throw new ArgumentNullException("position");

            this.position = position;
            this.size = size;
            this.drill = drill;
            this.shape = shape;
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany/diametre de la corona.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// <param name="shape">Forma de la corona.</param>
        /// <param name="layers">Capes a les que pertany.</param>
        /// 
        public ViaElement(Point position, double size, double drill, ViaShape shape, IEnumerable<Layer> layers) :
            base(layers) {

            if (position == null)
                throw new ArgumentNullException("position");

            this.position = position;
            this.size = size;
            this.drill = drill;
            this.shape = shape;
        }

        /// <summary>
        /// Comprova si partany a la capa.
        /// </summary>
        /// <param name="layer">La capa per verificar.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public override bool InLayer(Layer layer) {

            if (base.InLayer(layer))
                return true;
            else if (layer.Id == LayerId.Vias)
                return true;
            else if (layer.Id == LayerId.Drills)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna la posicio de la via.
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
        /// Obte o asigna el diametre del forat
        /// </summary>
        /// 
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
        /// 
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
        /// 
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
