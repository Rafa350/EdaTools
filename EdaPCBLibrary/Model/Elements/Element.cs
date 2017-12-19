namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System.Windows;

    public abstract class Element: IVisitable {

        private Point position;

        public Element () {

        }

        public Element(Point position) {

            this.position = position;
        }

        public abstract void AcceptVisitor(IVisitor visitor);

        /// <summary>
        /// Comprova si l'element pertany a la capa.
        /// </summary>
        /// <param name="layer">La capa a comprovar.</param>
        /// <returns>True si pertany a la capa. False en cas contraru.</returns>
        /// 
        public abstract bool IsOnLayer(Layer layer);

        /// <summary>
        /// Obte les coordinades de la posicio del element.
        /// </summary>
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }
    }
}
