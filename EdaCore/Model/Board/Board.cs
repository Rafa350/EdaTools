using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Core.Model.Board.IO.Serializers;
using MikroPic.EdaTools.v1.Core.Model.Common;
using NetSerializer.Attributes;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

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
    [NetSerializer(typeof(BoardSerializer), AliasName = "Board")]
    public sealed partial class Board : IVisitable<IBoardVisitor> {

        private Point _position;
        private Angle _rotation;

        /// <summary>
        /// Constructor del objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public Board() {
        }

        /// <inheritdoc/>
        /// 
        public void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna la posicio de la placa.
        /// </summary>
        /// 
        public Point Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// Obte o asigna l'angle de rotacio de la placa.
        /// </summary>
        /// 
        public Angle Rotation {
            get => _rotation;
            set => _rotation = value;
        }
    }
}
