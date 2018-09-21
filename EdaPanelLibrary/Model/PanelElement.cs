namespace MikroPic.EdaTools.v1.Panel.Model {

    using MikroPic.EdaTools.v1.Geometry;

    public abstract class PanelElement: IVisitable {

        private Point position;
        private Angle rotation;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// 
        public PanelElement() {

        }

        /// <summary>
        /// Contructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Rotacio.</param>
        /// 
        public PanelElement(Point position, Angle rotation) {

            this.position = position;
            this.rotation = rotation;
        }

        public abstract void AcceptVisitor(IVisitor visitor);

        /// <summary>
        /// Obte o asigna la posicio.
        /// </summary>
        /// 
        public Point Position {
            set {
                position = value;
            }
            get {
                return position;
            }
        }

        /// <summary>
        /// Obte o asigna la rotacio.
        /// </summary>
        /// 
        public Angle Rotation {
            set {
                rotation = value;
            }
            get {
                return rotation;
            }
        }
    }
}
