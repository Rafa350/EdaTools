namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Base.Geometry;

    /// <summary>
    /// Identifica la cara de la placa.
    /// </summary>
    /// 
    public enum BoardSide {
        None,
        Top,
        Inner,
        Bottom,
        Body
    }

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board : IBoardVisitable {

        private Point position;
        private Angle rotation;

        /// <summary>
        /// Constructor del objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public Board() {
        }

        /// <summary>
        /// Procesa un visitador.
        /// </summary>
        /// <param name="visitor">Visitador.</param>
        /// 
        public void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna la posicio de la placa.
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
        /// Obte o asigna l'angle de rotacio de la placa.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }
    }
}
