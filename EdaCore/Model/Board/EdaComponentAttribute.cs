namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa un atribut.
    /// </summary>
    /// 
    public sealed class EdaComponentAttribute: EdaAttributeBase {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="value">El valor.</param>
        /// 
        public EdaComponentAttribute(string name, string value = null) :
            base(name, value) { 
        }

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }
    }
}
