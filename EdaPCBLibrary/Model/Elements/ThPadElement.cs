namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class ThPadElement: Element, IPosition, IRotation {

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
        private double rotation;
        private double size;
        private bool autoSize = false;
        private double drill;

        /// <summary>
        /// Constructor per defecte del objecte.
        /// </summary>
        /// 
        public ThPadElement():
            base() {
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Orientacio.</param>
        /// <param name="size">Tamany/diametre del pad.</param>
        /// <param name="shape">Diametre del forat.</param>
        /// <param name="drill">Forma de la corona.</param>
        /// 
        public ThPadElement(string name, Point position, double rotation, double size, ThPadShape shape, double drill):
            base() {

            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.size = size;
            this.drill = drill;
            this.shape = shape;
        }

        /// <summary>
        /// Comprova si pertany a la capa especificada.
        /// </summary>
        /// <param name="layer">La capa a comprovar.</param>
        /// <returns>True si pertany, false en cas contraru.</returns>
        /// 
        public override bool IsOnLayer(Layer layer) {

            if ((layer.Id == LayerId.Pads) ||
                (layer.Id == LayerId.TopStop) ||
                (layer.Id == LayerId.BottomStop) ||
                (layer.Id == LayerId.Drills))
                return true;
            else
                return false;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna el nom.
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
        /// Obte o asigna la orientacio del pad.
        /// </summary>
        public double Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
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
