namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una linia.
    /// </summary>
    /// 
    public class EdaLineElement: EdaLineBaseElement, IEdaConectable {

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }
    }
}

