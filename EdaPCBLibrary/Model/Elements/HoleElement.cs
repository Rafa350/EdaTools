namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class HoleElement: Element, IPosition {

        private Point position;
        private double drill;

        /// <summary>
        /// Constructor per defecte de l'objecte.
        /// </summary>
        /// 
        public HoleElement() {
        }

        /// <summary>
        /// Constructir de l'objecte.
        /// </summary>
        /// <param name="position">Pocicio del centre.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// 
        public HoleElement(Point position, double drill) { 

            if (position == null)
                throw new ArgumentNullException("position");

            if (drill <= 0)
                throw new ArgumentOutOfRangeException("drill");

            this.position = position;
            this.drill = drill;
        }

        /// <summary>
        /// Comprova si esta en una capa.
        /// </summary>
        /// <param name="layer">La capa a comprovar.</param>
        /// <returns>True si es en la capa, false en cas contrari.</returns>
        /// 
        public override bool IsOnLayer(Layer layer) {

            return layer.Id == LayerId.Holes;
        }

        /// <summary>
        /// Accepta un visitador del objecte.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
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

        /// <summary>
        /// Obte o asigna el diametre del forat.
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
    }
}
